using System;
using System.IO;
using System.Text.Json;
using ChatbotApp.Utilities;

namespace ChatbotApp.UserData
{
    public class UserManager
    {
        private ErrorLogClient errorLogClient;
        private User currentUser;

        public UserManager()
        {
            errorLogClient = new ErrorLogClient();
        }

        public void SaveSession()
        {
            try
            {
                if (currentUser != null)
                {
                    string jsonString = JsonSerializer.Serialize(currentUser);
                    File.WriteAllText("CurrentSession.json", jsonString);
                    errorLogClient.AppendToDebugLog($"Session saved for user {currentUser.Username}.", "UserManager.cs");
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Failed to save session: {ex.Message}", "UserManager.cs");
            }
        }

        public void LoadSession()
        {
            try
            {
                if (File.Exists("CurrentSession.json"))
                {
                    string jsonString = File.ReadAllText("CurrentSession.json");
                    currentUser = JsonSerializer.Deserialize<User>(jsonString);
                    errorLogClient.AppendToDebugLog($"Session loaded for user {currentUser.Username}.", "UserManager.cs");
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Failed to load session: {ex.Message}", "UserManager.cs");
            }
        }
    }
}
