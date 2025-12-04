-- Create AmenityLookup table for global amenities lookup
CREATE TABLE IF NOT EXISTS AmenityLookups (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    AmenityLookupId INT NOT NULL,
    Name VARCHAR(100) NOT NULL UNIQUE,
    DisplayName VARCHAR(100) NOT NULL,
    Category VARCHAR(50) NOT NULL DEFAULT 'General',
    IconClass VARCHAR(100) NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    SortOrder INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL,
    UpdatedDate DATETIME NOT NULL,
    INDEX idx_amenitylookup_name (Name),
    INDEX idx_amenitylookup_category (Category),
    INDEX idx_amenitylookup_isactive (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Insert standard amenities
INSERT INTO AmenityLookups (AmenityLookupId, Name, DisplayName, Category, IconClass, IsActive, SortOrder, CreatedDate, UpdatedDate) VALUES
(1, 'WiFi', 'WiFi', 'Utilities', 'fas fa-wifi', 1, 1, NOW(), NOW()),
(2, 'Electric-Hookup', 'Electric Hookup', 'Utilities', 'fas fa-plug', 1, 2, NOW(), NOW()),
(3, 'Water-Access', 'Water Access', 'Utilities', 'fas fa-tint', 1, 3, NOW(), NOW()),
(4, 'Sewage-Hookup', 'Sewage Hookup', 'Utilities', 'fas fa-toilet', 1, 4, NOW(), NOW()),
(5, 'Heating', 'Heating', 'Comfort', 'fas fa-fire', 1, 5, NOW(), NOW()),
(6, 'Air-Conditioning', 'Air Conditioning', 'Comfort', 'fas fa-snowflake', 1, 6, NOW(), NOW()),
(7, 'Fire-Pit', 'Fire Pit', 'Facilities', 'fas fa-campground', 1, 7, NOW(), NOW()),
(8, 'Picnic-Table', 'Picnic Table', 'Facilities', 'fas fa-table', 1, 8, NOW(), NOW()),
(9, 'BBQ-Grill', 'BBQ Grill', 'Facilities', 'fas fa-utensils', 1, 9, NOW(), NOW()),
(10, 'Pet-Friendly', 'Pet Friendly', 'General', 'fas fa-paw', 1, 10, NOW(), NOW()),
(11, 'Kitchen', 'Kitchen', 'Appliances', 'fas fa-blender', 1, 11, NOW(), NOW()),
(12, 'Bathroom', 'Private Bathroom', 'Facilities', 'fas fa-bath', 1, 12, NOW(), NOW()),
(13, 'Close-To-Playground', 'Close to Playground', 'General', 'fas fa-child', 1, 13, NOW(), NOW()),
(14, 'Senior-Friendly', 'Senior Friendly', 'Accessibility', 'fas fa-wheelchair', 1, 14, NOW(), NOW()),
(15, 'Handicap-Friendly', 'Handicap Friendly', 'Accessibility', 'fas fa-universal-access', 1, 15, NOW(), NOW()),
(16, 'Oven', 'Oven', 'Appliances', 'fas fa-fire-burner', 1, 16, NOW(), NOW()),
(17, 'Fridge', 'Fridge', 'Appliances', 'fas fa-temperature-low', 1, 17, NOW(), NOW()),
(18, 'Freezer', 'Freezer', 'Appliances', 'fas fa-icicles', 1, 18, NOW(), NOW()),
(19, 'Deposit-Box', 'Deposit Box', 'Security', 'fas fa-lock', 1, 19, NOW(), NOW()),
(20, 'TV', 'TV', 'Comfort', 'fas fa-tv', 1, 20, NOW(), NOW()),
(21, 'Surveilled', 'Surveilled', 'Security', 'fas fa-video', 1, 21, NOW(), NOW());

