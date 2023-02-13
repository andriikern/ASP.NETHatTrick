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
    public sealed class Outcome : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [Required, ForeignKey("MarketId"), XmlIgnore, JsonIgnore]
        public Market Market { get; set; } = new Market();

        [Required, ForeignKey("TypeId"), DataMember]
        public OutcomeType Type { get; set; } = new OutcomeType();

        [DataMember]
        public string? Value { get; set; }

        [DataMember]
        public decimal? Odds { get; set; }

        [DataMember]
        public DateTime AvailableFrom { get; set; }

        [DataMember]
        public DateTime AvailableUntil { get; set; }

        [DataMember]
        public bool IsResolved { get; set; }

        [DataMember]
        public bool? IsWinning { get; set; }

        [XmlIgnore, JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
