using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class Ticket : IExtensibleDataObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), DataMember]
        public int Id { get; set; }

        [Required, ForeignKey("UserId"), XmlIgnore, JsonIgnore]
        public User User { get; set; } = new User();

        [DataMember]
        public ICollection<Outcome> Selections { get; set; } = new List<Outcome>();

        [DataMember]
        public decimal PayInAmount { get; set; }

        [DataMember]
        public DateTime PayInTime { get; set; }

        [DataMember]
        public decimal TotalOdds { get; set; }

        [Required, ForeignKey("StatusId"), DataMember]
        public TicketStatus Status { get; set; } = new TicketStatus();

        [DataMember]
        public bool IsResolved { get; set; }

        [DataMember]
        public DateTime? ResolvedTime { get; set; }

        [DataMember]
        public decimal? CostAmount { get; set; }

        [DataMember]
        public decimal? WinAmount { get; set; }

        [DataMember]
        public DateTime? PayOutTime { get; set; }

        [XmlIgnore, JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
