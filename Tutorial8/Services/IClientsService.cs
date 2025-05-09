using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientsService
{
    Task<List<ClientsTrips>> GetTrips(int id); 
    Task<bool> CrateteClient(ClientsDTO client);
    Task<bool> RegisterClientToTrip(int clientId, int tripId);
    Task<bool> DeleteClientTrip(int id, int tripId);
}