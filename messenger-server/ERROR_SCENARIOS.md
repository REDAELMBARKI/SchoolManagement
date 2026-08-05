# WhatsApp Error Scenarios & Handling

## Error Categories

### 1. PHONE_OFFLINE (WiFi Dead, Phone On)
**Detection:**
- Error contains: "Phone not connected", "phone is not connected", "not ready"
- `client.getState()` returns anything other than "CONNECTED"

**Behavior:**
- WhatsApp Web shows: "Phone not connected" banner
- `client.sendMessage()` throws error immediately
- Worker marks message as FAILED
- Retry scheduled automatically (up to 5 attempts)

**Example:**
```javascript
Error: Phone not connected to web
Status: Failed → Pending (retry)
```

### 2. PHONE_DEAD (Phone Off/Rebooting)
**Detection:**
- Error contains: "Evaluation failed", "Session closed", "Protocol error", "Target closed", "browser has disconnected"
- Connection state changes unexpectedly

**Behavior:**
- WhatsApp Web shows: "Connecting..." spinner or crashes
- `client.sendMessage()` may hang then throw error
- Worker marks message as FAILED
- Retry scheduled automatically (up to 5 attempts)

**Example:**
```javascript
Error: Evaluation failed: Session closed. Most likely the page has been closed.
Status: Failed → Pending (retry)
```

### 3. TIMEOUT (Send Hangs)
**Detection:**
- Send operation exceeds 30 seconds
- Timeout wrapper around `client.sendMessage()`

**Behavior:**
- Phone likely offline or network issue
- Worker cancels operation after 30s
- Marks as FAILED
- Retry scheduled automatically

**Example:**
```javascript
Error: Send timeout - phone may be offline
Status: Failed → Pending (retry)
```

### 4. INVALID_NUMBER (Permanent Failure)
**Detection:**
- Error contains: "Number not registered", "is not a WhatsApp user"

**Behavior:**
- Number is not on WhatsApp
- Worker marks as FAILED
- **NO RETRY** - Sets RetryCount = 5 immediately
- Stays in Failed status permanently

**Example:**
```javascript
Error: Number not registered on WhatsApp
Status: Failed (permanent - no retry)
```

### 5. MESSAGE_TOO_LONG (Permanent Failure)
**Detection:**
- Error contains: "too long"
- Message exceeds 4096 characters

**Behavior:**
- WhatsApp message limit exceeded
- Worker marks as FAILED
- **NO RETRY** - Sets RetryCount = 5 immediately

**Example:**
```javascript
Error: Message too long
Status: Failed (permanent - no retry)
```

### 6. UNKNOWN_ERROR (Retry Anyway)
**Detection:**
- Any other error not matching above patterns

**Behavior:**
- Unknown issue occurred
- Worker marks as FAILED
- Retry scheduled automatically (up to 5 attempts)

**Example:**
```javascript
Error: [Some unexpected error]
Status: Failed → Pending (retry)
```

## Error Flow Chart

```
┌─────────────────┐
│ Send Attempt    │
└────────┬────────┘
         │
         ├──────────────────────────────────┐
         │                                  │
    ✅ SUCCESS                         ❌ ERROR
         │                                  │
         ▼                                  ▼
┌─────────────────┐              ┌──────────────────┐
│ Mark as SENT    │              │ Check Error Type │
│ Store MessageID │              └────────┬─────────┘
└─────────────────┘                       │
                                          ├─────────────────┐
                                          │                 │
                                    RETRYABLE         PERMANENT
                                          │                 │
                                          ▼                 ▼
                                ┌─────────────────┐  ┌─────────────┐
                                │ Mark as FAILED  │  │ Mark Failed │
                                │ RetryCount++    │  │ RetryCount=5│
                                └────────┬────────┘  └─────────────┘
                                         │                 │
                                         ▼                 │
                                ┌─────────────────┐        │
                                │ RetryCount < 5? │        │
                                └────────┬────────┘        │
                                         │                 │
                                    YES  │  NO             │
                                         │  │              │
                                         ▼  ▼              │
                                ┌─────────────────┐        │
                                │ Back to Pending │        │
                                │ (Auto Retry)    │        │
                                └─────────────────┘        │
                                                           │
                                                           ▼
                                                   ┌──────────────┐
                                                   │ Failed       │
                                                   │ (Permanent)  │
                                                   └──────────────┘
```

