using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using ChatbotApp.Utilities;

public class UserManager
{
    private Dictionary<string, User> users;
    private User currentUser;
    private const string UserFilePath = "UserDirectory.json";
    private readonly ErrorLogClient errorLogClient;

    public UserManager()
    {
        errorLogClient = ErrorLogClient.Instance; // Using the singleton instance
        users = LoadUsersFromFileAsync(UserFilePath).GetAwaiter().GetResult();
    }

    private async Task<Dictionary<string, User>> LoadUsersFromFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return new Dictionary<string, User>();

            string jsonString = await File.ReadAllTextAsync(filePath);
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

    public async Task SaveUsersToFileAsync()
    {
        try
        {
            var userData = new UserData { Users = new List<User>(users.Values) };
            string jsonString = JsonSerializer.Serialize(userData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(UserFilePath, jsonString);
            await errorLogClient.AppendToDebugLogAsync("User data saved successfully.", "UserManager.cs");
        }
        catch (Exception ex)
        {
            await errorLogClient.AppendToErrorLogAsync($"Failed to save user data: {ex.Message}", "UserManager.cs");
        }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        if (users.TryGetValue(username, out var user) && user.VerifyPassword(password))
        {
            currentUser = user;
            await errorLogClient.AppendToDebugLogAsync($"User {username} logged in successfully.", "UserManager.cs");
            return true;
        }

        await errorLogClient.AppendToDebugLogAsync($"Login failed for user {username}.", "UserManager.cs");
        return false;
    }

    public async Task LogoutAsync()
    {
        currentUser = null;
        await errorLogClient.AppendToDebugLogAsync("User logged out.", "UserManager.cs");
    }

    public async Task AddUserAsync(User user)
    {
        if (users.ContainsKey(user.Username))
        {
            await errorLogClient.AppendToErrorLogAsync($"User {user.Username} already exists.", "UserManager.cs");
            return;
        }

        users[user.Username] = user;
        await SaveUsersToFileAsync();
        await errorLogClient.AppendToDebugLogAsync($"User {user.Username} added successfully.", "UserManager.cs");
    }

    public User GetCurrentUser()
    {
        return currentUser;
    }

    private class UserData
    {
        public List<User> Users { get; set; }
    }
}
