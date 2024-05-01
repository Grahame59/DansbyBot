//test for tokenizer
using System;
using System.Collections.Generic;
using Tokenization;

// E:\CODES\DansbyBot\DansbyBot\NLP pipeline

 class Program
    {
        static void Main(string[] args)
        {
            Tokenizer tokenizer = new Tokenizer();

            // Example input text
            string inputText = "Hello world! This is a sample sentence.";

            // Tokenize the input text
            List<string> tokens = tokenizer.Tokenize(inputText);

            // Output the tokens
            Console.WriteLine("Tokens:");
            foreach (string token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }