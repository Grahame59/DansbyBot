using System;
using System.Collections.Generic;

namespace Tokenization
{
    public class Tokenizer
    {
        public List<string> Tokenize(string input)
        {
            // Split the input string into words based on whitespace
            string[] words = input.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Remove any punctuation from the words (optional)
            List<string> tokens = new List<string>();
            foreach (string word in words)
            {
                string cleanWord = RemovePunctuation(word);
                tokens.Add(cleanWord);
            }

            return tokens;
        }

        private string RemovePunctuation(string word)
        {
            // Implement logic to remove punctuation marks from a word
            // Example: Replace any punctuation marks with an empty string
            return word; // Placeholder, implement actual logic
        }
    }
}