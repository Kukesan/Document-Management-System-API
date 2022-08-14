/*using CompanyEmployees.Entities.Models;
*/
namespace CompanyEmployees.Services.EmailService
{
    public interface IEmailService
    {
        void SendEmail(EmailDto request);
    }
}
