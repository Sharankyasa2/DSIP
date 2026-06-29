using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delhivery.Data.Models
{
    public class ShipmentStatsDto
    {
        public int Booked { get; set; }

        public int InTransit { get; set; }

        public int OutForDelivery { get; set; }

        public int Delivered { get; set; }

        public int RTO { get; set; }

        // calculated automatically
        public int Total => Booked + InTransit + OutForDelivery + Delivered + RTO;
    }
    }
