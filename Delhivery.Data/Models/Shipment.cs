using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delhivery.Data.Models
{
    public class Shipment
    {
        public int ShipmentId { get; set; }

        public string AWBNumber { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }

        public string Origin { get; set; }

        public string Destination { get; set; }

        public decimal WeightKg { get; set; }

        public string Status { get; set; }

        public DateTime BookedAt { get; set; }

        public DateTime? DeliveredAt { get; set; }
    }
}
