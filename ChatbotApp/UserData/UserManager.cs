using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ChatbotApp.Utilities;

namespace ChatbotApp.UserData
{
    public class UserManager
    {
        private readonly ErrorLogClient errorLogClient;
        private User currentUser;
        private const string UsersFilePath = "Users.json";

        public UserManager()
        {
            errorLogClient = ErrorLogClient.Instance; // Using singleton instance
        }

        // Save current session to a JSON file
        public async Task SaveSessionAsync()
        {
            try
            {
                if (currentUser != null)
                {
                    string jsonString = JsonSerializer.Serialize(currentUser);
                    await File.WriteAllTextAsync("CurrentSession.json", jsonString);
                    await errorLogClient.AppendToDebugLogAsync($"Session saved for user {currentUser.Username}.", "UserManager.cs");
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Failed to save session: {ex.Message}", "UserManager.cs");
            }
        }

        // Load session from a JSON file
        public async Task LoadSessionAsync()
        {
            try
            {
                if (File.Exists("CurrentSession.json"))
                {
                    string jsonString = await File.ReadAllTextAsync("CurrentSession.json");
                    currentUser = JsonSerializer.Deserialize<User>(jsonString);
                    await errorLogClient.AppendToDebugLogAsync($"Session loaded for user {currentUser.Username}.", "UserManager.cs");
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Failed to load session: {ex.Message}", "UserManager.cs");
            }
        }

        // Get the current logged-in user
        public User GetCurrentUser()
        {
            return currentUser;
        }

        // Set the current logged-in user
        public void SetCurrentUser(User user)
        {
            currentUser = user;
        }

        // Validate user credentials from Users.json
        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            try
            {
                if (!File.Exists(UsersFilePath))
                {
                    await errorLogClient.AppendToErrorLogAsync($"Users file not found: {UsersFilePath}", "UserManager.cs");
                    return false;
                }

                string jsonString = await File.ReadAllTextAsync(UsersFilePath);
                var users = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

                if (users != null && users.ContainsKey(username) && users[username] == password)
                {
                    currentUser = new User { Username = username };
                    await SaveSessionAsync();
                    await errorLogClient.AppendToDebugLogAsync($"User {username} logged in successfully.", "UserManager.cs");
                    return true;
                }
                else
                {
                    await errorLogClient.AppendToErrorLogAsync($"Invalid login attempt for user {username}.", "UserManager.cs");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error validating user: {ex.Message}", "UserManager.cs");
                return false;
            }
        }

        // Add a new user to the system
        public async Task AddUserAsync(User user)
        {
            try
            {
                var users = await LoadUsersFromFileAsync();
                if (users.ContainsKey(user.Username))
                {
                    await errorLogClient.AppendToErrorLogAsync($"User {user.Username} already exists.", "UserManager.cs");
                    return;
                }

                users[user.Username] = user;
                await SaveUsersToFileAsync(users);
                await errorLogClient.AppendToDebugLogAsync($"User {user.Username} added successfully.", "UserManager.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Failed to add user: {ex.Message}", "UserManager.cs");
            }
        }

        // Load users from Users.json
        private async Task<Dictionary<string, User>> LoadUsersFromFileAsync()
        {
            try
            {
                if (!File.Exists(UsersFilePath)) return new Dictionary<string, User>();

                string jsonString = await File.ReadAllTextAsync(UsersFilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userData = JsonSerializer.Deserialize<UserData>(jsonString, options);

                var userDictionary = new Dictionary<string, User>();
                foreach (var user in userData.Users)
                {
                    userDictionary[user.Username] = user;
                }

                return userDictionary;
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Failed to load user data: {ex.Message}", "UserManager.cs");
                return new Dictionary<string, User>();
            }
        }

        // Save users to Users.json
        private async Task SaveUsersToFileAsync(Dictionary<string, User> users)
        {
            try
            {
                var userData = new UserData { Users = new List<User>(users.Values) };
                string jsonString = JsonSerializer.Serialize(userData, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(UsersFilePath, jsonString);
                await errorLogClient.AppendToDebugLogAsync("User data saved successfully.", "UserManager.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Failed to save user data: {ex.Message}", "UserManager.cs");
            }
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        // Method to verify password (hash comparison can be added)
        public bool VerifyPassword(string password)
        {
            return PasswordHash == password; // Simplified for now
        }
    }

    public class UserData
    {
        public List<User> Users { get; set; }
    }
}
