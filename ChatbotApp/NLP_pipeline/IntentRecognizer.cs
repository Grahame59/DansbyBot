using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tokenization;
using ChatbotApp.Utilities;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

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
                _= errorLogClient.AppendToDebugLogAsync($"Successfully loaded intents from {configFilePath}.", "IntentRecognizer.cs");
            }
            catch (Exception ex)
            {
                _= errorLogClient.AppendToErrorLogAsync($"Error loading intents: {ex.Message}", "IntentRecognizer.cs");
                intents = new List<Intent>();
            }
        }

        // Load intents from a JSON file asynchronously
        private async Task<List<Intent>> LoadIntentsAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _ = errorLogClient.AppendToErrorLogAsync($"Intent file not found at {filePath}.", "IntentRecognizer.cs");
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
                _ = errorLogClient.AppendToDebugLogAsync("User input is empty or null.", "IntentRecognizer.cs");
                return "unknown_intent";
            }

            _ = errorLogClient.AppendToDebugLogAsync($"Recognizing intent for input: \"{userInput}\"", "IntentRecognizer.cs");

            var userTokens = tokenizer.Tokenize(userInput);
            _ = errorLogClient.AppendToDebugLogAsync($"Tokenized input: {string.Join(", ", userTokens)}", "IntentRecognizer.cs");

            foreach (var intent in intents)
            {
                foreach (var example in intent.Examples)
                {
                    if (IsMatch(userTokens, example.Tokens, 0.5)) //50% Threshold match for partial matching
                    {
                        _ = errorLogClient.AppendToDebugLogAsync($"Recognized intent: \"{intent.Name}\"", "IntentRecognizer.cs");
                        return intent.Name;
                    }
                }
            }

            _ = errorLogClient.AppendToDebugLogAsync("No matching intent found. Returning 'unknown_intent'.", "IntentRecognizer.cs");
            return "unknown_intent";
        }

        // Check if tokens match with partial similarity
        private bool IsMatch(List<string> userTokens, List<string> exampleTokens, double threshold = 0.8)
        {
            // Calculate Jaccard similarity
            int intersectionCount = 0;
            HashSet<string> userTokenSet = new HashSet<string>(userTokens, StringComparer.OrdinalIgnoreCase);
            HashSet<string> exampleTokenSet = new HashSet<string>(exampleTokens, StringComparer.OrdinalIgnoreCase);

            foreach (var token in exampleTokenSet) // Compares token for token.
            {
                if (userTokenSet.Contains(token))
                {
                    intersectionCount++; // Increments to get a numerical comparison match.
                }
            }

            int unionCount = userTokenSet.Count + exampleTokenSet.Count - intersectionCount;

            // Avoid division by zero
            if (unionCount == 0)
            {
                return false;
            }

            double similarity = (double)intersectionCount / unionCount;

            string logMessage;
            if (similarity >= threshold )
            {
                logMessage = $"Matched! Similarity score: {similarity:F2}";
            } else
            {
                logMessage = "No match: Similarity score did not surpass the threshold.";

            }
            _= errorLogClient.AppendToDebugLogAsync(logMessage, "IntentRecognizer.cs");
            
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

