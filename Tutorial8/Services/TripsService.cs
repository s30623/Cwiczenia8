using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        // Use a dictionary to dedupe trips by IdTrip
        var tripDict = new Dictionary<int, TripDTO>();

        const string sql = @"
        SELECT 
            t.IdTrip, 
            t.Name AS TripName, 
            t.MaxPeople, 
            t.Description,
            t.DateFrom,
            t.DateTo,
            c.IdCountry, 
            c.Name AS CountryName
        FROM Trip t
        JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
        JOIN Country c ON c.IdCountry = ct.IdCountry;
    ";

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd  = new SqlCommand(sql, conn);

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();

        // Read every row
        while (await reader.ReadAsync())
        {
            // 1) Get the trip’s ID
            var idTrip = reader.GetInt32(reader.GetOrdinal("IdTrip"));

            // 2) If we haven't seen this trip yet, create it
            if (!tripDict.TryGetValue(idTrip, out var trip))
            {
                trip = new TripDTO
                {
                    Id         = idTrip,
                    Name       = reader.GetString(reader.GetOrdinal("TripName")),
                    MaxPeople  = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                    DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                    Countries  = new List<CountryDTO>()
                };
                tripDict[idTrip] = trip;
            }

            // 3) Always add the current country
            var country = new CountryDTO
            {
                Name = reader.GetString(reader.GetOrdinal("CountryName"))
            };
            trip.Countries.Add(country);
        }

        // Return all the trips we collected
        return tripDict.Values.ToList();
    }

    public async Task<List<TripDTO>> GetTrips(int id)
    {
        var trips = new List<TripDTO>();

        
        //Console.WriteLine(command);
        
        await using SqlConnection conn = new SqlConnection(_connectionString);
        await using SqlCommand cmd = new SqlCommand();
        
        cmd.Connection = conn;
        cmd.CommandText = "SELECT t.IdTrip, t.Name, t.MaxPeople, c.Name AS CountryName FROM Trip t JOIN Country_Trip ct ON t.IdTrip = ct.idTrip JOIN Country c ON c.IdCountry = ct.IdCountry WHERE t.idTrip = @idTrip";
        cmd.Parameters.AddWithValue("@idTrip", id);
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int idOrdinal = reader.GetOrdinal("IdTrip");
                    int MaxPeopleOrdinal = reader.GetOrdinal("MaxPeople");
                    //string Country = (string)reader["CountryName"];
                    trips.Add(new TripDTO()
                    {
                        Id = reader.GetInt32(idOrdinal),
                        Name = (string)reader["Name"],
                        MaxPeople = reader.GetInt32(MaxPeopleOrdinal),
                    });
                }
            }
        }
        

        return trips;
    }

    public async Task<bool> DoesTripExist(int id)
    {
        String command = $"";
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT 1 FROM Trip WHERE IdTrip = @idTrip";
        cmd.Parameters.AddWithValue("@idTrip", id);
        {
            await conn.OpenAsync();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return reader.HasRows;
                }
            }
        }
        return false;
    }

    public async Task<bool> MaxTripCount(int tripId)
    {
        using SqlConnection conn = new SqlConnection(_connectionString);
        using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT Count(*) AS Ilosc FROM Client_Trip WHERE IdTrip = @idTrip";
        cmd.Parameters.AddWithValue("@idTrip", tripId);
        int ilosc_uczestnikow = 0;
        int max_uczestnikow = 0;
        await conn.OpenAsync();
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                ilosc_uczestnikow = reader.GetOrdinal("Ilosc");
            }
        }

        cmd.CommandText = "SELECT MaxPeople FROM Trip WHERE IdTrip = @idTrip";
        cmd.Parameters.AddWithValue("@idTrip", tripId);
        await conn.OpenAsync();
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                max_uczestnikow = reader.GetOrdinal("MaxPeople");
            }
        }
        return ilosc_uczestnikow < max_uczestnikow;
    }
}