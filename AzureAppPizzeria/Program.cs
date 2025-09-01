using System.Text.Json.Serialization;
using Azure.Identity;
using AzureAppPizzeria.Core.Interfaces;
using AzureAppPizzeria.Core.Services;
using AzureAppPizzeria.Data;
using AzureAppPizzeria.Data.Configurations;
using AzureAppPizzeria.Data.Entities;
using AzureAppPizzeria.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AzureAppPizzeria.Data.Seeders;


var builder = WebApplication.CreateBuilder(args);

//konfigurera dependency injection för alla services, inkl UserManager och ApplicationUser/Identity
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddAzureWebAppDiagnostics();

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
       // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

var keyVaultUrl = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];

if (!string.IsNullOrEmpty(keyVaultUrl) && Uri.TryCreate(keyVaultUrl, UriKind.Absolute, out var validUri))
{
    builder.Configuration.AddAzureKeyVault(validUri, new DefaultAzureCredential());
    Console.WriteLine($"Successfully configured Azure key vault: {validUri}");
}
else
{
    Console.WriteLine("AZURE_KEY_VAULT is not configured, or invalid");
}

var connectionString = builder.Configuration["DefaultConnectionString"];
if (string.IsNullOrEmpty(connectionString))
{
    var errorMessage = "Connection string 'DefaultConnectionString' not found. " +
                       "Ensure it is set in Azure Key Vault (secret name 'DefaultConnectionString') " +
                       "and that the application has access to the Key Vault.";
    Console.Error.WriteLine(errorMessage);
    throw new InvalidOperationException(errorMessage);
}
else
{
    Console.WriteLine("Connection string 'DefaultConnection' retrieved successfully.");
}
builder.Services.AddSwaggerExtended();

var jwtSettings = new JwtSettings();
builder.Configuration.Bind("JwtSettings", jwtSettings); //bindning av Jwtsettings från appsettings.json
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    { //Här konfigureras identity alternativ som ex lösenordskrav
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddApplicationInsightsTelemetry(new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await RoleSeeder.SeedRoles(services);
        await UserSeeder.SeedAdminUserAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.UseSwaggerExtended();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerExtended(); 
}
else
{
    // app.UseExceptionHandler("/Error"); // Produktionsfelhanterare
    app.UseHsts();
}
//app.UseHttpsRedirection();
//app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
