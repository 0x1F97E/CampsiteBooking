-- Migration: Add missing columns to Discounts table
-- The initial migration only created Id and DiscountId columns
-- This adds all the other columns needed for the Discount entity

-- Add Code column (required, unique)
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS Code VARCHAR(20) NOT NULL DEFAULT '';

-- Add Description column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS Description VARCHAR(500) NULL;

-- Add Type column (required: 'Percentage' or 'Fixed')
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS Type VARCHAR(20) NOT NULL DEFAULT 'Percentage';

-- Add Value column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS Value DECIMAL(10, 2) NOT NULL DEFAULT 0;

-- Add ValidFrom column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS ValidFrom DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

-- Add ValidUntil column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS ValidUntil DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

-- Add UsedCount column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS UsedCount INT NOT NULL DEFAULT 0;

-- Add MaxUses column (0 = unlimited)
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS MaxUses INT NOT NULL DEFAULT 0;

-- Add MinimumBookingAmount column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS MinimumBookingAmount DECIMAL(10, 2) NOT NULL DEFAULT 0;

-- Add IsActive column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS IsActive TINYINT(1) NOT NULL DEFAULT 1;

-- Add CreatedDate column
ALTER TABLE Discounts ADD COLUMN IF NOT EXISTS CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

-- Create unique index on Code (if not exists)
-- Note: MySQL doesn't support IF NOT EXISTS for indexes directly, so we'll use a procedure or ignore errors
CREATE UNIQUE INDEX IX_Discounts_Code ON Discounts (Code);

-- Remove DiscountId column if it exists (it was a mistake in the initial migration)
-- This column is not needed as we have the Id column
ALTER TABLE Discounts DROP COLUMN IF EXISTS DiscountId;

