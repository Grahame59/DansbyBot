using System;
using System.Reflection;
using Intents;
using Tokenization;
using UserAuthentication; //for retriving users names when logged in
using System.Text.Json;
using TaskManagement; 
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Snake;
using System.Threading;
using System.Windows.Forms;
using ChatbotApp;

namespace Functions
{
    public class functionHoldings
    {

        private MainForm mainForm;
        private static UserManager userManager = new UserManager(); // Instantiate UserManager as static
        private static ListManager ListManager = new ListManager(); // Instantiate TodoListManager as static
        private const string ListsFilePath = "Functions.Methods.cs\\lists.json"; // Path to save the to-do lists


        public functionHoldings(MainForm mainForm)
        {
            this.mainForm = mainForm;
            LoadLists();
        }
        
        public void PerformExitDansby() //Exit function
        {
            //text explanation for convo flow w/ the exit method
            Console.WriteLine();
            Console.WriteLine("DANSBY: I feel so cold... don't leave me creator... goodbye");
            Console.WriteLine(" -> SUCCESFULLY EXITING DANSBYCHATBOT");
            Console.WriteLine();

            mainForm.AppendToChatHistory("DANSBY: I feel so cold... don't leave me creator... goodbye");
            MessageBox.Show(" -> SUCCESFULLY EXITING DANSBYCHATBOT");

            Environment.Exit(0);

        }

        public void GetTime()
        {
            // Get the current system time
            DateTime currentTime = DateTime.Now;

            // Format the time as a string
            string formattedTime = currentTime.ToString("hh:mm:ss tt"); // Example format: "03:15:27 PM"

            Console.WriteLine();
            Console.WriteLine(" Dansby: The time is: " + formattedTime);
            Console.WriteLine();

            mainForm.AppendToChatHistory(" Dansby: The time is: " + formattedTime);

        }

        public void GetDate()
        {
            
            // Get the current date and format it as a string
            DateTime currentDate = DateTime.Today;

            Console.WriteLine();
            Console.WriteLine("Dansby: The current date is: " + currentDate.ToString("MMMM dd, yyyy"));
            Console.WriteLine();

            mainForm.AppendToChatHistory("Dansby: The current date is: " + currentDate.ToString("MMMM dd, yyyy"));
        }

        public void GetDayOfTheWeek()
        {
        
            
            // Determine the day of the week for the given date
            string[] daysOfWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };


            Console.WriteLine();
            Console.WriteLine("Dansby: The date of the week is " + DateTime.Today.DayOfWeek);
            Console.WriteLine();

