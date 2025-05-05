using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = 
                "SELECT Trip.*, Country.IdCountry, Country.Name AS CountryName FROM Trip " +
                "RIGHT JOIN Country_Trip ON Trip.IdTrip = Country_Trip.IdTrip " + 
                "JOIN Country ON Country_Trip.IdCountry = Country.IdCountry ";
        
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    
                    int idTrip = (int) reader["IdTrip"];
                    string name = (string) reader["Name"];
                    string desctription = (string) reader["Description"];
                    DateTime dateFrom = (DateTime) reader["DateFrom"];
                    DateTime dateTo = (DateTime) reader["DateTo"];
                    int maxPeople = (int) reader["MaxPeople"];
                    int countryId = (int) reader["IdCountry"];
                    string countryName = (string) reader["CountryName"];
                    
                    var s = new TripDTO()
                    {
                        Id = idTrip,
                        Name = name,
                        Desctription = desctription,
                        DateFrom = dateFrom,
                        DateTo = dateTo,
                        maxPeople = maxPeople,
                        country = new CountryDTO(){
                            countryId = countryId,
                            countryName = countryName
                        }
                    
                    };
                    trips.Add(s);
                    
                }
            }
        }
        

        return trips;
    }
    

    public async Task<TripDTO> GetTripById(int id)
    {
        var trips = new List<TripDTO>();
        string command = 
            "SELECT Trip.*, Country.IdCountry, Country.Name AS CountryName FROM Trip " +
            "RIGHT JOIN Country_Trip ON Trip.IdTrip = Country_Trip.IdTrip " + 
            "JOIN Country ON Country_Trip.IdCountry = Country.IdCountry " + "WHERE Trip.IdTrip = @IdTrip";
        
        var result = new TripDTO();
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdTrip", id);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    
                    int idTrip = (int) reader["IdTrip"];
                    string name = (string) reader["Name"];
                    string desctription = (string) reader["Description"];
                    DateTime dateFrom = (DateTime) reader["DateFrom"];
                    DateTime dateTo = (DateTime) reader["DateTo"];
                    int maxPeople = (int) reader["MaxPeople"];
                    
                   result = new TripDTO() {
                        Id = idTrip,
                        Name = name,
                        Desctription = desctription,
                        DateFrom = dateFrom,
                        DateTo = dateTo,
                        maxPeople = maxPeople
                       
                    };
                    
                }
            }
        }
        

        return result;
    }

    
    
    

}