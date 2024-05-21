
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

# 5/7/24 - 1:22am
- Added a function that returns the current time of your time zone based of the system time. Also added crosscompatibility for multiple OS in the .NET configs... specifically for my MacOS so I can code when not home. Also added a function for the date as well. Want to implement a calender, dayOfTheWeek function, etc... and have it keep track of things. Think I need to work on an interface outside of the console next as well.

# 5/8/24 - 11:56pm
- Added some random updates for some polsihing... much work needed... Want to work on building a much stronger response/intent script but should probably focus on NLP and having the bot understand info... Want to add more functions as well will try to make up a framework plan instead of jumping all over the place as I currently am. Want to make all the scripts more modular and refernceable and less dependent on one another and certain inputs etc... 

# 5/10/24 - 5:06 pm 
- Reworked the Intent classifications a bit and tried to implement a PerformSimpleMath() Function but crashed and burned trying to seperate the tokens from userInput into the function need to work on a better way to implement... Might switch focus over right now just gonna revert the code pre- mtah function and save it all to github so I can play around with some more features wanna implement as much as possible today! 

# 5.10.24 - 8:16 pm 
- Added an addition function, having the problem of double output on all functions except PerformExitDansby(); ... Cannot find where it is double executing 

# 5.10.24 - 10:12pm
- Added subtraction, multiplication, and division functions and some more intent reworking... 

# 5.11.24 - 8:39pm
- Added User login and a directory for user data... worked on the paths and switched to relative paths and learned a bit more about path connections and exits. Also reworked and organized the file structure to be much cleaner.. Need to work on security Hash/Salt for security for passwords usernames, personal info etc...

# 5.16.24 - 2:38am
- I am working on the user script accesability across multiple scripts and specifically in making functions that can pull data from the UserAuthentication namespace and do things with the data but I am having problems with the info carrying over. Adding a ton of debugs in right now and once I fix this in my next coding dive I think I want to add admin only login accesiable commands and add in a ton of debugs for things that I can turn on and off with intents but only with admin permissions. Gonna focus towards a GUI or some non console interface soon to add a level of polish and that should involve a lot of refactoring the code where I can clean some stuff up because it is getting admitly pretty messy but this is still my first ever pure code project where I am really focusing on building something with depth.

# 5.21.24 - 2:14am
- Fixed a lot of bugs with the user script accesibility. Only havinf issues with listing current data such as GetCurrentUsername(). I think the user currently logged in needs to become a global property of a log in instance. Not really sure got to look into it more. Also added a isAdmin property to the user database to give admin permissions to apllicable users but it seems to recognized the class has admin privs. and then jump over the if bool statement in a method that has admin privs... Honestly was pretty confused so just felt like coming back to it later this is more of a personal reminder <- .... Might keep playing around and adding more functionality for the user aspect needs a lot of work but honestly I am the only current user who will have access to this bot for the most part. Will commit another push if I add any additions tonight. 