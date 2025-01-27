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
                _= errorLogClient.AppendToDebugLogAsync($"Successfully loaded intents from {configFilePath}.", "IntentRecognizer");
            }
            catch (Exception ex)
            {
                _= errorLogClient.AppendToErrorLogAsync($"Error loading intents: {ex.Message}", "IntentRecognizer");
                intents = new List<Intent>();
            }
        }

        // Load intents from a JSON file asynchronously
        private async Task<List<Intent>> LoadIntentsAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _ = errorLogClient.AppendToErrorLogAsync($"Intent file not found at {filePath}.", "IntentRecognizer");
                return new List<Intent>();
            }

            string json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<List<Intent>>(json);
        }

        // Recognize intent based on user input
        public string RecognizeIntent(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                _ = errorLogClient.AppendToDebugLogAsync("User input is empty or null.", "IntentRecognizer");
                return "unknown_intent";
            }

            _ = errorLogClient.AppendToDebugLogAsync($"Recognizing intent for input: \"{userInput}\"", "IntentRecognizer");

            var userTokens = tokenizer.Tokenize(userInput);
            _ = errorLogClient.AppendToDebugLogAsync($"Tokenized input: {string.Join(", ", userTokens)}", "IntentRecognizer");

            foreach (var intent in intents)
            {
                foreach (var example in intent.Examples)
                {
                    if (IsMatch(userTokens, example.Tokens, 0.8)) //80% Threshold match for partial matching
                    {
                        _ = errorLogClient.AppendToDebugLogAsync($"Recognized intent: \"{intent.Name}\"", "IntentRecognizer");
                        return intent.Name;
                    }
                }
            }

            _ = errorLogClient.AppendToDebugLogAsync("No matching intent found. Returning 'unknown_intent'.", "IntentRecognizer");
            return "unknown_intent";
        }

        // Check if tokens match with partial similarity
        private bool IsMatch(List<string> userTokens, List<string> exampleTokens, double threshold = 0.8)
        {
            // Calculate Jaccard similarity
            int intersectionCount = 0;
            HashSet<string> userTokenSet = new HashSet<string>(userTokens, StringComparer.OrdinalIgnoreCase);
            HashSet<string> exampleTokenSet = new HashSet<string>(exampleTokens, StringComparer.OrdinalIgnoreCase);

            foreach (var token in exampleTokenSet)
            {
                if (userTokenSet.Contains(token))
                {
                    intersectionCount++;
                }
            }

            int unionCount = userTokenSet.Count + exampleTokenSet.Count - intersectionCount;

            // Avoid division by zero
            if (unionCount == 0)
            {
                return false;
            }

            double similarity = (double)intersectionCount / unionCount;

            // Debug log for similarity score
            _ = errorLogClient.AppendToDebugLogAsync($"Similarity score: {similarity:F2}", "IntentRecognizer");

            // Return true if similarity meets or exceeds the threshold
            return similarity >= threshold;
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

