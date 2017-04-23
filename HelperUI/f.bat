@echo off
::==============================================================================================================
::	Replace the appropriate entries below to suit your installation (Default install example shown)
::==============================================================================================================

set FactorioExeLocation=C:\Program Files (x86)\Steam\steamapps\common\Factorio\bin\x64\

set SaveFileName=temp.zip
set SaveLocation=C:\Users\icer\AppData\Roaming\Factorio\saves\

set ServerSettingsFileName=server-settings.json
set ServerSettingsLocation=C:\Program Files (x86)\Steam\steamapps\common\Factorio\data\

set ServerConfigFileName=config.ini
set ServerConfigLocation=C:\Users\icer\AppData\Roaming\Factorio\serverconfig\
::==============================================================================================================

cd %FactorioExeLocation%

echo.
echo.
echo PREPARING TO LAUNCH FACTORIO SERVER...
echo.
echo 	Save to be loaded:
echo 		%SaveLocation%%SaveFileName% 
echo.
echo 	Server Settings to load: 
echo 		%ServerSettingsLocation%%ServerSettingsFileName%
echo.
echo.
echo *** Remember to use Ctrl+C to ensure saving when finished instead of simply closing this window ***
echo.
echo.
pause 3

::==============================================================================================================
:: ***NOTE: The line below launches factorio in headless mode with desired server settings, add any other
::          desired arguments to end of the line. Use "factorio.exe --help" for a list of all supported arguments.
::==============================================================================================================
factorio.exe --start-server "%SaveLocation%%SaveFileName%" --server-settings "%ServerSettingsLocation%%ServerSettingsFileName%" -c "%ServerConfigLocation%%ServerConfigFileName%" --rcon-port 12345 --rcon-password 12345

pause 3

