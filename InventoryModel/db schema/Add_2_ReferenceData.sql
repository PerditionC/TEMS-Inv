BEGIN TRANSACTION;

INSERT INTO `UnitOfMeasure` (id,name) VALUES 
 ('f3cbed82-b264-453a-90c1-1269c5ce4a8b','Box'),
 ('597bf084-d136-4377-8d2c-e38e34007050','Each'),
 ('d0fcd84c-2974-41f5-96f5-844d62c1de4b','Lb.'),
 ('cb928dbb-346d-400e-a6b0-fe8932382268','Pair');

INSERT INTO `BatteryType` (id,name) VALUES 
 ('584da062-0a37-4a6d-9d8b-202e202e202e','None'),
 ('6f37f79e-a4c0-4456-93ad-203220322032','3V CR2032'),
 ('a6d608d8-8c56-49cf-b5c0-075707570757','7.5V'),
 ('b1875d9b-5d5c-45a5-b9a5-009700970097','9V'),
 ('aa1322c9-655b-42ef-a361-00aa00aa00aa','AA'),
 ('9e2a4735-9eef-4908-a6e1-0aaa0aaa0aaa','AAA'),
 ('88ecd5db-bd06-4f37-907d-000c000c000c','C'),
 ('97b9b40d-e4e1-4f44-bd60-000d000d000d','D');

INSERT INTO `VehicleLocation` (id,name) VALUES 
 ('2504bf72-36bc-4953-8cd4-a15390247441','Comms Trailer'),
 ('fb5132ec-e043-4476-951f-076ae8ac65ac','Dodge 3500 Truck'),
 ('d5ddd86f-4a2b-494e-b348-02bf1da192f2','F650 Truck'),
 ('d38dc4a2-f4e6-4bcf-92a0-99e13afd7409','Logistics Truck'),
 ('f9c19eea-12d4-4920-bc2a-cf0c16c41a26','None / NA'),
 ('59685701-713d-492f-8199-5a05294c0faa','Trailer');

INSERT INTO `ItemCategory` (id,name) VALUES 
 ('4280c2bd-04fd-4536-bb78-4442e318241f','Treatment'),
 ('5b5c4ac4-12a6-4141-9e6f-4442fd28f315','Decon'),
 ('a11bfe19-5c48-478e-a48b-ab8d74b3c998','Documentation'),
 ('5b22f88e-3186-4fc3-8633-f809690f5aa7','Assessment'),
 ('9263761f-a7cc-4b9a-907d-0a5d4f790420','PPE'),
 ('5a985f50-0856-4219-be0e-6ab73c29f0e9','Crew Care'),
 ('9857dc8e-c27d-4a6b-8111-50c9818165b9','Rehabilitation'),
 ('92eb6787-944b-4f9d-a439-c7e24e53f840','Shelter'),
 ('1736c77c-d860-4fac-ba37-43bd90665a82','Sanitation'),
 ('05f9b71b-a8c5-44c9-8f55-1ccf9a3efa52','Command'),
 ('4a248a11-054b-481a-8610-b2aa01bb9dc0','Transportation'),
 ('217427dd-1844-445f-8ff1-1aeffb423459','Triage'),
 ('34c9d09b-6d21-436a-bf48-3180e93d6096','Tools/Utilities'),
 ('9a3c7e15-d79f-45cf-b93e-35abb039d4e8','Treatment Boxes'),
 ('e10be1df-2349-43d1-befb-acd01868e9b3','Monitor/Detection');

INSERT INTO `ItemStatus` (id,name) VALUES 
 ('ca7954fb-42cd-430c-9370-ce39c5615925','Available'),
 ('009c4bf9-0876-4b9e-b25a-c6296b436554','Contaminated'),
 ('13e9295e-bcff-458e-ac0e-c13359a8003a','Out for Service'),
 ('b248f8d8-bbd3-4730-89f5-e93bfc62bfe2','Removed From Inventory'),
 ('b2ea067e-0473-462a-9db0-892b4af6cfbc','Deployed'),
 ('f991873f-f685-4fec-ab0a-2751147ee4b9','Damaged'),
 ('7ccc3403-b98e-4d4d-9587-8fcd27487589','Missing');

