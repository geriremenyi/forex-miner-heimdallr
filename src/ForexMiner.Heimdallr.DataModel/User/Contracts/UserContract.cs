namespace ForexMiner.Heimdallr.DataModel.User.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserContract
    {
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }
}
