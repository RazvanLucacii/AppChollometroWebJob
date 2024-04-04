using AppChollometroWebJob.Data;
using AppChollometroWebJob.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Bienvenido a Chollos");
string connectionString = @"Data Source=sqlrazvan.database.windows.net;Initial Catalog=AZURETAJAMAR;Persist Security Info=True;User ID=adminsql;Password=Admin123;Trust Server Certificate=True";
var provider = new ServiceCollection()
    .AddTransient<RepositoryChollometro>()
    .AddDbContext<ChollometroContext>(options => options.UseSqlServer(connectionString))
    .BuildServiceProvider();

RepositoryChollometro repo = provider.GetService<RepositoryChollometro>();
await repo.PopulateChollosAzureAsync();

Console.WriteLine("Fin de las acciones");
