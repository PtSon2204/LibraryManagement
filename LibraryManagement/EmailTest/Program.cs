using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Test", "he181997phamtheson@gmail.com"));
            message.To.Add(MailboxAddress.Parse("he181997phamtheson@gmail.com"));
            message.Subject = "Test Email";
            message.Body = new TextPart("html") { Text = "This is a test." };

            using var client = new SmtpClient();
            Console.WriteLine("Connecting...");
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            
            Console.WriteLine("Authenticating...");
            await client.AuthenticateAsync("he181997phamtheson@gmail.com", "klgj mcii ivee zgpk");
            
            Console.WriteLine("Sending...");
            await client.SendAsync(message);
            
            Console.WriteLine("Disconnecting...");
            await client.DisconnectAsync(true);
            
            Console.WriteLine("SUCCESS!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
