using System.Net.Sockets;
using System;

public class ErrorLogClient 
{
    private static ErrorLogClient _instance;
    private static readonly object _lock = new object();

    public static ErrorLogClient Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ErrorLogClient();
                    }
                }
            }
            return _instance;
        }
    }
    private void SendErrorToServer(string error, string script)
    {
    try
    {
        using (TcpClient client = new TcpClient("localhost", 5000))
        using (NetworkStream stream = client.GetStream())
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"[{dateTime}] [{script}] {error}";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(logEntry);
            stream.Write(data, 0, data.Length);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending log to server: {ex.Message}");
    }
    }

    public void AppendToErrorLog(string error, string script)
    {
        SendErrorToServer(error, script);
    }

    public void AppendToDebugLog(string debug, string script)
    {
        SendErrorToServer(debug, script);
    }


}