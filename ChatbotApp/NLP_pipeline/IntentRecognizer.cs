using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tokenization;
using ChatbotApp.Utilities;
using System.Threading.Tasks;

namespace Intents
{
    public class IntentRecognizer
    {
        private List<Intent> intents;
        private Tokenizer tokenizer;
        private ErrorLogClient errorLogClient;

        public IntentRecognizer()
        {
            tokenizer = new Tokenizer();
            errorLogClient = ErrorLogClient.Instance; // Use Singleton instance
            intents = new List<Intent>(); // Initialize to prevent null reference issues
        }

        public async Task InitializeAsync(string configFilePath = "ChatbotApp\\NLP_pipeline\\intent_mappings.json")
        {
            try
            {
                intents = await LoadIntentsAsync(configFilePath);
                await errorLogClient.AppendToDebugLogAsync($"Successfully loaded intents from {configFilePath}.", "IntentRecognizer");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error loading intents: {ex.Message}", "IntentRecognizer");
                intents = new List<Intent>();
            }
        }

        // Load intents from a JSON file asynchronously
        private async Task<List<Intent>> LoadIntentsAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                await errorLogClient.AppendToErrorLogAsync($"Intent file not found at {filePath}.", "IntentRecognizer");
                return new List<Intent>();
            }

            string json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<List<Intent>>(json);
        }

        // Recognize intent based on user input
        public async Task<string> RecognizeIntentAsync(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                await errorLogClient.AppendToDebugLogAsync("User input is empty or null.", "IntentRecognizer");
                return "unknown_intent";
            }

            await errorLogClient.AppendToDebugLogAsync($"Recognizing intent for input: \"{userInput}\"", "IntentRecognizer");

            var userTokens = tokenizer.Tokenize(userInput);
            await errorLogClient.AppendToDebugLogAsync($"Tokenized input: {string.Join(", ", userTokens)}", "IntentRecognizer");

            foreach (var intent in intents)
            {
                foreach (var example in intent.Examples)
                {
                    if (IsMatch(userTokens, example.Tokens))
                    {
                        await errorLogClient.AppendToDebugLogAsync($"Recognized intent: \"{intent.Name}\"", "IntentRecognizer");
                        return intent.Name;
                    }
                }
            }

            await errorLogClient.AppendToDebugLogAsync("No matching intent found. Returning 'unknown_intent'.", "IntentRecognizer");
            return "unknown_intent";
        }

        // Check if tokens match
        private bool IsMatch(List<string> userTokens, List<string> exampleTokens)
        {
            // Check if all tokens from the example are found in the user's tokens, in the same order
            int userIndex = 0;
            int exampleIndex = 0;

            while (userIndex < userTokens.Count && exampleIndex < exampleTokens.Count)
            {
                if (userTokens[userIndex].Equals(exampleTokens[exampleIndex], StringComparison.OrdinalIgnoreCase))
                {
                    exampleIndex++;
                }
                userIndex++;
            }

            return exampleIndex == exampleTokens.Count;
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
