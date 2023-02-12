using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class FixtureType : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public bool IsPromoted { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [XmlIgnore, JsonIgnore]
        public ICollection<Fixture> Fixtures { get; set; } = new List<Fixture>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
