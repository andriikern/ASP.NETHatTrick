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
    public sealed class TicketStatus : IExtensibleDataObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), DataMember]
        public int Id { get; set; }

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore, JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
