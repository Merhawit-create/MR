using System.ComponentModel.DataAnnotations;
using jonson.Services;


namespace jonson;

public class Customer
{
    public int CustomerId {get; set;}
    [Required, MaxLength(100)]
    public string? CustomerName {get; set;}
    //public string?_CustomerEmail {get; set;}
    [Required, MaxLength(50)]
    public string? CustomerEmail {get; set;}
    
    public string? City {get; set;}

}