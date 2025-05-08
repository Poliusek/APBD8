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
        private readonly IClientsService _clientsService;

        public ClientsController(IClientsService clientsService)
        {
            _clientsService = clientsService;
        }
        
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetClientTrips(int id)
        {
            var trips = await _clientsService.GetTrips();
            return Ok(trips);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientsDTO client)
        {
            if (string.IsNullOrEmpty(client.Name) || string.IsNullOrEmpty(client.Surname))
            {
                return BadRequest("Name and Surname are required.");
            }

            bool result = await _clientsService.CrateteClient(client);

            return CreatedAtAction(nameof(GetClientTrips), new { id = client.Id }, client);
        }
        
        [HttpPut("api/clients/{id}/trips/{tripId}")]
        public IActionResult RegisterClientToTrip(int id, int tripId)
        {
            _clientsService.RegisterClientToTrip(id, tripId);

            return Ok("Zarejestrowano klienta na wycieczkę");
        }

        [HttpDelete("{id}/trips/{tripId}")]
        public IActionResult DeleteClientTrip(int id, int tripId)
        {
            _clientsService.DeleteClientTrip(id, tripId);

            return Ok("Usunięto klienta z wycieczki");
        }

    }
}
