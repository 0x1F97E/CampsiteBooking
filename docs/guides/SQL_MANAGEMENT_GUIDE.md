# SQL Management Guide for Campsite Booking System

This guide provides SQL queries to manage data in the Campsite Booking database.

## Database Connection

To connect to your MySQL database, use:
```bash
mysql -u root -p CampsiteBookingDb
```

Or use a GUI tool like MySQL Workbench, phpMyAdmin, or DBeaver.

---

## 1. Managing Bookings

### View All Bookings
```sql
SELECT * FROM Bookings;
```

### View Bookings with Details (Joined with Guests, Campsites, AccommodationTypes)
```sql
SELECT 
    b.Id AS BookingId,
    CONCAT(g.FirstName, ' ', g.LastName) AS GuestName,
    g.Email AS GuestEmail,
    c.Name AS CampsiteName,
    a.Type AS AccommodationType,
    b.StartDate AS CheckIn,
    b.EndDate AS CheckOut,
    b.NumberOfAdults,
    b.NumberOfChildren,
    b.TotalPrice_Amount AS TotalAmount,
    b.Status
FROM Bookings b
LEFT JOIN Guests g ON b.GuestId = g.Id
LEFT JOIN Campsites c ON b.CampsiteId = c.Id
LEFT JOIN AccommodationTypes a ON b.AccommodationTypeId = a.Id
ORDER BY b.CreatedDate DESC;
```

### Delete All Bookings
```sql
DELETE FROM Bookings;
```

### Delete Specific Booking by ID
```sql
DELETE FROM Bookings WHERE Id = 1;
```

### Delete Bookings by Status
```sql
-- Delete all cancelled bookings
DELETE FROM Bookings WHERE Status = 'Cancelled';

-- Delete all pending bookings
DELETE FROM Bookings WHERE Status = 'Pending';
```

### Delete Old Completed Bookings (older than 1 year)
```sql
DELETE FROM Bookings 
WHERE Status = 'Completed' 
AND EndDate < DATE_SUB(NOW(), INTERVAL 1 YEAR);
```

---

## 2. Managing Discounts

### View All Discounts
```sql
SELECT * FROM Discounts;
```

### View Active Discounts Only
```sql
SELECT * FROM Discounts WHERE IsActive = 1;
```

### View Discounts with Usage Statistics
```sql
SELECT 
    Code,
    Description,
    Type,
    Value,
    ValidFrom,
    ValidUntil,
    UsedCount,
    MaxUses,
    CASE 
        WHEN MaxUses = 0 THEN 'Unlimited'
        ELSE CONCAT(ROUND((UsedCount / MaxUses) * 100, 2), '%')
    END AS UsagePercentage,
    IsActive
FROM Discounts
ORDER BY CreatedDate DESC;
```

### Delete All Discounts
```sql
DELETE FROM Discounts;
```

### Delete Specific Discount by Code
```sql
DELETE FROM Discounts WHERE Code = 'SUMMER2024';
```

### Delete Expired Discounts
```sql
DELETE FROM Discounts WHERE ValidUntil < NOW();
```

### Delete Inactive Discounts
```sql
DELETE FROM Discounts WHERE IsActive = 0;
```

### Deactivate Discount (instead of deleting)
```sql
UPDATE Discounts SET IsActive = 0 WHERE Code = 'SUMMER2024';
```

---

## 3. Managing Seasonal Pricing

### View All Seasonal Pricing
```sql
SELECT * FROM SeasonalPricings;
```

### View Seasonal Pricing with Campsite and Accommodation Type Details
```sql
SELECT 
    sp.Id,
    c.Name AS CampsiteName,
    a.Type AS AccommodationType,
    sp.SeasonName,
    sp.StartDate,
    sp.EndDate,
    sp.PriceMultiplier,
    sp.IsActive
FROM SeasonalPricings sp
LEFT JOIN Campsites c ON sp.CampsiteId = c.Id
LEFT JOIN AccommodationTypes a ON sp.AccommodationTypeId = a.Id
ORDER BY sp.StartDate;
```

### Delete All Seasonal Pricing
```sql
DELETE FROM SeasonalPricings;
```

### Delete Seasonal Pricing by Season Name
```sql
DELETE FROM SeasonalPricings WHERE SeasonName = 'High Season';
```

### Delete Inactive Seasonal Pricing
```sql
DELETE FROM SeasonalPricings WHERE IsActive = 0;
```

