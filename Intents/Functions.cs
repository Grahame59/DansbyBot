using System;
using System.Reflection;

namespace Functions
{
    public class functionHoldings
    {
        
        public void PerformExitDansby() //Exit function
        {
            //text explanation for convo flow w/ the exit method
            Console.WriteLine();
            Console.WriteLine("DANSBY: I feel so cold... don't leave me creator... goodbye");
            Console.WriteLine(" -> SUCCESFULLY EXITING DANSBYCHATBOT");
            Console.WriteLine();

            Environment.Exit(0);

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


                default:
                    return "No description available.";
            }
        }


    } //end of class functionHoldings




}//end of namespace Functions