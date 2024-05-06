
# DansbyBot - Devlog
A chatbot that can grow with me and doing many things I want it to in my preference. Like Jarvis from Iron Man

# 4/28/24 - 1:45pm 
 - Added a xml for configs... Don't really understand as I've never useed xml before and don't comprehend it well... file added = (DansbyBot.csproj)
 - dependency for the xml console control (.NET 5.0 Runtime (v5.0.17) - Windows x64 Installer!) LINK BELOW 
 - https://download.visualstudio.microsoft.com/download/pr/a0832b5a-6900-442b-af79-6ffddddd6ba4/e2df0b25dd851ee0b38a86947dd0e42e/dotnet-runtime-5.0.17-win-x64.exe 

# 4/28/24 - 3:40pm
 - Added a json file directory and a dictionary that references it. It will hold all the info as the directory of intent grows so the Bot can learn more and more. 
 - CODE BELOW...

 -  //save intent mappings to the file
        SaveIntentMappings(intentMappings);

 -  private Dictionary<string, List<string>> LoadIntentMappings()
    {
        // Load intent mappings from file
        if (File.Exists("intent_mappings.json"))
        {
            string json = File.ReadAllText("intent_mappings.json");
            return JsonConvert.DeserializeObject<IDictionary<string, List<string>>>(json);
        }
        return new Dictionary<string, List<string>>();
    }

# 4/28/24 - 3:55 pm 
 - new dependency:  dotnet add package Newtonsoft.Json (should be permenant but if you get a type or namespace name "Newtonsoft" could not be found type that in the term...)

 # 4/29/24 - 9:48 pm
- working on adding mappings to a response json directory so I can start getting SUPER SIMPLE conversation flow... Not sure when I am gonna start working on tokenization, NLP pipeline, preprocessing, etc... I tried to implement and refactor the whole project to have a cleaner framework relying on this but I was super out of my depth and just left it in an untouched branch DansbyBot_V_0.1 <-- for reference thats the branch in the repository... 

# 4/30/24 - 12:29pm
- improved the map database with more maps and variety in responses and some more intent... Gonna have to make a neural network if I want to advance the 
fluid convo of Dansby... spent 3 hours debugging because I mismatched the intent json database names to the response database json names... It's always the simple mistakes that you spend hours on...............................

# 5/1/24 - 4:11 pm
- implemented the tokenizer into the driver as well as my intentRecognizer script... It now breaks up sentences or an "utterance" into word based tokenizers and matchs the tokens under a intent for it. It still just reguritates random responses I personally wrote
through a json mapping I made same as the intent mapping. Dansby has gotten slight smarter, going to focus on preprocessing and developing the NLP pipeline so I can train it on data for speech patterns but I am learning as I go. Don't want to dive into concepts I don't fully understand so I am really trying to build off the intentRecognizer logic but it seems like the rough for this complexity won't be all that high. Will most likely end up rebuilding and refactoring all the codes later than sooner.

# 5/3/24 - 11:52 pm
- Implemented the first function the chatbot can complete through recognized intents under a switch case... therefore can implement all future intents under the switch case at the moment... I can see this making the intent Recognition class kinda messy and long but I
also don't know what is considered to be a long script as the longest scipt I have made is like 300 lines so not that long... The function he can completed now is very simple and is just exiting the dotnet build and reentering the cd path by using (Environment.Exit(0);).

# 5/4/24 - 12:21 pm
- Ended up just adding a function holder script to keep the functions out of the IntentRecog. script so now to add functions I can code them in there and then add the Intents to the switch that they identify under. 

# 5/6/24 - 6:18 pm
- I added a function to called ListAllFunctions that lists all the functions that are user created on the functions script. I also added a sub function called GetFunctionDescription that uses a switch case and variables to essentially map functions to descriptions if they have a written description. Also added some more intents for responses and recognition as well as implementing some intent recog for the functions and a help intent that explains a little bit about what Dansby can do... Had a quick coding sess but will dive in a lot the next few months on this project as I have just finished my 3rd year of University today ;)... 

# 5/6/24 - 6:22pm 
- NOTE TO SELF: Do some .Net tutorials off this link... https://learn.microsoft.com/en-us/dotnet/core/tutorials/

  


