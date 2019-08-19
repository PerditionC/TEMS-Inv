Attribute VB_Name = "GenSQL"
' VBA source file - import into Excel with corresponding formatted sheets, will generate the SQL source data for initial ItemType, Item, & ItemInstance tables
Option OnExplicitOn

Private Enum ExpireRestockCategory
    None = 0
    AnnualRestock = 1
    DateSpecificRestock = 2
End Enum


' ***************************************************************************
' Generate a GUID in VBA taken from:
' http://www.cpearson.com/excel/CreateGUID.aspx

Private Declare Function CoCreateGuid Lib "OLE32.DLL" (pGuid As GUID) As Long

Private Type GUID
    Data1 As Long
    Data2 As Integer
    Data3 As Integer
    Data4(0 To 7) As Byte
End Type

Public Function CreateGUID() As String
    Dim G As GUID
    With G
    If (CoCreateGuid(G) = 0) Then
    CreateGUID = _
        String$(8 - Len(Hex$(.Data1)), "0") & Hex$(.Data1) & "-" & _
        String$(4 - Len(Hex$(.Data2)), "0") & Hex$(.Data2) & "-" & _
        String$(4 - Len(Hex$(.Data3)), "0") & Hex$(.Data3) & "-" & _
        IIf((.Data4(0) < &H10), "0", "") & Hex$(.Data4(0)) & _
        IIf((.Data4(1) < &H10), "0", "") & Hex$(.Data4(1)) & "-" & _
        IIf((.Data4(2) < &H10), "0", "") & Hex$(.Data4(2)) & _
        IIf((.Data4(3) < &H10), "0", "") & Hex$(.Data4(3)) & _
        IIf((.Data4(4) < &H10), "0", "") & Hex$(.Data4(4)) & _
        IIf((.Data4(5) < &H10), "0", "") & Hex$(.Data4(5)) & _
        IIf((.Data4(6) < &H10), "0", "") & Hex$(.Data4(6)) & _
        IIf((.Data4(7) < &H10), "0", "") & Hex$(.Data4(7))
    End If
    End With
End Function

' ***************************************************************************


' *** RUN ME ***
' generates all 3 tables, ItemTypes, Items, and ItemInstances
' Note: any existing file with these names will be OVERWRITTEN!!!
Public Sub GenSqlData()
    Debug.Print "Item Types"
    GenerateItemTypeTable "C:\DB\ItemTypes.sql"
    
    Debug.Print "Items"
    GenerateItemTable "C:\DB\Items.sql"
    
    Debug.Print "Item Instances"
    GenerateItemInstanceTable "C:\DB\ItemInstances.sql"
    
    Debug.Print "Done"
    MsgBox "Done", vbOKOnly
End Sub



' ************** Implementation details **************************************

' Opens file for dumping SQL, writes out header, and returns file handle
Private Function WriteHeader(ByRef filename As String, ByRef header As String) As Integer
    ' get an unused file handle
    Dim outFile As Integer
    outFile = FreeFile
    Open filename For Output Access Write Shared As #outFile Len = 1
    Print #outFile, "BEGIN TRANSACTION;"
    Print #outFile, ' blank line
    Print #outFile, header
    WriteHeader = outFile
End Function

' writes out footer and closes file handle
Private Sub WriteFooter(ByVal outFile As Integer)
    Print #outFile, ";"       ' terminate INSERT statement
    Print #outFile,           ' blank line
    Print #outFile, "COMMIT;" ' and ensure changes written to DB
    Close #outFile
End Sub



' returns string wrapped in single quotes, any embedded single quotes are escaped by doubling, i.e. ' -> ''
' WARNING: value should not be blank
Private Function GetQuotedString(ByRef value As String) As String
    GetQuotedString = "'" & Replace(value, "'", "''") & "'"
End Function

' returns either the string wrapped in single quotes or NULL value, if not NULL any embedded single quotes are escaped by doubling, i.e. ' -> ''
Private Function GetNullQuotedString(ByRef value As String) As String
    If Len(Trim(value)) = 0 Then
        GetNullQuotedString = "NULL"
    Else
        GetNullQuotedString = "'" & Replace(value, "'", "''") & "'"
    End If
End Function


' Outputs ItemType data to specified file
Private Sub GenerateItemTypeTable(ByRef filename As String)
    Dim fHandle As Integer
    fHandle = WriteHeader(filename, "INSERT INTO `ItemType` (id,itemTypeId,name,make,model,cost,weight,unitOfMeasureId,itemCategoryId,batteryCount,batteryTypeId,associatedItems,expirationRestockCategory,isBin,isModule,vendorId,notes,timestamp,lastSync) VALUES")
    WriteItemTypeTable fHandle
    WriteFooter fHandle
