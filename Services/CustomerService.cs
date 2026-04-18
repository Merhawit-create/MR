using jonson.Services;

namespace jonson;



using System.Text.Json;
using System.IO;
using System.Linq;



public class CustomerService
{
    // Vår "datorbasfil" en jsonfil i bin/
    public static readonly string Filename = Path.Combine(AppContext.BaseDirectory, "Customer.Json");

    // Snygg formatering
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    // READ
    public async Task<List<Customer>> LoadAllAsync()
    {
        if (!File.Exists(Filename))
        {
            return new List<Customer>();
        }

        var json = await File.ReadAllTextAsync(Filename);
        var customers = JsonSerializer.Deserialize<List<Customer>>(json, _options)
                        ?? new List<Customer>();
        // Dekryptera email efter läsning
        foreach (var customer in customers)
        {
            if (!string.IsNullOrEmpty(customer.CustomerEmail))
            {
                customer.CustomerEmail = Encryption.Decrypt(customer.CustomerEmail);
            }
        }
        // Säker fallback om JSON är tom eller felaktig
        return customers ;
    }

    // Spara listan till JSON filen
    private async Task SaveAsync(List<Customer> customers)
    {      
        var toSave = customers.Select(customer => new Customer
        {
            CustomerId = customer.CustomerId,
            CustomerName = customer.CustomerName,
            City = customer.City,
            CustomerEmail = string.IsNullOrEmpty(customer.CustomerEmail)
                ? customer.CustomerEmail
                : Encryption.Encrypt(customer.CustomerEmail)
           
        }).ToList();
        var json = JsonSerializer.Serialize(toSave, _options);
        await File.WriteAllTextAsync(Filename, json);
    }

    // enkel auto-inkremetring av customerId
    public async Task AddAsync(Customer customer)
    {
        if (customer is null) throw new ArgumentNullException(nameof(customer));

        var customers = await LoadAllAsync();

        customer.CustomerId = customers.Any() ? customers.Max(c => c.CustomerId) + 1 : 1;

        customers.Add(customer);
        await SaveAsync(customers);
    }

    public async Task UpdateAsync(int customerId, string? customerName, string? customerEmail, string? city)
    {
        var customers = await LoadAllAsync();

        var customer = customers.FirstOrDefault(c => c.CustomerId == customerId);
        if (customer is null)
        {
            Console.WriteLine("Customer not found");
            return;
        }

        if (!string.IsNullOrWhiteSpace(customerName))
            {
                customer.CustomerName = customerName;
            }

            if (!string.IsNullOrWhiteSpace(customerEmail))
            {
                customer.CustomerEmail = customerEmail;
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                customer.City = city;
            }

            await SaveAsync(customers);
        
    }

    public async Task DeleteAsync(int customerId)
        {
            var customers = await LoadAllAsync();
            var customer = customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer is null)
            {
                Console.WriteLine("Customer not found");
            }
            else
            {
                customers.Remove(customer);
                await SaveAsync(customers);
            }
        }

        // Lista kunder
        public static async Task ListCustomersAsync(CustomerService customerService)
        {
            var customers = await customerService.LoadAllAsync();
            if (!customers.Any())
            {
                Console.WriteLine("No Customers");
                return;
            }

            Console.WriteLine("Id | Name | Email | City");
            foreach (var customer in customers)
            {
                Console.WriteLine(
                    $"{customer.CustomerId} | {customer.CustomerName} | {customer.CustomerEmail} | {customer.City}");
            }
        }

        public static async Task AddCustomerAsync(CustomerService customerService)
        {
            Console.WriteLine("Enter Name: ");
            var customerName = Console.ReadLine() ?? "";
            Console.WriteLine("Enter Email: ");
            var customerEmail = Console.ReadLine() ?? "";
            Console.WriteLine("Enter City: ");
            var customerCity = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(customerName))
            {
                Console.WriteLine("Name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                Console.WriteLine("Email is required.");
                return;
            }
            var customer = new Customer()
            {
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                City = customerCity
            };
            await customerService.AddAsync(customer);
            Console.WriteLine("Customer added successfully");
        }




        public static async Task DeleteCustomerAsync(CustomerService customerService)
        {
            await ListCustomersAsync(customerService);
            Console.WriteLine("Enter ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid ID");
                return;
            }

            await customerService.DeleteAsync(customerId);
            Console.WriteLine("Customer deleted successfully");
        }
}
