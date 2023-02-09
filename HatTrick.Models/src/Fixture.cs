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
    public sealed class Fixture : IExtensibleDataObject
    {
        [Required, ForeignKey("EventId"), XmlIgnore, JsonIgnore]
        public Event Event { get; set; } = new Event();

        [Required, ForeignKey("TypeId"), DataMember]
        public FixtureType Type { get; set; } = new FixtureType();

        [DataMember]
        public DateTime AvailableFrom { get; set; }

        [DataMember]
        public DateTime AvailableUntil { get; set; }

        [DataMember]
        public ICollection<Market> Markets { get; set; } = new List<Market>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
