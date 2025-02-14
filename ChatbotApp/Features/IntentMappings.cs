using System.Collections.Generic;

namespace ChatbotApp.Features
{
    public class Example
    {
        public string Utterance { get; set; }
        public List<string> Tokens { get; set; }
    }

    public class IntentMapping
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public List<Example> Examples { get; set; }
    }
}
