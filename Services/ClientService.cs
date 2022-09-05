
using System.Text.RegularExpressions;

namespace stock_alert_quote.Services
{
    public class ClientService
    {
        public static string GetClientEmail()
        {
            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov|eb)(\.br|\.us)?$";

            Console.WriteLine("Type the email of the asset owner");
            string? email = Console.ReadLine();
            if (!string.IsNullOrEmpty(email) && Regex.IsMatch(email, regex, RegexOptions.IgnoreCase))
            {
                return email;

            }

            return "";
        }
    }
}
