using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ChatbotApp.Utilities;
using System.Threading.Tasks;

namespace ChatbotApp.Core
{
    public class ResponseGenerator
    {
        private Dictionary<string, List<string>> responseMappings;
        private readonly ErrorLogClient errorLogClient;

        // Path to the response mappings JSON file
        private readonly string responseMappingsFile;

        // Constructor
        public ResponseGenerator(ErrorLogClient errorLogClient, string responseMappingsFilePath = "ChatbotApp\\NLP_pipeline\\response_mappings.json")
        {
            this.errorLogClient = errorLogClient;
            this.responseMappingsFile = responseMappingsFilePath;

            // Load the response mappings asynchronously during initialization
            Task.Run(() => InitializeAsync()).Wait();
        }

        // Async initialization method to load response mappings
        private async Task InitializeAsync()
        {
            responseMappings = await LoadResponseMappings();
            _= errorLogClient.AppendToDebugLogAsync("Response mappings loaded successfully.", "ResponseGenerator.cs");
        }

        // Load response mappings from JSON file asynchronously
        private async Task<Dictionary<string, List<string>>> LoadResponseMappings()
        {
            try
            {
                if (File.Exists(responseMappingsFile))
                {
                    string json = await File.ReadAllTextAsync(responseMappingsFile);
                    return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json) ?? new Dictionary<string, List<string>>();
                }
                else
                {
                    _= errorLogClient.AppendToErrorLogAsync($"Response mappings file not found: {responseMappingsFile}", "ResponseGenerator.cs");
                    return new Dictionary<string, List<string>>();
                }
            }
            catch (Exception ex)
            {
                _= errorLogClient.AppendToErrorLogAsync($"Error loading response mappings: {ex.Message}", "ResponseGenerator.cs");
                return new Dictionary<string, List<string>>();
            }
        }

        // Generate a response based on intent
        public string GenerateResponse(string intent, string userInput)
        {
            if (string.IsNullOrEmpty(intent))
            {
                _ = errorLogClient.AppendToErrorLogAsync("Intent is null or empty.", "ResponseGenerator.cs");
                return "I'm not sure how to respond to that.";
            }

            if (responseMappings != null && responseMappings.ContainsKey(intent))
            {
                List<string> possibleResponses = responseMappings[intent];
                string selectedResponse = SelectRandomResponse(possibleResponses);
                return selectedResponse;
            }
            else
            {
                _ = errorLogClient.AppendToErrorLogAsync($"No response mapping found for intent: {intent}", "ResponseGenerator.cs");
                return $"I'm not sure how to respond to that intent: {intent}.";
            }
        }

        // Select a random response from the list of possible responses
        private string SelectRandomResponse(List<string> responses)
        {
            if (responses == null || responses.Count == 0)
            {
                return "I don't have a suitable response for that.";
            }

            Random random = new Random();
            int index = random.Next(responses.Count);
            return responses[index];
        }

        // Add a new response mapping dynamically
        public async Task AddResponseMappingAsync(string intent, List<string> responses)
        {
            if (string.IsNullOrEmpty(intent) || responses == null || responses.Count == 0)
            {
                _= errorLogClient.AppendToErrorLogAsync("Invalid intent or responses provided for AddResponseMapping.", "ResponseGenerator.cs");
                return;
            }

            if (responseMappings.ContainsKey(intent))
            {
                responseMappings[intent].AddRange(responses);
            }
            else
            {
                responseMappings[intent] = responses;
            }

            await SaveResponseMappingsAsync();
        }

        // Save response mappings to JSON file asynchronously
        private async Task SaveResponseMappingsAsync()
        {
            try
            {
                string json = JsonConvert.SerializeObject(responseMappings, Formatting.Indented);
                await File.WriteAllTextAsync(responseMappingsFile, json);
                _= errorLogClient.AppendToDebugLogAsync("Response mappings saved successfully.", "ResponseGenerator.cs");
            }
            catch (Exception ex)
            {
                _= errorLogClient.AppendToErrorLogAsync($"Error saving response mappings: {ex.Message}", "ResponseGenerator.cs");
            }
        }

        // Retrieve responses by tag
        public List<string> GetTaggedResponses(string tag)
        {
            List<string> taggedResponses = new List<string>();

            if (responseMappings != null)
            {
                foreach (var mapping in responseMappings)
                {
                    if (mapping.Key.Contains(tag, StringComparison.OrdinalIgnoreCase))
                    {
                        taggedResponses.AddRange(mapping.Value);
                    }
                }
            }

            return taggedResponses;
        }
    }
}
