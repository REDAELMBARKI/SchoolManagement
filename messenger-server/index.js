const { Client, LocalAuth } = require('whatsapp-web.js');
const qrcode = require('qrcode-terminal');
const express = require('express');
require('dotenv').config();

const app = express();
app.use(express.json());

// WhatsApp Client with session persistence
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

let isReady = false;
let qrCodeData = null;

// QR Code generation event
client.on('qr', (qr) => {
    console.log('\n📱 WhatsApp QR Code Generated!');
    console.log('Scan this QR code with your WhatsApp mobile app:\n');
    qrcode.generate(qr, { small: true });
    qrCodeData = qr;
    console.log('\n✨ Or visit: http://localhost:' + (process.env.PORT || 3001) + '/qr to see QR code\n');
});

// Client ready event
client.on('ready', () => {
    console.log('✅ WhatsApp client is ready!');
    isReady = true;
    qrCodeData = null;
});

// Authentication success
client.on('authenticated', () => {
    console.log('✅ WhatsApp authenticated successfully!');
});

// Authentication failure
client.on('auth_failure', (msg) => {
    console.error('❌ Authentication failed:', msg);
    isReady = false;
});

// Client disconnected
client.on('disconnected', (reason) => {
    console.log('⚠️  WhatsApp client disconnected:', reason);
    isReady = false;
    qrCodeData = null;
});

// Message received event
client.on('message', async (message) => {
    console.log('📨 Message received:', message.from, '-', message.body);
    
    // Auto-reply example (optional)
    // if (message.body.toLowerCase() === 'ping') {
    //     message.reply('pong');
    // }
});

// Initialize WhatsApp client
client.initialize();

// ==================== REST API ENDPOINTS ====================

// Health check
app.get('/health', (req, res) => {
    res.json({
        status: 'running',
        whatsappReady: isReady,
        timestamp: new Date().toISOString()
    });
});

