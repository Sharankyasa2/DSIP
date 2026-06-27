using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delhivery.Data.Exceptions
{
    public class ShipmentNotFoundException : Exception
    {
        public ShipmentNotFoundException()
            : base("Shipment not found.")
        {
        }

        public ShipmentNotFoundException(string awb)
            : base($"Shipment with AWB '{awb}' was not found.")
        {
        }
    }
}
