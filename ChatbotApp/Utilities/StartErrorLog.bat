@echo off 
:: Batch file to quickly start ErrorLogger w/o going into the ide.
:: WIP

:: Change directory to the location of the project (the folder containing the .csproj file).
cd /d "E:\CODES\ErrorLogger"

:: Build the project.
dotnet build

:: Run the application if the build was successful.
if %errorlevel% neq 0 (
    echo Build failed. Please check the errors above.
    exit /b %errorlevel%
) 

:: Run the application.
dotnet run