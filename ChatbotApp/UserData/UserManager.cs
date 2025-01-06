using System;
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

        public UserManager()
        {
            errorLogClient = ErrorLogClient.Instance; // Using singleton instance
        }

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

        public User GetCurrentUser()
        {
            return currentUser;
        }

        public void SetCurrentUser(User user)
        {
            currentUser = user;
        }
    }
}
