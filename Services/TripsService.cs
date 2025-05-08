using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;";

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
                    int idTrip = (int)reader["IdTrip"];
                    string name = (string)reader["Name"];
                    string desctription = (string)reader["Description"];
                    DateTime dateFrom = (DateTime)reader["DateFrom"];
                    DateTime dateTo = (DateTime)reader["DateTo"];
                    int maxPeople = (int)reader["MaxPeople"];
                    int countryId = (int)reader["IdCountry"];
                    string countryName = (string)reader["CountryName"];

                    if (check(trips, idTrip))
                    {
                        trips.Find(trip => trip.Id == idTrip).Countries.Add(new CountryDTO
                        {
                            countryId = countryId,
                            countryName = countryName,
                        });
                    }
                    else
                    {
                        var s = new TripDTO()
                        {
                            Id = idTrip,
                            Name = name,
                            Desctription = desctription,
                            DateFrom = dateFrom,
                            DateTo = dateTo,
                            maxPeople = maxPeople,
                            Countries = new List<CountryDTO>
                            {
                                new CountryDTO()
                                {
                                    countryId = countryId,
                                    countryName = countryName
                                }
                            }
                        };
                        trips.Add(s);
                    }
                }
            }
        }


        return trips;
    }

    
    private bool check(List<TripDTO> trips, int idTrip)
    {
        foreach (var trip in trips)
        {
            if (trip.Id == idTrip)
            {
                return true;
            }
        }

        return false;
    }
}