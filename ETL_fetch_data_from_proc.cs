/*
-- Create the stored procedure to fetch customers with a minimum balance
CREATE PROCEDURE GetCustomers
    @MinBalance DECIMAL(18, 2) -- Input parameter for minimum balance
AS
BEGIN
    SELECT Id, Name, Balance 
    FROM Customers
    WHERE Balance >= @MinBalance; -- Filter based on the input parameter
END
*/

static DataTable FetchData(decimal minBalance)
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        conn.Open();

        using (SqlCommand cmd = new SqlCommand("GetCustomers", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure; // Specify that this is a stored procedure

            // Add the input parameter for minimum balance
            cmd.Parameters.AddWithValue("@MinBalance", minBalance);

            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
