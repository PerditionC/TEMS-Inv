Overview of database structure:
===============================

The database is normalized into the following tables:

-- Site and equipment definitions
* EquipmentUnitType - these are the different trailers, e.g. DMSU, SSU
* SiteLocation - these are the localities hosting a trailer, e.g. Norfolk
  * Note: each SiteLocation can only support one of a given trailer, if a
  locality has more than one then the name of the city should be repeated 
  but with a different suffix provided: e.g. VB1 & VB2 if Virginia Beach
  had two different SSU trailers, one would have suffix VB1 and other VB2
* SiteLocationEquipmentUnitTypeMapping - this table is a matrix of which
  trailer(s) is/are at which locality

-- User information
* UserDetail - simple table of usernames, passwords, and basic profile
  details.  Password is hashed and salted but should still not be a
  reused password as the whole DB file may not be properly protected
  allowing offline attacks.
* UserSiteMapping - determines which users can modify which locality info
* UserActivity - audit log

-- Item information

--- Reference tables:
* UnitOfMeasure
* BatteryType
* VehicleLocation
* VendorDetail
* ItemCategory
* ItemStatus

--- Item information (the meat of the program/DB)
* ItemType - one record for each unique item, e.g. traffic cone
* Item - one record for each unique position of item in a given trailer, e.g.
  one for traffic cones in front corner of trailer and one for extra cone
  stored in back of trailer
* ItemInstance - one record for each Item at each locality, i.e. one record
  for each physical item on a trailer.  There is an ItemInstance record for the
  traffic cone in back of trailer for Norfolk and a different record for the
  cone in the same back of trailer in same trailer type but at Suffolk

--- optional additional information stored for items
* Image - maintains a table of attached picture files, e.g. to see what an ItemType is
* Document - maintains a table of attached other documents, e.g. user manuals

 -- Service and usage events
* ServiceCategory - reference for different types of service activities
* ItemService - tracks all service activities that need to occur
* ItemServiceHistory - log of all service activities that have occurred
* DamagedMissingEvent - tracks damage and loss of items
* DeployEvent - tracks deployment and restocking of items


See TEMS_Inv.db.sql for detailed schema DDL to create a blank DB.
Additionally, the included Add_#_*.sql files can be used to provide the initial data.

---

The database, and corresponding application, is designed to run both in normal
times and when an event has occurred requirement deployment of the trailers.
In such case there may not be Internet access or at least to a centralized 
server.  Therefore the database is designed and application can perform
synchronization / replication from a disconnected state assuming a copy of
the updated database files can be transported by some other means, e.g. a USB
flash drive, online file sharing, SMB/NFS share drive access, etc.  In this
offline mode, there can be any number of replicas and in any state of 
sychronization.  Currently last change wins if there is a conflict with an
update.  The only potential conflich that can not be automatically handled
is if two or more sites add new items to the ItemType table an assigne the
same ItemTypeId to differing items.  Although not currently provided, the
database also supports online mode - a centralized server can be used to 
keep all remote databases in sync, allowing realtime updates from other
locations.  The lastSync field is reserved for this use case.  Note: a DB
file can be used for both offline and online mode alternating as often as needed.

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
