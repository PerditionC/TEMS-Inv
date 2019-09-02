-- SQL for SQLite3 database schema TEMS_Inv.db 
-- version: initial deployment

/*
 *   Each of tables exist along with a shadow table which
 * contains all deleted rows [unless purged].
 *   The shadow table has same name with a _ suffix and
 * the same columns as primary table with an additional
 * `del_timestamp` column indicating when the row was 
 * deleted from the primary table and added to shadow 
 * table.
 *   All primary keys contain a text representation of
 * a GUID unless the pk is naturally globally unique - 
 * this is required to allow proper replication of 
 * disconnected clients [i.e. AUTONUMBER INTEGER rowids
 * can not be used or distinct changes may clash when
 * synchronizing client databases].
 *   All timestamp fields are stored as a bigint INTEGER
 * alias (minimal 64 bit integer assumed).  We create a
 * timestamp column for every table that we want to be
 * replicated - this ensures we can use a last change
 * wins policy for conflicts and other similar scenarios.
 *   All date/times are stored as a bigint INTEGER alias
 * assuming local time.  It is expected that all users
 * are in the same timezone.  All date/times are when
 * an event did or is going to occur or are a relative
 * time frame of when the next event should occur, e.g.
 * 6 weeks from some arbitrary date.
 *   All strings are stored in UTF8 encoding [mostly ASCII]
 * and lengths provided are the assumed length for any
 * programmatic manipulation (i.e. display, copy, etc).
 * Note: SQLite does not enforce length but the program
 * does and may truncate any data larger than specified.
 *  Blob data is of unspecified length and generally will
 * contain a complete embedded file such that if written
 * to disk with no changes will be a valid file of expected
 * format (image file, word processor document, ...)
 *   All foreign keys have an index, see SQLite foreign keys
 * documentation, recommended as those columns queried if
 * parent table row is deleted.  Constraints of foreign keys
 * are deferred until the transaction is committed, this
 * relaxes the order rows must be inserted into the DB
 * which may help avoid some replication issues (since we
 * have limited control over order tables processed during
 * synchronization).  Check is done immediately if not
 * currently within a TRANSACTION block.  Note that 
 * ON UPDATE CASCADE clause is not used as the primary key
 * of the parent table must not change if replicas are to
 * see changes to the row - therefore if a pk must be 
 * changed it must be done via an add parent row, update
 * child row, and delete original row so the replicas can
 * also see and make the changes.  
 *   Note that NOT NULL is required on the primary keys, 
 * otherwise SQLite will allow NULL pk values due to backward 
 * compatibility with earlier versions.  Additionally to
 * ensure the current transaction and not just current SQL
 * statement is aborted (rolled back), the INSERT or UPDATE
 * statement should add either OR ROLLBACK or OR REPLACE
 * as needed instead of default OR ABORT; do NOT add to
 * the schema a conflict resolution on constraints of
 * ON CONFLICT ROLLBACK; this will cause an error during
 * replication as the changeset apply triggers a conflict
 * causing unexpected rollback to occur.
 *   On each UPDATE we invoke a trigger that sets the row''s
 * [last modified] timestamp value.  To allow the timestamp to be 
 * explicitly set (e.g. when replicating, we use the original 
 * timestamp instead of when the replication modified the row), 
 * we only modify the timestamp if new value is NULL or the same 
 * as the old value (i.e. not provided or unchanged). Note that
 * we use AFTER UPDATE to avoid undefined behavior that may 
 * occur due to changing row using BEFORE UPDATE.
 * 635019330320182000 ticks to Jan 1, 1970
 * Note: to avoid needing extra math and cross platform issues,
 * the timestamp is stored as a simple milliseconds since the
 * Unix epoch (nanoseconds, i.e. ticks) are not needed.
 *   On each DELETE we invoke a trigger that will either add
 * a new row to shadow table with values prior to deleting row 
 * and set del_timestamp to current date & time or if the row
 * was previously deleted [and restored] then we replace values
 * in shadow table with most recent values and update the 
 * del_timestamp to current date & time. Note we must use
 * BEFORE DELETE for the shadow table to actually be populated
 * with data.
 *  For synchronization with a !central db only! there is a
 * field `lastSync` which contains the timestamp of when
 * this item last sync'd; if NULL or before timestamp then
 * this row has changes that need to be be sent to central db
 * # two methods of sync'ing are used, one to central db service
 * if available and computer has networking; second is manual
 * sync'ing directly with another sqlite db (both can be used,
 * i.e. online computers maintain up to date via connecting to
 * central db; if central db is not available either due to 
 * local issues or issues with server then db can be manually
 * sync'd with other locations by direct method; and then back
 * to server if connectivity returns)
 *
*/


-- Site and equipment definitions
/* 
 EquipmentUnitType
 SiteLocation
 SiteLocationEquipmentUnitTypeMapping
*/

-- User information
/*
 UserDetail
 UserSiteMapping
 UserActivity
*/

-- Item information
/*
 Image
 Document
 UnitOfMeasure
 BatteryType
 VehicleLocation
 VendorDetail
 ItemCategory
 ItemStatus
 ItemType
 Item
 ItemInstance
 */

 -- Service and usage events
 /*
  ServiceCategory
  ItemService
  ItemServiceHistory
  DamagedMissingEvent
  DeployEvent
 */


-- PRAGMA foreign_keys = "1";

--BEGIN TRANSACTION;  -- causes issue as when running in DB Browser for SQLite already implied TRANSACTION started

