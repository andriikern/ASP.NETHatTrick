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
    public sealed class Market : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [Required, ForeignKey("FixtureId"), XmlIgnore, JsonIgnore]
        public Fixture Fixture { get; set; } = new Fixture();

        [Required, ForeignKey("TypeId"), DataMember]
        public MarketType Type { get; set; } = new MarketType();

        [MaxLength(64), DataMember]
        public string? Value { get; set; }

        [DataMember]
        public DateTime AvailableFrom { get; set; }

        [DataMember]
        public DateTime AvailableUntil { get; set; }

        [DataMember]
        public ICollection<Outcome> Outcomes { get; set; } = new List<Outcome>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