// Get QR Code (for web display)
app.get('/qr', (req, res) => {
    if (qrCodeData) {
        res.send(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>WhatsApp QR Code</title>
                <style>
                    body { 
                        font-family: Arial; 
                        text-align: center; 
                        padding: 50px;
                        background: #f0f0f0;
                    }
                    .container {
                        background: white;
                        padding: 30px;
                        border-radius: 10px;
                        display: inline-block;
                        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                    }
                    h1 { color: #25D366; }
                    img { margin: 20px 0; }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>📱 WhatsApp QR Code</h1>
                    <p>Scan this QR code with WhatsApp on your phone</p>
                    <p><strong>Open WhatsApp → Settings → Linked Devices → Link a Device</strong></p>
                    <img src="https://api.qrserver.com/v1/create-qr-code/?size=300x300&data=${encodeURIComponent(qrCodeData)}" alt="QR Code" />
                    <p><small>Refresh this page if QR code expires</small></p>
                </div>
            </body>
            </html>
        `);
    } else if (isReady) {
        res.send(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>WhatsApp Connected</title>
                <style>
                    body { 
                        font-family: Arial; 
                        text-align: center; 
                        padding: 50px;
                        background: #f0f0f0;
                    }
                    .container {
                        background: white;
                        padding: 30px;
                        border-radius: 10px;
                        display: inline-block;
                        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                    }
                    h1 { color: #25D366; }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>✅ WhatsApp Connected!</h1>
                    <p>Your WhatsApp is ready to send messages</p>
                </div>
            </body>
            </html>
        `);
    } else {
        res.send(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>WhatsApp Status</title>
                <meta http-equiv="refresh" content="5">
                <style>
                    body { 
                        font-family: Arial; 
                        text-align: center; 
                        padding: 50px;
                        background: #f0f0f0;
                    }
                    .container {
                        background: white;
                        padding: 30px;
                        border-radius: 10px;
                        display: inline-block;
                        box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                    }
                    h1 { color: #FFA500; }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>⏳ Initializing WhatsApp...</h1>
                    <p>Please wait while we generate the QR code</p>
                    <p><small>This page will auto-refresh</small></p>
                </div>
            </body>
            </html>
        `);
    }
});

// Send message to a single number
app.post('/send', async (req, res) => {
    try {
        if (!isReady) {
            return res.status(503).json({ 
                success: false, 
                error: 'WhatsApp client not ready. Please scan QR code first.' 
            });
        }

        const { phone, message } = req.body;

        if (!phone || !message) {
            return res.status(400).json({ 
                success: false, 
                error: 'Phone number and message are required' 
            });
        }

        // Format phone number (remove spaces, dashes, add country code if needed)
        let formattedPhone = phone.replace(/[^\d]/g, '');
        
        // If doesn't start with country code, assume Morocco (+212)
        if (!formattedPhone.startsWith('212') && !formattedPhone.startsWith('1')) {
            formattedPhone = '212' + formattedPhone;
        }

        const chatId = formattedPhone + '@c.us';

        // Send message
        await client.sendMessage(chatId, message);

        console.log(`✅ Message sent to ${phone}: ${message}`);

        res.json({
            success: true,
            phone: formattedPhone,
            message: 'Message sent successfully',
            timestamp: new Date().toISOString()
        });

    } catch (error) {
        console.error('❌ Error sending message:', error);
        res.status(500).json({ 
            success: false, 
            error: error.message 
        });
    }
});

// Send bulk messages
app.post('/send-bulk', async (req, res) => {
    try {
        if (!isReady) {
            return res.status(503).json({ 
                success: false, 
                error: 'WhatsApp client not ready. Please scan QR code first.' 
            });
        }

        const { recipients, message } = req.body;

        if (!recipients || !Array.isArray(recipients) || recipients.length === 0) {
            return res.status(400).json({ 
                success: false, 
                error: 'Recipients array is required' 
            });
        }

        if (!message) {
            return res.status(400).json({ 
                success: false, 
                error: 'Message is required' 
            });
        }

        const results = [];

        for (const phone of recipients) {
            try {
                let formattedPhone = phone.replace(/[^\d]/g, '');
                
                if (!formattedPhone.startsWith('212') && !formattedPhone.startsWith('1')) {
                    formattedPhone = '212' + formattedPhone;
                }

                const chatId = formattedPhone + '@c.us';
                await client.sendMessage(chatId, message);

                results.push({
                    phone: formattedPhone,
                    success: true
                });

                console.log(`✅ Bulk message sent to ${phone}`);

                // Delay between messages to avoid spam detection
                await new Promise(resolve => setTimeout(resolve, 2000));

            } catch (error) {
                results.push({
                    phone: phone,
                    success: false,
                    error: error.message
                });
                console.error(`❌ Failed to send to ${phone}:`, error.message);
            }
        }

        const successCount = results.filter(r => r.success).length;
        const failedCount = results.filter(r => !r.success).length;

        res.json({
            success: true,
            total: recipients.length,
            sent: successCount,
            failed: failedCount,
            results: results,
            timestamp: new Date().toISOString()
        });

    } catch (error) {
        console.error('❌ Error in bulk send:', error);
        res.status(500).json({ 
            success: false, 
            error: error.message 
        });
    }
});

// Get connection status
app.get('/status', async (req, res) => {
    try {
        if (!isReady) {
            return res.json({
                connected: false,
                message: 'WhatsApp not connected. Scan QR code at /qr'
            });
        }

        const info = client.info;
        res.json({
            connected: true,
            phone: info.wid.user,
            name: info.pushname,
            platform: info.platform,
            message: 'WhatsApp connected and ready'
        });
    } catch (error) {
        res.status(500).json({
            connected: false,
            error: error.message
        });
    }
});

// Logout/disconnect
app.post('/logout', async (req, res) => {
    try {
        await client.logout();
        isReady = false;
        qrCodeData = null;
        res.json({
            success: true,
            message: 'Logged out successfully'
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            error: error.message
        });
    }
});

// Start Express server
const PORT = process.env.PORT || 3001;
app.listen(PORT, () => {
    console.log('\n🚀 WhatsApp Messenger Server Started!');
    console.log(`📡 Server running on: http://localhost:${PORT}`);
    console.log(`📱 View QR Code at: http://localhost:${PORT}/qr`);
    console.log(`🔍 Health Check: http://localhost:${PORT}/health`);
    console.log(`📊 Status Check: http://localhost:${PORT}/status`);
    console.log('\n⏳ Initializing WhatsApp client...\n');
});
