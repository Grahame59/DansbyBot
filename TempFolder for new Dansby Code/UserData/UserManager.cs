public void SaveSession()
{
    try
    {
        if (currentUser != null)
        {
            string jsonString = JsonSerializer.Serialize(currentUser);
            File.WriteAllText("CurrentSession.json", jsonString);
            ErrorLoggerClient.SendDebug($"Session saved for user {currentUser.Username}.");
        }
    }
    catch (Exception ex)
    {
        ErrorLoggerClient.SendError($"Failed to save session: {ex.Message}");
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
            ErrorLoggerClient.SendDebug($"Session loaded for user {currentUser.Username}.");
        }
    }
    catch (Exception ex)
    {
        ErrorLoggerClient.SendError($"Failed to load session: {ex.Message}");
    }
}
