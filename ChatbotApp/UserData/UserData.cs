using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System;
using ChatbotApp.Utilities;

public class UserManager
{
    private Dictionary<string, User> users;
    private User currentUser;
    private const string UserFilePath = "UserDirectory.json";
    private ErrorLogClient errorLogClient;

    public UserManager()
    {
        errorLogClient = new ErrorLogClient();
        users = LoadUsersFromFile(UserFilePath);
    }

    private Dictionary<string, User> LoadUsersFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) return new Dictionary<string, User>();

            string jsonString = File.ReadAllText(filePath);
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
            errorLogClient.AppendToErrorLog($"Failed to load user data: {ex.Message}", "UserData.cs");
            return new Dictionary<string, User>();
        }
    }

    public void SaveUsersToFile()
    {
        try
        {
            var userData = new UserData { Users = new List<User>(users.Values) };
            string jsonString = JsonSerializer.Serialize(userData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(UserFilePath, jsonString);
            errorLogClient.AppendToDebugLog("User data saved successfully.", "UserData.cs");
        }
        catch (Exception ex)
        {
            errorLogClient.AppendToErrorLog($"Failed to save user data: {ex.Message}", "UserData.cs");
        }
    }

    public bool Login(string username, string password)
    {
        if (users.TryGetValue(username, out var user) && user.VerifyPassword(password))
        {
            currentUser = user;
            errorLogClient.AppendToDebugLog($"User {username} logged in successfully.", "UserData.cs");
            return true;
        }
        errorLogClient.AppendToDebugLog($"Login failed for user {username}.", "UserData.cs");
        return false;
    }

    public void Logout()
    {
        currentUser = null;
        errorLogClient.AppendToDebugLog("User logged out.", "UserData.cs");
    }

    public void AddUser(User user)
    {
        if (users.ContainsKey(user.Username))
        {
            errorLogClient.AppendToErrorLog($"User {user.Username} already exists.", "UserData.cs");
            return;
        }

        users[user.Username] = user;
        SaveUsersToFile();
        errorLogClient.AppendToDebugLog($"User {user.Username} added successfully.", "UserData.cs");
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
