const { Client, LocalAuth } = require('whatsapp-web.js');
const qrcode = require('qrcode-terminal');
const sql = require('mssql');
require('dotenv').config();

// ==================== DATABASE CONFIGURATION ====================
const dbConfig = {
    server: process.env.DB_SERVER || 'localhost',
    database: process.env.DB_NAME || 'SchoolManagementDB',
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,
    port: parseInt(process.env.DB_PORT || '1433'),
    options: {
        encrypt: process.env.DB_ENCRYPT === 'true',
        trustServerCertificate: process.env.DB_TRUST_SERVER_CERTIFICATE === 'true',
        enableArithAbort: true
    },
    pool: {
        max: 10,
        min: 0,
        idleTimeoutMillis: 30000
    }
};

let pool = null;
let isReady = false;
let isProcessing = false;

const POLL_INTERVAL = parseInt(process.env.POLL_INTERVAL_MS || '10000');
const BATCH_SIZE = parseInt(process.env.BATCH_SIZE || '50');

// ==================== DATABASE CONNECTION ====================
async function connectDatabase() {
    try {
        pool = await sql.connect(dbConfig);
        console.log('✅ Connected to SQL Server database');
        return true;
    } catch (error) {
        console.error('❌ Database connection failed:', error.message);
        return false;
    }
}

// ==================== WHATSAPP CLIENT SETUP ====================
const client = new Client({
    authStrategy: new LocalAuth({
        dataPath: process.env.SESSION_PATH || './whatsapp-session'
    }),
    puppeteer: {
        headless: true,
        args: [
            '--no-sandbox',
            '--disable-setuid-sandbox',
            '--disable-dev-shm-usage',
            '--disable-accelerated-2d-canvas',
            '--no-first-run',
            '--no-zygote',
            '--disable-gpu'
        ]
    }
});

// QR Code generation
client.on('qr', (qr) => {
    console.log('\n📱 WhatsApp QR Code Generated!');
    console.log('Scan this QR code with your WhatsApp mobile app:\n');
    qrcode.generate(qr, { small: true });
    console.log('\n⚠️  Worker paused until WhatsApp is connected\n');
});

// Client ready
client.on('ready', () => {
    console.log('✅ WhatsApp client is ready!');
    console.log('📱 Phone: ' + client.info.wid.user);
    console.log('👤 Name: ' + client.info.pushname);
    isReady = true;
});

client.on('authenticated', () => {
    console.log('✅ WhatsApp authenticated successfully!');
});

client.on('auth_failure', (msg) => {
    console.error('❌ Authentication failed:', msg);
    isReady = false;
});

client.on('disconnected', (reason) => {
    console.log('⚠️  WhatsApp disconnected:', reason);
    console.log('📱 Phone may be offline - messages will be queued and retried');
    isReady = false;
});

client.on('change_state', (state) => {
    console.log('📱 WhatsApp state changed:', state);
    if (state !== 'CONNECTED') {
        console.log('⚠️  Phone not in CONNECTED state - will pause processing');
        isReady = false;
    } else {
        console.log('✅ Phone reconnected - resuming processing');
        isReady = true;
    }
});

// ==================== DATABASE QUERIES ====================
async function getPendingMessages() {
    try {
        const result = await pool.request()
            .input('batchSize', sql.Int, BATCH_SIZE)
            .query(`
                SELECT TOP (@batchSize) 
                    Id, PhoneNumber, Message, MessageType, 
                    EntityType, EntityId, RetryCount, ScheduledFor
                FROM WhatsAppMessages
                WHERE Status = 0 -- Pending
                  AND ScheduledFor <= GETUTCDATE()
                ORDER BY ScheduledFor ASC
            `);
        
        return result.recordset;
    } catch (error) {
        console.error('❌ Error fetching pending messages:', error.message);
        return [];
    }
}

async function markAsProcessing(messageId) {
    try {
        await pool.request()
            .input('messageId', sql.UniqueIdentifier, messageId)
            .query(`
                UPDATE WhatsAppMessages
                SET Status = 1, -- Processing
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @messageId
            `);
    } catch (error) {
        console.error('❌ Error marking as processing:', error.message);
    }
}

