using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Intents;

namespace Responses
{
    public class ResponseRecognizer 
    {
        private Dictionary<string, List<string>> responseMappings;

        // Instance of IntentRecognizer
        private IntentRecognizer intentRecognizer; 

        public ResponseRecognizer()
        {
            // Initialize response mappings
            responseMappings = LoadResponseMappings();

            // Create an instance of IntentRecognizer
            intentRecognizer = new IntentRecognizer();
        }

        private Dictionary<string, List<string>> LoadResponseMappings()
        {
            // Load response mappings from file
            if (File.Exists("response_mappings.json"))
            {
                string json = File.ReadAllText("response_mappings.json");
                return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            }
            return new Dictionary<string, List<string>>();
        }

        public string RecognizeResponse(string userInput)
        {
            // Recognize intent based on user input
            string intent = intentRecognizer.RecognizeIntent(userInput);

            // Check if the intent has a corresponding response
            if (responseMappings.ContainsKey(intent))
            {
                // Return a response from the mapped responses
                List<string> responses = responseMappings[intent];
                Random rand = new Random();
                int index = rand.Next(responses.Count); // Select a random response
                return responses[index];
            }
            else
            {
                return null;
            }
        }
    }
}