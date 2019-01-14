BEGIN TRANSACTION;

INSERT INTO `ItemInstance` (id,itemNumber,itemId,siteLocationId,serialNumber,grantNumber,statusId,inServiceDate,removedServiceDate,isSealBroken,hasBarcode,notes,timestamp,lastSync) VALUES 
 ('a8c80dc5-37f0-440d-bce0-c4b6d4c840ea',NULL,'933600c6-1cb8-4671-99bf-06f00f14938f','27d62928-2263-4cda-83cf-77031cea1d68',NULL,NULL,'ca7954fb-42cd-430c-9370-ce39c5615925',1528879541,NULL,0,0,'test item',1528880071,NULL);

COMMIT;
