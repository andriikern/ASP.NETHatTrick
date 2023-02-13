using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class Sport : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public int Priority { get; set; }

        [XmlIgnore, JsonIgnore]
        public ICollection<Event> Events { get; set; } = new List<Event>();

        [XmlIgnore, JsonIgnore]
        public ICollection<MarketType> Markets { get; set; } = new List<MarketType>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
