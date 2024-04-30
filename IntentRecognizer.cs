using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Intents
{
    public class IntentRecognizer
    {
        private Dictionary<string, List<string>> intentMappings;

        public IntentRecognizer()
        {
            // Initialize intent mappings
            intentMappings = LoadIntentMappings();
        }

        public void AddIntent(string intentName, List<string> exampleUtterances)
        {
            // Check if the intent already exists
            if (intentMappings.ContainsKey(intentName))
            {
                // If the intent exists, append the new utterances to the existing ones
                intentMappings[intentName].AddRange(exampleUtterances);
            }
            else
            {
                // If the intent doesn't exist, add it with the new utterances
                intentMappings[intentName] = exampleUtterances;
            }
            // Save intent mappings to the file
            SaveIntentMappings(intentMappings);
        }

        private Dictionary<string, List<string>> LoadIntentMappings()
        {
            // Load intent mappings from file
            if (File.Exists("intent_mappings.json"))
            {
                string json = File.ReadAllText("intent_mappings.json");
                return (Dictionary<string, List<string>>)JsonConvert.DeserializeObject(json, typeof(Dictionary<string, List<string>>));
            }
            return new Dictionary<string, List<string>>();
        }


        private void SaveIntentMappings(Dictionary<string, List<string>> mappings)
        {
            // Serialize intent mappings to JSON and save to file
            string json = JsonConvert.SerializeObject(mappings, Formatting.Indented);
            File.WriteAllText("intent_mappings.json", json);
        }

        public string RecognizeIntent(string userInput)
        {
            // Convert user input to lowercase for case-insensitive matching
            userInput = userInput.ToLower();

            // Match user input to intents based on predefined mappings
            foreach (var mapping in intentMappings)
            {
                foreach (var utterance in mapping.Value)
                {
                    if (userInput.Contains(utterance))
                    {
                        return mapping.Key; // Return the recognized intent
                    }
                }
            }

            // If intent is not recognized, prompt user to provide meaning
            Console.WriteLine($"I didn't understand what you meant by: \"{userInput}\"." +  " Please provide the meaning (intent) for this input:");
            string newIntent = Console.ReadLine().ToLower().Trim();

            // Add the new intent and example utterance
            AddIntent(newIntent, new List<string> { userInput });

            return newIntent; // Return the newly added intent
        }

    }
} //end of Intents namespace
    