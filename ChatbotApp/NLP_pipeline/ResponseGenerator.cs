using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ChatbotApp.Utilities;

namespace ChatbotApp.Core
{
    public class ResponseGenerator
    {
        private readonly Dictionary<string, List<string>> responseMappings;
        private readonly ErrorLogClient errorLogClient;

        // Path to the response mappings JSON file
        private string responseMappingsFile;

        // Constructor
        public ResponseGenerator(ErrorLogClient errorLogClient, string responseMappingsFilePath = "ChatbotApp\\NLP_pipeline\\response_mappings.json")
        {
            this.errorLogClient = errorLogClient;
            this.responseMappingsFile = responseMappingsFilePath;

            // Load response mappings on initialization
            responseMappings = LoadResponseMappings();
        }

        // Load response mappings from JSON file
        private Dictionary<string, List<string>> LoadResponseMappings()
        {
            try
            {
                if (File.Exists(responseMappingsFile))
                {
                    string json = File.ReadAllText(responseMappingsFile);
                    return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json) ?? new Dictionary<string, List<string>>();
                }
                else
                {
                    errorLogClient.AppendToErrorLog($"Response mappings file not found: {responseMappingsFile}", "ResponseGenerator.cs");
                    return new Dictionary<string, List<string>>();
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error loading response mappings: {ex.Message}", "ResponseGenerator.cs");
                return new Dictionary<string, List<string>>();
            }
        }

        // Generate a response based on intent
        public string GenerateResponse(string intent, string userInput)
        {
            if (string.IsNullOrEmpty(intent))
            {
                errorLogClient.AppendToErrorLog("Intent is null or empty.", "ResponseGenerator.cs");
                return "I'm not sure how to respond to that.";
            }

            if (responseMappings.ContainsKey(intent))
            {
                List<string> possibleResponses = responseMappings[intent];
                string selectedResponse = SelectRandomResponse(possibleResponses);
                return selectedResponse;
            }
            else
            {
                errorLogClient.AppendToErrorLog($"No response mapping found for intent: {intent}", "ResponseGenerator.cs");
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
        public void AddResponseMapping(string intent, List<string> responses)
        {
            if (string.IsNullOrEmpty(intent) || responses == null || responses.Count == 0)
            {
                errorLogClient.AppendToErrorLog("Invalid intent or responses provided for AddResponseMapping.", "ResponseGenerator.cs");
                return;
            }

            if (!responseMappings.ContainsKey(intent))
            {
                responseMappings[intent] = responses;
            }
            else
            {
                responseMappings[intent].AddRange(responses);
            }

            SaveResponseMappings();
        }

        // Save response mappings to JSON file
        private void SaveResponseMappings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(responseMappings, Formatting.Indented);
                File.WriteAllText(responseMappingsFile, json);
                errorLogClient.AppendToDebugLog("Response mappings saved successfully.", "ResponseGenerator.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error saving response mappings: {ex.Message}", "ResponseGenerator.cs");
            }
        }

        // Retrieve responses by tag
        public List<string> GetTaggedResponses(string tag)
        {
            List<string> taggedResponses = new List<string>();

            foreach (var mapping in responseMappings)
            {
                if (mapping.Key.Contains(tag))
                {
                    taggedResponses.AddRange(mapping.Value);
                }
            }

            return taggedResponses;
        }
    }
}