INSERT INTO `ServiceCategory` (id,name) VALUES 
 ('9d52e316-8cc8-4c06-86bf-62c7edabf07c','Calibration'),
 ('d75f5961-ca40-467a-8f49-ca4373a0ce2d','Hydrostatic_Test'),
 ('e97f92d2-d48e-4b49-b791-4c993efc8d5b','Battery Conditioning'),
 ('b6351664-45f3-444c-bdcb-f34a7a36e642','Battery Replacement'),
 ('a02a0697-5d9a-47bd-a781-d0b736e76606','Filter Replacement'),
 ('7201fe0c-314a-4202-b3bd-e85ffcf294e5','Refill'),
 ('451c67f9-0262-4562-80d6-454da8b8fecc','Rotate Stock'),
 ('112c514f-9742-4234-a61a-55e3e62c4636','Replace'),
 ('95193815-2629-4020-b524-6142efad8e1a','Replace Test Strips'),
 ('2e0f8426-8d4d-40b8-99bf-c2579945a38e','Clean'),
 ('8a2c4c98-ecc7-4a90-99af-0c86277a364c','Inspection/PM'),
 ('ba1bcc13-0619-4e4e-ae50-ec3129a9f53e','Flow Test');

INSERT INTO `VendorDetail` (id,name,category,addressLine1,addressLine2,city,state,zipcode,phoneNumber,faxNumber,website,isActive,contactName,contactPhoneNumber,contactEmail,accountReference) VALUES 
 ('10b7e34d-b283-4512-8d7d-7b44da214bd9','Aramsco','','1000 Parliament Ct Suite 500','','Durham','NC','27703','856-686-7700','856-686-4439','www.aramsco.com',1,'Myles Farley','941-6368','mfarley@aramsco.com','23678'),
 ('3eb51d2a-1eb9-4345-9c72-15efe83e7f22','BoundTree','','223','','','','23227','','','',1,'','','',''),
 ('b5bc149b-fe08-4292-8825-e08b9cd796d0','Centennial Products, Inc.','','Po Box 23905','','Jacksonville','FL','32241','904-322-0404','','http//www.centennialproducts.com',1,'Lisa Smith','','',''),
 ('d8970bd0-c4f2-4395-8334-9eba518f0100','Columbia Business Forms','','PO Box 1329','','Columbia','SC','29202','','803-772-6754','',1,'Mike Pulaski','772-6746 x 2','mpulaski@mindspring.com','5725'),
 ('86eba669-3c2b-48be-a74c-b999e6a812cc','Cool Draft Scientific, Inc.','','66059 McGregor Rd.','','Bellaire','OH','43906','866-676-1636','740-676-1728','www.cooldraft.com',1,'Amy','','',''),
 ('dffe05c6-fe5e-4163-9f26-a214a6f44c9d','Disaster Response Solutions','','PO Box 193','','Milford','OH','45150','513-290-6130','513-831-0489','www.mcitrailer.com',1,'Dan Mack','','mcitrailer@yahoo.com',''),
 ('8ccfa4e4-cf16-431d-97e2-ec3d15b36829','Emergency Medical Products, Inc.','','1711 Paramount Court','','Waukesha','WI','53186','','800-558-1551','www.BuyEMP.com',1,'','','','76593'),
 ('1f19f032-c468-4cbb-adc2-e09e17c51ffc','Emergency Medical Supply, Inc.','','385 East Drive','','Melbourne','FL','32904','321-308-2997','321-308-2930','www.goemsusa.com',1,'David Hammond','443-4962','davidh@emscorp.com',''),
 ('94ca1431-c07f-4210-b529-93d7651e07cb','E-Z UP Direct','','2275 La Crosse Avenue # 112','','Colton','CA','92324','909-433-2331','911-426-0065','www.ezupdirect.com',1,'Jeanine Facen','','',''),
 ('fb504b68-9c30-4f3e-b1b9-f11786057343','FERNO','','70 Weil Way','','Wilmington','OH','45177','800-733-3766','888-388-1349','www.ferno.com',1,'Kevin Brosi','992-1911','',''),
 ('0e230fba-333c-446e-94cc-c48612e79a69','Fire Protection Equipment','','7206 Impala Dr.','','Richmond','VA','23228','800-296-5594','804-262-1594','',1,'Herb Redfield','','',''),
 ('51136303-f5c0-4720-b3a7-1d03187aff04','Fischer Scientific','','3970 John`s Creek Court Suite 500','','Suwanee','GA','30024','800-226-4732','800-897-9946','www.fishersci.com',1,'Tracey Cornett','','',''),
 ('7acf665d-b71f-4377-9556-46fc096b1e42','Fisher Scientific Company','','3976 John`s Creek Court Suite 500','','Suwanee','GA','30024','800-226-4738','806-897-9952','www.fishersci.com',1,'Tracey Cornett','','tracy.cornett@thermofisher.com',''),
 ('61740f95-4456-4b5b-a008-9201c6b42785','Flambeau','','','','','','','','','',1,'','','',''),
 ('226c9eb7-1ac5-4230-946a-6b00b6559c1d','Fuerte Cases','','10767 Puebla Drive','','La Mesa','CA','91941','619-444-3583','801-705-2552','www.fuertecases.com',1,'Mike Geck','','mikeg@fuertecases.com',''),
 ('98fdb534-cc80-4069-ab7a-f97012088d18','Galls','','2680 Palumbo Dr.','','Lexington','KY','40509','800-477-7766','800-944-2557','www.galls.com',1,'','','','58853136'),
 ('f844f8c3-ef08-4a2a-bb41-1affd137e346','Ground Control','','3101 El Camino Real','','Atascadero','CA','93422','800-773-7169','806-542-0689','',1,'','','',''),
 ('48cf4c20-0b64-41b5-89b8-9277c8ace72d','International Paper','','723 Fenway Avenue','','Chesapeake','VA','23323','757-487-2506','757-485-4270','',1,'Cindy Iman','','cindy.iman@ipaper.com',''),
 ('2d4343a5-ef34-4071-ae1a-2738fb5d5d2c','Lionville','','','','','','','','','',1,'','','',''),
 ('dd2239cf-cffc-45b2-8c1c-e9ef404d25ef','Lowes','','1081 N. Military highway','','Norfolk','VA','23502','757-455-5205','757-455-8982','www.lowes.com',1,'','','',''),
 ('98c1f9c6-d5a6-4d04-a7d4-60bb97912e3b','MedTronic','','11811 Willows Road NE','PO Box 97048','Redmond','WA','98052','425-867-4000','425-867-4948','',1,'Carl Spruil','','',''),
 ('c3836acb-2208-44e4-9476-df712fbc2c49','Motorola','','','','','','','','','',1,'','','',''),
 ('0ab89e29-a1b6-4c2f-9643-f9bb611ba61c','Northern Safety Co.','','PO Box 4250','','Utica','NY','13504','800-631-1246','800-635-1591','www.northernsafety.com',1,'','','','10056703'),
 ('a29653fe-5cbb-407c-9f7c-a5b3d85a9ac7','OfficeMax','','263 Shuman Blvd','','Naperville','IL','60563','630-438-7800','877-969-1629','www.officemaxsolutions.com',1,'','','',''),
 ('1922e2e7-3a32-494c-82e8-7a2da9498605','Safeware, Inc','','5641 Laburnum Ave','','Richmond','VA','23231','804-222-1238','804-226-0987','www.Safewareinc.com',1,'Mandy Hough','222-1238','mhough@safewareinc.com',''),
 ('7e22c43a-aa28-4412-ad96-9f9bf621bcc9','Sam`s Club','','3357 Virginia Beach Blvd','','Virginia Beach','VA','23452','757-631-9803','','www.samsclub.com',1,'','','',''),
 ('a5cdee73-02ba-448d-ac2f-cdedeb96a727','Southeastern Emergency Equipment','','PO Box 1197','','Wake Forest','NC','27588','800-334-6657','889-556-1049','www.seequip.com',1,'Rhoda Holbrook','','',''),
 ('c93ec426-3cd8-4372-8829-4c9fbc6c8da1','Sovereign Medical','','4221 McKee School Rd','','Hurdle Mills','NC','27541','919-621-0379','919-644-2805','',1,'Wayne Grooters','621-0379','Wayne@SovMed.com',''),
 ('ec15bce7-9b78-4950-964f-62cc1aa34ea5','The 50 Degree Company','','315 Stan Drive Unit 1','','West Melbourne','FL','32904','321-254-5006','321-956-0988','www.50degree.com',1,'Steve Miller','937-7327','sales50degree@aol.com',''),
 ('5e8bdfe2-c623-4a97-bb67-fe810b8f9f82','TheFireStore.com','','104 Independence Way','','Coatesville','PA','19320','800-852-6088','888-335-9800','www.TheFireStore.com',1,'','','sales@thefirestore.com',''),
 ('c78f38e8-21a7-4f23-9f7f-8e0d9392ad07','Tri-Anim','','13170 Telfair Avenue','','Sylmar','CA','91342','818-362-6882','703-940-0395','www.tri-anim.com',1,'Dan Blom','347-1147','D.Blom@Tri-anim.com','46683'),
 ('0f64f99c-191d-40c4-b684-84ff104a4af7','Welch Allyn','Monitors','','','','','','','','',1,'','','',''),
 ('21853157-1b3f-4117-805d-32a8666e911d','Western Shelter Systems','','830 Wilson Street','','Eugene','OR','97402','800-971-7201','541-284-2820','www.westernshelter.com',1,'JJ Urhausen','','jjurhausen@westernshelter.com','');

COMMIT;
