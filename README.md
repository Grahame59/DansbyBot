
# DansbyBot - Devlog
A chatbot that can grow with me and doing many things I want it to in my preference. Like Jarvis from Iron Man

# 4/28/24 - 1:45pm #1
 - Added a xml for configs... Don't really understand as I've never useed xml before and don't comprehend it well... file added = (DansbyBot.csproj)
 - dependency for the xml console control (.NET 5.0 Runtime (v5.0.17) - Windows x64 Installer!) LINK BELOW 
 - https://download.visualstudio.microsoft.com/download/pr/a0832b5a-6900-442b-af79-6ffddddd6ba4/e2df0b25dd851ee0b38a86947dd0e42e/dotnet-runtime-5.0.17-win-x64.exe 

# 4/28/24 - 3:40pm #2
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

# 4/28/24 - 3:55 pm #3
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

# 5/6/24 - 6:18 pm #1
- I added a function to called ListAllFunctions that lists all the functions that are user created on the functions script. I also added a sub function called GetFunctionDescription that uses a switch case and variables to essentially map functions to descriptions if they have a written description. Also added some more intents for responses and recognition as well as implementing some intent recog for the functions and a help intent that explains a little bit about what Dansby can do... Had a quick coding sess but will dive in a lot the next few months on this project as I have just finished my 3rd year of University today ... 

# 5/6/24 - 6:22pm #2
- NOTE TO SELF: Do some .Net tutorials off this link... https://learn.microsoft.com/en-us/dotnet/core/tutorials/

# 5/7/24 - 1:22am
- Added a function that returns the current time of your time zone based of the system time. Also added crosscompatibility for multiple OS in the .NET configs... specifically for my MacOS so I can code when not home. Also added a function for the date as well. Want to implement a calender, dayOfTheWeek function, etc... and have it keep track of things. Think I need to work on an interface outside of the console next as well.

# 5/8/24 - 11:56pm
- Added some random updates for some polsihing... much work needed... Want to work on building a much stronger response/intent script but should probably focus on NLP and having the bot understand info... Want to add more functions as well will try to make up a framework plan instead of jumping all over the place as I currently am. Want to make all the scripts more modular and refernceable and less dependent on one another and certain inputs etc... 

# 5/10/24 - 5:06 pm #1
- Reworked the Intent classifications a bit and tried to implement a PerformSimpleMath() Function but crashed and burned trying to seperate the tokens from userInput into the function need to work on a better way to implement... Might switch focus over right now just gonna revert the code pre- mtah function and save it all to github so I can play around with some more features wanna implement as much as possible today! 

# 5.10.24 - 8:16 pm #2
- Added an addition function, having the problem of double output on all functions except PerformExitDansby(); ... Cannot find where it is double executing 

# 5.10.24 - 10:12pm #3
- Added subtraction, multiplication, and division functions and some more intent reworking... 

# 5.11.24 - 8:39pm
- Added User login and a directory for user data... worked on the paths and switched to relative paths and learned a bit more about path connections and exits. Also reworked and organized the file structure to be much cleaner.. Need to work on security Hash/Salt for security for passwords usernames, personal info etc...

# 5.16.24 - 2:38am
- I am working on the user script accesability across multiple scripts and specifically in making functions that can pull data from the UserAuthentication namespace and do things with the data but I am having problems with the info carrying over. Adding a ton of debugs in right now and once I fix this in my next coding dive I think I want to add admin only login accesiable commands and add in a ton of debugs for things that I can turn on and off with intents but only with admin permissions. Gonna focus towards a GUI or some non console interface soon to add a level of polish and that should involve a lot of refactoring the code where I can clean some stuff up because it is getting admitly pretty messy but this is still my first ever pure code project where I am really focusing on building something with depth.

# 5.21.24 - 2:14am
- Fixed a lot of bugs with the user script accesibility. Only havinf issues with listing current data such as GetCurrentUsername(). I think the user currently logged in needs to become a global property of a log in instance. Not really sure got to look into it more. Also added a isAdmin property to the user database to give admin permissions to apllicable users but it seems to recognized the class has admin privs. and then jump over the if bool statement in a method that has admin privs... Honestly was pretty confused so just felt like coming back to it later this is more of a personal reminder <- .... Might keep playing around and adding more functionality for the user aspect needs a lot of work but honestly I am the only current user who will have access to this bot for the most part. Will commit another push if I add any additions tonight. 

