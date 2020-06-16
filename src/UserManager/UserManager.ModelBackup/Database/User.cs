namespace ForexMiner.Heimdallr.UserManager.Model.Database
{
    using System;

    public class User
    {
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }
}
