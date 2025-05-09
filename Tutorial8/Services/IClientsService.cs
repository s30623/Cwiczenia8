using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<TripDTO>> GetClientTrips(int idClient);
    Task<bool> DoesClientExist(int id);
    Task<bool> CreateClient(ClientDTO newClientDto, CancellationToken cancellationToken);
    Task<bool> RegisterClient(int IdClient, int IdTrip, CancellationToken cancellationToken);
    Task<bool> UnregisterClient(int IdClient, int IdTrip, CancellationToken cancellationToken);
    Task<bool> ClientRegisterdToTrip(int IdClient, int IdTrip, CancellationToken cancellationToken);
}