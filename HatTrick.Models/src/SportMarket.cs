using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class SportMarket : IExtensibleDataObject
    {
        [Required, ForeignKey("SportId"), XmlIgnore, JsonIgnore]
        public Sport Sport { get; set; } = new Sport();

        [Required, ForeignKey("MarketId"), DataMember]
        public MarketType Market { get; set; } = new MarketType();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
