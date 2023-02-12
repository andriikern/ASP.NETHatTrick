using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class EventStatus : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore, JsonIgnore]
        public ICollection<Event> Events { get; set; } = new List<Event>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
