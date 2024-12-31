using Microsoft.EntityFrameworkCore;
using Test.Data;

namespace Test.Extentions;

public static class EFCoreExtensions
{
    public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(
            options => options.UseSqlServer(config.GetConnectionString("MyConnectionString"))
        );
        return services;
    }
}