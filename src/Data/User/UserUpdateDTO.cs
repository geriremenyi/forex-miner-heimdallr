namespace ForexMiner.Heimdallr.Data.User
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserUpdateDTO
    {
        [DataMember]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataMember]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [DataMember]
        [MaxLength(50)]
        public string LastName { get; set; }

        [DataMember]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
