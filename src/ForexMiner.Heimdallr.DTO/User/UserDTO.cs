namespace ForexMiner.Heimdallr.Contracts.User
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserDTO
    {
        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataMember]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [DataMember]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
    }
}
