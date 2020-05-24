namespace ForexMiner.Heimdallr.UserManager.Model.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AuthenticationContract
    {
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
