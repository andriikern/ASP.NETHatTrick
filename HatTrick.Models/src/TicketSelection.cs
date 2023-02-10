using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class TicketSelection : IExtensibleDataObject
    {
        [Required, ForeignKey("TicketId"), XmlIgnore, JsonIgnore]
        public Ticket Ticket { get; set; } = new Ticket();

        [Required, ForeignKey("SelectionId"), DataMember]
        public Outcome Selection { get; set; } = new Outcome();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
