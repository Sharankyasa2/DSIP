using Delhivery.Data;
using Delhivery.Data.Exceptions;
using Delhivery.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Delhivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private static readonly string[] ValidStatuses =
        {
            "Booked",
            "In Transit",
            "Out for Delivery",
            "Delivered",
            "RTO"
        };

        private readonly ShipmentRepository _repository;

        public ShipmentsController(ShipmentRepository repository)
        {
            // repository comes from dependency injection
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<List<Shipment>> GetAll()
        {
            // getting all shipments
            List<Shipment> shipments = _repository.GetAll();

            return Ok(shipments);
        }

        [HttpGet("{awb}")]
        public ActionResult<Shipment> GetByAWB(string awb)
        {
            try
            {
                // getting shipment using awb
                Shipment shipment = _repository.GetByAWB(awb);

                return Ok(shipment);
            }
            catch (ShipmentNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult<Shipment> BookShipment(Shipment shipment)
        {
            // checking required fields
            if (string.IsNullOrWhiteSpace(shipment.AWBNumber))
            {
                return BadRequest(new { message = "AWB Number is required." });
            }

            if (string.IsNullOrWhiteSpace(shipment.SenderName))
            {
                return BadRequest(new { message = "Sender Name is required." });
            }

            if (string.IsNullOrWhiteSpace(shipment.ReceiverName))
            {
                return BadRequest(new { message = "Receiver Name is required." });
            }

            if (string.IsNullOrWhiteSpace(shipment.Origin))
            {
                return BadRequest(new
                {
                    message = "Origin is required."
                });
            }

            if (string.IsNullOrWhiteSpace(shipment.Destination))
            {
                return BadRequest(new
                {
                    message = "Destination is required."
                });
            }

            if (shipment.WeightKg <= 0)
            {
                return BadRequest(new { message = "Weight must be greater than 0." });
            }

            // default status while booking
            if (string.IsNullOrWhiteSpace(shipment.Status))
            {
                shipment.Status = "Booked";
            }

            try
            {
                _repository.Book(shipment);

                return CreatedAtAction(
                    nameof(GetByAWB),
                    new { awb = shipment.AWBNumber },
                    shipment);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
                when (ex.Number == 2627 || ex.Number == 2601)
            {
                // duplicate awb number
                return BadRequest(new
                {
                    message = "AWB Number already exists."
                });
            }
        }


        [HttpPut("{awb}/status")]
        public ActionResult<Shipment> UpdateStatus(string awb, [FromBody] UpdateStatusRequest request)
        {
            // checking status is provided
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return BadRequest(new
                {
                    message = "Status is required."
                });
            }
            // checking valid status
            if (!ValidStatuses.Contains(request.Status))
            {
                return BadRequest(new
                {
                    message = $"Invalid status. Valid values are: {string.Join(", ", ValidStatuses)}"
                });
            }

            try
            {
                _repository.UpdateStatus(awb, request.Status);

                // getting updated shipment
                Shipment shipment = _repository.GetByAWB(awb);

                return Ok(shipment);
            }
            catch (ShipmentNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult CancelShipment(int id)
        {
            // deleting shipment using id
            _repository.Cancel(id);

            return NoContent();
        }

        /*[HttpGet("stats")]
        public ActionResult<List<ShipmentStats>> GetShipmentStats()
        {
            // getting count of shipments by status
            List<ShipmentStats> stats = _repository.GetShipmentStats();

            return Ok(stats);
        }*/

        [HttpGet("stats")]
        public ActionResult<ShipmentStatsDto> GetShipmentStats()
        {
            // getting dashboard stats
            ShipmentStatsDto stats = _repository.GetShipmentStats();

            return Ok(stats);
        }
    }
}
