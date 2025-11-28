-- Update existing AccommodationTypes to set AccommodationTypeId = Id
UPDATE AccommodationTypes SET AccommodationTypeId = Id;

-- Update existing AccommodationSpots to set AccommodationTypeId from the accommodation type
UPDATE AccommodationSpots AS s
INNER JOIN AccommodationTypes AS t ON s.Type = t.Type AND s.CampsiteId = t.CampsiteId
SET s.AccommodationTypeId = t.Id
WHERE s.AccommodationTypeId = 0
LIMIT 1000;

-- Create trigger to automatically set AccommodationTypeId when inserting
DELIMITER $$

DROP TRIGGER IF EXISTS AccommodationTypes_BeforeInsert$$
CREATE TRIGGER AccommodationTypes_BeforeInsert
BEFORE INSERT ON AccommodationTypes
FOR EACH ROW
BEGIN
  -- The Id will be auto-generated, but we can't access it in BEFORE INSERT
  -- So we'll use an AFTER INSERT trigger instead
  SET NEW.AccommodationTypeId = 0;
END$$

DROP TRIGGER IF EXISTS AccommodationTypes_AfterInsert$$
CREATE TRIGGER AccommodationTypes_AfterInsert
AFTER INSERT ON AccommodationTypes
FOR EACH ROW
BEGIN
  UPDATE AccommodationTypes SET AccommodationTypeId = Id WHERE Id = NEW.Id;
END$$

DELIMITER ;

