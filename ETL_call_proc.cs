/*
CREATE PROCEDURE TransformAndLoadCustomers
    @MinBalance DECIMAL(18, 2),  -- Input parameter for minimum balance
    @RecordsProcessed INT OUTPUT   -- Output parameter for number of records processed
AS
BEGIN
    SET NOCOUNT ON;

    -- Create a temporary table to hold the transformed data
    CREATE TABLE #TempCustomers (
        Id INT,
        Name NVARCHAR(100),
        Balance DECIMAL(18, 2)
    );

    -- Insert transformed data into the temporary table based on the input parameter
    INSERT INTO #TempCustomers (Id, Name, Balance)
    SELECT Id, Name,
           CASE 
               WHEN Balance > @MinBalance THEN Balance * 0.9 
               ELSE Balance 
           END AS Balance
    FROM Customers;

    -- Perform the MERGE operation
    MERGE INTO Customers_Transformed AS Target
    USING #TempCustomers AS Source
    ON Target.Id = Source.Id
    WHEN MATCHED THEN
        UPDATE SET Name = Source.Name, Balance = Source.Balance
    WHEN NOT MATCHED THEN
        INSERT (Id, Name, Balance) VALUES (Source.Id, Source.Name, Source.Balance);

    -- Get the number of records processed
    SELECT @RecordsProcessed = COUNT(*) FROM #TempCustomers;

    -- Drop the temporary table
    DROP TABLE #TempCustomers;
END;
*/

using System;
using System.Data;
using System.Data.SqlClient;

namespace ETL
{
    class Program
    {
        private const string connectionString = @"Data Source=Your Server;Initial Catalog=Your DB;Integrated Security=True;";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Reading...");
                DataTable customers = FetchData();

                Console.WriteLine("Writing...");
                int recordsProcessed = WriteData(1000); // Example input parameter for minimum balance

                Console.WriteLine($"ETL completed. Records processed: {recordsProcessed}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }

            Console.ReadLine();
        }

        static DataTable FetchData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Id, Name, Balance FROM Customers";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        static int WriteData(decimal minBalance)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Define the output parameter
                SqlParameter recordsProcessedParam = new SqlParameter("@RecordsProcessed", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                // Call the stored procedure to transform and load data
                using (SqlCommand cmd = new SqlCommand("TransformAndLoadCustomers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add input parameter
                    cmd.Parameters.AddWithValue("@MinBalance", minBalance);
                    // Add output parameter
                    cmd.Parameters.Add(recordsProcessedParam);

                    cmd.ExecuteNonQuery();

                    // Retrieve the output parameter value
                    return (int)recordsProcessedParam.Value;
                }
            }
        }
    }
}
