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

                foreach (DataRow row in dt.Rows)
                {
                    int id = (int)row["Id"];
                    string name = row["Name"].ToString();
                    decimal balance = (decimal)row["Balance"];

                    string mergeSql = @"
                        MERGE INTO Customers_Transformed AS Target
                        USING (SELECT @Id AS Id) AS Source
                        ON Target.Id = Source.Id
                        WHEN MATCHED THEN
                            UPDATE SET Name = @Name, Balance = @Balance
                        WHEN NOT MATCHED THEN
                            INSERT (Id, Name, Balance) VALUES (@Id, @Name, @Balance);";

                    using (SqlCommand cmd = new SqlCommand(mergeSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Balance", balance);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}


/*
-- Create the Customers table
CREATE TABLE Customers (
    Id INT PRIMARY KEY,
    Name NVARCHAR(100),
    Balance DECIMAL(18, 2)
);

-- Insert test data into the Customers table
INSERT INTO Customers (Id, Name, Balance) VALUES
(1, 'Alice', 1500.00),
(2, 'Bob', 800.00),
(3, 'Charlie', 1200.00),
(4, 'David', 500.00),
(5, 'Eve', 2000.00);

-- Create the Customers_Transformed table
CREATE TABLE Customers_Transformed (
    Id INT PRIMARY KEY,
    Name NVARCHAR(100),
    Balance DECIMAL(18, 2)
);
-- Query to check the contents of the Customers_Transformed table
SELECT Id, Name, Balance
FROM Customers_Transformed
ORDER BY Id;
*/