### Delete Past Seasonal Pricing (ended more than 1 year ago)
```sql
DELETE FROM SeasonalPricings 
WHERE EndDate < DATE_SUB(NOW(), INTERVAL 1 YEAR);
```

---

## 4. Managing Accommodation Types

### View All Accommodation Types
```sql
SELECT * FROM AccommodationTypes;
```

### View Accommodation Types with Campsite Details
```sql
SELECT 
    a.Id,
    c.Name AS CampsiteName,
    a.Type,
    a.Description,
    a.MaxCapacity,
    a.BasePrice_Amount AS PricePerNight,
    a.AvailableUnits,
    a.IsActive
FROM AccommodationTypes a
LEFT JOIN Campsites c ON a.CampsiteId = c.Id
ORDER BY c.Name, a.Type;
```

### Delete All Accommodation Types
```sql
DELETE FROM AccommodationTypes;
```

### Delete Accommodation Types for Specific Campsite
```sql
DELETE FROM AccommodationTypes WHERE CampsiteId = 1;
```

### Delete Inactive Accommodation Types
```sql
DELETE FROM AccommodationTypes WHERE IsActive = 0;
```

---

## 5. Reset Auto-Increment Counters

After deleting all records, you may want to reset the auto-increment counters:

```sql
-- Reset Bookings
ALTER TABLE Bookings AUTO_INCREMENT = 1;

-- Reset Discounts
ALTER TABLE Discounts AUTO_INCREMENT = 1;

-- Reset SeasonalPricings
ALTER TABLE SeasonalPricings AUTO_INCREMENT = 1;

-- Reset AccommodationTypes
ALTER TABLE AccommodationTypes AUTO_INCREMENT = 1;
```

---

## 6. Backup Before Deleting

**IMPORTANT**: Always backup your data before performing DELETE operations!

### Create a Backup
```bash
mysqldump -u root -p CampsiteBookingDb > backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restore from Backup
```bash
mysql -u root -p CampsiteBookingDb < backup_20241124_120000.sql
```

---

## 7. Common Scenarios

### Scenario 1: Clear All Test Data
```sql
-- Delete all bookings
DELETE FROM Bookings;
ALTER TABLE Bookings AUTO_INCREMENT = 1;

-- Delete all discounts
DELETE FROM Discounts;
ALTER TABLE Discounts AUTO_INCREMENT = 1;

-- Delete all seasonal pricing
DELETE FROM SeasonalPricings;
ALTER TABLE SeasonalPricings AUTO_INCREMENT = 1;
```

### Scenario 2: Delete Only Test/Sample Data (keep production data)
```sql
-- Delete bookings created in the last hour (assuming test data was just created)
DELETE FROM Bookings WHERE CreatedDate > DATE_SUB(NOW(), INTERVAL 1 HOUR);

-- Delete discounts with "TEST" in the code
DELETE FROM Discounts WHERE Code LIKE '%TEST%';
```

### Scenario 3: Archive Old Data (move to archive table before deleting)
```sql
-- Create archive table (if not exists)
CREATE TABLE IF NOT EXISTS Bookings_Archive LIKE Bookings;

-- Copy old bookings to archive
INSERT INTO Bookings_Archive 
SELECT * FROM Bookings 
WHERE Status = 'Completed' 
AND EndDate < DATE_SUB(NOW(), INTERVAL 1 YEAR);

-- Delete archived bookings from main table
DELETE FROM Bookings 
WHERE Status = 'Completed' 
AND EndDate < DATE_SUB(NOW(), INTERVAL 1 YEAR);
```

---

## 8. Safety Tips

1. **Always use WHERE clause**: Never run `DELETE FROM table_name` without a WHERE clause unless you really want to delete ALL records
2. **Test with SELECT first**: Before running DELETE, run SELECT with the same WHERE clause to see what will be deleted
3. **Use transactions**: Wrap DELETE operations in transactions so you can rollback if needed
4. **Backup regularly**: Always have a recent backup before performing bulk deletions

### Example: Using Transactions
```sql
START TRANSACTION;

-- Perform your DELETE operations
DELETE FROM Bookings WHERE Status = 'Cancelled';

-- Check the results
SELECT COUNT(*) FROM Bookings;

-- If everything looks good, commit
COMMIT;

-- If something went wrong, rollback
-- ROLLBACK;
```

---

## Need Help?

If you encounter any issues or need to perform more complex operations, consult the MySQL documentation or ask for assistance.

