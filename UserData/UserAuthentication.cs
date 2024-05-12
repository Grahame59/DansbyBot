using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UserAuthentication
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }
        public string FavoriteColor { get; set; }
        public string Interests { get; set; }
        public string Location { get; set; }

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
            string filePath = "UserData\\UserDirectory.json";
            users = LoadUsersFromFile(filePath);
        }

        public Dictionary<string, User> LoadUsersFromFile(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userData = JsonSerializer.Deserialize<UserData>(jsonString, options);
                
                // Convert the list of users to a dictionary with username as the key
                var userDictionary = new Dictionary<string, User>();
                foreach (var user in userData.Users)
                {
                    userDictionary.Add(user.Username, user);
                }
                
                return userDictionary;
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
                    Console.WriteLine($"Logged in as: {username}");
                    Console.WriteLine();
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
    }

    // Helper class to match the JSON structure
    public class UserData
    {
        public List<User> Users { get; set; }
    }
}
