using System;
using Intents;
using Responses;
using System.Collections.Generic;
using UserAuthentication;


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
    public static void Main(string[] args)
    {
        // Create and initialize IntentRecognizer, ResponseRecognizer, & UserManager instance
        IntentRecognizer IntentRecog = new IntentRecognizer();
        ResponseRecognizer ResponseRecog = new ResponseRecognizer();
        UserManager userManager = new UserManager();

        
        // Test intent recognition
        Console.WriteLine("Welecome to your chat interface. I am Dansby also known as Dansby bot. May I assist you?");
        while (true)
        {

            // Prompt user for login credentials
            Console.WriteLine();
            Console.WriteLine("Enter your username:");
            string username = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();

            // Attempt to login
            userManager.Login(username, password);

            //prompts User for input and scans it
            Console.WriteLine(); //convo flow format
            Console.WriteLine(username,":");
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