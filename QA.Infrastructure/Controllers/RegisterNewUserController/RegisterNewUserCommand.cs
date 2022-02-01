namespace QA.Infrastructure.Controllers.RegisterNewUserController
{
    public class RegisterNewUserCommand
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }

    }
}
