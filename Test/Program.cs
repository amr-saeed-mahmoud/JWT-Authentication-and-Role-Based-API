using Test.Extentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.InjectDbContext(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .AddIdentityAuth(builder.Configuration)
                .AddSwaggerExplorer();

var app = builder.Build();

app.ConfigureSwaggerExplorer()
   .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
