@echo off
:: Batch file to quickly start ErrorLogger without going into the IDE.
:: WIP - Dynamic Path Detection

:: Check if the project exists on E:
if exist "E:\CODES\ErrorLogger" (
    cd /d "E:\CODES\ErrorLogger"
    goto Build
)

:: Check if the project exists on C:
if exist "C:\CODES\ErrorLogger" (
    cd /d "C:\CODES\ErrorLogger"
    goto Build
)

:: If the directory is not found, display an error and exit
echo ErrorLogger project directory not found on either E: or C:.
exit /b 1

:Build
:: Build the project
dotnet build

:: Check if the build was successful
if %errorlevel% neq 0 (
    echo Build failed. Please check the errors above.
    exit /b %errorlevel%
)

:: Run the application
dotnet run
