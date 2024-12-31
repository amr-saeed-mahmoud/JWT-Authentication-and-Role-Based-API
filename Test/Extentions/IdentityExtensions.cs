
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Test.Data;
using Test.Data.Models;

namespace Test.Extentions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();
        
        return services;
    }

    // Auth = Authontication + Authorization
    public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration Config)
    {
        services.AddAuthentication(o => {
            o.DefaultAuthenticateScheme = 
            o.DefaultChallengeScheme = 
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => {
            o.RequireHttpsMetadata = false;
            o.SaveToken = false;
            o.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = Config["JWT:Issuer"],
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["JWT:SecretKey"]!))      
            };
        });

        services.AddAuthorization(options => {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

        });


        return services;
    }

    public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
    {
      app.UseAuthentication();
      app.UseAuthorization();
      return app;
    }

}