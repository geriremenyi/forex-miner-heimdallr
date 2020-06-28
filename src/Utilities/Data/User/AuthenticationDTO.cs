namespace ForexMiner.Heimdallr.Utilities.Data.User
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class AuthenticationDTO
    {
        [DataMember]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataMember]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
