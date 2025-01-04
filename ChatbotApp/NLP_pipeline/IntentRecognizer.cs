using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tokenization;
using ChatbotApp.Utilities;

namespace Intents
{
    public class IntentRecognizer
    {
        private List<Intent> intents;
        private Tokenizer tokenizer;
        private ErrorLogClient errorLogClient;

        public IntentRecognizer(string configFilePath = "ChatbotApp\\NLP_pipeline\\intent_mappings.json")
        {
            tokenizer = new Tokenizer();
            errorLogClient = new ErrorLogClient();

            try
            {
                intents = LoadIntents(configFilePath);
                errorLogClient.AppendToDebugLog($"Successfully loaded intents from {configFilePath}.", "IntentRecognizer");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error loading intents: {ex.Message}", "IntentRecognizer");
                intents = new List<Intent>(); // Initialize an empty list to prevent crashes
            }
        }

        // Load intents from a JSON file
        private List<Intent> LoadIntents(string filePath)
        {
            if (!File.Exists(filePath))
            {
                errorLogClient.AppendToErrorLog($"Intent file not found at {filePath}.", "IntentRecognizer");
                return new List<Intent>();
            }

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<Intent>>(json);
        }

        // Recognize intent based on user input
        public string RecognizeIntent(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                errorLogClient.AppendToDebugLog("User input is empty or null.", "IntentRecognizer");
                return "unknown_intent";
            }

            errorLogClient.AppendToDebugLog($"Recognizing intent for input: \"{userInput}\"", "IntentRecognizer");

            var userTokens = tokenizer.Tokenize(userInput);
            errorLogClient.AppendToDebugLog($"Tokenized input: {string.Join(", ", userTokens)}", "IntentRecognizer");

            foreach (var intent in intents)
            {
                foreach (var example in intent.Examples)
                {
                    if (IsMatch(userTokens, example.Tokens))
                    {
                        errorLogClient.AppendToDebugLog($"Recognized intent: \"{intent.Name}\"", "IntentRecognizer");
                        return intent.Name;
                    }
                }
            }

            errorLogClient.AppendToDebugLog("No matching intent found. Returning 'unknown_intent'.", "IntentRecognizer");
            return "unknown_intent";
        }

        // Check if tokens match
        private bool IsMatch(List<string> userTokens, List<string> exampleTokens)
        {
            if (userTokens.Count < exampleTokens.Count)
            {
                return false;
            }

            for (int i = 0; i < exampleTokens.Count; i++)
            {
                if (!userTokens.Contains(exampleTokens[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class Intent
    {
        public string Name { get; set; }
        public List<Example> Examples { get; set; }
        public List<string> Tags { get; set; }
    }

    public class Example
    {
        public string Utterance { get; set; }
        public List<string> Tokens { get; set; }
    }
}
