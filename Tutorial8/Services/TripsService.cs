using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;";
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        string command = "SELECT IdTrip, Name, MaxPeople FROM Trip";

        await using (SqlConnection conn = new SqlConnection(_connectionString))
        await using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int IdTrip = (int)reader["IdTrip"];
                    string Name = (string)reader["Name"];
                    int MaxPeople = (int)reader["MaxPeople"];
                    trips.Add(new TripDTO()
                    {
                        Id = IdTrip,
                        Name = Name,
                        MaxPeople = MaxPeople,
                    });
                }
            }
        }
        

        return trips;
    }
    public async Task<List<TripDTO>> GetTrips(int id)
    {
        var trips = new List<TripDTO>();

        
        //Console.WriteLine(command);
        
        await using SqlConnection conn = new SqlConnection(_connectionString);
        await using SqlCommand cmd = new SqlCommand();
        
        cmd.Connection = conn;
        cmd.CommandText = "SELECT IdTrip, Name FROM Trip WHERE idTrip = @idTrip";
        cmd.Parameters.AddWithValue("@idTrip", id);
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
}