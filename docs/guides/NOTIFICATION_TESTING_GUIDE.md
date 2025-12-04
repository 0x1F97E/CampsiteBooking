# Complete Notification System Testing Guide

## Overview
This guide will help you test the complete end-to-end notification flow with **real SMS delivery** to your phone.

---

## Prerequisites

### 1. Twilio Account Setup (FREE Trial)

#### Step 1: Create Twilio Account
1. Go to: https://www.twilio.com/try-twilio
2. Sign up with your email
3. ✅ **FREE $15 trial credit** (no credit card required)
4. Verify your email address

#### Step 2: Verify Your Phone Number
1. Twilio will ask you to verify your phone number
2. Enter your phone number (e.g., `+1234567890` for US, `+4512345678` for Denmark)
3. Enter the verification code sent to your phone
4. ✅ **This phone number can now receive test SMS**

#### Step 3: Get Your Credentials
1. Go to Console Dashboard: https://console.twilio.com/
2. Copy **Account SID** (starts with `AC...`)
3. Click **Show** to reveal **Auth Token**
4. Copy **Auth Token**

#### Step 4: Get a Twilio Phone Number
1. Go to **Phone Numbers** → **Manage** → **Buy a number**
2. Select your country (or use US number for testing)
3. Check **SMS** capability
4. Click **Search** and select a number
5. Click **Buy** (uses trial credit - FREE)
6. Copy the phone number (format: `+1234567890`)

---

### 2. Gmail App Password Setup (FREE)

#### Step 1: Enable 2-Factor Authentication
1. Go to: https://myaccount.google.com/security
2. Click **2-Step Verification** → **Get Started**
3. Follow the prompts to enable 2FA

#### Step 2: Create App Password
1. Go to: https://myaccount.google.com/apppasswords
2. Select **Mail** and **Other (Custom name)**
3. Enter "CampsiteBooking"
4. Click **Generate**
5. Copy the 16-character password (e.g., `abcd efgh ijkl mnop`)
6. Remove spaces: `abcdefghijklmnop`

---

## Configuration Steps

### Step 1: Install Twilio SDK

```bash
cd /Users/pc/CampsiteBooking-1
dotnet add package Twilio
```

### Step 2: Update appsettings.json

Open `appsettings.json` and update these sections:

```json
"SMTP": {
  "Host": "smtp.gmail.com",
  "Port": "587",
  "Username": "your-email@gmail.com",
  "Password": "abcdefghijklmnop",
  "FromEmail": "your-email@gmail.com",
  "FromName": "CampsiteBooking"
},
"Twilio": {
  "AccountSid": "ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "AuthToken": "your_auth_token_here",
  "PhoneNumber": "+1234567890"
}
```

**Replace:**
- `your-email@gmail.com` → Your Gmail address
- `abcdefghijklmnop` → Your Gmail app password (no spaces)
- `ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx` → Your Twilio Account SID
- `your_auth_token_here` → Your Twilio Auth Token
- `+1234567890` → Your Twilio phone number

### Step 3: Update Your User's Phone Number in Database

Run this SQL command to update John Doe's phone number to YOUR phone number:

```sql
UPDATE Users 
SET Phone = '+1234567890'  -- Replace with YOUR phone number
WHERE Email = 'john.doe@example.com';
```

**Important:** Use international format (e.g., `+1234567890` for US, `+4512345678` for Denmark)

### Step 4: Update Communication Preference to SMS

```sql
UPDATE Users 
SET PreferredCommunication = 'SMS'  -- or 'Both' for email + SMS
WHERE Email = 'john.doe@example.com';
```

---

## Testing Steps

### Step 1: Start Kafka

```bash
cd /Users/pc/CampsiteBooking-1
docker-compose up -d kafka zookeeper
```

**Verify Kafka is running:**
```bash
docker-compose ps
```

You should see:
```
NAME                    STATUS
bookmyhome-kafka        Up
bookmyhome-zookeeper    Up
```

### Step 2: Start the Application

```bash
dotnet run
```

**Look for these logs:**
```
info: CampsiteBooking.Infrastructure.Kafka.KafkaConsumer[0]
      Kafka consumer subscribed to topic: bookmyhome.events
```

### Step 3: Login as John Doe

1. Go to: http://localhost:5063/login
2. Login with:
   - Email: `john.doe@example.com`
   - Password: `Password123!`

### Step 4: Create a Test Booking

1. Go to: http://localhost:5063/search
2. Select filters and search for accommodations
3. Click **Book Now** on any accommodation
4. Fill out the booking form (guest info should auto-populate)
5. Click **Submit Booking**

### Step 5: Check Your Phone! 📱

You should receive an SMS within 5-10 seconds:

```
Hi John Doe, your booking at Copenhagen Beach Camp is confirmed! 
Check-in: Jan 15, Check-out: Jan 20. See you soon!
```

### Step 6: Verify Logs

Check the application logs for:

```
✅ Created booking with ID: 6
✅ Published BookingCreatedEvent to Kafka for booking 6
info: CampsiteBooking.Infrastructure.Kafka.KafkaConsumer[0]
      Received event: BookingCreatedEvent
info: CampsiteBooking.Infrastructure.EventHandlers.NotificationEventHandler[0]
      Sending booking created notification to John Doe via SMS
✅ SMS sent successfully to +1234567890 - SID: SMxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

---

## Troubleshooting

### Issue: No SMS Received

**Check 1: Twilio Trial Restrictions**
- Trial accounts can only send SMS to **verified phone numbers**
- Verify your phone number at: https://console.twilio.com/us1/develop/phone-numbers/manage/verified

**Check 2: Phone Number Format**
- Must use international format: `+1234567890` (not `1234567890`)
- Include country code: `+1` for US, `+45` for Denmark

**Check 3: Twilio Logs**
- Go to: https://console.twilio.com/us1/monitor/logs/sms
- Check for error messages

**Check 4: Application Logs**
- Look for errors in the console output
- Check if Kafka consumer is running

### Issue: Kafka Not Running

```bash
# Check Kafka status
docker-compose ps

# View Kafka logs
docker-compose logs kafka

# Restart Kafka
docker-compose restart kafka
```

### Issue: Email Not Sent

**Check 1: Gmail App Password**
- Make sure you copied the password correctly (no spaces)
- Try generating a new app password

**Check 2: Gmail Security**
- Check if Gmail blocked the login attempt
- Go to: https://myaccount.google.com/notifications

---

## Cost Breakdown

### Twilio Trial (FREE)
- ✅ **$15 trial credit**
- SMS cost: ~$0.0075 per message (US)
- **~2000 SMS messages** with trial credit
- No credit card required

### Gmail SMTP (FREE)
- ✅ **Completely free**
- No limits for personal use

### Kafka (FREE)
- ✅ **Running locally via Docker**
- No cost

---

## Next Steps

After successful testing:

1. **Upgrade Twilio** (optional):
   - Add credit card to remove trial restrictions
   - Send SMS to any phone number (not just verified)

2. **Production Deployment**:
   - Use environment variables for secrets (not appsettings.json)
   - Set up Kafka cluster (AWS MSK, Confluent Cloud, etc.)
   - Configure email domain (not Gmail)

3. **Monitoring**:
   - Set up Twilio webhooks for delivery status
   - Monitor Kafka consumer lag
   - Track notification success/failure rates

