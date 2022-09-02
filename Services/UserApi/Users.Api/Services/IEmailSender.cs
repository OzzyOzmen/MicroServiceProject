using System.Threading.Tasks;

namespace Users.Api.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string emailAddress, string subject, string content);
    }
}