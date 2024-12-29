using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class UserManager
{
    private Dictionary<string, User> users;
    private User currentUser;
    private const string UserFilePath = "UserDirectory.json";

    public UserManager()
    {
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
            ErrorLoggerClient.SendError($"Failed to load user data: {ex.Message}");
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
            ErrorLoggerClient.SendDebug("User data saved successfully.");
        }
        catch (Exception ex)
        {
            ErrorLoggerClient.SendError($"Failed to save user data: {ex.Message}");
        }
    }

    public bool Login(string username, string password)
    {
        if (users.TryGetValue(username, out var user) && user.VerifyPassword(password))
        {
            currentUser = user;
            ErrorLoggerClient.SendDebug($"User {username} logged in successfully.");
            return true;
        }
        ErrorLoggerClient.SendDebug($"Login failed for user {username}.");
        return false;
    }

    public void Logout()
    {
        currentUser = null;
        ErrorLoggerClient.SendDebug("User logged out.");
    }

    public void AddUser(User user)
    {
        if (users.ContainsKey(user.Username))
        {
            ErrorLoggerClient.SendError($"User {user.Username} already exists.");
            return;
        }

        users[user.Username] = user;
        SaveUsersToFile();
        ErrorLoggerClient.SendDebug($"User {user.Username} added successfully.");
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
