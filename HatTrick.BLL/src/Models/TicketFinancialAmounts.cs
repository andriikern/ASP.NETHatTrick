using System;
using System.Runtime.Serialization;

namespace HatTrick.BLL.Models
{
    [DataContract, Serializable]
    public sealed class TicketFinancialAmounts : IExtensibleDataObject
    {
        public const decimal DefaultManipulativeCostRate = Business.ManipulativeCostRate;

        [DataMember]
        public decimal ManipulativeCostRate { get; set; } = DefaultManipulativeCostRate;

        [DataMember]
        public decimal PayInAmount { get; set; }

        [DataMember]
        public decimal ActiveAmount { get; set; }

        [DataMember]
        public decimal TotalOdds { get; set; }

        [DataMember]
        public decimal GrossPotentialWinAmount { get; set; }

        [DataMember]
        public decimal Tax { get; set; }

        [DataMember]
        public decimal NetPotentialWinAmount { get; set; }

        ExtensionDataObject? IExtensibleDataObject.ExtensionData { get; set; }
    }
}