async function markAsSent(messageId, whatsAppMessageId) {
    try {
        await pool.request()
            .input('messageId', sql.UniqueIdentifier, messageId)
            .input('whatsAppMessageId', sql.NVarChar, whatsAppMessageId)
            .query(`
                UPDATE WhatsAppMessages
                SET Status = 2, -- Sent
                    SentAt = GETUTCDATE(),
                    WhatsAppMessageId = @whatsAppMessageId,
                    ErrorMessage = NULL,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @messageId
            `);
    } catch (error) {
        console.error('❌ Error marking as sent:', error.message);
    }
}

async function markAsFailed(messageId, errorMessage) {
    try {
        await pool.request()
            .input('messageId', sql.UniqueIdentifier, messageId)
            .input('errorMessage', sql.NVarChar, errorMessage.substring(0, 1000))
            .query(`
                UPDATE WhatsAppMessages
                SET Status = 3, -- Failed
                    FailedAt = GETUTCDATE(),
                    ErrorMessage = @errorMessage,
                    RetryCount = RetryCount + 1,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @messageId
            `);
    } catch (error) {
        console.error('❌ Error marking as failed:', error.message);
    }
}

// ==================== MESSAGE SENDING LOGIC ====================
async function sendWhatsAppMessage(message) {
    const { Id, PhoneNumber, Message, RetryCount } = message;

    try {
        // Mark as processing
        await markAsProcessing(Id);

        // Check if WhatsApp is actually connected before attempting send
        if (!isReady) {
            throw new Error('WhatsApp client not ready - phone may be disconnected');
        }

        // Check if client is still authenticated
        const state = await client.getState();
        if (state !== 'CONNECTED') {
            throw new Error(`WhatsApp state is ${state} (not CONNECTED)`);
        }

        // Format phone number for WhatsApp (phone@c.us)
        const chatId = PhoneNumber + '@c.us';

        // Send message with timeout
        const sentMessage = await Promise.race([
            client.sendMessage(chatId, Message),
            new Promise((_, reject) => 
                setTimeout(() => reject(new Error('Send timeout - phone may be offline')), 30000)
            )
        ]);

        // Mark as sent with WhatsApp message ID
        await markAsSent(Id, sentMessage.id.id);

        console.log(`✅ Message sent to ${PhoneNumber} (ID: ${Id})`);
        return true;

    } catch (error) {
        console.error(`❌ Failed to send to ${PhoneNumber}:`, error.message);

        // Categorize error for better retry logic
        let errorMessage = error.message;
        let shouldRetry = true;
        
        // Phone offline (WiFi dead but phone on)
        if (error.message.includes('Phone not connected') || 
            error.message.includes('phone is not connected') ||
            error.message.includes('not ready')) {
            errorMessage = 'PHONE_OFFLINE: ' + error.message;
            console.log(`⚠️  Phone offline (WiFi dead) - will retry (attempt ${RetryCount + 1}/5)`);
            shouldRetry = true;
        } 
        // Phone dead/rebooting (session closed)
        else if (error.message.includes('Evaluation failed') || 
                 error.message.includes('Session closed') ||
                 error.message.includes('Protocol error') ||
                 error.message.includes('Target closed') ||
                 error.message.includes('browser has disconnected') ||
                 error.message.includes('state is')) {
            errorMessage = 'PHONE_DEAD: ' + error.message;
            console.log(`⚠️  Phone dead/rebooting - will retry (attempt ${RetryCount + 1}/5)`);
            shouldRetry = true;
        }
        // Send timeout (likely phone offline)
        else if (error.message.includes('timeout')) {
            errorMessage = 'TIMEOUT: ' + error.message;
            console.log(`⚠️  Send timeout - phone may be offline - will retry (attempt ${RetryCount + 1}/5)`);
            shouldRetry = true;
        }
        // Invalid WhatsApp number (permanent failure)
        else if (error.message.includes('Number not registered') ||
                 error.message.includes('is not a WhatsApp user')) {
            errorMessage = 'INVALID_NUMBER: ' + error.message;
            console.log(`❌ Invalid WhatsApp number: ${PhoneNumber} - will NOT retry`);
            shouldRetry = false;
        }
        // Message too long (permanent failure)
        else if (error.message.includes('too long')) {
            errorMessage = 'MESSAGE_TOO_LONG: ' + error.message;
            console.log(`❌ Message too long - will NOT retry`);
            shouldRetry = false;
        }
        // Unknown error (retry anyway)
        else {
            errorMessage = 'UNKNOWN_ERROR: ' + error.message;
            console.log(`⚠️  Unknown error - will retry (attempt ${RetryCount + 1}/5)`);
            shouldRetry = true;
        }

        // Mark as failed
        await markAsFailed(Id, errorMessage);

        // If permanent failure (invalid number, too long), mark retry count as max
        if (!shouldRetry && RetryCount < 5) {
            await markMaxRetriesReached(Id);
        }

        return false;
    }
}

