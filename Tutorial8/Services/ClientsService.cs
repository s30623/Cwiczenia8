using Microsoft.Data.SqlClient;
using Tutorial8.Controllers;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;";

    public async Task<List<TripDTO>> GetClientTrips(int idClient)
    {
        var trips = new List<TripDTO>();
        if (await DoesClientExist(idClient))
        {
                
                //Console.WriteLine(command);
        
                await using SqlConnection conn = new SqlConnection(_connectionString);
                await using SqlCommand cmd = new SqlCommand();
        
                cmd.Connection = conn;
                cmd.CommandText = "SELECT t.IdTrip, t.Name FROM Trip t JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip WHERE ct.IdClient = @IDClient";
                cmd.Parameters.AddWithValue("@IDClient", idClient);
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
            
        }

        return trips;

    }

    public async Task<bool> DoesClientExist(int id)
    {
        await using SqlConnection conn = new SqlConnection(_connectionString);
        await using SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT 1 FROM Client WHERE IdClient = @IdClient";
        cmd.Parameters.AddWithValue("IdClient",id);
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