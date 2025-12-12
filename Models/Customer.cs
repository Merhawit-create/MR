using System.ComponentModel.DataAnnotations;
using jonson.Services;


namespace jonson;

public class Customer
{
    public int CustomerId {get; set;}
    public string? CustomerName {get; set;}
    //public string?_CustomerEmail {get; set;}
    private string? _customerEmail;

    [Required, MaxLength(50)]
    public string? CustomerEmail
    {
        get => _customerEmail == null ? null : Encryption.Decrypt(_customerEmail);
        set => _customerEmail = string.IsNullOrEmpty(value) ? null : Encryption.Encrypt(value);
    }
    public string? City {get; set;}

}