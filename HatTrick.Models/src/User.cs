using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class User : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [RegularExpression(@"[\-\.\w]+"), MaxLength(32), Required, DataMember]
        public string Username { get; set; } = string.Empty;

        [MaxLength(64), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [MaxLength(64), Required, DataMember]
        public string Surname { get; set; } = string.Empty;

        [RegularExpression(@"F|M"), MaxLength(8), DataMember]
        public string? Sex { get; set; }

        [EmailAddress, MaxLength(64), Required, DataMember]
        public string Email { get; set; } = string.Empty;

        [MaxLength(64), Required, DataMember]
        public string Address { get; set; } = string.Empty;

        [MaxLength(32), Required, DataMember]
        public string City { get; set; } = string.Empty;

        [MaxLength(32), Required, DataMember]
        public string Country { get; set; } = string.Empty;

        [Phone, MaxLength(64), Required, DataMember]
        public string Phone { get; set; } = string.Empty;

        [DataMember]
        public DateTime Birthdate { get; set; }

        [DataMember]
        public DateTime RegisteredOn { get; set; }

        [DataMember]
        public DateTime? DeactivatedOn { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        [DataMember]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
