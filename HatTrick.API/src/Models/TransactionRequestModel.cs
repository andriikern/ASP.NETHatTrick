using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HatTrick.API.Models
{
    [DataContract, Serializable]
    public sealed class TransactionRequestModel : IExtensibleDataObject
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public TransactionRequestType Type { get; set; }

        [Range(1.00, 250_000.00), DataMember]
        public decimal Amount { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
