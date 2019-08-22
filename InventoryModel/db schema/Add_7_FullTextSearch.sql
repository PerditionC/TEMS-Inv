-- we create a FTS5 full text search virtual table
-- then populate with variations of names, notes, and ItemNumbers along with corresponding GUID id and matching table name
-- then we add triggers so as data is updated the FTS tables are kept in sync
-- these are then used to query for items based on ItemNumber or any combination of name and notes
CREATE VIRTUAL TABLE searchText USING fts5(content, id UNINDEXED, tableName UNINDEXED, limitTo UNINDEXED, prefix='2 3 4 5 6 7 8');
-- search ItemType rows by name and notes
INSERT INTO searchText(content, id, tableName, limitTo) SELECT COALESCE(ItemType.name, '') || COALESCE(' :: ' || ItemType.notes, ''), ItemType.id AS id, 'ItemType', NULL FROM ItemType;
-- search Item rows by itemNumber (itemType#-item#) or by name and notes
INSERT INTO searchText(content, id, tableName, limitTo) SELECT (EquipmentUnitType.unitCode || ItemType.itemTypeId || '-' || Item.itemId) AS itemNumber, Item.id AS id, 'Item', 'itemNumber' FROM
ItemType INNER JOIN (Item INNER JOIN EquipmentUnitType ON Item.unitTypeName=EquipmentUnitType.name) ON Item.itemTypeId=ItemType.id;
INSERT INTO searchText(content, id, tableName, limitTo) SELECT COALESCE(ItemType.name, '') || COALESCE(' :: ' || ItemType.notes, '') || COALESCE(' :: ' || Item.notes, ''), Item.id AS id, 'Item', NULL FROM
ItemType INNER JOIN (Item INNER JOIN EquipmentUnitType ON Item.unitTypeName=EquipmentUnitType.name) ON Item.itemTypeId=ItemType.id;
-- search ItemInstance by itemNumber (itemType#-item#Suffix) or by name and notes
INSERT INTO searchText(content, id, tableName, limitTo) SELECT itemNumber, id, 'ItemInstance', 'itemNumber' FROM ActiveItemNumbers;
INSERT INTO searchText(content, id, tableName, limitTo) SELECT COALESCE(ItemType.name, '') || COALESCE(' :: ' || ItemType.notes, '') || COALESCE(' :: ' || Item.notes, '') || COALESCE(' :: ' || ItemInstance.notes, ''), ItemInstance.id AS id, 'ItemInstance', NULL FROM
ItemInstance INNER JOIN (ItemType INNER JOIN (Item INNER JOIN EquipmentUnitType ON Item.unitTypeName=EquipmentUnitType.name) ON Item.itemTypeId=ItemType.id) ON ItemInstance.itemId=Item.id;
-- search by name only (no notes)
INSERT INTO searchText(content, id, tableName, limitTo) SELECT COALESCE(ItemType.name, ''), ItemType.id AS id, 'ItemType', 'name' FROM ItemType WHERE NOT (ItemType.name IS NULL);
INSERT INTO searchText(content, id, tableName, limitTo) SELECT COALESCE(ItemType.name, ''), Item.id AS id, 'Item', 'name' FROM
ItemType INNER JOIN (Item INNER JOIN EquipmentUnitType ON Item.unitTypeName=EquipmentUnitType.name) ON Item.itemTypeId=ItemType.id WHERE NOT (ItemType.name IS NULL);
INSERT INTO searchText(content, id, tableName, limitTo) SELECT COALESCE(ItemType.name, ''), ItemInstance.id AS id, 'ItemInstance', 'name' FROM
ItemInstance INNER JOIN (ItemType INNER JOIN (Item INNER JOIN EquipmentUnitType ON Item.unitTypeName=EquipmentUnitType.name) ON Item.itemTypeId=ItemType.id) ON ItemInstance.itemId=Item.id WHERE NOT (ItemType.name IS NULL);
-- now that initial data is populated go ahead and merge b-trees
INSERT INTO searchText(searchText) VALUES('optimize');
--select * from searchText;
--SELECT id FROM searchText WHERE tableName='ItemInstance' AND content MATCH('my query ...');
-- for autofill purposes with ?? characters known
--SELECT *, rank FROM searchText WHERE limitTo='name' AND tableName='Item' AND content match '??' || '*' ORDER BY rank DESC LIMIT 5;
