using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class MarketType : IExtensibleDataObject
    {
        [DataMember]
        public int Id { get; set; }

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public int Priority { get; set; }

        [XmlIgnore, JsonIgnore]
        public ICollection<OutcomeType> Outcomes { get; set; } = new List<OutcomeType>();

        [XmlIgnore, JsonIgnore]
        public ICollection<Market> Markets { get; set; } = new List<Market>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
