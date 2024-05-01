
# DansbyBot
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
- update tokenizer... explain when back from work...