End Sub


' Outputs a single VALUE row for ItemType table
' id GUID, itemTypeId INTEGER, name TEXT, make TEXT, model TEXT, cost INTEGER, weight REAL, unitOfMeasureId GUID, itemCategoryId GUID, batteryCount INTEGER, batteryTypeId GUID, associatedItems TEXT, expirationRestockCategory ENUM, isBin BOOL, isModule BOOL, vendorId GUID, notes TEXT
Private Sub WriteItemTypeRow(ByVal outFile As Integer, id As String, itemTypeId As Integer, name As String, Optional make As String = "", Optional model As String = "", Optional cost As Long, Optional weight As Double = 0#, Optional unitOfMeasureId As String = "", _
                                                       Optional itemCategoryId = "", Optional batteryCount As Integer = 1, Optional batteryTypeId As String = "", Optional associatedItems As String = "", _
                                                       Optional expirationRestockCatagory As ExpireRestockCategory = ExpireRestockCategory.None, Optional isBin As Boolean = False, Optional isModule As Boolean = False, Optional vendorId As String = "", Optional notes As String = "")
    Print #outFile, "(" & GetQuotedString(LCase(id)) & "," & CLng(itemTypeId) & "," & GetQuotedString(name) & "," & GetQuotedString(make) & "," & GetQuotedString(model) & "," & CLng(cost) & "," & Format(weight, "0.0") & "," & _
                    GetNullQuotedString(LCase(unitOfMeasureId)) & "," & GetNullQuotedString(LCase(itemCategoryId)) & "," & CLng(batteryCount) & "," & GetNullQuotedString(LCase(batteryTypeId)) & "," & _
                    GetNullQuotedString(associatedItems) & "," & CLng(expirationRestockCatagory) & "," & IIf(isBin, "1", "0") & "," & IIf(isModule, "1", "0") & "," & GetNullQuotedString(LCase(vendorId)) & "," & GetNullQuotedString(notes) & ",1528869237,NULL)";
End Sub
                                                       

' Outputs Item data to specified file
Private Sub GenerateItemTable(ByRef filename As String)
    Dim fHandle As Integer
    fHandle = WriteHeader(filename, "INSERT INTO `Item` (id,itemId,itemTypeId,unitTypeName,vehicleLocationId,vehicleCompartment,count,bagNumber,expirationDate,parentId,notes,timestamp,lastSync) VALUES")
    WriteItemTable fHandle
    WriteFooter fHandle
End Sub


' Outputs a single VALUE row for Item table
' id GUID, itemId INTEGER, itemTypeId GUID, unitTypeName TEXT, vehicleLocationId GUID, vehicleCompartment TEXT, count INTEGER, bagNumber TEXT, expirationDate DATE, parentId GUID, notes TEXT
Private Sub WriteItemRow(ByVal outFile As Integer, id As String, itemId As Integer, itemTypeId As String, unitTypeName As String, Optional vehicleLocationId As String = "", Optional vehicleCompartment As String = "", Optional count As Integer = 1, _
                                                   Optional bagNumber As String = "", Optional expirationDate As String = "", Optional parentId As String = "", Optional notes As String = "")
    Print #outFile, "(" & GetQuotedString(LCase(id)) & "," & CLng(itemId) & "," & GetQuotedString(LCase(itemTypeId)) & "," & GetQuotedString(UCase(unitTypeName)) & "," & GetNullQuotedString(LCase(vehicleLocationId)) & "," & GetQuotedString(vehicleCompartment) & "," & _
                    CLng(count) & "," & GetNullQuotedString(bagNumber) & "," & GetNullQuotedString(expirationDate) & "," & GetNullQuotedString(LCase(parentId)) & "," & GetNullQuotedString(notes) & ",1528869237,NULL)";
End Sub
                                                   

' Outputs ItemInstance data to specified file
Private Sub GenerateItemInstanceTable(ByRef filename As String)
    Dim fHandle As Integer
    fHandle = WriteHeader(filename, "INSERT INTO `ItemInstance` (id,itemNumber,itemId,siteLocationId,serialNumber,grantNumber,statusId,inServiceDate,removedServiceDate,isSealBroken,hasBarcode,notes,timestamp,lastSync) VALUES")
    WriteItemInstanceTable fHandle
    WriteFooter fHandle
