using System;
using System.Reflection;
using Newtonsoft.Json;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using static ErrorLogClient;

namespace Functions
{
    public class functionHoldings
    {
        private MainForm mainForm;
        private ErrorLogClient errorLogClient = new ErrorLogClient();

        public functionHoldings(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void PerformExitDansby() //Exit function
        {
            try
            {
                // Shutdown message
                string exitMessage = "DANSBY: I feel so cold... don't leave me creator... goodbye";
                mainForm.AppendToChatHistory(exitMessage);
                MessageBox.Show(" -> SUCCESFULLY EXITING DANSBYCHATBOT");

                errorLogClient.AppendToDebugLog("Application is exiting.", "Functions.cs");

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog(ex, "Error during PerformExitDansby.", "Functions.cs");
            }
        }

        public void GetCurrentDateTime()
        {
            try
            {
                DateTime now = DateTime.Now;
                string formattedDateTime = now.ToString("MMMM dd, yyyy hh:mm:ss tt");

                string response = $"Dansby: The current date and time is {formattedDateTime}, and it's a {now.DayOfWeek}.";
                mainForm.AppendToChatHistory(response);

                errorLogClient.AppendToDebugLog("Fetched current date and time.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog(ex, "Error fetching current date and time.", "Functions.cs");
            }
        }

        public void GetTime()
        {
            try
            {
                DateTime now = DateTime.Now;
                string formattedTime = now.ToString("hh:mm:ss tt");

                string response = $"Dansby: The time is {formattedTime}.";
                mainForm.AppendToChatHistory(response);

                errorLogClient.AppendToDebugLog("Fetched current time.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog(ex, "Error fetching time.", "Functions.cs");
            }
        }

        public void GetDate()
        {
            try
            {
                DateTime now = DateTime.Now;
                string formattedDate = now.ToString("MMMM dd, yyyy");

                string response = $"Dansby: The current date is {formattedDate}.";
                mainForm.AppendToChatHistory(response);

                errorLogClient.AppendToDebugLog("Fetched current date.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog(ex, "Error fetching date.", "Functions.cs");
            }
        }

        public void GetDayOfTheWeek()
        {
            try
            {
                string dayOfWeek = DateTime.Now.DayOfWeek.ToString();

                string response = $"Dansby: Today is a {dayOfWeek}.";
                mainForm.AppendToChatHistory(response);

                errorLogClient.AppendToDebugLog("Fetched day of the week.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog(ex, "Error fetching day of the week.", "Functions.cs");
            }
        }

        public void ListAllFunctions()
        {
            try
            {
                string introMessage = "Dansby: Available Functions I can perform currently:";
                mainForm.AppendToChatHistory(introMessage);
                Console.WriteLine(introMessage);

                var methods = typeof(functionHoldings).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (var method in methods)
                {
                    string functionName = method.Name;
                    string functionDescription = GetFunctionDescription(functionName);

                    string message = $"- {functionName}: {functionDescription}";
                    mainForm.AppendToChatHistory(message);
                    Console.WriteLine(message);
                }

                errorLogClient.AppendToDebugLog("Listed all available functions.", "Functions.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog(ex, "Error listing functions.", "Functions.cs");
            }
        }

        public string GetFunctionDescription(string functionName)
        {
            switch (functionName)
            {
                case "PerformExitDansby":
                    return "This function exits the application.";
                case "GetTime":
                    return "This function gives the current time of your time zone.";
                case "GetDate":
                    return "This function gives you the current date.";
                case "GetDayOfTheWeek":
                    return "This function gives you the day of the week.";
                case "GetCurrentDateTime":
                    return "This function gives you the current date, time, and day of the week.";
                case "ListAllFunctions":
                    return "This function lists all the available functions Dansby can complete.";
                default:
                    return "No description available.";
            }
        }
    }
}
