// Handles Obsidian autosave functionality -> Lorehaven Vault
using System;
using System.Diagnostics;
using System.Timers;
using ChatbotApp.Utilities;

namespace ChatbotApp.Features
{
    public class AutosaveManager
    {
        private readonly Timer autosaveTimer;
        private readonly ErrorLogClient errorLogClient;
        private readonly string autosaveScriptPath;

        public AutosaveManager(string scriptPath, double intervalInMilliseconds = 300000)
        {
            autosaveScriptPath = scriptPath;
            errorLogClient = new ErrorLogClient();

            autosaveTimer = new Timer(intervalInMilliseconds)
            {
                AutoReset = true,
                Enabled = false // Start manually after initialization
            };
            autosaveTimer.Elapsed += OnAutosaveTriggered;
        }

        public void StartAutosave()
        {
            autosaveTimer.Start();
            errorLogClient.AppendToDebugLog("Autosave timer started.", "AutosaveManager.cs");
        }

        public void StopAutosave()
        {
            autosaveTimer.Stop();
            errorLogClient.AppendToDebugLog("Autosave timer stopped.", "AutosaveManager.cs");
        }

        private void OnAutosaveTriggered(object sender, ElapsedEventArgs e)
        {
            ExecuteAutosave();
        }

        public void ExecuteAutosave()
        {
            try
            {
                if (string.IsNullOrEmpty(autosaveScriptPath) || !System.IO.File.Exists(autosaveScriptPath))
                {
                    errorLogClient.AppendToErrorLog($"Autosave script not found: {autosaveScriptPath}", "AutosaveManager.cs");
                    return;
                }

                // Run the autosave script
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = autosaveScriptPath,
                        UseShellExecute = true
                    }
                };
                process.Start();
                process.WaitForExit();

                errorLogClient.AppendToDebugLog($"Autosave executed successfully at {DateTime.Now}.", "AutosaveManager.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error during autosave execution: {ex.Message}", "AutosaveManager.cs");
            }
        }

        public void Dispose()
        {
            autosaveTimer?.Dispose();
        }
    }
}