# 5.23.24 - 12:58 am
- So I am having file path issue compatability And i think I need to follow the guidelines for peoper Json file paths and make its own script. Going to write all the errors I currently am having below...

    ERRORS:

    - Double console print when functions are called like it being called upon twice.
    (README 5.10.24 #1)

    - User data inheritance issues unless I simulate a login 
    (README 5.16.24 & 5.21.24)

    - Path compatibility for MAC... Need to add proper JSON documentation
    (README 5.23.24)

# 5.24.24 - 2:43pm #1
- This isn't a commit update but a reminder to Hash all user data sooner than later for future protection... 

# 5.24.24 - 2:50pm #2
- Fixed the user data inheritance by creating a static variable in the driver when login info is logged... Need to work on implementing more features or fixing the double console print now. <- I CANNOT figure out the reason for double console output... extremely frustrating... (3:29pm)

# 5.25.24 - 9:48pm 
- Working on more robust convo responses and intent recog. Want to start adding more complex functions like lists and events and stuff...

# 5.27.31 - 2:32pm #1
- Fixed the AdminPriv error so now I can implement more Admin View Only commands as well as hopefully a debug on and off so I can make it more user friendly without removing my debugs with comments. 

# 5.27.24 - 3:36pm #2
- Created a couple function pretaining to lists and like creating lists, additems to lists, removing items from lists, displaying a list and displaying all lists. Its working pretty well other than the data not being saved onto the json file but it also is pretaining the data through multiple sessions so Ill come back and polish it up and add more details later b/c its kind of boring me at the moment. 

# 5.27.24 - 4:50pm #3
- Implemented a rough snake game through console but it has a lot of issues like independent movement in turn base for each char that represents the body and lack of constant forced movement in one direction... Needs MUCH better game logic plus a border for the game space as well as polish for over all look. Going to take a break for some time or remainder of the day... 

# 6.3.24 - 3:28 am
- So snake made me finally start creating an interface for Dansby so i deleted Snake game and started refactoring a lot of code for winForms... Had a rough time starting to learn WinForms a bit focusing on porting all the console environment code over to WinForms... gonna try to just refactor everything and then push it all to git when it is done... 

# 6.3.24 - 3:56 pm #2 
- SO currently worked over the functions partially but when you prompt for it for a function or convo flow mapping it opens a new textbox. This is NOT what I want so I tried to create a MainForm mainForm = new Mainform yatayatayata so i could use my AppendTextmethod... in the ChatBotApp driver script but this created an infinte loop so gotta play around and fix this but it is 4 am sooo until next session! (Also will have to go through UserAuthentication next to change from console to MessageBox output and I am sure this is more to refactor btw this entry is more personal note devlog currently...)

# 6.3.24 - 3:16 pm #3
- Added Handling for enter key acceptance to send event controls so you don't have to physcially press the send button with the mouse in the Interface. Added color difference in font for DansbyBot and User input...

# 6.4.23 - 1:43 am
- Basically have just been refactoring all the code to work in winforms over a console environment... Not going to explain everything I did as it was pretty boring and technical and a lot of instance issues and so on... 

# 6.5.24 - 2:54 am
- Added more UI polishing such as a label for a logged user, auto scroll when ur chat starts entering the bottom of the physical text box, color block work, all still very work in progress still need to refactor a lot of code from console environment. 

# 6.6.24 - 2:27 am
- Polished the UI interface, worked on the colorblock more going to try to have dark black and greys with vibrant royal purple highlights. Added anchors to everything and a toggle button that will be capable of switching JSON mappings or GPT convo flow on and off so I can build GPT without diverting my full attention to it on the project. Ported over a lot more of code specifically refactored all the User Manager instance into the MainForm for WinForms usage. ETC... Defintely did more but this is all I am recalling right now. 

# 6.8.24 - 2:16 am
- So I added all the packages and the APIs for the GPT AI from Open AI it should be working but I seem to be getting a 401 error. So maybe I just have to wait for it to be activate because I just made it. Not really sure but I am done for now and will test tomorrow. 

# 6.17.24 - 2:16 pm : 
- Need to make a .Make File that will download all dependencies as well as refactor again out of WinForms to Avalon or similiar UI platfrom API for Cross platform functionality so I don't have to refactor more code in the long run... Didn't change any Code yet this is self message trying to cut back my attention on Dansby to 50-50 with another project so I can develope more at once... 

# 7.6.24 - 5:53 pm : #1 
- Adding a Monte Carlo sphere estimation Sim with a Winforms render. Have the logic completed but going to work towards a pop up bar in the DansbyBot Application with multiple buttons that lead to different applications.

# 7.6.24 - 7:02 pm #2
- Added the sidebar for future simulation and extended applications unrelated to the chatbot flow and services. The specific sim I added is a MonteCarlo Simulation of the estimation of a sphere that is access through a sidebar that you can open through the refactored API button for gpt which is now removed from the project. Was just for fun and got the side bar set up want to make some more pleasing simulations as well as add the book form for to do lists off that side bar and make it double accesible through chat flow etc....  Much room for additions with this sidebar implement. 

# 7.8.24 - 10:15 pm #1
- Restructure the project file base and added a new .sln and project using WPF for the Terrarium extension Sim...

# 7.11.24 - 2:47 am 
- So I spent way to long trying to get both WPF and WinForms to work with WinForms hosting WPF with 2 projects but I was having a TONNNNNN of auto generated file repetition of them declaring same data for variables a lot of it dealing with .Net and Assembly.info ... I was deleted the repitions and trying to brute force the framework to work for a while and ended up having a huge issue with 2 entry points for main and from my understanding the main entry in WPF projects is hidden and not accessible through code for editing (This is a top contender for the worlds longest Run on Sentence <---- ) So logically I GAVE UP on that for now. But I got a tonnnnn of work done on the BookForm for recreating a Mincecraft Book as a listing method in Dansby and I have almost all the logic done. All that is lef tto complete it fully for my current wants and needs for its implementation is the RichTextBox Data is not being Loaded properly from the .txt file and I need to fix logic for it... Also fix page numbers make by 2 labels Top left and Top right to correctly display page numbers as individuals (And I just realized this might actually be part of the issue of incorrect data loading into the textboxes), Implement chapterDisplayScreen features for add, delete, edit, move, etc... as well as rework visual appeal of the book to resemble MinecraftBook and add animation to the book. Once complete log everything into the book and use it as lists etc.... But Overall got a lot done for the first time on Dansby in this code session, everything else I have been writing for him has kinda been Uni related (MonteCarloSimForm) or trying to get stupid WPF framework to mesh with WinForms.... 

# 7.25.24 - 12:49 am #1
- Have been away and getting back into Dansby and Terrarium Project. NEED to finish the bookForm but its kinda making me want to rip my hair out so I am probably going to stop for the night here did a little work and then reverted a lot... Figured out the saving/loading error for Page Content is based off new line or edge case problems... Basically if a line isn't full of characters there is some kind of index error and it won't load the next line... Kinda dealing with it but spamming spaces right now lol. Finding this very boring so I am going to leave the bookForm behind for a bit and try to focus on Terrarium next session or maybe overall polishing for Dansby (He is pretty damn rough around the edges right now... ) Gonna attempt to list some things I need to do for Dansby in my BookForm. 

# 7.27.24 - 1:21 am 
- Fixed the multi line saving and loading error For the McBookForm... Going to look into animation for the bookform and style it for simulation...

# 7.31.24 - 3:56 pm 
- Working on adding a soundtrack system to Dansby with the flexibility of changing tracks (Like a Game Menu would have...) Working on fixing the FilePath logiv for the tracks being loaded properly... I believe it is loading the independent files of the mp3's but I am unsure have to add more debugs... Running out of time in this session have to go to work now... Will fix next session and also need to rework the form positioning of the soundtrack button UI... most likely will move them and make them much prettier... 

# 8.9.24 - 3:58 pm 
- Fixed the soundtracks not playing by caring FilePath root in a public variable... And then using Path.Combine to get the full filePath for multiple soundtracks. Reworking the form to be more visually appealing now before I commit... Reworked the MainFrom Aesthetic as well as added some more variety of soundtracks... Have the double output for soundtracks like I did with my functions... This error is HAUNTING ME... 

# 8.12.24 - 6:43 pm
- Added some context through saving lastIntents in the IntentRecognizer script... Dansby can now respond to repeated dates and repeated times with snarky answers b/c why would you repeat that intentAction.

# 8.13.24 - 12:56 pm
- Added a little animated Slime that runs across the top of the RichTextBox and jumps. Plan on adding sounds to him and he randomly appears... Basically is just a little EasterEgg I implemented to learn some PictureBox Functionality through WinForms as well as animating gifs in and such... 

# 8.14.24 - 3:41 pm 
- Been bouncing around polsihing little things here and there... But now working on a clean UI for a seperate form that logs all my Errors and Debugs and saves them so I can work on saving many past issues as well as fixing current errors... Got to go to work soon so this commit will most likely be the only one for today. :/ 

# 8.16.24 - 3:06 am 
- Just did a lot of bug fixing and some Ui changes, generally polishing. 

# 8.16.24 - 5:54 pm 
- Working on more bug fixes, completed the errorlogforms save and load logic for json. Working on context for multiple intent correlations so he can have context between current and last intent. 

# 8.26.24 - 12:50 am 
- Just been doing a ton of bug fixes here and there over past week+ as well as I built a Network Listener into Dansby's ErrorLogForm as well as making it semi independent off of the main form without giving it its own entry point... Going to be gearing towards working on Terrarium Project a lot to learn a lot about Network communication as well as having real time data coming in... Will hopefully be a big first step towards making Dansby have more control and real time data ... 

# 8.27.24 - 2:24 pm
- Was implementing a parser for external errors but ran into some difficulties so instead just used newlines format when the message is sent as one string...

# 9.2.24 - 1:13 am
- Added a death animation state for slime sprite... Need to go through a lot of bug fixes in next Code Sess, been trying to get procedural terrain generation done for Terrarium.