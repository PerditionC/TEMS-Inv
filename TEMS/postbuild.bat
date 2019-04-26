::@ECHO OFF
@ECHO Creating assembly containers
::update as needed, use hardcoded relative path
@ECHO %CD%
@SET BASE_PATH=%CD%\..\..\..
@SET LIBZ_PATH=%BASE_PATH%\packages\LibZ.Tool.1.2.0.0\tools
@set LIBZ=%LIBZ_PATH%\libz.exe
@ECHO Using libz=%LIBZ%
::remove any previous containers, avoids errors
del /Q *.libz
::GOTO skipLibz

::embedd assemblies into the respective containers
::%LIBZ% add --libz TEMS.libz --include Inventory*.dll 
::%LIBZ% add --libz TEMS.libz --include BarcodeLib.dll
::%LIBZ% add --libz TEMS.libz --include Wpf*.dll 
::%LIBZ% add --libz TEMS.libz --include Microsoft*.*
::%LIBZ% add --libz TEMS.libz --include NLog.*
::%LIBZ% add --libz TEMS.libz --include Newtonsoft.Json.*
%LIBZ% add --libz TEMS.libz --include *.dll


md %BASE_PATH%\lib
copy *.libz %BASE_PATH%\lib\
:skipLibz
@ECHO Assembly container creation complete.