End Sub


' Outputs a single VALUE row for Item Instance table
' id GUID, itemNumber TEXT, itemId GUID, siteLocationId GUID, serialNumber TEXT, grantNumber TEXT, statusID GUID, inServiceDate DATE, removedServiceDate DATE=NULL, isSealBroken BOOL=false, hasBarcode BOOL=false, notes TEXT
Private Sub WriteItemInstanceRow(ByVal outFile As Integer, id As String, itemNumber As String, itemId As String, siteLocationId As String)
    Print #outFile, "(" & GetQuotedString(LCase(id)) & "," & GetNullQuotedString(itemNumber) & "," & GetQuotedString(LCase(itemId)) & "," & GetQuotedString(LCase(siteLocationId)) & ",NULL,NULL,'ca7954fb-42cd-430c-9370-ce39c5615925',1528879541,NULL,0,0,NULL,1528869237,NULL)";
End Sub


' returns true is blank value, false otherwise
Private Function isBlank(ByRef value As Variant) As Boolean
    On Error Resume Next
    isBlank = (LenB(Trim(CStr(value))) = 0)
End Function


' converts text/name unit of measure into a GUID value
Private Function unitOfMeasureNameToGuid(ByRef value As String) As String
    If (LCase(value) = "each") Or isBlank(value) Then
        unitOfMeasureNameToGuid = "597bf084-d136-4377-8d2c-e38e34007050"
    Else
        Select Case LCase(value)
            Case "Box"
                unitOfMeasureNameToGuid = "f3cbed82-b264-453a-90c1-1269c5ce4a8b"
                Break
            Case "Lb."
                unitOfMeasureNameToGuid = "d0fcd84c-2974-41f5-96f5-844d62c1de4b"
                Break
            Case "Pair"
                unitOfMeasureNameToGuid = "cb928dbb-346d-400e-a6b0-fe8932382268"
                Break
            Case Else
                Stop
        End Select
    End If
End Function

' converts text/name item category into GUID value
Private Function categoryNameToGuid(ByRef value As String) As String
    If (LCase(value) = "treatment") Or isBlank(value) Then
        categoryNameToGuid = "4280c2bd-04fd-4536-bb78-4442e318241f"
    Else
        Select Case LCase(value)
            Case "Decon"
                categoryNameToGuid = "5b5c4ac4-12a6-4141-9e6f-4442fd28f315"
                Break
            Case "Documentation"
                categoryNameToGuid = "a11bfe19-5c48-478e-a48b-ab8d74b3c998"
                Break
            Case "Assessment"
                categoryNameToGuid = "5b22f88e-3186-4fc3-8633-f809690f5aa7"
                Break
            Case "PPE"
                categoryNameToGuid = "9263761f-a7cc-4b9a-907d-0a5d4f790420"
                Break
            Case "Crew Care"
                categoryNameToGuid = "5a985f50-0856-4219-be0e-6ab73c29f0e9"
                Break
            Case "Rehabilitation"
                categoryNameToGuid = "9857dc8e-c27d-4a6b-8111-50c9818165b9"
                Break
            Case "Shelter"
                categoryNameToGuid = "92eb6787-944b-4f9d-a439-c7e24e53f840"
                Break
            Case "Sanitation"
                categoryNameToGuid = "1736c77c-d860-4fac-ba37-43bd90665a82"
                Break
            Case "Command"
                categoryNameToGuid = "05f9b71b-a8c5-44c9-8f55-1ccf9a3efa52"
                Break
            Case "Transportation"
                categoryNameToGuid = "4a248a11-054b-481a-8610-b2aa01bb9dc0"
                Break
            Case "Triage"
                categoryNameToGuid = "217427dd-1844-445f-8ff1-1aeffb423459"
                Break
            Case "Tools/Utilities"
                categoryNameToGuid = "34c9d09b-6d21-436a-bf48-3180e93d6096"
                Break
            Case "Treatment Boxes"
                categoryNameToGuid = "9a3c7e15-d79f-45cf-b93e-35abb039d4e8"
                Break
            Case "Monitor/Detection"
                categoryNameToGuid = "e10be1df-2349-43d1-befb-acd01868e9b3"
                Break
            Case Else
                Stop
        End Select
    End If
End Function

