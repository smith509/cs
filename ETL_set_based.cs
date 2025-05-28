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

                Console.WriteLine("Transforming...");
                TransformData(customers);

                Console.WriteLine("Writing...");
                WriteData(customers);

                Console.WriteLine("ETL completed.");
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

        static void TransformData(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                decimal balance = (decimal)row["Balance"];
                if (balance > 1000)
                {
                    row["Balance"] = balance * 0.9m; // Apply 10% discount
                }
            }
        }

        static void WriteData(DataTable dt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Create a temporary table to hold the transformed data
                string createTempTableSql = @"
                    CREATE TABLE #TempCustomers (
                        Id INT,
                        Name NVARCHAR(100),
                        Balance DECIMAL(18, 2)
                    );";

                using (SqlCommand createTempTableCmd = new SqlCommand(createTempTableSql, conn))
                {
                    createTempTableCmd.ExecuteNonQuery();
                }

                // Bulk insert the transformed data into the temporary table
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = "#TempCustomers";
                    bulkCopy.WriteToServer(dt);
                }

                // Perform the MERGE operation
                string mergeSql = @"
                    MERGE INTO Customers_Transformed AS Target
                    USING #TempCustomers AS Source
                    ON Target.Id = Source.Id
                    WHEN MATCHED THEN
                        UPDATE SET Name = Source.Name, Balance = Source.Balance
                    WHEN NOT MATCHED THEN
                        INSERT (Id, Name, Balance) VALUES (Source.Id, Source.Name, Source.Balance);";

                using (SqlCommand mergeCmd = new SqlCommand(mergeSql, conn))
                {
                    mergeCmd.ExecuteNonQuery();
                }

                // Drop the temporary table
                string dropTempTableSql = "DROP TABLE #TempCustomers;";
                using (SqlCommand dropTempTableCmd = new SqlCommand(dropTempTableSql, conn))
                {
                    dropTempTableCmd.ExecuteNonQuery();
                }
            }
        }
    }
}
