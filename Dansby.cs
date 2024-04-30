using System;
using Intents;
using Responses;
using System.Collections.Generic;



//------------------------------------------------------------------------------------------------//
//  cd E:\CODES\DansbyChatBot\DansbyBot\DansbyBot\NaturalLanguageProcessing
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
        while (true)
        {
            //prompts User for input and scans it
            Console.WriteLine("USER:");
            string userInput = Console.ReadLine();

            //sets recognizedIntent = to the intent grouping
            string recognizedIntent = IntentRecog.RecognizeIntent(userInput);
            Console.WriteLine(); //convo flow format
            Console.WriteLine($"Recognized intent: {recognizedIntent}"); //prints the intent group

            Console.WriteLine(); //convo flow format
            string returnResponse = ResponseRecog.RecognizeResponse(userInput);
            Console.WriteLine($"DANSBY: {returnResponse}");
            //


        }
    }
}