-- allows retrieving current time as ticks
-- use SELECT strftime('%Y-%m-%d %H:%M:%S', [ticks] / 10000000 - 62135596800, 'unixepoch','localtime'); to get date & time as UTC
CREATE VIEW IF NOT EXISTS current_timestamp_ticks AS SELECT (((CAST(strftime('%s', 'now', 'utc') AS bigint) + 62135596800) * 10000000) + (strftime('%f', 'now')-round(strftime('%f', 'now')))*1000) AS `timestamp`;

-- defines meta data about this table
-- Note: we currently only store DB version which could use SQLite PRAGMA option, but this is simpler and allows future additional information, e.g. last sync information
CREATE TABLE IF NOT EXISTS `META`
(
  `key` TEXT NOT NULL,
  `value` TEXT,
  PRIMARY KEY(`key`)
);
INSERT INTO `META` (`key`, `value`) VALUES ('dbVersion', 2);


CREATE TABLE IF NOT EXISTS `EquipmentUnitType`
(
  `name` varchar ( 6 ) NOT NULL,
  `description` varchar ( 256 ) NOT NULL,
  `unitCode` varchar ( 1 ) NOT NULL UNIQUE,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint DEFAULT NULL,
  PRIMARY KEY(`name`),
  -- checks to ensure only values of expected type are stored since SQLite largely ignores type
  --CHECK (TYPEOF(unitCode)=='TEXT'),
  --CHECK (TYPEOF(name)=='TEXT'),
  --CHECK (TYPEOF(timestamp)=='INTEGER'),
  --CHECK (TYPEOF(lastSync)='INTEGER'),
  UNIQUE(`name`,`unitCode`)
);
CREATE TABLE IF NOT EXISTS `EquipmentUnitType_`
(
  `name` varchar ( 6 ) NOT NULL,
  `description` varchar ( 256 ) NOT NULL,
  `unitCode` varchar ( 1 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint DEFAULT NULL,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`name`)
);
CREATE TRIGGER IF NOT EXISTS EquipmentUnitType_timestamp AFTER UPDATE ON `EquipmentUnitType`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `EquipmentUnitType` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS EquipmentUnitType_onDelete BEFORE DELETE ON `EquipmentUnitType`
BEGIN
  INSERT OR REPLACE INTO `EquipmentUnitType_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `EquipmentUnitType` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `SiteLocation`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 64 ) NOT NULL, -- same site may exists multiple times but each must have unique locSuffix
  `locSuffix` varchar ( 6 ) NOT NULL UNIQUE,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`),
  UNIQUE(`name`,`locSuffix`)
);
CREATE TABLE IF NOT EXISTS `SiteLocation_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 64 ) NOT NULL,
  `locSuffix` varchar ( 6 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS SiteLocation_timestamp AFTER UPDATE ON `SiteLocation`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `SiteLocation` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS SiteLocation_onDelete BEFORE DELETE ON `SiteLocation`
BEGIN
  INSERT OR REPLACE INTO `SiteLocation_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `SiteLocation` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `SiteLocationEquipmentUnitTypeMapping`
