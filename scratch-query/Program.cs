using System;
using System.Threading.Tasks;
using Npgsql;

namespace ScratchQuery
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=Practice121;Username=postgres;Password=Password@123;SSL Mode=Prefer;Trust Server Certificate=true";
            
            try 
            {
                await using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();

                Console.WriteLine("Searching for 'Kandaliyadda'...");

                await using var cmd1 = new NpgsqlCommand("SELECT id, clinic_name FROM practice_centres WHERE clinic_name ILIKE '%Kandaliyadda%'", conn);
                await using var reader1 = await cmd1.ExecuteReaderAsync();
                bool hasCentres = false;
                while (await reader1.ReadAsync())
                {
                    hasCentres = true;
                    Console.WriteLine($"Found in PracticeCentres -> ID: {reader1.GetGuid(0)}, Name: {reader1.GetString(1)}");
                }
                await reader1.CloseAsync();

                await using var cmd2 = new NpgsqlCommand("SELECT id, name FROM places WHERE name ILIKE '%Kandaliyadda%'", conn);
                await using var reader2 = await cmd2.ExecuteReaderAsync();
                bool hasPlaces = false;
                while (await reader2.ReadAsync())
                {
                    hasPlaces = true;
                    Console.WriteLine($"Found in Places -> ID: {reader2.GetGuid(0)}, Name: {reader2.GetString(1)}");
                }

                if (!hasCentres && !hasPlaces) {
                    Console.WriteLine("Kandaliyadda was not found in either table.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
