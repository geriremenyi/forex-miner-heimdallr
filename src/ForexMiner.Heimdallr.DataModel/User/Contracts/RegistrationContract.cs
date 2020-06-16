namespace ForexMiner.Heimdallr.DataModel.User.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class RegistrationContract
    {
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}
