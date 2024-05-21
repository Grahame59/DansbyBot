using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tokenization;
using Functions;
using UserAuthentication;


namespace Intents
{
    public class IntentRecognizer
    {
        private List<Intent> intents;
        private Tokenizer tokenizer;
        functionHoldings FunctionScript = new functionHoldings(); //Instiate functions class
        public IntentRecognizer()
        {
            // Initialize intent mappings
            intents = LoadIntents();
            tokenizer = new Tokenizer();
        }

        public void AddIntent(Intent intent)
        {
            // Add the new intent
            intents.Add(intent);
            // Save intents to the file
            SaveIntents(intents);
        }

        private List<Intent> LoadIntents()
        {
            // Load intents from file
            if (File.Exists("NLP_pipeline\\intents_mappings.json")) //   file : (intent_mappings.json)
            {
                string json = File.ReadAllText("NLP_pipeline\\intents_mappings.json");
                return JsonConvert.DeserializeObject<List<Intent>>(json);
            }
            return new List<Intent>();
        }

        private void SaveIntents(List<Intent> intents)
        {
            // Serialize intents to JSON and save to file
            string json = JsonConvert.SerializeObject(intents, Formatting.Indented);
            File.WriteAllText("NLP_pipeline\\intents_mappings.json", json);
        }

        public string RecognizeIntent(string userInput)
        {
            
            // Tokenize user input
            List<string> userTokens = tokenizer.Tokenize(userInput);

            // Match tokenized input to intents based on predefined mappings
            foreach (var intent in intents)
            {
                foreach (var example in intent.Examples)
                {
                    // Check if all tokens in example are present in user input
                    bool match = true;
                    foreach (var token in example.Tokens)
                    {
                        if (!userTokens.Contains(token))
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match) //this is the intent match to the functions script, implement functions below in the switch case
                    {
                        // Call PerformIntentAction
                        PerformIntentAction(intent.Name, userInput);
                        // Return the recognized intent name
                        return intent.Name;
                    }
                }
            }

            // If intent is not recognized, prompt user to provide meaning
            Console.WriteLine($"I didn't understand what you meant by: \"{userInput}\". Please provide the meaning (intent) for this input:");
            string newIntentName = Console.ReadLine().ToLower().Trim();

            // Create a new intent with the user input as an example
            Intent newIntent = new Intent
            {
                Name = newIntentName,
                Examples = new List<Example>
                {
                    new Example
                    {
                        Utterance = userInput,
                        Tokens = userTokens
                    }
                }
            };

            // Add the new intent
            AddIntent(newIntent);

            return newIntentName; // Return the newly added intent
        }

        //functions from intents if an intentName is recognized that is not mapped to a response
        private string PerformIntentAction(string intentName, string userInput)
        {
            // Perform specific action based on recognized intent
            switch (intentName.ToLower()) //switch intent name to lowercase for function matches to ensure match
            {   
                //Functions/methods under the cases
                case "performexitdansby":
                    FunctionScript.PerformExitDansby();
                    return "exiting application...";

                //case "method2":
                    //... ; 
                    //return "whatever happens "; 
                
                case "listallfunctions" : 
                    FunctionScript.ListAllFunctions();
                    return "Listing All Functions: ";

                case "time" :
                    FunctionScript.GetTime(); 
                    return "The time.";

                case "date" :
                    FunctionScript.GetDate();
                    return "The date.";

                case "dayofweek" :
                    FunctionScript.GetDayOfTheWeek();
                    return "The day of the week. "; 

                case "subtraction" : 
                    FunctionScript.DoSubtraction();
                    return "Does subtraction of two doubles";

                case "multiplication" :
                    FunctionScript.DoMultiplication();
                    return " Does Multiplication of two doubles.";

                case "division" : 
                    FunctionScript.DoDivision();
                    return " Does Divison of two doubles.";

                case "addition" :
                    FunctionScript.DoAddition();
                    return "Does addition of two doubles. ";   

                case "getcurrentusername" :
                    FunctionScript.GetCurrentUserName();
                    return "Pulls the current Users username and prints it";    
                
                case "listcurrentuserlogininfo" :
                    FunctionScript.ListCurrentUserData();
                    return "Pulls and lists the data of the current user who is logged in";

                case "testuserloginanddisplaydata" :

                    Console.WriteLine("Whats the Username you would like to check?");
                    string testUser = Console.ReadLine();

                    Console.WriteLine();
                    Console.WriteLine("Whats the Password for that username?");
                    string testPass = Console.ReadLine();

                    FunctionScript.TestUserLoginAndDisplayData(testUser, testPass);
                    return "Debugging user data info";


                //if intent is not defined
                default:
                    return $"Action for {intentName} not defined.";
            }
        }    
        
        
    } //end of class IntentRecognizer

    public class Intent
    {
        public string Name { get; set; }
        public List<Example> Examples { get; set; }
    }

    public class Example
    {
        public string Utterance { get; set; }
        public List<string> Tokens { get; set; }
    }

    
} 