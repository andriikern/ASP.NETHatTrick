using System;
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

        [DataMember]
        public decimal Amount { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
