using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TestTaskAPI.Data
{
    public class DatabaseInitializer
    {
        private readonly string _adminConnectionString;
        private readonly string _appConnectionString;

        public DatabaseInitializer(IConfiguration config)
        {
            _adminConnectionString = config.GetConnectionString("AdminPgConnection");
            _appConnectionString = config.GetConnectionString("DefaultConnectionTest");
        }

        public void Initialize()
        {
            CreateDatabaseIfNotExists();
            ApplyMigrations();
        }

        private void CreateDatabaseIfNotExists()
        {
            try
            {
                using var conn = new NpgsqlConnection(_adminConnectionString);
                conn.Open();

                var dbName = new NpgsqlConnectionStringBuilder(_appConnectionString).Database;

                // Проверяем существование БД
                var checkCmd = new NpgsqlCommand(
                    $"SELECT 1 FROM pg_database WHERE datname = '{dbName}'",
                    conn);

                if (checkCmd.ExecuteScalar() == null)
                {
                    var createCmd = new NpgsqlCommand($"CREATE DATABASE {dbName}", conn);
                    createCmd.ExecuteNonQuery();
                    Console.WriteLine($"База '{dbName}' создана");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании БД: {ex.Message}");
                throw;
            }
        }

        private void ApplyMigrations()
        {
            try
            {
                var options = new DbContextOptionsBuilder<TesttaskdbContext>()
                    .UseNpgsql(_appConnectionString)
                .Options;

                using var context = new TesttaskdbContext(options);

                if (context.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine("Применяем миграции...");
                    context.Database.Migrate();
                    Console.WriteLine("Миграции применены");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка миграций: {ex.Message}");
                throw;
            }
        }
    }
}

