using System;
using System.Runtime.Serialization;

namespace HatTrick.API.Models
{
    [DataContract, Serializable]
    public sealed class TransactionRequestModel : IExtensibleDataObject
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
