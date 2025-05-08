using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IClientsService _clientService;

    public ClientsController(IClientsService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    [Route("{id}/trips")]
    public async Task<IActionResult> GetClientTrips(int id)
    {
        if(!await _clientService.DoesClientExist(id)){ 
            return NotFound();
        }

        var trip = await _clientService.GetClientTrips(id);
        return Ok(trip);
    }
}