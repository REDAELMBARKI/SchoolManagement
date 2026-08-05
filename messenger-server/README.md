# WhatsApp Queue Worker

Queue-based WhatsApp messaging service for School Management System.

## Architecture

```
.NET Backend → Writes to WhatsAppMessages table
Node.js Worker → Polls queue every 10s → Sends via WhatsApp → Updates status
```

## Features

✅ **Queue-based messaging** - Messages persist even if phone is offline  
✅ **Automatic retry** - Failed messages retry up to 5 times  
✅ **Phone offline handling** - Queues messages when phone is disconnected  
✅ **Message logging** - Full history in database  
✅ **Batch processing** - Sends 50 messages per batch with 2s delay  

## Setup

### 1. Install Dependencies
```bash
npm install
```

### 2. Configure Environment
Edit `.env` file:
```env
DB_SERVER=localhost
DB_NAME=SchoolManagementDB
DB_USER=sa
DB_PASSWORD=YourPassword123
```

### 3. Run Worker
```bash
npm start
```

### 4. Scan QR Code
- QR code appears in terminal
- Open WhatsApp on your phone
- Go to: **Settings → Linked Devices → Link a Device**
- Scan the QR code

## Database Schema

```sql
CREATE TABLE WhatsAppMessages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    BranchId UNIQUEIDENTIFIER NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    Message NVARCHAR(4000) NOT NULL,
    Status INT NOT NULL, -- 0=Pending, 1=Processing, 2=Sent, 3=Failed
    MessageType INT NOT NULL,
    EntityType NVARCHAR(50) NULL,
    EntityId UNIQUEIDENTIFIER NULL,
    RetryCount INT NOT NULL DEFAULT 0,
    ScheduledFor DATETIME2 NOT NULL,
    SentAt DATETIME2 NULL,
    FailedAt DATETIME2 NULL,
    ErrorMessage NVARCHAR(1000) NULL,
    WhatsAppMessageId NVARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL
);
```

## Message Status Flow

```
Pending (0) → Processing (1) → Sent (2)
                            ↓
                        Failed (3) → Retry → Pending (0)
                                  (max 5 retries)
```

## Error Handling

| Error | Status | Retry? |
|-------|--------|--------|
| Phone offline (WiFi dead) | Failed → Pending | ✅ Yes (up to 5x) |
| Phone dead/rebooting | Failed → Pending | ✅ Yes (up to 5x) |
| Invalid WhatsApp number | Failed | ❌ No (permanent) |
| Message too long | Failed | ❌ No (permanent) |

## Usage from .NET

```csharp
// Create a message (saves to database)
var message = WhatsAppMessage.Create(
    branchId: currentBranchId,
    phoneNumber: "0612345678", // or "+212612345678"
    message: "Hello! Your invoice is ready.",
    messageType: WhatsAppMessageType.InvoiceIssued,
    entityType: "Invoice",
    entityId: invoiceId,
    scheduledFor: DateTime.UtcNow // or schedule for later
);

await _whatsAppRepository.AddAsync(message);

// Worker automatically picks it up within 10 seconds
```

## Configuration

| Variable | Default | Description |
|----------|---------|-------------|
| `POLL_INTERVAL_MS` | 10000 | Queue poll interval (10s) |
| `BATCH_SIZE` | 50 | Messages per batch |
| `SESSION_PATH` | ./whatsapp-session | WhatsApp auth data |

## Monitoring

Check worker logs:
```bash
npm start

# Output:
✅ Connected to SQL Server database
✅ WhatsApp client is ready!
📦 Processing batch: 15 messages
✅ Message sent to 212612345678 (ID: abc-123)
📊 Batch complete: ✅ 15 sent, ❌ 0 failed
```

## Troubleshooting

**Phone shows "not connected"**  
→ Messages stay in queue, worker retries automatically

**QR code expired**  
→ Restart worker: `npm start`

**Database connection failed**  
→ Check SQL Server credentials in `.env`

**Messages not sending**  
→ Check `WhatsAppMessages` table for `ErrorMessage` column
