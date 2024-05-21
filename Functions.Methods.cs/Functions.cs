using System;
using System.Reflection;
using Intents;
using Tokenization;
using UserAuthentication; //for retriving users names when logged in
using System.Text.Json;


namespace Functions
{
    public class functionHoldings
    {
       private static UserManager userManager = new UserManager(); // Instantiate UserManager as static
        
        public void PerformExitDansby() //Exit function
        {
            //text explanation for convo flow w/ the exit method
            Console.WriteLine();
            Console.WriteLine("DANSBY: I feel so cold... don't leave me creator... goodbye");
            Console.WriteLine(" -> SUCCESFULLY EXITING DANSBYCHATBOT");
            Console.WriteLine();

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

        }

        public void GetDate()
        {
            
            // Get the current date and format it as a string
            DateTime currentDate = DateTime.Today;

            Console.WriteLine();
            Console.WriteLine("Dansby: The current date is: " + currentDate.ToString("MMMM dd, yyyy"));
            Console.WriteLine();
        }

        public void GetDayOfTheWeek()
        {
        
            
            // Determine the day of the week for the given date
            string[] daysOfWeek = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };


            Console.WriteLine();
            Console.WriteLine("Dansby: The date of the week is " + DateTime.Today.DayOfWeek);
            Console.WriteLine();
        }
        public void ListAllFunctions()
        {
            Console.WriteLine();
            Console.WriteLine("Dansby: Available Functions I can perform currently: ");
            Console.WriteLine();
            var methods = typeof(functionHoldings).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
            string functionName = method.Name;
            string functionDescription = GetFunctionDescription(functionName);
            Console.WriteLine($"- {functionName}: {functionDescription}");
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
                    return "This function gives you the current";

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
                

                default:
                    return "No description available.";
            }

        } //end of GetFunctionDescriptions method


         public void ListCurrentUserData()
        {
            userManager.ListCurrentUserData(); // Call UserManager method to list user data
            User currentUser = userManager.GetCurrentUser();
        }

        // Method to get the current username
        public void GetCurrentUserName()
        {
            userManager.GetCurrentUserName(); // Call UserManager method to get current username
            User currentUser = userManager.GetCurrentUser();
        }

        //DEBUGGING method
        public void TestUserLoginAndDisplayData(string username, string password)
        {
            if (userManager.IsCurrentUserAdmin())
            {
                // Simulate user login
                bool loginSuccess = userManager.Login(username, password);
                if (loginSuccess)
                {
                    // Display user data after successful login
                    userManager.ListCurrentUserData();
                    User currentUser = userManager.GetCurrentUser();
                }
                else
                {
                    Console.WriteLine("Login failed. Please check your username and password.");
                }
            }
            else 
            {
                Console.WriteLine("Dansby: You do not have high enough privledge to view peasant!");
            }
        }

        //DEBUGGING END


 

















        //ALL MATH FUNCTIONS 
        //------------------------------------------------------------------------------------------------------------------------//
        public void DoAddition()
        {
            Console.WriteLine();
            Console.WriteLine("Dansby: Input the first number you want to add: ");
            double num1 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();
            Console.WriteLine("Dansby: Input the second number you want to add: ");
            double num2 = double.Parse(Console.ReadLine()); // Convert string to double

            Console.WriteLine();

            double result = num1 + num2;
            Console.WriteLine($"Dansby: {num1} + {num2} = {result}"); // Convert double to string for output
        }
        public void DoSubtraction()
        {
            Console.WriteLine();
            Console.WriteLine("Dansby: Input the first number you want to subtract: ");
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