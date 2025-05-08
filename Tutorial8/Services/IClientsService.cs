using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<TripDTO>> GetClientTrips(int idClient);
    Task<bool> DoesClientExist(int id);
}