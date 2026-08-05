# WhatsApp Integration Setup

## Architecture

```
┌─────────────────┐         ┌──────────────────┐         ┌─────────────────┐
│  .NET Backend   │         │  SQL Server      │         │  Node.js Worker │
│                 │         │                  │         │                 │
│  Creates        │────────>│  WhatsAppMessages│<────────│  Polls Queue    │
│  Messages       │  INSERT │  Table (Queue)   │  SELECT │  Every 10s      │
└─────────────────┘         └──────────────────┘         └─────────────────┘
                                                                    │
                                                                    ▼
                                                          ┌─────────────────┐
                                                          │  WhatsApp Web   │
                                                          │  (via phone)    │
                                                          └─────────────────┘
```

## Setup Steps

### 1. Run Database Migration

```bash
cd c:\SchoolManagement\Backend
dotnet ef migrations add AddWhatsAppMessages
dotnet ef database update
```

### 2. Configure Node.js Worker

Edit `c:\SchoolManagement\messenger-server\.env`:

```env
DB_SERVER=localhost
DB_NAME=SchoolManagementDB
DB_USER=sa
DB_PASSWORD=YourActualPassword
```

### 3. Install Node.js Dependencies

```bash
cd c:\SchoolManagement\messenger-server
npm install
```

### 4. Start WhatsApp Worker

```bash
cd c:\SchoolManagement\messenger-server
npm start
```

**Scan QR Code:**
- QR code appears in terminal
- Open WhatsApp on phone: **Settings → Linked Devices → Link a Device**
- Scan the QR code
- Worker starts processing queue automatically

### 5. Test from .NET Backend

```csharp
// Example: Queue a message from any service
await _whatsAppService.QueueMessageAsync(
    phoneNumber: "0612345678",
    message: "Hello! Your invoice is ready.",
    messageType: WhatsAppMessageType.InvoiceIssued,
    entityType: "Invoice",
    entityId: invoiceId
);
```

## Usage Examples

### Send Invoice Notification

```csharp
var message = $"Hello {student.FirstName},\n\n" +
             $"Your invoice #{invoiceNumber} for {amount:N2} MAD is ready.\n" +
             $"Due date: {dueDate:yyyy-MM-dd}\n\n" +
             $"Thank you!";

await _whatsAppService.QueueMessageAsync(
    phoneNumber: student.Phone,
    message: message,
    messageType: WhatsAppMessageType.InvoiceIssued,
    entityType: "Invoice",
    entityId: invoiceId
);
```

### Send Payment Receipt

```csharp
var message = $"Payment received: {amount:N2} MAD\n" +
             $"Receipt #{receiptNumber}\n" +
             $"Thank you for your payment!";

await _whatsAppService.QueueMessageAsync(
    phoneNumber: student.Phone,
    message: message,
    messageType: WhatsAppMessageType.PaymentReceipt,
    entityType: "Payment",
    entityId: paymentId
);
```

### Bulk Send to All Students

```csharp
var phoneNumbers = students.Select(s => s.Phone).ToList();

await _whatsAppService.QueueBulkMessagesAsync(
    phoneNumbers: phoneNumbers,
    message: "School will be closed tomorrow due to holiday.",
    messageType: WhatsAppMessageType.GeneralNotification
);
```

### Schedule Message for Later

```csharp
await _whatsAppService.QueueMessageAsync(
    phoneNumber: student.Phone,
    message: "Reminder: Class starts in 1 hour",
    messageType: WhatsAppMessageType.GeneralNotification,
    scheduledFor: DateTime.UtcNow.AddHours(1)
);
```

## Message Status Flow

```
┌──────────┐
│ Pending  │ Status = 0, waiting in queue
└────┬─────┘
     │
     ▼
┌──────────────┐
│ Processing   │ Status = 1, Node.js worker picked it up
└──────┬───────┘
       │
       ├─────────────────┐
       │                 │
       ▼                 ▼
┌──────────┐      ┌──────────┐
│   Sent   │      │  Failed  │ Status = 3
│          │      │          │ (retries up to 5 times)
└──────────┘      └────┬─────┘
Status = 2             │
                       ▼
                ┌──────────────┐
                │ Retry → Pending
                └──────────────┘
```

## Error Handling

| Scenario | Worker Behavior | Status |
|----------|----------------|--------|
| Phone offline (WiFi dead) | Marks as Failed → Auto-retry | Failed → Pending |
| Phone dead/rebooting | Marks as Failed → Auto-retry | Failed → Pending |
| Invalid WhatsApp number | Marks as Failed → No retry | Failed (permanent) |
| Message too long (>4096) | Marks as Failed → No retry | Failed (permanent) |
| 5 retries exhausted | Stays Failed | Failed (permanent) |

## Monitoring

### Check Queue Status (SQL)

```sql
-- Pending messages
SELECT COUNT(*) FROM WhatsAppMessages WHERE Status = 0;

-- Failed messages
SELECT * FROM WhatsAppMessages 
WHERE Status = 3 
ORDER BY FailedAt DESC;

-- Sent today
SELECT COUNT(*) FROM WhatsAppMessages 
WHERE Status = 2 AND CAST(SentAt AS DATE) = CAST(GETDATE() AS DATE);
```

### Check Worker Logs

```bash
cd c:\SchoolManagement\messenger-server
npm start

# Output:
✅ Connected to SQL Server database
✅ WhatsApp client is ready!
📦 Processing batch: 15 messages
✅ Message sent to 212612345678
📊 Batch complete: ✅ 15 sent, ❌ 0 failed
```

## API Endpoints

```
POST   /api/whatsapp/send              - Queue single message
POST   /api/whatsapp/send-bulk         - Queue bulk messages
GET    /api/whatsapp/status/{id}       - Get message status
GET    /api/whatsapp/entity/{type}/{id} - Get all messages for entity
POST   /api/whatsapp/retry/{id}        - Retry failed message
```

## Troubleshooting

**QR Code not appearing?**
- Check terminal output
- Restart: `npm start`

**Messages not sending?**
- Check if worker is running: `npm start`
- Check WhatsApp connection: Phone should show "Linked Devices"
- Check database: `SELECT * FROM WhatsAppMessages WHERE Status = 3`

**Phone shows "not connected"?**
- Messages stay in queue automatically
- Worker retries every 10 seconds
- Once phone reconnects, messages send automatically

**Database connection failed?**
- Verify credentials in `.env`
- Check SQL Server is running
- Test connection: `sqlcmd -S localhost -U sa -P YourPassword`

## Production Deployment

1. **Run worker as Windows Service** (using `node-windows`) or Linux systemd
2. **Monitor queue size** - alert if > 1000 pending messages
3. **Clean old messages** - DELETE sent messages older than 90 days
4. **Backup session** - Copy `whatsapp-session/` folder for disaster recovery
5. **Rate limiting** - Current: 50 messages/batch, 2s delay between messages
