using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tokenization
{
    public class Tokenizer
    {
        public List<string> Tokenize(string input)
        {
            // Return an empty list if the input is null or empty
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            // Convert input to lowercase for consistency
            input = input.ToLower();

            // Split the input string into words based on whitespace and special characters
            string[] words = Regex.Split(input, @"\W+");

            // Remove empty entries and return as a list
            return new List<string>(words);
        }

        private string RemovePunctuation(string word)
        {
            // Define a regular expression pattern to match punctuation marks
            string pattern = "[\\p{P}]";

            // Replace punctuation marks with an empty string
            return Regex.Replace(word, pattern, "");
        }
    }
}
