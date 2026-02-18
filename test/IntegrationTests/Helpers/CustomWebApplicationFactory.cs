using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;

namespace IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1. Buscamos el descriptor del DbContext original (SQL Server / LocalDB)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                // 2. Si existe, lo removemos para que no intente conectar a la BD real
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 3. Agregamos un proveedor de Base de Datos en Memoria para los tests
                // Creamos un nombre de base de datos único (un GUID) para que no choquen
                var dbName = Guid.NewGuid().ToString();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName); // <--- Cada instancia tendrá su propia DB
                });

                // 4. Crear la base de datos y asegurar que esté lista
                var sp = services.BuildServiceProvider();

                // Creamos un Scope para obtener el servicio de forma segura
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                    // Aseguramos que la base de datos esté creada
                    db.Database.EnsureCreated();

                    try
                    {
                        Utilities.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al sembrar la base de datos de test: {ex.Message}");
                    }
                }
            });
        }
    }
}