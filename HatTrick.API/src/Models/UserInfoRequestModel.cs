using System;
using System.Runtime.Serialization;

namespace HatTrick.API.Models
{
    [DataContract, Serializable]
    public sealed class UserInfoRequestModel : IExtensibleDataObject
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public bool IncludeTickets { get; set; }

        [DataMember]
        public bool IncludeTicketSelections { get; set; }

        [DataMember]
        public bool IncludeTransactions { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
