using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientServices
{
    
    Task<List<ClientTripDTO>> GetClient(int id);
    
    Task<int?> AddClient(NewClientDTO client);
    
    Task<bool> RegisterClientToTrip(int clientId, int tripId);
    
    Task<bool> UnregisterClientFromTrip(int clientId, int tripId);
    
}