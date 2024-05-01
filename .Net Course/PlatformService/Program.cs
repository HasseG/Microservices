using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        if(builder.Environment.IsProduction())
        {
            Console.WriteLine("--> Using SqlServer Db");
            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsDbConnection")));
        }
        else
        {
            Console.WriteLine("--> Using InMem Db");
            builder.Services.AddDbContext<AppDbContext>
            (opt => opt.UseInMemoryDatabase("InMem"));
        }

        //Dependeny Injection
        builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
        builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();
        
        Console.WriteLine($"--> CommandService Endpoint {app.Configuration["CommandService"]}");

        PrepDb.PrepPopulation(app, app.Environment.IsProduction());
        

        app.Run();
    }
}