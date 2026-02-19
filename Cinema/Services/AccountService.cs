using Azure.Core;
using CinemaECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CinemaECommerce.Services
{
    public enum MsgType
    {
       ConfirmationEmail,
       ResendEmail,
       ForgetPassword
    }
    public class AccountService: IAccountService
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(IEmailSender emailSender,UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task ReSendEmailAsync(MsgType msgType,string msg,ApplicationUser applicationUser)
        {
            if(msgType == MsgType.ConfirmationEmail) 
            await _emailSender.SendEmailAsync(applicationUser.Email!, "Confirm Your account",msg);
           
            else if(msgType == MsgType.ResendEmail)
            await _emailSender.SendEmailAsync(applicationUser.Email!, "Confirm Your account",msg);
           
            else 
            await _emailSender.SendEmailAsync(applicationUser.Email!, "No of confirmation your password",msg);
        }
    }
}
