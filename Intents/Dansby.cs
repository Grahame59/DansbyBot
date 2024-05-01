using System;
using Intents;
using Responses;
using System.Collections.Generic;



//------------------------------------------------------------------------------------------------//
//  cd E:\CODES\DansbyBot\DansbyBot\Intents
//  dotnet build
//  dotnet run
//------------------------------------------------------------------------------------------------//


//testing with driver 
public class Driver
{
    public static void Main(string[] args)
    {
        // Create and initialize IntentRecognizer instance
        IntentRecognizer IntentRecog = new IntentRecognizer();
        ResponseRecognizer ResponseRecog = new ResponseRecognizer();
        
        // Test intent recognition
        Console.WriteLine("Welecome to your chat interface. I am Dansby also known as Dansby bot. May I assist you?");
        while (true)
        {
            //prompts User for input and scans it
            Console.WriteLine(); //convo flow format
            Console.WriteLine("USER:");
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