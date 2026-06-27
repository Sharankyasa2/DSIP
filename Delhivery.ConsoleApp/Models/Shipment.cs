using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delhivery.ConsoleApp.Models
{
    public class Shipment
    {
        public int ShipmentId { get; set; }

        public string AWBNumber { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; } 

        public string Origin { get; set; }

        public string Destination { get; set; }

        public double WeightKg { get; set; }

        //setting booked status at start as Default
        public string Status { get; set; } = "Booked";

        //setting time while booking by default
        public DateTime BookedAt { get; set; } = DateTime.Now;
    }
}
