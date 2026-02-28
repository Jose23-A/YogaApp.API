using YogaApp.API.Entities;

namespace YogaApp.API.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task<Student> RegisterAsync(string email, string password);
    }
}
