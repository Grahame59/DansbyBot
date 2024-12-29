// Core chatbot logic (intent and response recognition)
using System;
using ChatbotApp.Utilities;
using ChatbotApp.Features;
using Intents;
using Responses;

namespace ChatbotApp.Core
{
    public class DansbyCore
    {
        private readonly IntentRecognizer intentRecognizer;
        private readonly ResponseRecognizer responseRecognizer;
        private readonly SoundtrackManager soundtrackManager;
        private readonly AnimationManager animationManager;
        private readonly AutosaveManager autosaveManager;
        private readonly ErrorLogClient errorLogClient;

        public DansbyCore()
        {
            intentRecognizer = new IntentRecognizer();
            responseRecognizer = new ResponseRecognizer();
            soundtrackManager = new SoundtrackManager();
            animationManager = new AnimationManager();
            autosaveManager = new AutosaveManager("E:\\Lorehaven\\autosave.bat"); // Path to Obsidian Vault autosave file
            errorLogClient = new ErrorLogClient();

            InitializeManagers();
        }

        private void InitializeManagers()
        {
            try
            {
                // Initialize soundtracks
                soundtrackManager.InitializeSoundtracks();

                // Start autosave functionality
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
            try
            {
                // Recognize intent
                string intent = intentRecognizer.RecognizeIntent(userInput);
                errorLogClient.AppendToDebugLog($"Recognized intent: {intent}", "DansbyCore.cs");

                // Handle specific intents
                if (intent == "SummonSlimeIntent")
                {
                    animationManager.InitializeAnimation(null); // Replace `null` with the appropriate UI parent control
                    return "Summoning a slime!";
                }

                // Generate response
                string response = responseRecognizer.RecognizeResponse(userInput);
                return response;
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error processing user input: {ex.Message}", "DansbyCore.cs");
                return "I'm sorry, something went wrong.";
            }
        }
        
        public bool ShouldSummonSlime()
        {
            return new Random().Next(1, 101) <= 10; // 10% chance
        }

        public void SummonSlime(Form parent)
        {
            animationManager.InitializeAnimation(parent);
        }


        public void Shutdown()
        {
            try
            {
                autosaveManager.StopAutosave();
                errorLogClient.AppendToDebugLog("DansbyCore shutdown successfully.", "DansbyCore.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error during shutdown: {ex.Message}", "DansbyCore.cs");
            }
        }
    }
}
