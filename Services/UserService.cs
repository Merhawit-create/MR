using System.Text.Json;

namespace jonson;

public class UserService
{
       private static readonly string FileName = Path.Combine(AppContext.BaseDirectory, "users.json");

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    private async Task<List<User>> LoadAsync()
    {
        if (!File.Exists(FileName))
            return new List<User>();

        var json = await File.ReadAllTextAsync(FileName);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    private async Task SaveAsync(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, _options);
        await File.WriteAllTextAsync(FileName, json);
    }

    // REGISTER
    public async Task RegisterAsync()
    {
        var users = await LoadAsync();

        Console.Write("Username: ");
        var username = Console.ReadLine() ?? "";

        if (users.Any(u => u.Username == username))
        {
            Console.WriteLine("User already exists.");
            return;
        }

        Console.Write("Password: ");
        var password = Console.ReadLine() ?? "";

        var (hash, salt) = PasswordService.CreateHash(password);

        var user = new User
        {
            UserId = users.Any() ? users.Max(u => u.UserId) + 1 : 1,
            Username = username,
            PasswordHash = hash,
            PasswordSalt = salt
        };

        users.Add(user);
        await SaveAsync(users);

        Console.WriteLine("User registered.");
    }

    // LOGIN
    public async Task LoginAsync()
    {
        var users = await LoadAsync();

        Console.Write("Username: ");
        var username = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        var password = Console.ReadLine() ?? "";

        var user = users.FirstOrDefault(u => u.Username == username);

        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        bool ok = PasswordService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);

        if (ok)
            Console.WriteLine("Login successful.");
        else
            Console.WriteLine("Wrong password.");
    }
}