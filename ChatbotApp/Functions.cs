using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using ChatbotApp;
using ChatbotApp.Utilities;
using ChatbotApp.Features;
using System.Diagnostics;
using System.Drawing;
using ChatbotApp.Core;

namespace Functions
{
    public class functionHoldings
    {
        private MainForm mainForm;
        private ErrorLogClient errorLogClient = new ErrorLogClient();

        public functionHoldings(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void PerformExitDansby() //Exit function
        {
            try
            {
                // Shutdown message
                string exitMessage = "DANSBY: I feel so cold... don't leave me creator... goodbye";
                mainForm.AppendToChatHistory(exitMessage, Color.MediumPurple);
                MessageBox.Show(" -> SUCCESSFULLY EXITING DANSBYCHATBOT");

                errorLogClient.AppendToDebugLog("Application is exiting.", "Functions.cs");

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error during PerformExitDansby: {ex}", "Functions.cs");
            }
        }

        public void GetCurrentDateTime()
        {
            try
            {
                DateTime now = DateTime.Now;
                string formattedDateTime = now.ToString("MMMM dd, yyyy hh:mm:ss tt");

                string response = $"Dansby: The current date and time is {formattedDateTime}, and it's a {now.DayOfWeek}.";
                mainForm.AppendToChatHistory(response, Color.MediumPurple);

                errorLogClient.AppendToDebugLog("Fetched current date and time.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error fetching current date and time: {ex}", "Functions.cs");
            }
        }

        public async Task<string> ExecuteFunctionAsync(string intent, string userInput)
        {
            try
            {
                switch (intent)
                {
                    case "performexitdansby":
                        PerformExitDansby();
                        return "Exiting the application.";

                    case "time":
                        GetTime();
                        return "Time fetched.";

                    case "date":
                        GetDate();
                        return "Date fetched.";

                    case "dayofweek":
                        GetDayOfTheWeek();
                        return "Day of the week fetched.";

                    case "searchvault":
                        string searchResult = await SearchVaultAsync(userInput);
                        return searchResult;

                    case "openerrorlog":
                        await OpenErrorLogForm();
                        return "ErrorLogForm opened.";

                    case "forcesavelorehaven":
                        if (Directory.Exists(@"E:\\Lorehaven"))
                        {
                            var autosaveManager = new AutosaveManager("E:\\Lorehaven\\autosave.bat", 300000);
                            _= autosaveManager.ExecuteAutosaveAsync();
                            return "Obsidian Vault LoreHaven was commited and pushed.";
                        }
                        if (Directory.Exists("@C:\\Lorehaven"))
                        {
                            var autosaveManager2 = new AutosaveManager("C:\\Lorehaven\autosave.bat", 300000);
                            _= autosaveManager2.ExecuteAutosaveAsync();
                            return "Obsidian Vault LoreHaven was commited and pushed.";
                        }
                        return "Could not find the directory for Lorehaven";

                    default:
                        return "Sorry, I don't recognize that command.";
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error executing function for intent {intent}: {ex.Message}", "Functions.cs");
                return "An error occurred while processing your request.";
            }
        }

        private void PauseAutosaveTimer()
        {
            try
            {
                if (Directory.Exists(@"E:\Lorehaven") || Directory.Exists(@"C:\Lorehaven"))
                {
                    _= DansbyCore.GlobalAutosaveManager.StopAutosaveAsync();
                    errorLogClient.AppendToDebugLog("Debug: Autosave timer stopped", "Function.cs");
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error stopping autosave timer: {ex.Message}", "Functions.cs");
            }
        }

        private void GetTime()
        {
            try
            {
                DateTime now = DateTime.Now;
                string formattedTime = now.ToString("hh:mm:ss tt");

                string response = $"Dansby: The time is {formattedTime}.";
                mainForm.AppendToChatHistory(response, Color.MediumPurple);

                errorLogClient.AppendToDebugLog("Fetched current time.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error fetching time: {ex}", "Functions.cs");
            }
        }

        private void GetDate()
        {
            try
            {
                DateTime now = DateTime.Now;
                string formattedDate = now.ToString("MMMM dd, yyyy");

                string response = $"Dansby: The current date is {formattedDate}.";
                mainForm.AppendToChatHistory(response, Color.MediumPurple);

                errorLogClient.AppendToDebugLog("Fetched current date.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error fetching date: {ex}", "Functions.cs");
            }
        }

        private void GetDayOfTheWeek()
        {
            try
            {
                string dayOfWeek = DateTime.Now.DayOfWeek.ToString();

                string response = $"Dansby: Today is a {dayOfWeek}.";
                mainForm.AppendToChatHistory(response, Color.MediumPurple);

                errorLogClient.AppendToDebugLog("Fetched day of the week.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error fetching day of the week: {ex}", "Functions.cs");
            }
        }

        private async Task<string> SearchVaultAsync(string keyword)
        {
            try
            {
                //---------EDIT PATH 
                VaultManager vaultManager = new VaultManager("E:\\Lorehaven\\gitconnect");
                List<string> searchResults = await vaultManager.SearchVaultAsync(keyword);

                if (searchResults.Count > 0)
                {
                    string response = $"Dansby: Found {searchResults.Count} matching files.";
                    mainForm.AppendToChatHistory(response, Color.MediumPurple);

                    foreach (string result in searchResults)
                    {
                        mainForm.AppendToChatHistory(result);
                    }

                    return response;
                }
                else
                {
                    return "Dansby: No matches found in the vault.";
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error searching vault: {ex.Message}", "Functions.cs");
                return "An error occurred while searching the vault.";
            }
        }

        public async Task ListAllFunctionsAsync()
        {
            try
            {
                string introMessage = "Dansby: Available Functions I can perform currently:";
                mainForm.AppendToChatHistory(introMessage, Color.MediumPurple);

                var methods = typeof(functionHoldings).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    string functionName = method.Name;
                    string functionDescription = GetFunctionDescription(functionName);

                    string message = $"- {functionName}: {functionDescription}";
                    mainForm.AppendToChatHistory(message, Color.MediumPurple);
                }

                await errorLogClient.AppendToDebugLogAsync("Listed all available functions.", "Functions.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error listing functions: {ex.Message}", "Functions.cs");
            }
        }

        private async Task OpenErrorLogForm()
        {
            try
            {
                // Possible Path correction to be done here dynamically. 
                string errorLogFormBatchPath = "CODES\\DansbyBot\\Utilities\\StartErrorLog.bat";

                if (File.Exists(errorLogFormBatchPath))
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = errorLogFormBatchPath,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };

                    Process process = Process.Start(processInfo);
                    await process.WaitForExitAsync();  // Wait for the process to complete
                }
                else
                {
                    throw new FileNotFoundException($"Batch file not found at {errorLogFormBatchPath}");
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error running error log batch file: {ex.Message}", "Functions.cs");
            }
        }


        private string GetFunctionDescription(string functionName)
        {
            return functionName switch
            {
                "PerformExitDansby" => "This function exits the application.",
                "GetTime" => "This function gives the current time of your time zone.",
                "GetDate" => "This function gives you the current date.",
                "GetDayOfTheWeek" => "This function gives you the day of the week.",
                "SearchVaultAsync" => "This function searches your vault for matching keywords.",
                "ListAllFunctionsAsync" => "This function lists all the available functions Dansby can complete.",
                _ => "No description available.",
            };
        }
    }
}
