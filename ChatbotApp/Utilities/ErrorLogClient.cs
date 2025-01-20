using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class ErrorLogClient
{
    private static ErrorLogClient _instance;
    private static readonly object _lock = new object();

    // Singleton instance
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

    // Method to send error to the server
    private async Task SendErrorToServerAsync(string message, string script)
    {
        try
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync("192.168.1.38", 5000);

                using (var stream = client.GetStream())
                {
                    string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string logEntry = $"[{dateTime}] [{script}] {message}";

                    byte[] data = Encoding.UTF8.GetBytes(logEntry);
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log to server: {ex.Message}");
        }
    }

    // Async method to append an error log
    public async Task AppendToErrorLogAsync(string error, string script)
    {
        await SendErrorToServerAsync(error, script);
    }

    // Async method to append a debug log
    public async Task AppendToDebugLogAsync(string debug, string script)
    {
        await SendErrorToServerAsync(debug, script);
    }

    // Sync method to send error to the server
    private void SendErrorToServer(string message, string script)
    {
        try
        {
            using (var client = new TcpClient("192.168.1.38", 5000))
            {
                using (var stream = client.GetStream())
                {
                    string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string logEntry = $"[{dateTime}] [{script}] {message}";

                    byte[] data = Encoding.UTF8.GetBytes(logEntry);
                    stream.Write(data, 0, data.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending log to server: {ex.Message}");
        }
    }

    // Sync method to append an error log
    public void AppendToErrorLog(string error, string script)
    {
        SendErrorToServer(error, script);
    }

    // Sync method to append a debug log
    public void AppendToDebugLog(string debug, string script)
    {
        SendErrorToServer(debug, script);
    }
    
}
