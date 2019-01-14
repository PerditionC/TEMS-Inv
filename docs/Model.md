## Commands:

### updates model
* SearchItems - updates items collection
* LoadItem - loads complete *item* (new selected item)
* SaveItem - inserts/updates *item* 

* CreateItem
* CloneItem
* DeleteItem

-- these also update additional tables
* Damaged - updates status to Damaged
* Missing - updates status to Missing

* ReturnToInventory - updates status to Available
* SendForService - updates status to Out for Service
* ReplaceItem - updates status to Removed from Inventory and creates new replacement item.

* Deploy - updates status to Deployed
* RecoverAsAvailable - returns status to Available after deployed
* RecoverAsContaminated - returns status as Contaminated after deployed

* SetItemExpiration - sets when an item expires (annually or explicit date)
* ReplaceExpiredItem - updates expiration date, indicate new item otherwise identical replacement


### triggers flow (view) changes
* SelectItem - triggers load and updates displayed *item* to edit
* EditItem - opens *item* edit window
* PrintBarcodeLabel - opens window to print barcode
* ItemSelect - opens window to select item to perform some action with [Service, Damaged, Missing]
* History - opens window to view history [Service, Damage/Missing]
* ServiceDetails - opens window to insert / update specific service event
* ViewDocument - opens window/external program to view document

