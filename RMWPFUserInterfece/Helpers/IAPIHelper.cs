using RMWPFUserInterfece.Models;
using System.Threading.Tasks;

namespace RMWPFUserInterfece.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string userName, string password);
    }
}