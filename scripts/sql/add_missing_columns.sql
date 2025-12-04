-- Add missing columns to AccommodationTypes table
ALTER TABLE AccommodationTypes ADD COLUMN CampsiteId INT NULL;
ALTER TABLE AccommodationTypes ADD COLUMN Type VARCHAR(50) NULL;
ALTER TABLE AccommodationTypes ADD COLUMN Description TEXT NULL;
ALTER TABLE AccommodationTypes ADD COLUMN MaxCapacity INT NULL DEFAULT 0;
ALTER TABLE AccommodationTypes ADD COLUMN BasePrice DECIMAL(65,30) NULL;
ALTER TABLE AccommodationTypes ADD COLUMN ImageUrl TEXT NULL;
ALTER TABLE AccommodationTypes ADD COLUMN AvailableUnits INT NULL DEFAULT 0;
ALTER TABLE AccommodationTypes ADD COLUMN IsActive TINYINT(1) NULL DEFAULT 1;
ALTER TABLE AccommodationTypes ADD COLUMN CreatedDate DATETIME NULL;

-- Add missing columns to AccommodationSpots table
ALTER TABLE AccommodationSpots ADD COLUMN SpotIdentifier VARCHAR(50) NULL;
ALTER TABLE AccommodationSpots ADD COLUMN CampsiteId INT NULL;
ALTER TABLE AccommodationSpots ADD COLUMN CampsiteName VARCHAR(200) NULL;
ALTER TABLE AccommodationSpots ADD COLUMN AccommodationTypeId INT NULL;
ALTER TABLE AccommodationSpots ADD COLUMN Latitude DOUBLE NULL;
ALTER TABLE AccommodationSpots ADD COLUMN Longitude DOUBLE NULL;
ALTER TABLE AccommodationSpots ADD COLUMN Type VARCHAR(50) NULL;
ALTER TABLE AccommodationSpots ADD COLUMN Status VARCHAR(50) NULL DEFAULT 'Available';
ALTER TABLE AccommodationSpots ADD COLUMN PriceModifier DECIMAL(65,30) NULL DEFAULT 1.0;
ALTER TABLE AccommodationSpots ADD COLUMN CreatedDate DATETIME NULL;

-- Add missing columns to Amenities table
ALTER TABLE Amenities ADD COLUMN CampsiteId INT NULL;
ALTER TABLE Amenities ADD COLUMN Name VARCHAR(100) NULL;
ALTER TABLE Amenities ADD COLUMN Description TEXT NULL;
ALTER TABLE Amenities ADD COLUMN IconUrl TEXT NULL;
ALTER TABLE Amenities ADD COLUMN Category VARCHAR(50) NULL;
ALTER TABLE Amenities ADD COLUMN IsAvailable TINYINT(1) NULL DEFAULT 1;
ALTER TABLE Amenities ADD COLUMN CreatedDate DATETIME NULL;
ALTER TABLE Amenities ADD COLUMN UpdatedDate DATETIME NULL;

