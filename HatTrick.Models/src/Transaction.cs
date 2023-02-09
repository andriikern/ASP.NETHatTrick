using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class Transaction : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [Required, ForeignKey("UserId"), XmlIgnore, JsonIgnore]
        public User User { get; set; } = new User();

        [Required, ForeignKey("TypeId"), DataMember]
        public TransactionType Type { get; set; } = new TransactionType();

        [ForeignKey("TicketId"), DataMember]
        public Ticket? Ticket { get; set; }

        [DataMember]
        public DateTime Time { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
