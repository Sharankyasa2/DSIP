using Delhivery.ConsoleApp.Models;
using Delhivery.ConsoleApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delhivery.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString =
                @"Server=YOUR_SERVER;Database=DSIP;Trusted_Connection=True;";

            ShipmentService service = new ShipmentService();

            while (true)
            {
                Console.Clear();

                Console.WriteLine("=================================");
                Console.WriteLine("   Delhivery Shipment System");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Book Shipment");
                Console.WriteLine("2. List Shipments");
                Console.WriteLine("3. Update Status");
                Console.WriteLine("4. Cancel Shipment");
                Console.WriteLine("0. Exit");
                Console.Write("\nEnter Choice: ");

                string choice = Console.ReadLine().Trim();
                
                switch (choice)
                {
                    case "1":

                        Shipment shipment = new Shipment();

                        Console.Write("AWB Number: ");
                        shipment.AWBNumber = Console.ReadLine() ?? "";

                        Console.Write("Sender Name: ");
                        shipment.SenderName = Console.ReadLine() ?? "";

                        Console.Write("Receiver Name: ");
                        shipment.ReceiverName = Console.ReadLine() ?? "";

                        Console.Write("Origin: ");
                        shipment.Origin = Console.ReadLine() ?? "";

                        Console.Write("Destination: ");
                        shipment.Destination = Console.ReadLine() ?? "";

                        Console.Write("Weight (Kg): ");
                        double weight;

                        while (!double.TryParse(Console.ReadLine(), out weight) || weight <= 0)
                        {
                            Console.Write("Enter a valid weight: ");
                        }

                        shipment.WeightKg = weight;

                        Console.Write("Status: ");
                        shipment.Status = Console.ReadLine() ?? "";

                        if (service.BookShipment(shipment, out string bookMessage))
                            Console.WriteLine("\n" + bookMessage);
                        else
                            Console.WriteLine("\nError: " + bookMessage);

                        break;

                    case "2":

                        var shipments = service.ListShipments();

                        if (shipments.Count == 0)
                        {
                            Console.WriteLine("\nNo shipments found.");
                        }
                        else
                        {
                            foreach (var s in shipments)
                            {
                                Console.WriteLine("-------------------------------------");
                                Console.WriteLine($"ID          : {s.ShipmentId}");
                                Console.WriteLine($"AWB         : {s.AWBNumber}");
                                Console.WriteLine($"Sender      : {s.SenderName}");
                                Console.WriteLine($"Receiver    : {s.ReceiverName}");
                                Console.WriteLine($"Origin      : {s.Origin}");
                                Console.WriteLine($"Destination : {s.Destination}");
                                Console.WriteLine($"Weight      : {s.WeightKg}");
                                Console.WriteLine($"Status      : {s.Status}");
                                Console.WriteLine($"Booked At   : {s.BookedAt}");
                            }
                        }

                        break;

                    case "3":

                        Console.Write("Enter AWB Number: ");
                        string awb = Console.ReadLine() ?? "";

                        Console.Write("Enter New Status: ");
                        string status = Console.ReadLine() ?? "";

                        if (service.UpdateStatus(awb, status, out string updateMessage))
                            Console.WriteLine("\n" + updateMessage);
                        else
                            Console.WriteLine("\nError: " + updateMessage);

                        break;

                    case "4":

                        Console.Write("Enter AWB Number: ");
                        string cancelAwb = Console.ReadLine() ?? "";

                        if (service.CancelShipment(cancelAwb, out string cancelMessage))
                            Console.WriteLine("\n" + cancelMessage);
                        else
                            Console.WriteLine("\nError: " + cancelMessage);

                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("\nInvalid Choice.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