            mainForm.AppendToChatHistory("Dansby: The date of the week is " + DateTime.Today.DayOfWeek);
        }
        public void ListAllFunctions()
        {

            mainForm.AppendToChatHistory("Dansby: Available Functions I can perform currently: ");
            
            Console.WriteLine();
            Console.WriteLine("Dansby: Available Functions I can perform currently: ");
            Console.WriteLine();
            var methods = typeof(functionHoldings).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
            string functionName = method.Name;
            string functionDescription = GetFunctionDescription(functionName);
            Console.WriteLine($"- {functionName}: {functionDescription}");

            mainForm.AppendToChatHistory($"- {functionName}: {functionDescription}");
            }
        }

        // Function description method
        public string GetFunctionDescription(string functionName)
        {
            switch (functionName)
            {
                case "PerformExitDansby":
                    return "This function exits the application.";
                // Add descriptions for other functions as needed

                case "ListAllFunctions": 
                    return "This function lists all the available functions Dansby can complete.";

                case "GetFunctionDescription" : 
                    return "This is a sub function that assists ListAllFunctions by mapping explanations to the functions when they are listed.";

                case "GetTime" :
                    return "This function gives the current time of your time zone";

                case "GetDate" : 
                    return "This function gives you the current date";

                case "GetDayOfTheWeek" :
                    return "This function gives you the day of the week";

                case "DoAddition" :
                    return "This function prompts you for input for 2 nums and adds them. ";

                case "DoSubtraction" :
                    return "This function prompts you for input for 2 nums and subtracts them.";

                case "DoMultiplication" : 
                    return "This function prompts you for input for 2 nums and multiplys them.";

                case "DoDivision" :
                    return "This function prompts you for input for 2 nums and divides them.";

                case "ListCurrentUserData" :
                    return "This function lists your current logged in User data.";

                case "GetCurrentUserName" : 
                    return "This function returns your current logged in Username. ";
                    
                case "TestUserLoginAndDisplayData" :
                    return "This is and Admin only command that allows you to view, a user of your selection, information. ";
                
                case "CreateList" :
                    return "This is a function that can create lists.";

                case "AddItemToList" :
                    return "This is a function that can add items to a defined listName.";

                case "RemoveItemFromList" :
                    return "This is a function that can remove items from a defined listName.";

                case "DisplayList" :
                    return "This is a function that can display a single list of choice from a defined ListName.";

                case "ListAllLists" :
                    return "This is a function that list all lists that are already defined and saved.";
                
                case "StartSnakeGame" : 
                    return "This is a function that loads up snake in console and you can play!";
                

                default:
                    return "No description available.";
            }

        } //end of GetFunctionDescriptions method


         public void ListCurrentUserData()
        {
            // Simulate user login to carry in current user data
            bool loginSuccess = userManager.Login(MainForm.CurInstanceLoginUser, MainForm.CurInstanceLoginPass);
            if (loginSuccess)
            {

                userManager.ListCurrentUserData(); // Call UserManager method to list user data
                User currentUser = userManager.GetCurrentUser();
            }
        }

        // Method to get the current username
        public void GetCurrentUserName()
        {
            // Simulate user login to carry in current user data
            bool loginSuccess = userManager.Login(MainForm.CurInstanceLoginUser, MainForm.CurInstanceLoginPass);
            if (loginSuccess)
            {

                userManager.GetCurrentUserName(); // Call UserManager method to get current username
                User currentUser = userManager.GetCurrentUser();
            }
        }

        //DEBUGGING method
        public void TestUserLoginAndDisplayData(string username, string password)
        {
            if (!MainForm.CurInstanceIsAdmin)
            {
                mainForm.AppendToChatHistory("Dansby: You do not have high enough privilege to view this information.");
                return;
            }
            // Simulate user login
            bool loginSuccess = userManager.Login(username, password);
            if (loginSuccess)
            {
                // Display user data after successful login
                userManager.ListCurrentUserData();
            }
            else 
            {
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory("Login failed. Please check your username and password.");  
            }
        }

        //DEBUGGING END

        // List Functions
        //-----------------------------------------------------------------
        
        public void CreateList(string listName)
        {
            if (!ListManager.ListExists(listName))
            {
                ListManager.CreateList(listName);
                SaveLists();
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory($"Dansby: List '{listName}' created.");
                
            }
            else
            {
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory($"Dansby: List '{listName}' already exists.");
            }
        }

        public void AddItemToList(string listName, string item)
        {
            if (ListManager.ListExists(listName))
            {
                ListManager.AddItemToList(listName, item);
                SaveLists();
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory($"Dansby: Item '{item}' added to list '{listName}'.");
                
            }
            else
            {
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory($"Dansby: List '{listName}' does not exist.");
            }
        }

        public void RemoveItemFromList(string listName, string item)
        {
            if (ListManager.ListExists(listName))
            {
                if (ListManager.RemoveItemFromList(listName, item))
                {
                    SaveLists();
                    mainForm.AppendToChatHistory("");
                    mainForm.AppendToChatHistory($"Dansby: Item '{item}' removed from list '{listName}'.");
                }
                else
                {
                    mainForm.AppendToChatHistory("");
                    mainForm.AppendToChatHistory($"Dansby: Item '{item}' not found in list '{listName}'.");
                }
            }
            else
            {
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory($"Dansby: List '{listName}' does not exist.");
            }
        }

        public void DisplayList(string listName)
        {
            if (ListManager.ListExists(listName))
            {
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory($"Dansby: Items in list '{listName}':");
                foreach (var item in ListManager.GetItemsInList(listName))
                {
                    mainForm.AppendToChatHistory($"- {item}");
                }
            }
            else
            {
                mainForm.AppendToChatHistory("");
                mainForm.AppendToChatHistory($"Dansby: List '{listName}' does not exist.");
            }
        }

        public void ListAllLists()
        {
            mainForm.AppendToChatHistory("");
            mainForm.AppendToChatHistory("Dansby: All Lists:");
            foreach (var listName in ListManager.GetAllLists())
            {
                mainForm.AppendToChatHistory($"- {listName}");
            }
        }

        private void SaveLists()
        {
            string json = JsonConvert.SerializeObject(ListManager.GetAllListsWithItems(), Formatting.Indented);
            File.WriteAllText(ListsFilePath, json);
        }

        private void LoadLists()
        {
            if (File.Exists(ListsFilePath))
            {
                string json = File.ReadAllText(ListsFilePath);
                var lists = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
                ListManager.LoadLists(lists);
            }
        }

 
        //End of List Functions
        //-----------------------------------------------------------------
        public void StartSnakeGame()
        {
            Thread guiThread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SnakeGameForm());
            });
            guiThread.SetApartmentState(ApartmentState.STA);
            guiThread.Start();
        }










        //HAVE TO REFACTOR ALL MATH METHODS BELOW WITH AppendToChatHistory() and Readline() -> SaveResponse() ... 



        //ALL MATH FUNCTIONS 
        //------------------------------------------------------------------------------------------------------------------------//
        public void DoAddition()
        {
            mainForm.AppendToChatHistory("");
            mainForm.AppendToChatHistory("Dansby: Input the first number you want to add: ");
            double num1 = double.Parse(Console.ReadLine()); // Convert string to double

            mainForm.AppendToChatHistory("");
            mainForm.AppendToChatHistory("Dansby: Input the second number you want to add: ");
            double num2 = double.Parse(Console.ReadLine()); // Convert string to double

            mainForm.AppendToChatHistory("");

            double result = num1 + num2;
            mainForm.AppendToChatHistory($"Dansby: {num1} + {num2} = {result}"); // Convert double to string for output
        }
        public void DoSubtraction()
        {
            mainForm.AppendToChatHistory("");
            mainForm.AppendToChatHistory("Dansby: Input the first number you want to subtract: ");
            double num1 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();
            Console.WriteLine("Dansby: Input the second number you want to subtract: ");
            double num2 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();

            double result = num1 - num2;
            Console.WriteLine($"Dansby: {num1} - {num2} = {result}"); // Convert double to string for output
        }

        public void DoMultiplication()
        {
            Console.WriteLine();
            Console.WriteLine("Dansby: Input the first number you want to multiply: ");
            double num1 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();
            Console.WriteLine("Dansby: Input the second number you want to multiply: ");
            double num2 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();

            double result = num1 * num2;
            Console.WriteLine($"Dansby: {num1} * {num2} = {result}"); // Convert double to string for output
        }

        public void DoDivision()
        {
            Console.WriteLine();
            Console.WriteLine("Dansby: Input the first number you want to divide: ");
            double num1 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();
            Console.WriteLine("Dansby: Input the second number you want to divide: ");
            double num2 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();

            double result = num1 / num2;
            Console.WriteLine($"Dansby: {num1} / {num2} = {result}"); // Convert double to string for output
        }
        //------------------------------------------------------------------------------------------------------------------------//
        //END OF MATH FUNCTIONS



    } //end of class functionHoldings




}//end of namespace Functions