' converts text/name battery type into GUID value
' Note: currently only None is supported!
Private Function batteryTypeNameToGuid(ByRef value As String) As String
    If (LCase(value) = "none") Or isBlank(value) Then
        batteryTypeNameToGuid = "584da062-0a37-4a6d-9d8b-202e202e202e"
    Else
        Select Case LCase(value)
            Case "3V CR2032"
                batteryTypeNameToGuid = "6f37f79e-a4c0-4456-93ad-203220322032"
                Break
            Case "7.5V"
                batteryTypeNameToGuid = "a6d608d8-8c56-49cf-b5c0-075707570757"
                Break
            Case "9V"
                batteryTypeNameToGuid = "b1875d9b-5d5c-45a5-b9a5-009700970097"
                Break
            Case "AA"
                batteryTypeNameToGuid = "aa1322c9-655b-42ef-a361-00aa00aa00aa"
                Break
            Case "AAA"
                batteryTypeNameToGuid = "9e2a4735-9eef-4908-a6e1-0aaa0aaa0aaa"
                Break
            Case "C"
                batteryTypeNameToGuid = "88ecd5db-bd06-4f37-907d-000c000c000c"
                Break
            Case "D"
                batteryTypeNameToGuid = "97b9b40d-e4e1-4f44-bd60-000d000d000d"
                Break
            Case Else
                Stop
        End Select
    End If
End Function

' converts text/name vehicle location into GUID value
Private Function vehicleLocationNameToGuid(ByRef value As String) As String
    If (LCase(value) = "trailer") Or isBlank(value) Then
        vehicleLocationNameToGuid = "59685701-713d-492f-8199-5a05294c0faa"
    Else
        Select Case LCase(value)
            Case "Comms Trailer"
                vehicleLocationNameToGuid = "2504bf72-36bc-4953-8cd4-a15390247441"
                Break
            Case "Dodge 3500 Truck"
                vehicleLocationNameToGuid = "fb5132ec-e043-4476-951f-076ae8ac65ac"
                Break
            Case "F650 Truck"
                vehicleLocationNameToGuid = "d5ddd86f-4a2b-494e-b348-02bf1da192f2"
                Break
            Case "Logistics Truck"
                vehicleLocationNameToGuid = "d38dc4a2-f4e6-4bcf-92a0-99e13afd7409"
                Break
            Case "None / NA"
                vehicleLocationNameToGuid = "f9c19eea-12d4-4920-bc2a-cf0c16c41a26"
                Break
            Case Else
                Stop
        End Select
    End If
End Function


' converts text/name vendor into GUID value
' Note: currently only BoundTree is supported!
Private Function vendorNameToGuid(ByRef value As String) As String
    If (LCase(value) = "boundtree") Or isBlank(value) Then
        vendorNameToGuid = "3eb51d2a-1eb9-4345-9c72-15efe83e7f22"
    Else
        Select Case LCase(value)
            Case Else
                Stop
        End Select
    End If
End Function


' finds item GUID based on item#
' where item# is first column in sheet and GUID is second column
Private Function LookupItemGuid(ByRef sht As Worksheet, ByRef value As String) As String
    On Error GoTo ErrHandler
    Dim row As Long: row = 2
    With sht
        Do While LCase(CStr(.Cells(row, 1))) <> LCase(value)
            row = row + 1
            If row > 9999 Then
                MsgBox "Error: unmatched parent id referenced! [" & value & "]"
                Exit Function
            End If
        Loop
        
        LookupItemGuid = .Cells(row, 2)
    End With
    Exit Function
ErrHandler:
    Debug.Print Err.Description
    Stop
    Resume
End Function


Private Sub WriteItemTypeTable(ByVal outFile As Integer)
    On Error GoTo ErrHandler
        Dim firstRow As Boolean: firstRow = True
        Dim sht As Worksheet
        Set sht = Sheets("Item Types")
        Dim row As Long: row = 2
        With sht
        Do While Not isBlank(.Cells(row, 2)) ' id (GUID) is not blank
            If Not isBlank(.Cells(row, 4)) Then ' skip output if no name
                If firstRow Then
                    firstRow = False
                Else
                    Print #outFile, "," ' add comma to previous line if not first row and writing another row
                End If
                WriteItemTypeRow outFile, .Cells(row, 2), .Cells(row, 3), .Cells(row, 4), .Cells(row, 5), .Cells(row, 6), .Cells(row, 7), .Cells(row, 8), unitOfMeasureNameToGuid(.Cells(row, 9)), categoryNameToGuid(.Cells(row, 10)), Cells(row, 11), _
                                          batteryTypeNameToGuid(.Cells(row, 12)), .Cells(row, 13), IIf(isBlank(.Cells(row, 14)), ExpireRestockCategory.None, ExpireRestockCategory.DateSpecificRestock), _
                                          Not isBlank(.Cells(row, 15)), Not isBlank(.Cells(row, 16)), vendorNameToGuid(.Cells(row, 18)), .Cells(row, 19)
            End If
            
            row = row + 1
        Loop
        End With
    Exit Sub
