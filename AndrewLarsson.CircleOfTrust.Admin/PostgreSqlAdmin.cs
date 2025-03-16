using Npgsql;

public static class PostgreSqlAdmin {
	public static async Task DeleteAllRecordsFromTable(string connectionString, string table) {
		await using var connection = new NpgsqlConnection(connectionString);
		await connection.OpenAsync();

		string query = $"DELETE FROM {table};";
		await using var command = new NpgsqlCommand(query, connection);
		await command.ExecuteNonQueryAsync();

		Console.WriteLine($"Deleted all records from {table}");
	}
}
