using System.Text.Json.Serialization;
using BCrypt.Net;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    [JsonIgnore] // Do not serialize the password hash for extra safety
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }

    // Constructor for creating new users
    public User(string username, string password)
    {
        Username = username;
        PasswordHash = HashPassword(password);
    }

    // Method to hash a password
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Verify if a plain password matches the hashed password
    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }
}