ErrHandler:
    Debug.Print Err.Description
    Stop
    Resume
End Sub


' loop through all Items for given trailer type and output
Private Sub WriteItemTableForUnit(ByVal outFile As Integer, ByRef sht As Worksheet, ByRef firstRow As Boolean)
        Dim row As Long
        row = 2
        With sht
            Do While Not isBlank(.Cells(row, 2)) ' id (GUID) is not blank
                If Not isBlank(.Cells(row, 4)) Then ' skip output if no name
                    If firstRow Then
                        firstRow = False
                    Else
                        Print #outFile, "," ' add comma to previous line if not first row and writing another row
                    End If
                    WriteItemRow outFile, .Cells(row, 2), .Cells(row, 3), .Cells(row, 5), .Cells(row, 6), vehicleLocationNameToGuid(.Cells(row, 7)), .Cells(row, 8), .Cells(row, 9), .Cells(row, 10), .Cells(row, 11), LookupItemGuid(sht, .Cells(row, 12)), .Cells(row, 13)
                End If
            
                row = row + 1
            Loop
        End With
End Sub

Private Sub WriteItemTable(ByVal outFile As Integer)
    On Error GoTo ErrHandler
        Dim firstRow As Boolean: firstRow = True
        Dim sht As Worksheet
        
        Set sht = Sheets("MMRS Items")
        WriteItemTableForUnit outFile, sht, firstRow
        
        Set sht = Sheets("DMSU Items")
        WriteItemTableForUnit outFile, sht, firstRow
        
        Set sht = Sheets("SSU Items")
        WriteItemTableForUnit outFile, sht, firstRow
    
    Exit Sub
ErrHandler:
    Debug.Print Err.Description
    Stop
    Resume
End Sub


' loop through all Items for given trailer type and output Item Instances for given Location
Private Sub WriteItemInstanceTableForUnitAtLocation(ByVal outFile As Integer, ByRef sht As Worksheet, ByRef firstRow As Boolean, locationId As String, locSuffix As String)
        Dim row As Long
        row = 2
        With sht
            Do While Not isBlank(.Cells(row, 2)) ' id (GUID) is not blank
                If Not isBlank(.Cells(row, 4)) Then ' skip output if no name
                    If firstRow Then
                        firstRow = False
                    Else
                        Print #outFile, "," ' add comma to previous line if not first row and writing another row
                    End If
                    Dim GUID As String: GUID = LCase(CreateGUID)
                    WriteItemInstanceRow outFile, GUID, .Cells(row, 1) & locSuffix, .Cells(row, 2), locationId
                End If
            
                row = row + 1
            Loop
        End With
End Sub


' loop through all locations, for each trailer unit that location has, generate an ItemInstance for each Item of that unit for given location
Private Sub WriteItemInstanceTable(ByVal outFile As Integer)
    On Error GoTo ErrHandler
        Dim firstRow As Boolean: firstRow = True
        Dim sht As Worksheet
        
        Dim locationSheet As Worksheet: Set locationSheet = Sheets("Locations")
        Dim locationRow As Integer: locationRow = 2
        With locationSheet
            Do While (Not isBlank(.Cells(locationRow, 2))) ' ignore rows without GUID
                If Not isBlank(.Cells(locationRow, 4)) Then
                    Set sht = Sheets("MMRS Items")
                    WriteItemInstanceTableForUnitAtLocation outFile, sht, firstRow, .Cells(locationRow, 2), .Cells(locationRow, 3)
                End If
        
                If Not isBlank(.Cells(locationRow, 5)) Then
                    Set sht = Sheets("DMSU Items")
                    WriteItemInstanceTableForUnitAtLocation outFile, sht, firstRow, .Cells(locationRow, 2), .Cells(locationRow, 3)
                End If
        
                If Not isBlank(.Cells(locationRow, 6)) Then
                    Set sht = Sheets("SSU Items")
                    WriteItemInstanceTableForUnitAtLocation outFile, sht, firstRow, .Cells(locationRow, 2), .Cells(locationRow, 3)
                End If
                
                locationRow = locationRow + 1
            Loop
        End With
    Exit Sub
ErrHandler:
    Debug.Print Err.Description
    Stop
    Resume
End Sub




