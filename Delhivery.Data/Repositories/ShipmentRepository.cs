using Delhivery.Data.Exceptions;
using Delhivery.Data.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Delhivery.Data
{
    public class ShipmentRepository
    {
        private readonly string _connectionString;

        public ShipmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public void Book(Shipment shipment)
        {
            // inserting new shipment
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Shipments
                        (AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status)
                        VALUES
                        (@AWBNumber, @SenderName, @ReceiverName, @Origin, @Destination, @WeightKg, @Status)";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@AWBNumber", shipment.AWBNumber);
                command.Parameters.AddWithValue("@SenderName", shipment.SenderName);
                command.Parameters.AddWithValue("@ReceiverName", shipment.ReceiverName);
                command.Parameters.AddWithValue("@Origin", shipment.Origin);
                command.Parameters.AddWithValue("@Destination", shipment.Destination);
                command.Parameters.AddWithValue("@WeightKg", shipment.WeightKg);
                command.Parameters.AddWithValue("@Status", shipment.Status);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Shipment> GetAll()
        {
            List<Shipment> shipments = new List<Shipment>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("usp_GetAllShipments", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                connection.Open();

                // reading all shipment records
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        shipments.Add(MapShipment(reader));
                    }
                }
            }

            return shipments;
        }



        public Shipment GetByAWB(string awb)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("usp_GetShipmentByAWB", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@AWBNumber", awb);

                connection.Open();

                // reading all shipment records
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        return MapShipment(reader);
                    }
                }

                throw new ShipmentNotFoundException(awb);
            }
        }

        public void UpdateStatus(string awb, string status)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("usp_UpdateShipmentStatus", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@AWBNumber", awb);
                command.Parameters.AddWithValue("@NewStatus", status);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Cancel(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Shipments WHERE ShipmentId = @ShipmentId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ShipmentId", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public ShipmentStatsDto GetShipmentStats()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Status, COUNT(*) AS Count
                         FROM Shipments
                         GROUP BY Status";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                List<ShipmentStats> stats = new List<ShipmentStats>();

                // reading status count from database
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats.Add(new ShipmentStats
                        {
                            Status = reader["Status"].ToString(),
                            Count = Convert.ToInt32(reader["Count"])
                        });
                    }
                }

                return new ShipmentStatsDto
                {
                    Booked = stats.FirstOrDefault(s => s.Status == "Booked")?.Count ?? 0,
                    InTransit = stats.FirstOrDefault(s => s.Status == "In Transit")?.Count ?? 0,
                    OutForDelivery = stats.FirstOrDefault(s => s.Status == "Out for Delivery")?.Count ?? 0,
                    Delivered = stats.FirstOrDefault(s => s.Status == "Delivered")?.Count ?? 0,
                    RTO = stats.FirstOrDefault(s => s.Status == "RTO")?.Count ?? 0
                };
            }
        }

        /*public List<ShipmentStats> GetShipmentStats()
        {
            List<ShipmentStats> stats = new List<ShipmentStats>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT Status, COUNT(*) AS Count
                         FROM Shipments
                         GROUP BY Status";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                // getting count for each status
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stats.Add(new ShipmentStats
                        {
                            Status = reader["Status"].ToString(),
                            Count = Convert.ToInt32(reader["Count"])
                        });
                    }
                }
            }

            return stats;
        }*/

        private Shipment MapShipment(SqlDataReader reader)
        {
            Shipment shipment = new Shipment();

            shipment.ShipmentId = (int)reader["ShipmentId"];
            shipment.AWBNumber = reader["AWBNumber"].ToString();
            shipment.SenderName = reader["SenderName"].ToString();
            shipment.ReceiverName = reader["ReceiverName"].ToString();
            shipment.Origin = reader["Origin"].ToString();
            shipment.Destination = reader["Destination"].ToString();
            shipment.WeightKg = (decimal)reader["WeightKg"];
            shipment.Status = reader["Status"].ToString();
            shipment.BookedAt = (DateTime)reader["BookedAt"];

            if (reader["DeliveredAt"] != DBNull.Value)
            {
                shipment.DeliveredAt = (DateTime)reader["DeliveredAt"];
            }

            return shipment;
        }

    }
}
