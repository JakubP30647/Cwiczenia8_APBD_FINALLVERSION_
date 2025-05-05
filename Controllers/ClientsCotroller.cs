using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientServices _clientServices;

        public ClientsController(IClientServices clientServices)
        {
            _clientServices = clientServices;
        }

        // GET: api/Clients/{id}/trips
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetClientTrips(string id)
        {
            if (!int.TryParse(id, out int clientId))
            {
                return BadRequest("The provided ID is not a valid number.");
            }

            var clientTrip = await _clientServices.GetClient(clientId);

            if (clientTrip.Count == 0)
            {
                return NotFound($"Client with id {id} not found or has no associated trips.");
            }

            return Ok(clientTrip);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddClient([FromBody] NewClientDTO client)
        {
            if (string.IsNullOrWhiteSpace(client.FirstName) || string.IsNullOrWhiteSpace(client.LastName) ||
                string.IsNullOrWhiteSpace(client.Email) || string.IsNullOrWhiteSpace(client.Pesel))
            {
                return BadRequest("Missing required fields.");
            }

            var newId = await _clientServices.AddClient(client);
            return Created($"api/clients/{newId}", new { Id = newId });
        }
        
        
        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterClientToTrip(int id, int tripId)
        {
            var success = await _clientServices.RegisterClientToTrip(id, tripId);
            if (!success)
                return BadRequest("Could not register client. Possible reasons: non-existent client/trip or trip full.");

            return Ok("Client registered to trip successfully.");
        }
        
        
        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId)
        {
            var success = await _clientServices.UnregisterClientFromTrip(id, tripId);
            if (!success)
                return NotFound("Client was never registered to this trip.");

            return Ok("Client unregistered from trip successfully.");
        }
    }
}