using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class TransactionType : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
