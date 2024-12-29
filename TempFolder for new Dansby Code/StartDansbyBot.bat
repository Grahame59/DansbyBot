@echo off 
:: Batch file to quickly start DansbyBot without going into the IDE.
:: WIP

:: Change directory to the location of the project (the folder containing the .csproj file).
cd /d "E:\CODES\DansbyBot"

:: Build the project.
dotnet build

:: Run the application if the build was successful.
if %errorlevel% neq 0 (
    echo Build failed. Please check the errors above.
    exit /b %errorlevel%
) 

:: Run the application.
dotnet run
