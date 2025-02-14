using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatbotApp.Utilities;
using Intents;
using ChatbotApp.Features;
using Functions;
using System.IO;

namespace ChatbotApp.Core
{
    public class DansbyCore
    {
        private readonly IntentRecognizer intentRecognizer;
        private readonly ResponseGenerator responseGenerator;
        public static SoundtrackManager soundtrackManager {get; private set;} = new SoundtrackManager();
        private readonly AnimationManager animationManager;
        private readonly AutosaveManager autosaveManager;
        private readonly VaultManager vaultManager;
        private readonly ErrorLogClient errorLogClient;
        private readonly functionHoldings functionHoldings;
        private readonly IntentEditorManager intentEditorManager;
        public IntentEditorManager IntentEditor => intentEditorManager; //Getter to access w/o modifying directly


        public DansbyCore(MainForm mainForm)
        {
            errorLogClient = ErrorLogClient.Instance;

            // Determine the base path dynamically
            string basePath = GetBasePath();

            // Initialize Managers
            intentRecognizer = new IntentRecognizer();
            responseGenerator = new ResponseGenerator(errorLogClient);
            soundtrackManager = new SoundtrackManager();
            animationManager = new AnimationManager();
            autosaveManager = new AutosaveManager(Path.Combine(basePath, "autosave.bat"));
            GlobalAutosaveManager = autosaveManager;
            
            vaultManager = new VaultManager(Path.Combine(basePath, "gitconnect"));
            functionHoldings = new functionHoldings(mainForm);
            intentEditorManager = new IntentEditorManager(mainForm.intentPanel);

        }

        // Helper method to determine the base path
        private string GetBasePath()
        {
            Console.WriteLine("Checking if E:\\Lorehaven exists...");
            if (Directory.Exists(@"E:\Lorehaven"))
            {
                Console.WriteLine("E:\\Lorehaven found. Returning E:\\Lorehaven.");
                return @"E:\Lorehaven";
            }

            Console.WriteLine("Checking if C:\\Lorehaven exists...");
            if (Directory.Exists(@"C:\Lorehaven"))
            {
                Console.WriteLine("C:\\Lorehaven found. Returning C:\\Lorehaven.");
                return @"C:\Lorehaven";
            }

            Console.WriteLine("Neither E:\\Lorehaven nor C:\\Lorehaven was found.");
            throw new DirectoryNotFoundException("Lorehaven directory not found on either E: or C: drive.");
        }

        // Async initialization to be called from MainForm
        public async Task InitializeAsync()
        {
            try
            {
                await intentRecognizer.InitializeAsync();
                await soundtrackManager.InitializeSoundtracksAsync();
                await autosaveManager.StartAutosaveAsync();
                intentEditorManager.InitializeUI();
                await errorLogClient.AppendToDebugLogAsync("DansbyCore managers initialized successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error during DansbyCore initialization: {ex.Message}", "DansbyCore.cs");
            }
        }

        // Method to refresh the vault cache
        public async Task RefreshVaultCacheAsync()
        {
            try
            {
                await vaultManager.RefreshCacheAsync();
                await errorLogClient.AppendToDebugLogAsync("Vault cache refreshed successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error refreshing vault cache: {ex.Message}", "DansbyCore.cs");
            }
        }

        // Async method for processing user input
        public async Task<string> ProcessUserInputAsync(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                _ = errorLogClient.AppendToDebugLogAsync("Received empty user input.", "DansbyCore.cs");
                return "Please say something!";
            }

            try
            {
                // Recognize the intent from user input
                string intent = intentRecognizer.RecognizeIntent(userInput);
                _ = errorLogClient.AppendToDebugLogAsync($"Recognized intent: {intent}", "DansbyCore.cs");

                // Check if the intent matches a function in Function.cs
                var functionResponse = await functionHoldings.ExecuteFunctionAsync(intent, userInput);
                if (functionResponse != "Sorry, I don't recognize that command.")
                {
                    _ = errorLogClient.AppendToDebugLogAsync($"Function executed for intent: {intent}", "DansbyCore.cs");
                    return functionResponse;
                }

                // If no function matches, fall back to generating a response
                string response = responseGenerator.GenerateResponse(intent, userInput);
                _ = errorLogClient.AppendToDebugLogAsync($"Response generated from recognized intent: {response}.", "DansbyCore.cs");

                return response;
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error processing user input: {ex.Message}", "DansbyCore.cs");
                return "Sorry, something went wrong while processing your input.";
            }
        }



        // Async method for retrieving soundtrack names
        public async Task<List<string>> GetSoundtrackNamesAsync()
        {
            try
            {
                return await Task.Run(() => soundtrackManager.GetSoundtrackNames());
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error fetching soundtrack names: {ex.Message}", "DansbyCore.cs");
                return new List<string>();
            }
        }

        // Async method to play a soundtrack
        public async Task PlaySoundtrackAsync(string soundtrackName)
        {
            try
            {
                // Play button Doesn't remember or save the state of the song (Resets song to beginning)
                await soundtrackManager.PlaySoundtrackAsync(soundtrackName);
                await errorLogClient.AppendToDebugLogAsync($"Playing soundtrack: {soundtrackName}", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error playing soundtrack: {ex.Message}", "DansbyCore.cs");
            }
        }

        // Async method to pause playback
        public async Task PausePlaybackAsync()
        {
            try
            {
                await soundtrackManager.PausePlaybackAsync();
                await errorLogClient.AppendToDebugLogAsync("Playback paused.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error pausing playback: {ex.Message}", "DansbyCore.cs");
            }
        }

        // Slime Animation Methods (UI-related logic stays in MainForm)
        public bool ShouldSummonSlime()
        {
            return new Random().Next(1, 101) <= 10; // 10% chance
        }

        public async Task SummonSlimeAsync(RichTextBox chatRichTextBox)
        {
            try
            {
                animationManager.InitializeAnimation(chatRichTextBox);
                await errorLogClient.AppendToDebugLogAsync("Slime summoned successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error summoning slime: {ex.Message}", "DansbyCore.cs");
            }
        }


        // Shutdown logic
        public async Task ShutdownAsync()
        {
            try
            {
                await autosaveManager.StopAutosaveAsync();
                await errorLogClient.AppendToDebugLogAsync("DansbyCore shut down successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error during shutdown: {ex.Message}", "DansbyCore.cs");
            }
        }
        public static AutosaveManager GlobalAutosaveManager { get; private set; }

    }
}
