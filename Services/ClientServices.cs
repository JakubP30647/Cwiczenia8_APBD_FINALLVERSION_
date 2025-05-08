using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientServices : IClientServices
{
    private readonly string _connectionString =
        "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;";


    public async Task<List<ClientTripDTO>> GetClient(int id)
    {
        var result = new List<ClientTripDTO>();
        string command =
            "SELECT Client_Trip.*, Trip.*, Country.IdCountry as IdCountry, Country.Name as CountryName FROM Client \nJOIN Client_Trip ON Client.IdClient = Client_Trip.IdClient\nJOIN Trip ON Client_Trip.IdTrip = Trip.IdTrip\nJOIN Country_Trip ON Trip.IdTrip = Country_Trip.IdTrip\nJOIN Country ON Country_Trip.IdCountry = Country.IdCountry\nWHERE Client.IdClient = @IdClient";


        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdClient", id);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int? paymentDate = reader["PaymentDate"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(reader["PaymentDate"]);
                    int idTrip = (int)reader["IdTrip"];
                    string name = (string)reader["Name"];
                    string desctription = (string)reader["Description"];
                    DateTime dateFrom = (DateTime)reader["DateFrom"];
                    DateTime dateTo = (DateTime)reader["DateTo"];
                    int maxPeople = (int)reader["MaxPeople"];
                    int countryId = (int)reader["IdCountry"];
                    string countryName = (string)reader["CountryName"];

                    var c = new ClientTripDTO()
                    {
                        trip = new TripDTO()
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
                        },
                        paymentDate = paymentDate != null
                            ? DateTime.ParseExact(paymentDate.ToString(), "yyyyMMdd", null)
                            : null
                    };

                    result.Add(c);
                }
            }
        }

        return result;
    }

    public async Task<int?> AddClient(NewClientDTO client)
    {
        string insertQuery =
            "INSERT INTO CLIENT (FirstName,LastName,Email,Telephone,Pesel) VALUES (@FirstName,@LastName,@Email,@Telephone,@Pesel); SELECT SCOPE_IDENTITY()";

        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(insertQuery, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
            cmd.Parameters.AddWithValue("@LastName", client.LastName);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@Telephone", client.Telephone);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            if (result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }

            return 0;
        }
    }

    public async Task<bool> RegisterClientToTrip(int clientId, int tripId)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();


            var checkClient = new SqlCommand("SELECT COUNT(*) FROM Client WHERE IdClient = @id", conn);
            checkClient.Parameters.AddWithValue("@id", clientId);
            if ((int)await checkClient.ExecuteScalarAsync() == 0) return false;


            var checkTrip = new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @id", conn);
            checkTrip.Parameters.AddWithValue("@id", tripId);
            var maxPeopleObj = await checkTrip.ExecuteScalarAsync();
            if (maxPeopleObj == null) return false;
            int maxPeople = (int)maxPeopleObj;


            var checkCount = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @id", conn);
            checkCount.Parameters.AddWithValue("@id", tripId);
            int current = (int)await checkCount.ExecuteScalarAsync();
            if (current >= maxPeople) return false;


            var checkExists =
                new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdClient = @idClient AND IdTrip = @idTrip",
                    conn);
            checkExists.Parameters.AddWithValue("@idClient", clientId);
            checkExists.Parameters.AddWithValue("@idTrip", tripId);
            if ((int)await checkExists.ExecuteScalarAsync() > 0) return false;

            var register = new SqlCommand(
                "INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) VALUES (@clientId, @tripId, @now)", conn);
            register.Parameters.AddWithValue("@clientId", clientId);
            register.Parameters.AddWithValue("@tripId", tripId);
            register.Parameters.AddWithValue("@now", DateTime.Now.ToString("yyyyMMdd"));

            await register.ExecuteNonQueryAsync();
            return true;
        }
    }

    public async Task<bool> UnregisterClientFromTrip(int clientId, int tripId)
    {
        using (var conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            var check = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdClient = @cid AND IdTrip = @tid",
                conn);
            check.Parameters.AddWithValue("@cid", clientId);
            check.Parameters.AddWithValue("@tid", tripId);

            if ((int)await check.ExecuteScalarAsync() == 0) return false;

            var delete = new SqlCommand("DELETE FROM Client_Trip WHERE IdClient = @cid AND IdTrip = @tid", conn);
            delete.Parameters.AddWithValue("@cid", clientId);
            delete.Parameters.AddWithValue("@tid", tripId);
            await delete.ExecuteNonQueryAsync();

            return true;
        }
    }
}