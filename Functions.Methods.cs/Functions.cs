using System;
using System.Reflection;
using Intents;
using Tokenization;
using UserAuthentication; //for retriving users names when logged in
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Forms;
using ChatbotApp;
using System.Linq;
using System.Threading.Tasks;
using static ErrorLogClient;



namespace Functions
{
    public class functionHoldings
    {

        private MainForm mainForm;
        private static UserManager userManager;
        private const string ListsFilePath = "Functions.Methods.cs\\lists.json"; // Path to save the to-do lists
        private ErrorLogClient errorLogClient = new ErrorLogClient();



        public functionHoldings(MainForm mainForm)
        {
            this.mainForm = mainForm;
            userManager = new UserManager(mainForm);
            
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

        public void ForceSaveLorehaven(object state)
        {
            mainForm.Autosave(state);
            mainForm.AppendToChatHistory("Dansby: Lorehaven has been pushed to the main branch.");
            errorLogClient.AppendToDebugLog("Dansby performed a manual pull, add, commit, push to Lorehaven repo.", "Functions.cs");
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
                
                case "ForceSaveLorehaven" :
                    return "This is a method that runs a Autosave.bat file that pulls, adds, commits, and pushes my Obisidian Vault to my Github(Grahame59/Lorehaven).";

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

        


    } //end of class functionHoldings




}//end of namespace Functions