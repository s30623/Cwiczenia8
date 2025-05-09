using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
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
            return NotFound("Klient nie istnieje");
        }

        var trip = await _clientService.GetClientTrips(id);
        return Ok(trip);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClientTrip(ClientDTO newClientDto , CancellationToken cancellationToken)
    {
        
        if (await _clientService.CreateClient(newClientDto, cancellationToken))
        {
            return Ok("Dodano klienta");
        }
        return BadRequest();
    }

    [HttpPut]
    [Route("{id}/trips/{tripId}")]
    public async Task<IActionResult> UpdateClientTrip(int id, int tripId,CancellationToken cancellationToken)
    {
        if (!await _clientService.DoesClientExist(id))
        {
            return NotFound("Klient nie istnieje");
        }

        if (!await new TripsService().DoesTripExist(tripId))
        {
            return NotFound("Wycieczka nie istnieje");
        }

        if (!await new TripsService().DoesTripExist(tripId))
        {
            return NotFound("Przekroczno max ilosc osob na wycieczke");
        }

        if (await _clientService.RegisterClient(id, tripId, cancellationToken))
        {
            return Ok("Zarejestrowano klienta");
        }
        return BadRequest("Nie udalo sie zarejestrowac");
    }
}