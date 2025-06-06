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
using System.Text.RegularExpressions; //For SoundtrackManager calls...

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

                    case "openerrorlog":
                        await OpenErrorLogForm();
                        return "ErrorLogForm opened.";

                    case "forcesavelorehaven":
                        await SaveLorehaven();
                        return "Could not find the directory for Lorehaven";

                    case "pauseautosavetimer":
                        PauseAutosaveTimer();
                        return "Autosave Timer Paused!";

                    case "resumeautosavetimer":
                        ResumeAutosaveTimer();
                        return "Autosave Timer Started!";

                    case "handlevolumeintent":
                        await HandleVolumeIntent(userInput);
                        return "Volume Edited!";

                    case "listallfunctions":
                        await ListAllFunctionsAsync();
                        return "Listed all functions";

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
        public async Task HandleVolumeIntent(string command)
        {
            if (DansbyCore.soundtrackManager == null)
            {
                await errorLogClient.AppendToErrorLogAsync("SoundtrackManager instance is null.", "Function.cs");
                return;
            }

            float currentVolume = DansbyCore.soundtrackManager.GetCurrentVolume();
            float newVolume = currentVolume;

            if (command.Contains("increase"))
            {
                newVolume += 0.1f; // Increase by 10%
            }
            else if (command.Contains("decrease"))
            {
                newVolume -= 0.1f; // Decrease by 10%
            }
            else if (command.Contains("volume"))
            {
                // Extract percentage (e.g., "Set volume to 50%")
                var match = Regex.Match(command, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int volumePercentage))
                {
                    newVolume = volumePercentage / 100f; // Convert to 0.0 - 1.0 scale
                }
            }

            // Ensure the volume stays between 0 and 1
            newVolume = Math.Clamp(newVolume, 0.0f, 1.0f);

            await DansbyCore.soundtrackManager.EditVolumeOfSoundtracks(newVolume);
        }

        private async Task SaveLorehaven()
        {
            try
            {
                string lorehavenPath = Directory.Exists(@"E:\Lorehaven") ? @"E:\Lorehaven" :
                                    Directory.Exists(@"C:\Lorehaven") ? @"C:\Lorehaven" : null;

                if (lorehavenPath != null)
                {
                    string autosavePath = Path.Combine(lorehavenPath, "autosave.bat");

                    if (File.Exists(autosavePath))
                    {
                        var autosaveManager = new AutosaveManager(autosavePath, 300000);
                        await autosaveManager.ExecuteAutosaveAsync();
                    }
                    else
                    {
                        await errorLogClient.AppendToDebugLogAsync($"Autosave script not found: {autosavePath}", "Functions.cs");
                    }
                }
                else
                {
                    await errorLogClient.AppendToDebugLogAsync("Lorehaven directory not found on E: or C:", "Functions.cs");
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToDebugLogAsync($"Error in SaveLorehaven: {ex.Message}", "Functions.cs");
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

        private void ResumeAutosaveTimer()
        {
            try
            {
                if(Directory.Exists(@"E:\Lorehaven") || Directory.Exists(@"C:\Lorehaven"))
                {
                    _= DansbyCore.GlobalAutosaveManager.StartAutosaveAsync();
                    errorLogClient.AppendToDebugLog("Debug: Autosave Timer started", "Functions.cs");
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error starting autosave timer: {ex.Message}", "Functions.cs");
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

        public async Task ListAllFunctionsAsync()
        {
            try
            {
                string introMessage = "Dansby: Available Functions I can perform currently:";
                mainForm.AppendToChatHistory(introMessage, Color.MediumPurple);

                var methods = typeof(functionHoldings).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic);
                foreach (var method in methods)
                {
                    string functionName = method.Name.ToLower();
                    string functionDescription = GetFunctionDescription(functionName);

                    string message = $"- {method.Name}: {functionDescription}";
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
                string errorLogFormBatchPath = "DansbyBot\\Utilities\\StartErrorLog.bat";

                if (File.Exists(errorLogFormBatchPath))
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = errorLogFormBatchPath,
                        UseShellExecute = true
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
            return functionName.ToLower() switch
            {
                "performexitdansby" => "This function exits the application.",
                "getcurrentdatetime" => "This function retrieves the current date and time.",
                "executefunctionasync" => "Handles execution of requested functions asynchronously.",
                "handlevolumeintent" => "This function edits the volume of the soundtracks.",
                "listallfunctionsasync" => "This function lists all the available functions Dansby can complete.",
                "gettime" => "This function retrieves the current time of your time zone.",
                "getdate" => "This function retrieves the current date.",
                "getdayoftheweek" => "This function retrieves the current day of the week.",
                "pauseautosavetimer" => "This function pauses the autosave timer for the batch file that saves the Lorehaven Vault (My Personal Notes Vault <- Exclusive to me w/o setup).",
                "openerrorlogform" => "This function opens the ErrorLog form to display logged events. (My Logging Project)",
                "resumeautosavetimer" => "This function starts the autosave timer for the batch file that saves the Lorehaven vault (My Personal Notes Vault <- Exclusive to me w/o setup)",
                "forcesavelorehaven" => "This function forces a Git commit and push of the Lorehaven Vault.",
                _ => "No description available.",
            };
        }

    }
}
