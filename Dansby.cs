//this is the main driver for console output and will now be refactored into WinForms... Will save incase I 
//want to revert or reference...

/*

using System;
using Intents;
using Responses;
using System.Collections.Generic;
using UserAuthentication;
using System.Security.Cryptography.X509Certificates;


//------------------------------------------------------------------------------------------------//
//  cd E:\CODES\DansbyBot\DansbyBot\Intents
//  dotnet build
//  dotnet run
//------------------------------------------------------------------------------------------------//
//  for macOS
//  cd /Users/kylergrahame/Desktop/TIcTacToeDev/DansbyBot/Intents
//------------------------------------------------------------------------------------------------//





//testing with driver 
public class Driver
{
    //
    public static string CurInstanceLoginUser;
    public static string CurInstanceLoginPass;
    public static bool CurInstanceIsAdmin;
    public static void Main(string[] args)
    {
        // Create and initialize IntentRecognizer, ResponseRecognizer, & UserManager instance
        IntentRecognizer IntentRecog = new IntentRecognizer();
        ResponseRecognizer ResponseRecog = new ResponseRecognizer();
        UserManager userManager = new UserManager();

        //USER LOGIN BEFORE CHAT ENVIRONMENT LOOP

        // Prompt user for login credentials
        Console.WriteLine();
        Console.WriteLine("Enter your username:");
        string username = Console.ReadLine();
        CurInstanceLoginUser = username;

        Console.WriteLine();
        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();
        CurInstanceLoginPass = password;

        // Attempt to login
        Console.WriteLine();
        bool loginSuccess = userManager.Login(username, password);
        if (loginSuccess)
        {
            CurInstanceIsAdmin = userManager.IsCurrentUserAdmin();
            Console.WriteLine($"Logged in as: {username}");
        }
        else
        {
            Console.WriteLine("Login failed. Logging in as Guest.");
            userManager.Login("guest", "guest");
        }
        
        
        // Test intent recognition
        Console.WriteLine("Welecome to your chat interface. I am Dansby also known as Dansby bot. May I assist you?");
        while (true)
        {

            

            //prompts User for input and scans it
            Console.WriteLine(); //convo flow format
            Console.Write(username + ": "); // Print the username inline with the prompt
            string userInput = Console.ReadLine();

            //sets recognizedIntent = to the intent grouping
            string recognizedIntent = IntentRecog.RecognizeIntent(userInput);
            Console.WriteLine(); //convo flow format
            Console.WriteLine($"Recognized intent: {recognizedIntent}"); //prints the intent group
            
            Console.WriteLine(); //convo flow format
            string returnResponse = ResponseRecog.RecognizeResponse(userInput);
            Console.WriteLine(); //convo format
            Console.WriteLine($"DANSBY: {returnResponse}");
            //


        }
    }
}

*/