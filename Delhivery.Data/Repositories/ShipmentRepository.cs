using Delhivery.Data.Exceptions;
using Delhivery.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delhivery.Data
{
    public class ShipmentRepository
    {
        private readonly string _connectionString;

        public ShipmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Shipment> GetAll()
        {
            List<Shipment> shipments = new List<Shipment>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand("usp_GetAllShipments", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    shipments.Add(MapShipment(reader));
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

                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapShipment(reader);
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
