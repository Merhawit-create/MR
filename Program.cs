namespace jonson.Services;
using System;
using System.Threading.Tasks;
using jonson.Services;



public class Program
    {

        
        
        
        public static async Task Main(string[] args){

           
            var service = new CustomerService();
            var userService = new UserService();
           

           while (true)
           {
               Console.WriteLine("1 = List | 2 = Add | 3 = Edit | 4 = Delete | 5 = Register | 6 = Login | 0 = Exit");
               var input = Console.ReadLine();

               switch (input)
               {
                   case "1":
                       await CustomerService.ListCustomersAsync(service);
                       break;

                   case "2":
                       await CustomerService.AddCustomerAsync(service);
                       break;

                   case "4":
                       await CustomerService.DeleteCustomerAsync(service);
                       break;

                   case "5":
                      
                       await userService.RegisterAsync();
                       break;

                   case "6":
                       await userService.LoginAsync();
                       break;

                   case "0":
                       return;

                   default:
                       Console.WriteLine("Invalid choice");
                       break;
               }
           }
        }
    }
