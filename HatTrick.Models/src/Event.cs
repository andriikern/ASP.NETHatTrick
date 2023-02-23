using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class Event : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [MaxLength(64), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [Required, ForeignKey("SportId"), DataMember]
        public Sport Sport { get; set; } = new Sport();

        [DataMember]
        public DateTime StartsAt { get; set; }

        [DataMember]
        public DateTime EndsAt { get; set; }

        [Required, ForeignKey("StatusId"), DataMember]
        public EventStatus Status { get; set; } = new EventStatus();

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public ICollection<Fixture> Fixtures { get; set; } = new List<Fixture>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
