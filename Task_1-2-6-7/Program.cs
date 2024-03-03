using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["GroceryDB"].ConnectionString;

        Console.WriteLine("Виберіть СКБД: 1 - SQL Server, 2 - MySQL, 3 - PostgreSQL");
        var dbChoice = Console.ReadLine();

        string providerName = dbChoice switch
        {
            "1" => "System.Data.SqlClient",
            "2" => "MySql.Data.MySqlClient",
            "3" => "Npgsql",
            _ => throw new Exception("Невідомий вибір СКБД")
        };

        DbProviderFactories.RegisterFactory(providerName, SqlClientFactory.Instance);

        DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

        using (var connection = factory.CreateConnection())
        {
            connection.ConnectionString = connectionString;

            try
            {
                var stopwatch = Stopwatch.StartNew();

                connection.Open();
                Console.WriteLine("Подключение к базе данных «Grocery» успешно открыто.\n");

                var command = factory.CreateCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Products";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        Console.WriteLine(string.Format(
                            "Название: {0,-10}\tТип: {1,-10}\tЦвет: {2,-10}\tКалорийность: {3,-10}",
                            reader["name"], reader["type"], reader["color"], reader["calories"]));
                }

                Console.WriteLine($"Час виконання запиту: {stopwatch.Elapsed.TotalSeconds} секунд.\n");

                command.CommandText = "SELECT Name FROM Products";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        Console.WriteLine($"Название: {reader["name"]}");
                }

                Console.WriteLine($"Час виконання запиту: {stopwatch.Elapsed.TotalSeconds} секунд.\n");

                command.CommandText = "SELECT DISTINCT color FROM Products";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        Console.WriteLine($"Цвет: {reader["color"]}");
                }

                Console.WriteLine($"Час виконання запиту: {stopwatch.Elapsed.TotalSeconds} секунд.\n");

                command.CommandText = "SELECT MAX(calories) FROM Products";
                Console.WriteLine($"Максимальная калорийность: {command.ExecuteScalar()}");
                Console.WriteLine($"Час виконання запиту: {stopwatch.Elapsed.TotalSeconds} секунд.\n");

                command.CommandText = "SELECT MIN(calories) FROM Products";
                Console.WriteLine($"Минимальная калорийность: {command.ExecuteScalar()}");
                Console.WriteLine($"Час виконання запиту: {stopwatch.Elapsed.TotalSeconds} секунд.\n");

                command.CommandText = "SELECT AVG(calories) FROM Products";
                Console.WriteLine($"Средняя калорийность: {command.ExecuteScalar()}");
                Console.WriteLine($"Час виконання запиту: {stopwatch.Elapsed.TotalSeconds} секунд.\n");

                connection.Close();
            }
            catch (DbException ex)
            {
                Console.WriteLine("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
    }
}