## Code Implementation

### Pre-Send Checks
```javascript
// Check if WhatsApp is ready
if (!isReady) {
    throw new Error('WhatsApp client not ready');
}

// Check connection state
const state = await client.getState();
if (state !== 'CONNECTED') {
    throw new Error(`WhatsApp state is ${state}`);
}
```

### Send with Timeout
```javascript
const sentMessage = await Promise.race([
    client.sendMessage(chatId, Message),
    new Promise((_, reject) => 
        setTimeout(() => reject(new Error('Send timeout')), 30000)
    )
]);
```

### Error Categorization
```javascript
catch (error) {
    let errorMessage = error.message;
    let shouldRetry = true;
    
    if (error.message.includes('Phone not connected')) {
        errorMessage = 'PHONE_OFFLINE: ' + error.message;
        shouldRetry = true;
    } else if (error.message.includes('Session closed')) {
        errorMessage = 'PHONE_DEAD: ' + error.message;
        shouldRetry = true;
    } else if (error.message.includes('Number not registered')) {
        errorMessage = 'INVALID_NUMBER: ' + error.message;
        shouldRetry = false;
    }
    
    await markAsFailed(Id, errorMessage);
    
    if (!shouldRetry) {
        await markMaxRetriesReached(Id);
    }
}
```

## Testing Scenarios

### Test 1: Phone WiFi Disabled
```
1. Send message queue
2. Disable phone WiFi
3. Worker attempts send
4. Sees error: "Phone not connected"
5. Marks as Failed, schedules retry
6. Enable WiFi
7. Next poll cycle (10s later) picks up message
8. Sends successfully
```

### Test 2: Phone Powered Off
```
1. Send message queue
2. Power off phone
3. Worker attempts send
4. Sees error: "Session closed" or timeout
5. Marks as Failed, schedules retry
6. Power on phone (takes 30s to reconnect)
7. Next poll cycle picks up message
8. Sends successfully
```

### Test 3: Invalid Number
```
1. Send message to invalid number (e.g., 1234567890)
2. Worker attempts send
3. Sees error: "Number not registered"
4. Marks as Failed with RetryCount = 5 (permanent)
5. Message stays Failed, no retry
```

## Monitoring Queries

### Failed Messages by Error Type
```sql
SELECT 
    CASE 
        WHEN ErrorMessage LIKE 'PHONE_OFFLINE%' THEN 'Phone Offline'
        WHEN ErrorMessage LIKE 'PHONE_DEAD%' THEN 'Phone Dead'
        WHEN ErrorMessage LIKE 'INVALID_NUMBER%' THEN 'Invalid Number'
        WHEN ErrorMessage LIKE 'TIMEOUT%' THEN 'Timeout'
        ELSE 'Other'
    END AS ErrorType,
    COUNT(*) AS Count
FROM WhatsAppMessages
WHERE Status = 3 -- Failed
GROUP BY 
    CASE 
        WHEN ErrorMessage LIKE 'PHONE_OFFLINE%' THEN 'Phone Offline'
        WHEN ErrorMessage LIKE 'PHONE_DEAD%' THEN 'Phone Dead'
        WHEN ErrorMessage LIKE 'INVALID_NUMBER%' THEN 'Invalid Number'
        WHEN ErrorMessage LIKE 'TIMEOUT%' THEN 'Timeout'
        ELSE 'Other'
    END
ORDER BY Count DESC;
```

### Messages Stuck in Processing
```sql
-- If a message is in Processing for >5 minutes, something is wrong
SELECT * FROM WhatsAppMessages
WHERE Status = 1 -- Processing
  AND UpdatedAt < DATEADD(MINUTE, -5, GETUTCDATE());
```

### Retry Statistics
```sql
SELECT 
    RetryCount,
    COUNT(*) AS MessageCount,
    AVG(DATEDIFF(SECOND, CreatedAt, COALESCE(SentAt, GETUTCDATE()))) AS AvgTimeToSendSeconds
FROM WhatsAppMessages
WHERE Status IN (2, 3) -- Sent or Failed
GROUP BY RetryCount
ORDER BY RetryCount;
```
