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
        public async Task<IActionResult> GetTrip(int id)
        {
            if( await DoesTripExist(id)){
                return NotFound();
            }
            var trip = _tripsService.GetTrips();
            return Ok(trip.Result.Where(x=>x.Id == id));
        }

        private async Task<bool> DoesTripExist(int id)
        {
            var trips = await _tripsService.GetTrips();
            foreach (var trip in trips)
                if (trip.Id == id)
                    return false;
            return true;
        }
    }
}
