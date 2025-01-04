using System;
using System.Windows.Forms;
using ChatbotApp.Utilities;
using Intents;
using ChatbotApp.Features;
using ChatbotApp.Core;
using System.Collections.Generic;

namespace ChatbotApp.Core
{
    public class DansbyCore
    {
        private readonly IntentRecognizer intentRecognizer;
        private readonly ResponseGenerator responseGenerator;
        private readonly SoundtrackManager soundtrackManager;
        private readonly AnimationManager animationManager;
        private readonly AutosaveManager autosaveManager;
        private readonly ErrorLogClient errorLogClient;

        public DansbyCore()
        {
            errorLogClient = new ErrorLogClient();

            try
            {
                intentRecognizer = new IntentRecognizer();
                responseGenerator = new ResponseGenerator(errorLogClient);
                soundtrackManager = new SoundtrackManager();
                animationManager = new AnimationManager();
                autosaveManager = new AutosaveManager("E:\\Lorehaven\\autosave.bat");

                InitializeManagers();
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error initializing DansbyCore: {ex.Message}", "DansbyCore.cs");
            }
        }

        private void InitializeManagers()
        {
            try
            {
                soundtrackManager.InitializeSoundtracks();
                autosaveManager.StartAutosave();

                errorLogClient.AppendToDebugLog("DansbyCore managers initialized successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error initializing managers: {ex.Message}", "DansbyCore.cs");
            }
        }

        public string ProcessUserInput(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                errorLogClient.AppendToDebugLog("Received empty user input.", "DansbyCore.cs");
                return "Please say something!";
            }

            try
            {
                // Recognize the user's intent
                string intent = intentRecognizer.RecognizeIntent(userInput);
                errorLogClient.AppendToDebugLog($"Recognized intent: {intent}", "DansbyCore.cs");

                // Generate an appropriate response
                string response = responseGenerator.GenerateResponse(intent, userInput);
                return response;
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error processing user input: {ex.Message}", "DansbyCore.cs");
                return "Sorry, something went wrong while processing your input.";
            }
        }

        public bool ShouldSummonSlime()
        {
            return new Random().Next(1, 101) <= 10; // 10% chance
        }

        public void SummonSlime(Form parent)
        {
            try
            {
                animationManager.InitializeAnimation(parent);
                errorLogClient.AppendToDebugLog("Slime summoned successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error summoning slime: {ex.Message}", "DansbyCore.cs");
            }
        }

// FIX ----------------------------------------------------------
        public List<string> GetSoundtrackNames()
        {
            return new List<string> { "Track1", "Track2", "Track3" }; // Replace with actual logic
        }

        public void PlaySoundtrack(string trackName)
        {
            // Logic to play the selected soundtrack
            errorLogClient.AppendToDebugLog($"Playing soundtrack: {trackName}", "DansbyCore.cs");
        }

        public void PausePlayback()
        {
            // Logic to pause the soundtrack
            errorLogClient.AppendToDebugLog("Playback paused.", "DansbyCore.cs");
        }
// FIX ----------------------------------------------------------


        public void Shutdown()
        {
            try
            {
                autosaveManager.StopAutosave();
                errorLogClient.AppendToDebugLog("DansbyCore shut down successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error during shutdown: {ex.Message}", "DansbyCore.cs");
            }
        }
    }
}
