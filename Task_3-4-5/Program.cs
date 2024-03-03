using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;

class Program
{
    static async Task Main()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString;

        DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

        DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

        using (var connection = factory.CreateConnection())
        {
            connection.ConnectionString = connectionString;

            try
            {
                await connection.OpenAsync();
                Console.WriteLine("Подключение к базе данных «Grocery» успешно открыто.\n");

                var command = factory.CreateCommand();
                command.Connection = connection;

                // Task 3: async read
                command.CommandText = "SELECT * FROM Products";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                        Console.WriteLine(string.Format(
                            "Название: {0,-10}\tТип: {1,-10}\tЦвет: {2,-10}\tКалорийность: {3,-10}",
                            reader["name"], reader["type"], reader["color"], reader["calories"])
                        );
                }

                // Task 4: async update
                command.CommandText = "UPDATE Products SET name = 'Морковка' WHERE id = 1";
                await command.ExecuteNonQueryAsync();

                // Task 5: async delete
                command.CommandText = "DELETE FROM Products WHERE id = 30";
                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            catch (DbException ex)
            {
                Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }
}
