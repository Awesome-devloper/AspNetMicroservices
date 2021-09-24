using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Api.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailbility = retry.Value;
            using (var scope=host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Maigration postgrasql database");
                    using var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
                    connection.Open();

                    using var command = new NpgsqlCommand { Connection = connection };

                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();


                    command.CommandText = @"	CREATE TABLE Coupon(ID SERIAL PRIMARY KEY         NOT NULL,
                                                                        ProductName     VARCHAR(24) NOT NULL,
                                                                        Description     TEXT,
		                                                                Amount INT
	                                                                ); ";
                    command.ExecuteNonQuery();


                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Maigrated postgrasql database");

                }
                catch (NpgsqlException ex)
                {
                    logger.LogError("An Error occurred while migration the postgresql database");

                    if(retryForAvailbility<50)
                    {
                        retryForAvailbility++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailbility);
                    }
                }
                return host;
            }
        }
    }
}
