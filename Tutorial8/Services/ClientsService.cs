using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";
    
    public async Task<List<ClientsTrips>> GetTrips()
    {
        var clients = new List<ClientsTrips>();

        string command = "SELECT IdClient,FirstName, LastName FROM Client";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdClient");
                    clients.Add(new ClientsTrips()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Surname = reader.GetString(2),
                    });
                }
            }
        }
        
        foreach (var client in clients)
        {
            command = "SELECT * FROM Trip join Client_Trip on Trip.IdTrip = Client_Trip.IdTrip WHERE Client_Trip.IdClient = @IdClient";
            using (SqlConnection conn = new(_connectionString))
            using (SqlCommand cmd = new(command, conn))
            {
                cmd.Parameters.AddWithValue("@IdClient", client.Id);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var trips = new List<TripDTO>();
                    while (await reader.ReadAsync())
                    {
                        trips.Add(new TripDTO()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            maxPeople = reader.GetInt32(5)
                        });
                    }
                    client.Trips = trips;
                }
            }
            
            foreach (var trip in client.Trips)
            {
                command = "SELECT Name FROM Country join Country_Trip on Country.IdCountry = Country_Trip.IdCountry WHERE Country_Trip.IdTrip = @IdTrip";
                using (SqlConnection conn = new(_connectionString))
                using (SqlCommand cmd = new(command, conn))
                {
                    cmd.Parameters.AddWithValue("@IdTrip", trip.Id);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var countries = new List<CountryDTO>();
                        while (await reader.ReadAsync())
                        {
                            countries.Add(new CountryDTO()
                            {
                                Name = reader.GetString(0),
                            });
                        }
                        trip.Countries = countries;
                    }
                }
            }
        }
        

        return clients;
    }

    public Task<bool> CrateteClient(ClientsDTO client)
    {
        string command = "INSERT INTO Client (FirstName, LastName, Email, PhoneNumber, Pesel) VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Pesel)";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@FirstName", client.Name);
            cmd.Parameters.AddWithValue("@LastName", client.Surname);
            cmd.Parameters.AddWithValue("@Email", client.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
            cmd.Parameters.AddWithValue("@Pesel", client.Pesel);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        
        return Task.FromResult(true);
    }

    public async Task<bool> RegisterClientToTrip(int clientId, int tripId)
    {
        using (var con = new SqlConnection(_connectionString))
        {
            con.Open();

            var checkClient = new SqlCommand("SELECT COUNT(*) FROM Client WHERE IdClient = @id", con);
            checkClient.Parameters.AddWithValue("@id", clientId);
            if ((int)checkClient.ExecuteScalar() == 0)
                return false;

            var checkTrip = new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @tripId", con);
            checkTrip.Parameters.AddWithValue("@tripId", tripId);
            var maxPeople = checkTrip.ExecuteScalar();
            if (maxPeople == null)
                return false;

            var count = new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @tripId", con);
            count.Parameters.AddWithValue("@tripId", tripId);
            int current = (int)count.ExecuteScalar();
            if (current >= (int)maxPeople)
                return false;

            var insert = new SqlCommand(@"
            INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
            VALUES (@id, @tripId, @date)", con);
            insert.Parameters.AddWithValue("@id", clientId);
            insert.Parameters.AddWithValue("@tripId", tripId);
            insert.Parameters.AddWithValue("@date", DateTime.Now);
            insert.ExecuteNonQuery();
        }
        return true;
    }

    public async Task<bool> DeleteClientTrip(int id, int tripId)
    {
        using (var con = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(@"DELETE FROM Client_Trip WHERE IdClient = @id AND IdTrip = @tripId", con))
        {
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@tripId", tripId);
            con.Open();
            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                return false;
        }

        return true;
    }
}