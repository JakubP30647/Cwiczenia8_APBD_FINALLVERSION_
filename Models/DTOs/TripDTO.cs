namespace Tutorial8.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Desctription { get; set; }
    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int maxPeople { get; set; }
    
    public List<CountryDTO> Countries{ get; set; }
}

public class CountryDTO
{
    public int countryId { get; set; }
    public string countryName { get; set; }
    
}

public class ClientTripDTO
{
    public TripDTO trip { get; set; }
    public DateTime? paymentDate { get; set; }
    
}