(
  `id` varchar ( 36 ) NOT NULL,
  `siteId` varchar ( 36 ) NOT NULL,
  `unitName` varchar ( 6 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (siteId) REFERENCES SiteLocation(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (unitName) REFERENCES EquipmentUnitType(name) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`),
  UNIQUE(`siteId`,`unitName`)
);
CREATE INDEX IF NOT EXISTS `SiteLocationEquipmentUnitTypeMapping_siteId` ON `SiteLocationEquipmentUnitTypeMapping` ( `siteId` );
CREATE INDEX IF NOT EXISTS `SiteLocationEquipmentUnitTypeMapping_unitName` ON `SiteLocationEquipmentUnitTypeMapping` ( `unitName` );
CREATE TABLE IF NOT EXISTS `SiteLocationEquipmentUnitTypeMapping_`
(
  `id` varchar ( 36 ) NOT NULL,
  `siteId` varchar ( 36 ) NOT NULL,
  `unitName` varchar ( 6 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS SiteLocationEquipmentUnitTypeMapping_timestamp AFTER UPDATE ON `SiteLocationEquipmentUnitTypeMapping`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `SiteLocationEquipmentUnitTypeMapping` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS SiteLocationEquipmentUnitTypeMapping_onDelete BEFORE DELETE ON `SiteLocationEquipmentUnitTypeMapping`
BEGIN
  INSERT OR REPLACE INTO `SiteLocationEquipmentUnitTypeMapping_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `SiteLocationEquipmentUnitTypeMapping` WHERE OLD.rowid=rowid;
END;

CREATE VIEW IF NOT EXISTS EquipmentUnitTypeToSiteLocation AS
SELECT SiteLocationEquipmentUnitTypeMapping.id, SiteLocation.name AS siteLocation, SiteLocation.locSuffix, EquipmentUnitType.name as equipmentUnitType, EquipmentUnitType.description, EquipmentUnitType.unitCode
FROM SiteLocation INNER JOIN (SiteLocationEquipmentUnitTypeMapping INNER JOIN EquipmentUnitType ON SiteLocationEquipmentUnitTypeMapping.unitName=EquipmentUnitType.name) ON SiteLocation.id=SiteLocationEquipmentUnitTypeMapping.siteId
ORDER BY SiteLocation.name, EquipmentUnitType.name;



CREATE TABLE IF NOT EXISTS `UserDetail`
(
  `userId` varchar ( 32 ) NOT NULL,
  `hashedPassphrase` varchar ( 128 ),
  `isActive` integer NOT NULL DEFAULT 0 CHECK (isActive=0 OR isActive=1),
  `isPasswordExpired` integer NOT NULL DEFAULT 1 CHECK (isPasswordExpired=0 OR isPasswordExpired=1),
  `role` varchar ( 32 ) NOT NULL DEFAULT 'User' CHECK (role='User' OR role='Admin'),
  `siteId` varchar ( 36 ) NOT NULL, -- default site to start with
  `lastName` varchar ( 128 ),
  `firstName` varchar ( 128 ),
  `email` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (siteId) REFERENCES SiteLocation(id) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`userId`)
);
CREATE INDEX IF NOT EXISTS `UserDetail_siteId` ON `UserDetail` ( `siteId` );
CREATE TABLE IF NOT EXISTS `UserDetail_`
(
  `userId` varchar ( 32 ) NOT NULL,
  `hashedPassphrase` varchar ( 128 ),
  `isActive` integer NOT NULL DEFAULT 0 CHECK (isActive=0 OR isActive=1),
  `isPasswordExpired` integer NOT NULL DEFAULT 1 CHECK (isPasswordExpired=0 OR isPasswordExpired=1),
  `role` varchar ( 32 ) NOT NULL DEFAULT 'User' CHECK (role='User' OR role='Admin'),
  `siteId` varchar ( 36 ) NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000', -- default site to start with GUID.empty
  `lastName` varchar ( 128 ),
  `firstName` varchar ( 128 ),
  `email` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`userId`)
);
CREATE TRIGGER IF NOT EXISTS UserDetail_timestamp AFTER UPDATE ON `UserDetail`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `UserDetail` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS UserDetail_onDelete BEFORE DELETE ON `UserDetail`
BEGIN
  INSERT OR REPLACE INTO `UserDetail_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `UserDetail` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `UserSiteMapping`
(
  `id` varchar ( 36 ) NOT NULL,
  `siteId` varchar ( 36 ) NOT NULL,
  `userId` varchar ( 32 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (siteId) REFERENCES SiteLocation(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (userId) REFERENCES UserDetail(userId) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`),
  UNIQUE(`userId`,`siteId`)
);
CREATE INDEX IF NOT EXISTS `UserSiteMapping_siteId` ON `UserSiteMapping` ( `siteId` );
CREATE INDEX IF NOT EXISTS `UserSiteMapping_userId` ON `UserSiteMapping` ( `userId` );
CREATE TABLE IF NOT EXISTS `UserSiteMapping_`
(
  `id` varchar ( 36 ) NOT NULL,
  `siteId` varchar ( 36 ) NOT NULL,
  `userId` varchar ( 32 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS UserSiteMapping_timestamp AFTER UPDATE ON `UserSiteMapping`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `UserSiteMapping` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS UserSiteMapping_onDelete BEFORE DELETE ON `UserSiteMapping`
BEGIN
  INSERT OR REPLACE INTO `UserSiteMapping_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `UserSiteMapping` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `UserActivity`
(
  `id` varchar ( 36 ) NOT NULL,
  `userId` varchar ( 32 ) NOT NULL,
  `when` bigint NOT NULL,
  `action` integer NOT NULL,
  `details` varchar ( 1024 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (userId) REFERENCES UserDetail(userId) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY (`id`)
);
CREATE INDEX IF NOT EXISTS `UserActivity_userId` ON `UserActivity` ( `userId` );
CREATE TABLE IF NOT EXISTS `UserActivity_`
(
  `id` varchar ( 36 ) NOT NULL,
  `userId` varchar ( 32 ) NOT NULL,
  `when` bigint NOT NULL,
  `action` integer NOT NULL,
  `details` varchar ( 1024 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY (`id`)
);
CREATE TRIGGER IF NOT EXISTS UserActivity_timestamp AFTER UPDATE ON `UserActivity`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  -- UPDATE `UserActivity` SET `timestamp` = (SELECT `timestamp` FROM current_timestamp_ticks)  WHERE NEW.rowid=rowid;
  UPDATE `UserActivity` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS UserActivity_onDelete BEFORE DELETE ON `UserActivity`
BEGIN
  -- INSERT OR REPLACE INTO `UserActivity_` ( `id`, `userId`, `when`, `action`, `details`, `timestamp`, `del_timestamp` )
  -- VALUES ( OLD.`id`, OLD.`userId`, OLD.`when`, OLD.`action`, OLD.`details`, OLD.`timestamp`, (CAST(strftime('%s', 'now', 'utc') AS bigint)) );
  INSERT OR REPLACE INTO `UserActivity_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `UserActivity` WHERE OLD.rowid=rowid;
END;


-- all BLOB file data is in SQLite Archive format allowing external programs to list, extract, & add files
-- replication to be determined, does not support compressed data
CREATE TABLE sqlar(
  name TEXT PRIMARY KEY,  -- name of the file
  mode INT,               -- access permissions, not used
  mtime INT,              -- last modification time
  sz INT,                 -- original file size
  data BLOB               -- compressed content
);

-- the data is stored in sqlar table, a future version may move storage externally or to separate db
CREATE TABLE IF NOT EXISTS `Image`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` TEXT NOT NULL,  -- Warning: unspecified # of characters in name
  `description` varchar ( 128 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (name) REFERENCES sqlar(name) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `Image_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` TEXT NOT NULL,  -- Warning: unspecified # of characters in name
  `description` varchar ( 128 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS Image_timestamp AFTER UPDATE ON `Image`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `Image` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS Image_onDelete BEFORE DELETE ON `Image`
BEGIN
  INSERT OR REPLACE INTO `Image_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `Image` WHERE OLD.rowid=rowid;
END;

-- the data is stored in sqlar table, a future version may move storage externally or to separate db
CREATE TABLE IF NOT EXISTS `Document`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` TEXT NOT NULL,  -- Warning: unspecified # of characters in name
  `description` varchar ( 128 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (name) REFERENCES sqlar(name) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `Document_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` TEXT NOT NULL,  -- Warning: unspecified # of characters in name
  `description` varchar ( 128 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS Document_timestamp AFTER UPDATE ON `Document`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `Document` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS Document_onDelete BEFORE DELETE ON `Document`
BEGIN
  INSERT OR REPLACE INTO `Document_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `Document` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `UnitOfMeasure`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `UnitOfMeasure_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS UnitOfMeasure_timestamp AFTER UPDATE ON `UnitOfMeasure`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `UnitOfMeasure` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS UnitOfMeasure_onDelete BEFORE DELETE ON `UnitOfMeasure`
BEGIN
  INSERT OR REPLACE INTO `UnitOfMeasure_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `UnitOfMeasure` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `BatteryType`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `BatteryType_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS BatteryType_timestamp AFTER UPDATE ON `BatteryType`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `BatteryType` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS BatteryType_onDelete BEFORE DELETE ON `BatteryType`
BEGIN
  INSERT OR REPLACE INTO `BatteryType_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `BatteryType` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `VehicleLocation`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `VehicleLocation_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS VehicleLocation_timestamp AFTER UPDATE ON `VehicleLocation`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `VehicleLocation` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS VehicleLocation_onDelete BEFORE DELETE ON `VehicleLocation`
BEGIN
  INSERT OR REPLACE INTO `VehicleLocation_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `VehicleLocation` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `VendorDetail`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 32 ) NOT NULL,
  `category` varchar ( 64 ),
  `notes` varchar ( 256 ),
  `addressLine1` varchar ( 64 ),
  `addressLine2` varchar ( 64 ),
  `city` varchar ( 64 ),
  `state` varchar ( 32 ),
  `zipcode` varchar ( 16 ),
  `phoneNumber` varchar ( 16 ),
  `faxNumber` varchar ( 16 ),
  `website` varchar ( 256 ),
  `isActive` integer,
  `contactName` varchar ( 32 ),
  `contactPhoneNumber` varchar ( 16 ),
  `contactEmail` varchar ( 64 ),
  `accountReference` varchar ( 64 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `VendorDetail_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 32 ) NOT NULL,
  `category` varchar ( 64 ),
  `notes` varchar ( 256 ),
  `addressLine1` varchar ( 64 ),
  `addressLine2` varchar ( 64 ),
  `city` varchar ( 64 ),
  `state` varchar ( 32 ),
  `zipcode` varchar ( 16 ),
  `phoneNumber` varchar ( 16 ),
  `faxNumber` varchar ( 16 ),
  `website` varchar ( 256 ),
  `isActive` integer,
  `contactName` varchar ( 32 ),
  `contactPhoneNumber` varchar ( 16 ),
  `contactEmail` varchar ( 64 ),
  `accountReference` varchar ( 64 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS VendorDetail_timestamp AFTER UPDATE ON `VendorDetail`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `VendorDetail` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS VendorDetail_onDelete BEFORE DELETE ON `VendorDetail`
BEGIN
  INSERT OR REPLACE INTO `VendorDetail_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `VendorDetail` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `ItemCategory`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `ItemCategory_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemCategory_timestamp AFTER UPDATE ON `ItemCategory`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemCategory` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemCategory_onDelete BEFORE DELETE ON `ItemCategory`
BEGIN
  INSERT OR REPLACE INTO `ItemCategory_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemCategory` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `ItemStatus`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `ItemStatus_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemStatus_timestamp AFTER UPDATE ON `ItemStatus`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemStatus` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemStatus_onDelete BEFORE DELETE ON `ItemStatus`
BEGIN
  INSERT OR REPLACE INTO `ItemStatus_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemStatus` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `ItemType`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemTypeId` integer NOT NULL UNIQUE, -- externally visible id#, possible replication issues
  `name` varchar ( 128 ) NOT NULL,
  `make` varchar ( 64 ),
  `model` varchar ( 64 ),
  `expirationRestockCategory` integer NOT NULL DEFAULT ( 0 ) CHECK ((`expirationRestockCategory` >= 0 ) AND (`expirationRestockCategory` <= 2)),
  `cost` integer NOT NULL DEFAULT 0 CHECK (`cost`>=0),
  `weight` float,
  `unitOfMeasureId` varchar ( 36 ) NOT NULL,
  `itemCategoryId` varchar ( 36 ) NOT NULL,
  `batteryCount` integer NOT NULL DEFAULT 0 CHECK (`batteryCount`>=0),
  `batteryTypeId` varchar ( 36 ) NOT NULL,
  `associatedItems` varchar ( 32 ),
  `isBin` integer NOT NULL DEFAULT 0 CHECK (`isBin`=0 OR `isBin`=1),
  `isModule` integer NOT NULL DEFAULT 0 CHECK (`isModule`=0 OR `isModule`=1),
  `vendorId` varchar ( 36 ) NOT NULL,
  `notes` varchar ( 255 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (unitOfMeasureId) REFERENCES UnitOfMeasure(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (itemCategoryId) REFERENCES ItemCategory(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (batteryTypeId) REFERENCES BatteryType(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (vendorId) REFERENCES VendorDetail(id) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE INDEX IF NOT EXISTS `ItemType_itemTypeId` ON `ItemType` (  `itemTypeId` );
CREATE INDEX IF NOT EXISTS `ItemType_itemCategoryId` ON `ItemType`( `itemCategoryId` );
CREATE INDEX IF NOT EXISTS `ItemType_batteryTypeId` ON `ItemType` ( `batteryTypeId` );
CREATE INDEX IF NOT EXISTS `ItemType_vendorId` ON `ItemType` ( `vendorId` );
CREATE INDEX IF NOT EXISTS `ItemType_isModule` ON `ItemType` ( `isModule` );
CREATE INDEX IF NOT EXISTS `ItemType_isBin` ON `ItemType` ( `isBin` );
CREATE TABLE IF NOT EXISTS `ItemType_`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemTypeId` integer NOT NULL, -- externally visible id#
  `name` varchar ( 128 ) NOT NULL,
  `make` varchar ( 64 ),
  `model` varchar ( 64 ),
  `expirationRestockCategory` integer NOT NULL DEFAULT ( 0 ) CHECK ((`expirationRestockCategory` >= 0 ) AND (`expirationRestockCategory` <= 2)),
  `cost` integer NOT NULL DEFAULT 0 CHECK (`cost`>=0),
  `weight` float,
  `unitOfMeasureId` varchar ( 36 ) NOT NULL,
  `itemCategoryId` varchar ( 36 ) NOT NULL,
  `batteryCount` integer NOT NULL DEFAULT 0 CHECK (`batteryCount`>=0),
  `batteryTypeId` varchar ( 36 ) NOT NULL,
  `associatedItems` varchar ( 32 ),
  `isBin` integer NOT NULL DEFAULT 0 CHECK (`isBin`=0 OR `isBin`=1),
  `isModule` integer NOT NULL DEFAULT 0 CHECK (`isModule`=0 OR `isModule`=1),
  `vendorId` varchar ( 36 ) NOT NULL,
  `notes` varchar ( 255 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemType_timestamp AFTER UPDATE ON `ItemType`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemType` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemType_onDelete BEFORE DELETE ON `ItemType`
BEGIN
  INSERT OR REPLACE INTO `ItemType_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemType` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `ItemTypeImageMapping`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemTypeId` varchar ( 36 ) NOT NULL,
  `imageId` varchar ( 36 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (itemTypeId) REFERENCES ItemType(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (imageId) REFERENCES Image(id) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`),
  UNIQUE(`itemTypeId`,`imageId`)
);
CREATE INDEX IF NOT EXISTS `ItemTypeImageMapping_itemTypeId` ON `ItemTypeImageMapping` ( `itemTypeId` );
CREATE INDEX IF NOT EXISTS `ItemTypeImageMapping_imageId` ON `ItemTypeImageMapping` ( `imageId` );
CREATE TABLE IF NOT EXISTS `ItemTypeImageMapping_`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemTypeId` varchar ( 36 ) NOT NULL,
  `imageId` varchar ( 36 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemTypeImageMapping_timestamp AFTER UPDATE ON `ItemTypeImageMapping`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemTypeImageMapping` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemTypeImageMapping_onDelete BEFORE DELETE ON `ItemTypeImageMapping`
BEGIN
  INSERT OR REPLACE INTO `ItemTypeImageMapping_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemTypeImageMapping` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `ItemTypeDocumentMapping`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemTypeId` varchar ( 36 ) NOT NULL,
  `documentId` varchar ( 36 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (itemTypeId) REFERENCES ItemType(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (documentId) REFERENCES Document(id) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`),
  UNIQUE(`itemTypeId`,`documentId`)
);
CREATE INDEX IF NOT EXISTS `ItemTypeImageMapping_itemTypeId` ON `ItemTypeImageMapping` ( `itemTypeId` );
CREATE INDEX IF NOT EXISTS `ItemTypeImageMapping_imageId` ON `ItemTypeImageMapping` ( `imageId` );
CREATE TABLE IF NOT EXISTS `ItemTypeDocumentMapping_`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemTypeId` varchar ( 36 ) NOT NULL,
  `documentId` varchar ( 36 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemTypeDocumentMapping_timestamp AFTER UPDATE ON `ItemTypeDocumentMapping`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemTypeDocumentMapping` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemTypeDocumentMapping_onDelete BEFORE DELETE ON `ItemTypeDocumentMapping`
BEGIN
  INSERT OR REPLACE INTO `ItemTypeDocumentMapping_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemTypeDocumentMapping` WHERE OLD.rowid=rowid;
END;


CREATE TABLE IF NOT EXISTS `Item`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemId` integer NOT NULL, -- Warning: externally visible but only needs to be UNIQUE relative to specific trailer unit AND itemTypeId
  `itemTypeId` varchar ( 36 ) NOT NULL,
  `unitTypeName` varchar ( 6 ) NOT NULL,
  `vehicleLocationId` varchar ( 36 ) NOT NULL,
  `vehicleCompartment` varchar ( 32 ),
  `count` integer NOT NULL DEFAULT ( 0 ) CHECK ( `count` >= 0 ),
  `bagNumber` varchar ( 16 ),
  `expirationDate` bigint,
  `parentId` varchar ( 36 ), -- refers to another row in this table that is either a bin or module id
  `notes` varchar ( 255 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (itemTypeId) REFERENCES ItemType(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (unitTypeName) REFERENCES EquipmentUnitType(name) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (vehicleLocationId) REFERENCES VehicleLocation(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (parentId) REFERENCES Item(id) DEFERRABLE INITIALLY DEFERRED,
  UNIQUE (itemId, itemTypeId, unitTypeName),
  PRIMARY KEY(`id`)
);
CREATE INDEX IF NOT EXISTS `Item_itemId` ON `Item` ( `itemId` );
CREATE INDEX IF NOT EXISTS `Item_itemTypeId` ON `Item` ( `itemTypeId` );
CREATE INDEX IF NOT EXISTS `Item_vehicleLocationId` ON `Item` ( `vehicleLocationId` );
CREATE INDEX IF NOT EXISTS `Item_parentId` ON `Item` ( `parentId` );
CREATE INDEX IF NOT EXISTS `Item_unitTypeName` ON `Item` ( `unitTypeName` );
CREATE TABLE IF NOT EXISTS `Item_`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemId` integer NOT NULL, -- externally visible id#
  `itemTypeId` varchar ( 36 ) NOT NULL,
  `unitTypeName` varchar ( 6 ) NOT NULL,
  `vehicleLocationId` varchar ( 36 ) NOT NULL,
  `vehicleCompartment` varchar ( 32 ),
  `count` integer NOT NULL DEFAULT ( 0 ) CHECK ( `count` >= 0 ),
  `bagNumber` varchar ( 16 ),
  `expirationDate` bigint,
  `parentId` varchar ( 36 ), -- refers to another row in this table that is either a bin or module id
  `notes` varchar ( 255 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS Item_timestamp AFTER UPDATE ON `Item`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `Item` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS Item_onDelete BEFORE DELETE ON `Item`
BEGIN
  INSERT OR REPLACE INTO `Item_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `Item` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `ItemInstance`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemNumber` varchar UNIQUE, -- externally visible id#, readonly
  `itemId` varchar ( 36 ) NOT NULL,
  `siteLocationId` varchar ( 36 ) NOT NULL,
  `serialNumber` varchar ( 32 ),
  `grantNumber` varchar ( 16 ),
  `statusId` varchar ( 36 ) NOT NULL,
  `inServiceDate` bigint NOT NULL,
  `removedServiceDate` bigint,
  `isSealBroken` integer NOT NULL DEFAULT 0 CHECK (`isSealBroken`=0 OR `isSealBroken`=1),
  `hasBarcode` integer NOT NULL DEFAULT 0 CHECK (`hasBarcode`=0 OR `hasBarcode`=1),
  `notes` varchar ( 255 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (itemId) REFERENCES Item(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (siteLocationId) REFERENCES SiteLocation(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (statusId) REFERENCES ItemStatus(id) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE INDEX IF NOT EXISTS `ItemInstance_itemNumber` ON `ItemInstance` ( `itemNumber` );
CREATE INDEX IF NOT EXISTS `ItemInstance_itemId` ON `ItemInstance` ( `itemId` );
CREATE INDEX IF NOT EXISTS `ItemInstance_statusId` ON `ItemInstance` ( `statusId` );
CREATE INDEX IF NOT EXISTS `ItemInstance_siteLocationId` ON `ItemInstance` ( `siteLocationId` );
CREATE TABLE IF NOT EXISTS `ItemInstance_`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemNumber` varchar, -- externally visible id#, readonly
  `itemId` varchar ( 36 ) NOT NULL,
  `siteLocationId` varchar ( 36 ) NOT NULL,
  `serialNumber` varchar ( 32 ),
  `grantNumber` varchar ( 16 ),
  `statusId` varchar ( 36 ) NOT NULL,
  `inServiceDate` bigint NOT NULL,
  `removedServiceDate` bigint,
  `isSealBroken` integer NOT NULL DEFAULT 0 CHECK (`isSealBroken`=0 OR `isSealBroken`=1),
  `hasBarcode` integer NOT NULL DEFAULT 0 CHECK (`hasBarcode`=0 OR `hasBarcode`=1),
  `notes` varchar ( 255 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemInstance_timestamp AFTER UPDATE ON `ItemInstance`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemInstance` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemInstance_onDelete BEFORE DELETE ON `ItemInstance`
BEGIN
  INSERT OR REPLACE INTO `ItemInstance_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemInstance` WHERE OLD.rowid=rowid;
END;

-- This view is required to keep itemNumber field updated automatically as changes occur to the DB
CREATE VIEW IF NOT EXISTS ActiveItemNumbers AS 
SELECT 
(
  EquipmentUnitType.unitCode || 
  ItemType.itemTypeId || 
  '-' || 
  Item.itemId || 
  SiteLocation.locSuffix
) AS itemNumber, 
ItemInstance.id AS id, -- important that id corresponds to ItemInstance for trigger
removedServiceDate
FROM
SiteLocation INNER JOIN (
  ItemInstance INNER JOIN (
    ItemType INNER JOIN (
      Item INNER JOIN EquipmentUnitType ON Item.unitTypeName=EquipmentUnitType.name
	) ON Item.itemTypeId=ItemType.id
  ) ON ItemInstance.itemId=Item.id
) ON ItemInstance.siteLocationId=SiteLocation.id;

-- This view is not required & may be changed as needed, it is only used to view the information directly from the DB
CREATE VIEW IF NOT EXISTS ItemNumbers AS 
SELECT 
(
  EquipmentUnitType.unitCode || 
  ItemType.itemTypeId || 
  '-' || 
  Item.itemId || 
  SiteLocation.locSuffix
) AS itemNumber, 
ItemInstance.id AS id, -- important that id corresponds to ItemInstance for trigger
*
FROM
SiteLocation INNER JOIN (
  ItemInstance INNER JOIN (
    ItemType INNER JOIN (
      Item INNER JOIN EquipmentUnitType ON Item.unitTypeName=EquipmentUnitType.name
	) ON Item.itemTypeId=ItemType.id
  ) ON ItemInstance.itemId=Item.id
) ON ItemInstance.siteLocationId=SiteLocation.id;

-- Set itemNumber field if not provided on INSERT
-- Note: it is important that id is consistent (always refers to ItemInstance.id) to avoid errors on INSERT [so ActiveItemNumbers includes minimal fields without duplicate id fields]
-- We avoid updating if itemNumber is provided during INSERT to avoid unnecessary changes.
-- Warning: itemNumber is only Unique in regards to ItemsInstance with ItemStatus of Available; that is there should ever only be 1 ItemInstance of a given itemNumber Available.
-- However! when ItemInstances are taken out of service they can have the tame itemNumber as other out of service ItemInstances and the Available one (e.g. a damaged item may have to be replaced
-- leaving the original ItemInstance in RemovedFromService status and a new ItemInstance created with same itemNumber for the new ItemInstance - they will have different id values
CREATE TRIGGER IF NOT EXISTS ItemInstance_itemNumber_afterInsert AFTER INSERT ON `ItemInstance`
BEGIN
  UPDATE `ItemInstance` SET `itemNumber` = (SELECT `itemNumber` FROM `ActiveItemNumbers` WHERE `id` = NEW.`id`), `timestamp` = NEW.`timestamp` WHERE NEW.rowid=rowid AND ItemNumber IS NULL;
END;
CREATE TRIGGER IF NOT EXISTS ItemInstance_itemNumber_afterUpdate AFTER UPDATE ON `ItemInstance`
--exclude WHEN so trigger always fires as cannot seem to get to execute reliably with WHEN clause limiting to items that can change itemNumber value
--WHEN ( (NEW.`itemId` <> OLD.`itemId`) OR (NEW.`siteLocationId` <> OLD.`siteLocationId`) OR (NEW.`removedServiceDate` <> OLD.`removedServiceDate`) )
BEGIN
  UPDATE `ItemInstance` SET `itemNumber` = NULL, `timestamp` = NEW.`timestamp` WHERE (NEW.rowid=rowid) AND (NEW.`removedServiceDate` IS NOT NULL);
  UPDATE `ItemInstance` SET `itemNumber` = (SELECT `itemNumber` FROM `ActiveItemNumbers` WHERE `ActiveItemNumbers`.`id` = NEW.`id`), `timestamp` = NEW.`timestamp` WHERE (NEW.rowid=rowid) AND (NEW.`removedServiceDate` IS NULL);
END;


-- used for recursive lookups to display item trees
CREATE VIEW IF NOT EXISTS ItemList AS 
SELECT 
	Item.id AS id,
	itemId, unitTypeName, vehicleLocationId, vehicleCompartment, `count`, bagNumber, expirationDate, parentId, Item.notes as itemNotes, 
	ItemType.id as itemTypeGuid,
	ItemType.itemTypeId AS itemTypeId, name, make, model, expirationRestockCategory, cost, weight, unitOfMeasureId, itemCategoryId, batteryCount, batteryTypeId, associatedItems, isBin, isModule, vendorId, ItemType.notes as notes
FROM ItemType INNER JOIN Item ON Item.itemTypeId=ItemType.id
ORDER BY itemId;



CREATE TABLE IF NOT EXISTS `ServiceCategory`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  PRIMARY KEY(`id`)
);
CREATE TABLE IF NOT EXISTS `ServiceCategory_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 16 ) NOT NULL,
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ServiceCategory_timestamp AFTER UPDATE ON `ServiceCategory`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ServiceCategory` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ServiceCategory_onDelete BEFORE DELETE ON `ServiceCategory`
BEGIN
  INSERT OR REPLACE INTO `ServiceCategory_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ServiceCategory` WHERE OLD.rowid=rowid;
END;

CREATE TABLE IF NOT EXISTS `ItemService`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 64 ) NOT NULL,  -- this is user visible name for selection, not PK, no uniqueness requirement
  `itemInstanceId` varchar ( 36 ) NOT NULL,
  `categoryId` varchar ( 36 ) NOT NULL,
  `reoccurring` integer NOT NULL DEFAULT 0 CHECK (`reoccurring`=0 OR `reoccurring`=1),
  `lengthTilNextService` integer NOT NULL DEFAULT 0 CHECK (`lengthTilNextService` >= 0),
  `serviceFrequency` integer NOT NULL DEFAULT 0 CHECK ( `serviceFrequency` >= 0 AND `serviceFrequency` <= 3 ),
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (itemInstanceId) REFERENCES ItemInstance(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (categoryId) REFERENCES ServiceCategory(id) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE INDEX IF NOT EXISTS `ItemService_itemInstanceId` ON `ItemService` ( `itemInstanceId` );
CREATE INDEX IF NOT EXISTS `ItemService_categoryId` ON `ItemService` ( `categoryId` );
CREATE TABLE IF NOT EXISTS `ItemService_`
(
  `id` varchar ( 36 ) NOT NULL,
  `name` varchar ( 64 ) NOT NULL,  -- this is user visible name for selection, not PK, no uniqueness requirement
  `itemInstanceId` varchar ( 36 ) NOT NULL,
  `categoryId` varchar ( 36 ) NOT NULL,
  `reoccurring` integer NOT NULL DEFAULT 0 CHECK (`reoccurring`=0 OR `reoccurring`=1),
  `lengthTilNextService` integer NOT NULL DEFAULT 0 CHECK (`lengthTilNextService` >= 0),
  `serviceFrequency` integer NOT NULL DEFAULT 0 CHECK ( `serviceFrequency` >= 0 AND `serviceFrequency` <= 3 ),
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemService_timestamp AFTER UPDATE ON `ItemService`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemService` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemService_onDelete BEFORE DELETE ON `ItemService`
BEGIN
  INSERT OR REPLACE INTO `ItemService_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemService` WHERE OLD.rowid=rowid;
END;


CREATE TABLE IF NOT EXISTS `ItemServiceHistory`
(
  `id` varchar ( 36 ) NOT NULL,
  `serviceId` varchar ( 36 ) NOT NULL,
  `serviceDue` bigint NOT NULL,
  `serviceCompleted` bigint,
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (serviceId) REFERENCES ItemService(id) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE INDEX IF NOT EXISTS `ItemServiceHistory_serviceId` ON `ItemServiceHistory` ( `serviceId` );
CREATE TABLE IF NOT EXISTS `ItemServiceHistory_`
(
  `id` varchar ( 36 ) NOT NULL,
  `serviceId` varchar ( 36 ) NOT NULL,
  `serviceDue` bigint NOT NULL,
  `serviceCompleted` bigint,
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS ItemServiceHistory_timestamp AFTER UPDATE ON `ItemServiceHistory`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `ItemServiceHistory` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS ItemServiceHistory_onDelete BEFORE DELETE ON `ItemServiceHistory`
BEGIN
  INSERT OR REPLACE INTO `ItemServiceHistory_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `ItemServiceHistory` WHERE OLD.rowid=rowid;
END;


CREATE TABLE IF NOT EXISTS `DamageMissingEvent`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemInstanceId` varchar ( 36 ) NOT NULL,
  `inputBy` varchar ( 32 ) NOT NULL,
  `reportedBy` varchar ( 256 ) NOT NULL,
  `discoveryDate` bigint NOT NULL,
  `eventType` integer NOT NULL DEFAULT 0 CHECK ( `eventType` >= 0 AND `eventType` <= 1 ),
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (itemInstanceId) REFERENCES ItemInstance(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (inputBy) REFERENCES UserDetail(userId) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE INDEX IF NOT EXISTS `DamageMissingEvent_itemInstanceId` ON `DamageMissingEvent` ( `itemInstanceId` );
CREATE INDEX IF NOT EXISTS `DamageMissingEvent_inputBy` ON `DamageMissingEvent` ( `inputBy` );
CREATE TABLE IF NOT EXISTS `DamageMissingEvent_`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemInstanceId` varchar ( 36 ) NOT NULL,
  `inputBy` varchar ( 32 ) NOT NULL,
  `reportedBy` varchar ( 256 ) NOT NULL,
  `discoveryDate` bigint NOT NULL,
  `eventType` integer NOT NULL DEFAULT 0 CHECK ( `eventType` >= 0 AND `eventType` <= 1 ),
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS DamageMissingEvent_timestamp AFTER UPDATE ON `DamageMissingEvent`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `DamageMissingEvent` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS DamageMissingEvent_onDelete BEFORE DELETE ON `DamageMissingEvent`
BEGIN
  INSERT OR REPLACE INTO `DamageMissingEvent_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `DamageMissingEvent` WHERE OLD.rowid=rowid;
END;


CREATE TABLE IF NOT EXISTS `DeployEvent`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemInstanceId` varchar ( 36 ) NOT NULL,
  `deployBy` varchar ( 32 ) NOT NULL,
  `deployDate` bigint NOT NULL,
  `recoverBy` varchar ( 32 ),
  `recoverDate` bigint,
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  FOREIGN KEY (itemInstanceId) REFERENCES ItemInstance(id) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (deployBy) REFERENCES UserDetail(userId) DEFERRABLE INITIALLY DEFERRED,
  FOREIGN KEY (recoverBy) REFERENCES UserDetail(userId) DEFERRABLE INITIALLY DEFERRED,
  PRIMARY KEY(`id`)
);
CREATE INDEX IF NOT EXISTS `DeployEvent_itemInstanceId` ON `DeployEvent` ( `itemInstanceId` );
CREATE INDEX IF NOT EXISTS `DeployEvent_deployBy` ON `DeployEvent` ( `deployBy` DESC, `recoverBy` DESC );
CREATE INDEX IF NOT EXISTS `DeployEvent_recoverBy` ON `DeployEvent` ( `recoverBy` );
CREATE TABLE IF NOT EXISTS `DeployEvent_`
(
  `id` varchar ( 36 ) NOT NULL,
  `itemInstanceId` varchar ( 36 ) NOT NULL,
  `deployBy` varchar ( 32 ) NOT NULL,
  `deployDate` bigint NOT NULL,
  `recoverBy` varchar ( 32 ),
  `recoverDate` bigint,
  `notes` varchar ( 256 ),
  `timestamp` bigint NOT NULL DEFAULT (CAST(strftime('%s', 'now', 'utc') AS bigint)),
  `lastSync` bigint,
  `del_timestamp` bigint NOT NULL,
  PRIMARY KEY(`id`)
);
CREATE TRIGGER IF NOT EXISTS DeployEvent_timestamp AFTER UPDATE ON `DeployEvent`
WHEN ( (NEW.`timestamp` IS NULL) OR (NEW.`timestamp` = OLD.`timestamp`) )
BEGIN
  UPDATE `DeployEvent` SET `timestamp` = (CAST(strftime('%s', 'now', 'utc') AS bigint))  WHERE NEW.rowid=rowid;
END;
CREATE TRIGGER IF NOT EXISTS DeployEvent_onDelete BEFORE DELETE ON `DeployEvent`
BEGIN
  INSERT OR REPLACE INTO `DeployEvent_` 
  SELECT *, (CAST(strftime('%s', 'now', 'utc') AS bigint)) as `del_timestamp` FROM `DeployEvent` WHERE OLD.rowid=rowid;
END;


--COMMIT;
