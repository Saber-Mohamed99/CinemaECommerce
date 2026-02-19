using Microsoft.AspNetCore.Mvc;

namespace CinemaECommerce.Services.IServices
{
    public interface IAccountService
    {
        Task ReSendEmailAsync(MsgType msgType, string msg, ApplicationUser applicationUser);
    }
}
