using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class TransactionType : IExtensibleDataObject
    {
        [Key, DataMember]
        public int Id { get; set; }

        [MaxLength(32), Required, DataMember]
        public string Name { get; set; } = string.Empty;

        [XmlIgnore, JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
