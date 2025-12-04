-- Update John Doe's phone number and communication preference for SMS testing
-- Replace +1234567890 with YOUR actual phone number in international format

USE CampsiteBookingDb;

-- Step 1: Check current user data
SELECT 
    Id,
    Email,
    FirstName,
    LastName,
    Phone,
    PreferredCommunication
FROM Users
WHERE Email = 'john.doe@example.com';

-- Step 2: Update phone number to YOUR phone number
-- IMPORTANT: Use international format (e.g., +1234567890 for US, +4512345678 for Denmark)
UPDATE Users 
SET Phone = '+1234567890'  -- ⚠️ REPLACE THIS WITH YOUR PHONE NUMBER
WHERE Email = 'john.doe@example.com';

-- Step 3: Set communication preference to SMS (or 'Both' for email + SMS)
UPDATE Users 
SET PreferredCommunication = 'SMS'  -- Options: 'Email', 'SMS', 'Both'
WHERE Email = 'john.doe@example.com';

-- Step 4: Verify the changes
SELECT 
    Id,
    Email,
    FirstName,
    LastName,
    Phone,
    PreferredCommunication
FROM Users
WHERE Email = 'john.doe@example.com';

-- Expected result:
-- Id: 1
-- Email: john.doe@example.com
-- FirstName: John
-- LastName: Doe
-- Phone: +1234567890 (YOUR phone number)
-- PreferredCommunication: SMS

