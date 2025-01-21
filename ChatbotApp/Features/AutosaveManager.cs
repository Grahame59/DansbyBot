using System;
using System.Diagnostics;
using System.Timers;
using System.Threading.Tasks;
using ChatbotApp.Utilities;

namespace ChatbotApp.Features
{
    public class AutosaveManager : IDisposable
    {
        private readonly Timer autosaveTimer;
        private readonly ErrorLogClient errorLogClient;
        private readonly string autosaveScriptPath;

        public AutosaveManager(string scriptPath, double intervalInMilliseconds = 300000)
        {
            autosaveScriptPath = scriptPath;
            errorLogClient = ErrorLogClient.Instance;

            autosaveTimer = new Timer(intervalInMilliseconds)
            {
                AutoReset = true,
                Enabled = false // Start manually after initialization
            };
            autosaveTimer.Elapsed += async (sender, e) => await OnAutosaveTriggeredAsync();
        }

        /// <summary>
        /// Starts the autosave timer asynchronously.
        /// </summary>
        public async Task StartAutosaveAsync()
        {
            autosaveTimer.Start();
            await errorLogClient.AppendToDebugLogAsync("Autosave timer started.", "AutosaveManager.cs");
        }

        /// <summary>
        /// Stops the autosave timer asynchronously.
        /// </summary>
        public async Task StopAutosaveAsync()
        {
            autosaveTimer.Stop();
            await errorLogClient.AppendToDebugLogAsync("Autosave timer stopped.", "AutosaveManager.cs");
        }

        /// <summary>
        /// Triggered when the autosave timer elapses.
        /// </summary>
        private async Task OnAutosaveTriggeredAsync()
        {
            await ExecuteAutosaveAsync();
        }

        /// <summary>
        /// Executes the autosave process asynchronously.
        /// </summary>
        public async Task ExecuteAutosaveAsync()
        {
            try
            {
                await errorLogClient.AppendToDebugLogAsync($"Checking autosave script path: {autosaveScriptPath}", "AutosaveManager.cs");

                if (string.IsNullOrEmpty(autosaveScriptPath) || !System.IO.File.Exists(autosaveScriptPath))
                {
                    await errorLogClient.AppendToErrorLogAsync($"Autosave script not found: {autosaveScriptPath}", "AutosaveManager.cs");
                    return;
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = autosaveScriptPath,
                        UseShellExecute = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                await errorLogClient.AppendToDebugLogAsync($"Autosave executed successfully at {DateTime.Now}.", "AutosaveManager.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error during autosave execution: {ex.Message}", "AutosaveManager.cs");
            }
        }


        /// <summary>
        /// Disposes the autosave timer.
        /// </summary>
        public void Dispose()
        {
            autosaveTimer?.Dispose();
        }
    }
}
