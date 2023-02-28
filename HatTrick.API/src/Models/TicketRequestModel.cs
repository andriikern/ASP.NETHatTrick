using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HatTrick.API.Models
{
    [DataContract, Serializable]
    public sealed class TicketRequestModel : IExtensibleDataObject
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public int[] SelectionIds { get; set; } = Array.Empty<int>();

        [Range(0.25, 250_000.00), DataMember]
        public decimal Amount { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
