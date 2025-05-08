using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT IdTrip, Name, Description,DateFrom,DateTo,MaxPeople FROM Trip";
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        DateFrom = reader.GetDateTime(3),
                        DateTo = reader.GetDateTime(4),
                        maxPeople = reader.GetInt32(5)
                    });
                }
            }
        }
        
        foreach (var trip in trips)
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
        

        return trips;
    }
}