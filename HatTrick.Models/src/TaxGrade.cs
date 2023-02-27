using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace HatTrick.Models
{
    [DataContract, Serializable]
    public sealed class TaxGrade : IExtensibleDataObject
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), DataMember]
        public int Id { get; set; }

        [DataMember]
        public decimal? LowerBound { get; set; }

        [DataMember]
        public decimal? UpperBound { get; set; }

        [DataMember]
        public decimal Rate { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
