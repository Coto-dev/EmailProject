using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using EmailProject.WebApp.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

string fromAddress = "hwapp@internet.ru";
string fromPassword = "sQ69DtzshASJSBM4z7rA";
string subject = "Подтверждение почты";


var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "code-queue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var rawMessage = Encoding.UTF8.GetString(ea.Body.ToArray());
    var message = JsonSerializer.Deserialize<RabbitMqMessageDto>(rawMessage);
    if (message == null)
        throw new InvalidOperationException();
    
    Console.WriteLine($" {DateTime.UtcNow} {message.Email} {message.Code}");
    
    try
    {
        SmtpClient smtpClient = new SmtpClient("smtp.mail.ru", 587)
        {
            Credentials = new NetworkCredential(fromAddress, fromPassword),
            EnableSsl = true
        };

        MailMessage mailMessage = new MailMessage(fromAddress, message.Email, subject, message.Code);

        smtpClient.Send(mailMessage);

        Console.WriteLine("Email sent successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error sending email: " + ex.Message);
    }
    
};

channel.BasicConsume(queue: "code-queue",
    autoAck: true,
    consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();