BEGIN TRANSACTION;

INSERT INTO `EquipmentUnitType` (name,description,unitCode) VALUES 
 ('MMRS','MMRS - Metropolitan Medical Response System, Strike Team Module','M'),
 ('DMSU','DMSU - Disaster Medical Support Unit','D'),
 ('SSU','SSU - Shelter Support Unit','S');

INSERT INTO `SiteLocation` (id,name,locSuffix) VALUES 
 ('1a14004a-1a56-4880-a74a-bf907f12632e','Chesapeake','CFD'),
 ('b20d0707-4647-43ad-8320-3be1fcc444a1','Franklin','FFD'),
 ('6f625ab9-e838-4907-ad49-0607881d27f4','Gloucester','GFR'),
 ('585bed63-b504-42b3-a10b-f40bdc50a593','Hampton','HFR'),
 ('c7082510-0d77-4cce-a9a5-be70c540e7e4','Isle of Wight','IOW'),
 ('0378798e-ff90-49bb-ba1d-198b8c32c098','James City County','JCC'),
 ('263813a1-94da-4a9f-ad5e-3b4341be20a2','Newport News','NNFD'),
 ('27d62928-2263-4cda-83cf-77031cea1d68','Norfolk','NFR'),
 ('a3de206a-4a44-48a0-ad4e-8b959840ccdb','Portsmouth','PFR'),
 ('939f5284-8bcb-4fca-8eaa-d6414b5d0d03','Suffolk','SFD'),
 ('dd7ac889-c154-43f4-bbeb-f75cab3749c5','Virginia Beach','VBEMS'),
 ('54ec2e2a-bdca-4eaf-8900-7051c9b1d6d0','Virginia Beach 1','MCI1'),
 ('45a52372-94ef-4e02-aa75-8e95e0b0c999','Virginia Beach 2','MCI2'),
 ('b5b64d81-4081-41a8-b604-297a3cf3fc6e','Williamsburg','WFD'),
 ('e4020041-b468-4441-a025-e60455b502c7','York County','YCFLS'),
 ('a75b74bd-d270-48fd-9797-9b222f376077','York County','YCMRS');

-- EquipmentUnitType and SiteLocation tables should be populated
INSERT INTO `SiteLocationEquipmentUnitTypeMapping` (id,siteId,unitName) VALUES 
 ('317df6fb-cd37-4187-a715-468aa13c5ba9','1a14004a-1a56-4880-a74a-bf907f12632e','DMSU'),
 ('7713202c-2435-4160-b3e6-ef79f220ee40','1a14004a-1a56-4880-a74a-bf907f12632e','SSU'),
 ('30f65044-2b54-4905-94f1-1557da491cc1','b20d0707-4647-43ad-8320-3be1fcc444a1','SSU'),
 ('5904858f-3a28-493a-911e-627b869b17e0','6f625ab9-e838-4907-ad49-0607881d27f4','SSU'),
 ('96886c35-aaf9-4df2-b803-1ad2e9d8162c','585bed63-b504-42b3-a10b-f40bdc50a593','DMSU'),
 ('cdd1af15-5fff-49b5-8259-39d10e121bb2','585bed63-b504-42b3-a10b-f40bdc50a593','SSU'),
 ('8cecaf99-df31-47d6-a576-de8a24d040fc','c7082510-0d77-4cce-a9a5-be70c540e7e4','DMSU'),
 ('51ed7470-7aa8-4348-91e2-057457975ed3','c7082510-0d77-4cce-a9a5-be70c540e7e4','SSU'),
 ('ba26fbe7-36dc-4763-a1db-e6ad100e47bc','0378798e-ff90-49bb-ba1d-198b8c32c098','DMSU'),
 ('b8313656-8ab3-40cf-818f-b4958d0ccb83','0378798e-ff90-49bb-ba1d-198b8c32c098','SSU'),
 ('22acda80-1ada-450a-a5e8-b31ad61843c5','263813a1-94da-4a9f-ad5e-3b4341be20a2','DMSU'),
 ('76cea8fd-62ef-41c8-b6bc-81036e3c6495','263813a1-94da-4a9f-ad5e-3b4341be20a2','SSU'),
 ('dcf1adfd-2a2c-45cb-85cd-9f98e27eda0f','27d62928-2263-4cda-83cf-77031cea1d68','DMSU'),
 ('17bfaebd-01ee-4a06-b128-ece40742693b','27d62928-2263-4cda-83cf-77031cea1d68','MMRS'),
 ('9816e23a-539d-4e47-82aa-a326da44f950','27d62928-2263-4cda-83cf-77031cea1d68','SSU'),
 ('5ae52942-f5f5-43d8-b9ec-171652d284bf','a3de206a-4a44-48a0-ad4e-8b959840ccdb','DMSU'),
 ('817f8c89-fc21-49e2-b5f9-3ad681b80ff6','a3de206a-4a44-48a0-ad4e-8b959840ccdb','SSU'),
 ('60f81cbf-2cfc-4e9b-b47c-3362e4966466','939f5284-8bcb-4fca-8eaa-d6414b5d0d03','DMSU'),
 ('e0a89ab1-4fdd-4a46-bef7-44b43ae176bf','939f5284-8bcb-4fca-8eaa-d6414b5d0d03','SSU'),
 ('bbad9d50-eea2-41f4-b84f-5809713a8a2b','dd7ac889-c154-43f4-bbeb-f75cab3749c5','SSU'),
 ('d3e12c1e-989a-4e2f-a134-b183ede521a7','54ec2e2a-bdca-4eaf-8900-7051c9b1d6d0','DMSU'),
 ('c55297d0-c14f-4c2b-b499-3a463700e562','45a52372-94ef-4e02-aa75-8e95e0b0c999','DMSU'),
 ('1c04fc95-5548-40a4-8719-534cbac9e9f9','b5b64d81-4081-41a8-b604-297a3cf3fc6e','SSU'),
 ('3bf444e3-9d99-4598-b5e0-b56be6fb4bdf','e4020041-b468-4441-a025-e60455b502c7','DMSU'),
 ('8908c2ac-164a-4497-87b8-c7881ce1b359','e4020041-b468-4441-a025-e60455b502c7','SSU'),
 ('6d7fed72-fee2-4cc2-830f-cfcc3f6b3cf8','a75b74bd-d270-48fd-9797-9b222f376077','MMRS');

 COMMIT;
