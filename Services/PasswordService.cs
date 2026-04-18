using System.Security.Cryptography;
using System.Text;

namespace jonson;

public class PasswordService
{
    public static (string hash, string salt) CreateHash(string password)
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);

        using var sha256 = SHA256.Create();
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] combined = passwordBytes.Concat(saltBytes).ToArray();
        byte[] hashBytes = sha256.ComputeHash(combined);

        return (
            Convert.ToBase64String(hashBytes),
            Convert.ToBase64String(saltBytes)
        );
    }

    public static bool VerifyPassword(string password, string savedHash, string savedSalt)
    {
        byte[] saltBytes = Convert.FromBase64String(savedSalt);

        using var sha256 = SHA256.Create();
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

        byte[] combined = passwordBytes.Concat(saltBytes).ToArray();
        byte[] hashBytes = sha256.ComputeHash(combined);

        string newHash = Convert.ToBase64String(hashBytes);

        return newHash == savedHash;
    }
}