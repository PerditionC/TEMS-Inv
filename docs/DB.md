Overview of database structure:
===============================

The database is normalized into the following tables:

-- Site and equipment definitions
* EquipmentUnitType
* SiteLocation
* SiteLocationEquipmentUnitTypeMapping

-- User information
* UserDetail
* UserSiteMapping
* UserActivity

-- Item information
* Image
* Document
* UnitOfMeasure
* BatteryType
* VehicleLocation
* VendorDetail
* ItemCategory
* ItemStatus
* ItemType
* Item
* ItemInstance

 -- Service and usage events
* ServiceCategory
* ItemService
* ItemServiceHistory
* DamagedMissingEvent
* DeployEvent

See TEMS_Inv.db.sql for detailed schema DDL to create a blank DB.
Additionally, the included Add_#_*.sql files can be used to provide the initial data.

---

  Each of tables exist along with a shadow table which
contains all deleted rows [unless purged].

   The shadow table has same name with a _ suffix and
 the same columns as primary table with an additional
 `del_timestamp` column indicating when the row was 
 deleted from the primary table and added to shadow 
 table.
 
  All primary keys contain a text representation of
 a GUID unless the pk is naturally globally unique - 
 this is required to allow proper replication of 
 disconnected clients [i.e. AUTONUMBER INTEGER rowids
 can not be used or distinct changes may clash when
 synchronizing client databases].

   All timestamp fields are stored as a bigint INTEGER
 alias (minimal 64 bit integer assumed).  We create a
 timestamp column for every table that we want to be
 replicated - this ensures we can use a last change
 wins policy for conflicts and other similiar scenarios.

   All date/times are stored as a bigint INTEGER alias
 assuming local time.  It is expected that all users
 are in the same timezone.  All date/times are when
 an event did or is going to occur or are a relative
 time frame of when the next event should occur, e.g.
 6 weeks from some arbitrary date.

   All strings are stored in UTF8 encoding [mostly ASCII]
 and lengths provided are the assumed length for any
 programatic manipulation (i.e. display, copy, etc).
 Note: SQLite does not enforce length but the program
 does and may truncate any data larger than specified.

   Blob data is of unspecified length and generally will
 contain a complete embedded file such that if written
 to disk with no changes will be a valid file of expected
 format (image file, word processor document, ...)

   All foreign keys have an index, see SQLite foreign keys
 documentation, recommended as those columns queried if
 parent table row is deleted.  Contraints of foreign keys
 are deferred until the transaction is committed, this
 relaxes the order rows must be inserted into the DB
 which may help avoid some replication issues (since we
 have limited control over order tables processed during
 synchronization).  Check is done immediately if not
 currently within a TRANSACTION block.  Note that 
 ON UPDATE CASCADE clause is not used as the primary key
 of the parent table must not change if replicas are to
 see changes to the row - therefore if a pk must be 
 changed it must be done via an add parent row, update
 child row, and delete original row so the replicas can
 also see and make the changes.  

   Note that NOT NULL is required on the primary keys, 
 otherwise SQLite will allow NULL pk values due to backward 
 compatibility with earlier versions.  Additionally to
 ensure the current transaction and not just current SQL
 statement is aborted (rolled back), the INSERT or UPDATE
 statement should add either OR ROLLBACK or OR REPLACE
 as needed instead of default OR ABORT; do NOT add to
 the schema a conflict resolution on constraints of
 ON CONFLICT ROLLBACK; this will cause an error during
 replication as the changeset apply triggers a conflict
 causing unexpected rollback to occur.

   On each UPDATE we invoke a trigger that sets the row''s
 [last modified] timestamp value.  To allow the timestamp to be 
 explicitly set (e.g. when replicating, we use the original 
 timestamp instead of when the replication modified the row), 
 we only modify the timestamp if new value is NULL or the same 
 as the old value (i.e. not provided or unchanged).  
 635019330320182000 ticks to Jan 1, 1970
 Note: to avoid needing extra math and cross platform issues,
 the timestamp is stored as a simple milliseconds since the
 Unix epoch (nanoseconds, i.e. ticks) are not needed.

   On each DELETE we invoke a trigger that will either add
 a new row to shadow table with values prior to deleting row 
 and set del_timestamp to current date & time or if the row
 was previously deleted [and restored] then we replace values
 in shadow table with most recent values and update the 
 del_timestamp to current date & time. 
  
   For synchronization with a !central db only! there is a
 field `lastSync` which contains the timestamp of when
 this item last sync'd; if NULL or before timestamp then
 this row has changes that need to be be sent to central db#
 two methods of sync'ing are used, one to central db service
 if available and computer has networking; second is manual
 sync'ing directly with another sqlite db (both can be used,
 i.e. online computers maintain up to date via connecting to
 central db; if central db is not available either due to 
 local issues or issues with server then db can be manually
 sync'd with other locations by direct method; and then back
 to server if connectivitely returns)


-- PRAGMA foreign_keys = "1";
