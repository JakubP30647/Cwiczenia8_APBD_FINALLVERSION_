using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITripsService _tripsService;

        public TripsController(ITripsService tripsService)
        {
            _tripsService = tripsService;
        }
        
        
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await _tripsService.GetTrips();
            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(string id)
        {
            
            if (!int.TryParse(id, out int tripId))
            {
                return BadRequest("The provided ID is not a valid number.");
            }
            
            
            var trip = await _tripsService.GetTripById(int.Parse(id));
            
            if (trip.Name == null)
            {
                return NotFound($"Trip with id {id} not found.");
            }

            return Ok(trip);
        }
        
        
    }
}
