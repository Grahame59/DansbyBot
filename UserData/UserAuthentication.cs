using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using Functions;
using ChatbotApp;

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
        public bool IsAdmin { get; set; }


        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class UserManager
    {
        private MainForm mainform;
        private Dictionary<string, User> users;
        private User currentUser; // Variable to store the currently logged-in user

        public UserManager(MainForm mainform)
        {
            this.mainform = mainform;
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

                    //debug 
                    /* 
                    Console.WriteLine($"User Details from JSON: {JsonSerializer.Serialize(users[username])}");
                    Console.WriteLine();  
                    Console.WriteLine("User data from the ListCurrentUserData() method: ");
                    ListCurrentUserData();
                    */
                    //end debug 
                    
                    currentUser = users[username]; // Set the current user
                    return true;
                }
                else
                {
                    //Console.WriteLine("Dansby: Incorrect password. Please try again.");
                    Console.WriteLine();
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Dansby: User does not exist. Please sign up.");
                return false;
            }
        }
        public void ListCurrentUserData()
        {
            if (currentUser != null)
            {
                mainform.AppendToChatHistory("Dansby: Current User Data:");
                mainform.AppendToChatHistory("--------------------------");
                mainform.AppendToChatHistory($"Dansby: User ID: {currentUser.Id}");
                mainform.AppendToChatHistory($"Dansby: Username: {currentUser.Username}");
                mainform.AppendToChatHistory($"Dansby: Email: {currentUser.Email}");
                mainform.AppendToChatHistory($"Dansby: Age: {currentUser.Age}");
                mainform.AppendToChatHistory($"Dansby: Phone Number: {currentUser.PhoneNumber}");
                mainform.AppendToChatHistory($"Dansby: Birthday: {currentUser.Birthday}");
                mainform.AppendToChatHistory($"Dansby: Favorite Color: {currentUser.FavoriteColor}");
                mainform.AppendToChatHistory($"Dansby: Interests: {currentUser.Interests}");
                mainform.AppendToChatHistory($"Dansby: Location: {currentUser.Location}");
                mainform.AppendToChatHistory($"Dansby: Admin Privledge: {currentUser.IsAdmin}");
                mainform.AppendToChatHistory("--------------------------");
                mainform.AppendToChatHistory("");
            }
            else
            {
                mainform.AppendToChatHistory("Dansby: No user logged in.");
            }
        }
        public void GetCurrentUserName()
        {   
            if (currentUser != null)
            {
            Console.WriteLine();
            mainform.AppendToChatHistory($"Dansby: Hello, {currentUser.Username}");
            Console.WriteLine();
            }
            else
            {
            Console.WriteLine();
            mainform.AppendToChatHistory("Dansby: No user logged in.");
            Console.WriteLine();
            }
        }
        public bool IsCurrentUserAdmin()
        {
            return currentUser != null && currentUser.IsAdmin;
        }

          
        // Method to get the current user
        public User GetCurrentUser()
        {
            return currentUser;
        }


        // Helper class to match the JSON structure
        public class UserData
        {
            public List<User> Users { get; set; }
        }
    
    }//end of UserManager class
}//end of UserAuthentication namespace