async function markMaxRetriesReached(messageId) {
    try {
        await pool.request()
            .input('messageId', sql.UniqueIdentifier, messageId)
            .query(`
                UPDATE WhatsAppMessages
                SET RetryCount = 5
                WHERE Id = @messageId
            `);
    } catch (error) {
        console.error('❌ Error marking max retries:', error.message);
    }
}

// ==================== QUEUE PROCESSOR ====================
async function processQueue() {
    if (isProcessing) {
        console.log('⏭️  Skipping - already processing batch');
        return;
    }

    if (!isReady) {
        console.log('⏸️  WhatsApp not ready - waiting for connection (phone may be offline)');
        return;
    }

    // Double-check connection state before processing
    try {
        const state = await client.getState();
        if (state !== 'CONNECTED') {
            console.log(`⏸️  WhatsApp state is ${state} (not CONNECTED) - skipping batch`);
            isReady = false;
            return;
        }
    } catch (error) {
        console.log(`⏸️  Cannot get WhatsApp state: ${error.message} - skipping batch`);
        isReady = false;
        return;
    }

    isProcessing = true;

    try {
        const messages = await getPendingMessages();

        if (messages.length === 0) {
            console.log('✨ No pending messages');
            return;
        }

        console.log(`\n📦 Processing batch: ${messages.length} messages`);

        let successCount = 0;
        let failCount = 0;

        for (const message of messages) {
            // Re-check connection before each message
            try {
                const state = await client.getState();
                if (state !== 'CONNECTED') {
                    console.log(`⚠️  Phone disconnected during batch - stopping`);
                    isReady = false;
                    break;
                }
            } catch (stateError) {
                console.log(`⚠️  Cannot check phone state - stopping batch`);
                isReady = false;
                break;
            }

            const success = await sendWhatsAppMessage(message);
            
            if (success) {
                successCount++;
            } else {
                failCount++;
            }

            // Delay between messages to avoid spam detection
            await delay(2000);
        }

        console.log(`📊 Batch complete: ✅ ${successCount} sent, ❌ ${failCount} failed\n`);

    } catch (error) {
        console.error('❌ Error processing queue:', error.message);
        console.error('Stack trace:', error.stack);
        
        // If critical error, mark worker as not ready
        if (error.message.includes('Protocol error') || 
            error.message.includes('Session closed') ||
            error.message.includes('Target closed')) {
            console.log('⚠️  Critical error detected - marking worker as not ready');
            isReady = false;
        }
    } finally {
        isProcessing = false;
    }
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

// ==================== STARTUP ====================
async function start() {
    console.log('\n🚀 WhatsApp Queue Worker Starting...\n');

    // Connect to database
    const dbConnected = await connectDatabase();
    if (!dbConnected) {
        console.error('❌ Failed to connect to database. Exiting...');
        process.exit(1);
    }

    // Initialize WhatsApp client
    client.initialize();

    // Wait for WhatsApp to be ready
    await waitForWhatsApp();

    // Start polling queue
    console.log(`\n🔄 Starting queue processor (polling every ${POLL_INTERVAL}ms)`);
    console.log(`📦 Batch size: ${BATCH_SIZE} messages\n`);

    setInterval(async () => {
        await processQueue();
    }, POLL_INTERVAL);

    // Initial run
    await processQueue();
}

function waitForWhatsApp() {
    return new Promise((resolve) => {
        if (isReady) {
            resolve();
        } else {
            client.once('ready', () => {
                resolve();
            });
        }
    });
}

// ==================== GRACEFUL SHUTDOWN ====================
process.on('SIGINT', async () => {
    console.log('\n\n⚠️  Shutting down gracefully...');
    
    if (pool) {
        await pool.close();
        console.log('✅ Database connection closed');
    }
    
    await client.destroy();
    console.log('✅ WhatsApp client disconnected');
    
    console.log('👋 Goodbye!\n');
    process.exit(0);
});

// Start the worker
start().catch(error => {
    console.error('❌ Fatal error:', error);
    process.exit(1);
});
