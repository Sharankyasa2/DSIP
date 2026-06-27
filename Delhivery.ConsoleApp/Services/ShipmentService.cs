using Delhivery.ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delhivery.ConsoleApp.Services
{
    public class ShipmentService
    {
        private readonly List<Shipment> _shipments = new List<Shipment>
        {
            new Shipment
            {
                ShipmentId = 1,
                AWBNumber = "DEL2025001",
                SenderName = "Rahul Sharma",
                ReceiverName = "Amit Kumar",
                Origin = "Delhi",
                Destination = "Mumbai",
                WeightKg = 2.5,
                Status = "Booked",
                BookedAt = DateTime.Now.AddDays(-5)
            },
            new Shipment
            {
                ShipmentId = 2,
                AWBNumber = "DEL2025002",
                SenderName = "Priya Singh",
                ReceiverName = "Neha Patel",
                Origin = "Hyderabad",
                Destination = "Pune",
                WeightKg = 1.8,
                Status = "In Transit",
                BookedAt = DateTime.Now.AddDays(-4)
            },
            new Shipment
            {
                ShipmentId = 3,
                AWBNumber = "DEL2025003",
                SenderName = "Arjun Reddy",
                ReceiverName = "Kiran Rao",
                Origin = "Chennai",
                Destination = "Bengaluru",
                WeightKg = 3.2,
                Status = "Out for Delivery",
                BookedAt = DateTime.Now.AddDays(-3)
            },
            new Shipment
            {
                ShipmentId = 4,
                AWBNumber = "DEL2025004",
                SenderName = "Meena Joshi",
                ReceiverName = "Rohit Sharma",
                Origin = "Nagpur",
                Destination = "Surat",
                WeightKg = 5.6,
                Status = "Delivered",
                BookedAt = DateTime.Now.AddDays(-2)
            },
            new Shipment
            {
                ShipmentId = 5,
                AWBNumber = "DEL2025005",
                SenderName = "Anjali Verma",
                ReceiverName = "Deepak Singh",
                Origin = "Lucknow",
                Destination = "Patna",
                WeightKg = 4.1,
                Status = "RTO",
                BookedAt = DateTime.Now.AddDays(-1)
            }
        };

        private readonly List<string> _validStatuses = new List<string>()
        {
            "Booked",
            "In Transit",
            "Out for Delivery",
            "Delivered",
            "RTO"
        };

        public bool BookShipment(Shipment shipment, out string message)
        {
            if (string.IsNullOrWhiteSpace(shipment.AWBNumber))
            {
                message = "AWB Number cannot be empty.";
                return false;
            }

            if (_shipments.Any(s => s.AWBNumber.Equals(shipment.AWBNumber, StringComparison.OrdinalIgnoreCase)))
            {
                message = "AWB Number already exists.";
                return false;
            }

            if (shipment.WeightKg <= 0)
            {
                message = "Weight must be greater than zero.";
                return false;
            }

            if (!_validStatuses.Contains(shipment.Status))
            {
                message = "Invalid shipment status.";
                return false;
            }

            shipment.ShipmentId = _shipments.Max(s => s.ShipmentId) + 1;
            shipment.BookedAt = DateTime.Now;

            _shipments.Add(shipment);

            message = "Shipment booked successfully.";

            return true;
        }

        public List<Shipment> ListShipments()
        {
            return new List<Shipment>(_shipments);
        }

        public bool UpdateStatus(string awb, string newStatus, out string message)
        {
            var shipment = _shipments.FirstOrDefault(s =>
                s.AWBNumber.Equals(awb, StringComparison.OrdinalIgnoreCase));

            if (shipment == null)
            {
                message = "Shipment not found.";
                return false;
            }

            if (!_validStatuses.Contains(newStatus))
            {
                message = "Invalid status.";
                return false;
            }

            shipment.Status = newStatus;

            message = "Shipment status updated successfully.";
            return true;
        }

        public bool CancelShipment(string awb, out string message)
        {
            var shipment = _shipments.FirstOrDefault(s =>
                s.AWBNumber.Equals(awb, StringComparison.OrdinalIgnoreCase));

            if (shipment == null)
            {
                message = "Shipment not found.";
                return false;
            }

            _shipments.Remove(shipment);

            message = "Shipment cancelled successfully.";
            return true;
        }
    }
}
