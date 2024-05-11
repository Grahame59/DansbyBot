using System;
using System.Collections.Generic;
using System.IO; // This namespace is needed for File.ReadAllText
using System.Text.Json; // This namespace is needed for JsonSerializer


namespace UserAuthentication
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        // Additional user properties (e.g., email, birthday, etc.) can be added here

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
    public class UserManager
    {
        private Dictionary<string, User> users;

        public UserManager()
        {
            string filePath = "Intents\\UserData\\UserDirectory.json"; 
            users = LoadUsersFromFile(filePath);
        }

        public Dictionary<string, User> LoadUsersFromFile(string filePath)
        {
            try
            {
                // Read the JSON file
                string jsonString = File.ReadAllText(filePath);

                // Deserialize the JSON string to a Dictionary<string, User>
                return JsonSerializer.Deserialize<Dictionary<string, User>>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user data: {ex.Message}");
                return new Dictionary<string, User>();
            }
        }

        public bool Login(string username, string password)
        {
            // Check if the user exists
            if (users.ContainsKey(username))
            {
                // Validate the password
                if (users[username].Password == password)
                {
                    Console.WriteLine($"Welcome, {username}!");
                    return true;
                }
                else
                {
                    Console.WriteLine("Dansby: Incorrect password. Please try again.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Dansby: User does not exist. Please sign up.");
                return false;
            }
        }

            // Other methods for user registration, password recovery, etc.
    }//end of class user
}//end of namespace

