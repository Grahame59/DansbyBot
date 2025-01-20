@echo off
:: Batch file to quickly start DansbyBot without going into the IDE.
:: WIP - Dynamic Path Detection

:: Check if the project exists on E:
if exist "E:\CODES\DansbyBot" (
    cd /d "E:\CODES\DansbyBot"
    goto Build
)

:: Check if the project exists on C:
if exist "C:\CODES\DansbyBot" (
    cd /d "C:\CODES\DansbyBot"
    goto Build
)

:: If the directory is not found, display an error and exit
echo DansbyBot project directory not found on either E: or C:.
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
