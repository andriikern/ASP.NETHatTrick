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
    public sealed class OutcomeType : IExtensibleDataObject
    {
        [DataMember]
        public int Id { get; set; }

        [Required, ForeignKey("MarketId"), XmlIgnore, JsonIgnore]
        public MarketType Market { get; set; } = new MarketType();

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public int Priority { get; set; }

        [XmlIgnore, JsonIgnore]
        public ICollection<Outcome> Outcomes { get; set; } = new List<Outcome>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
