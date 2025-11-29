-- Update John Doe's email address for testing email notifications
-- This will allow you to receive booking confirmation emails in your Gmail inbox

USE CampsiteBookingDb;

-- Step 1: Check current user data
SELECT
    Id,
    Email,
    FirstName,
    LastName,
    Phone
FROM Users
WHERE Email = 'john.doe@example.com';

-- Step 2: Update email address to your Gmail address
UPDATE Users
SET Email = 'campsite.booking.agent@gmail.com'
WHERE Email = 'john.doe@example.com';

-- Step 3: Verify the changes
SELECT
    Id,
    Email,
    FirstName,
    LastName,
    Phone
FROM Users
WHERE Email = 'campsite.booking.agent@gmail.com';

-- Expected result:
-- Id: 1
-- Email: campsite.booking.agent@gmail.com
-- FirstName: John
-- LastName: Doe
-- Phone: +45 12 34 56 78

-- NOTE: PreferredCommunication is not stored in the database.
-- It defaults to "Email" in the Guest model, so email notifications will work.

-- IMPORTANT: After testing, you can restore the original email with:
-- UPDATE Users SET Email = 'john.doe@example.com' WHERE Id = 1;

