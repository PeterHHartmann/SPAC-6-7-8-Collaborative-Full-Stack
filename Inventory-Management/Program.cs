using Inventory_Management;
using Inventory_Management.Context;
using Inventory_Management.Factories;
using Inventory_Management.Helpers;
using Inventory_Management.Interfaces;
using Inventory_Management.Managers;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), "../.env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockFlow API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Configuration.AddEnvironmentVariables();

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

var AllowSpecificOrigins = "AllowFrontendOrigin";
builder.Services.AddCors(o => o.AddPolicy(AllowSpecificOrigins, builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    //options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;

    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

    options.JsonSerializerOptions.MaxDepth = 32;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextPool<InventoryDbContext>(opt =>
    opt.UseNpgsql(Environment.GetEnvironmentVariable("DefaultConnection")));

// If connection to the database is successful, print a message to the console
using (var db = new InventoryDbContext(builder.Services.BuildServiceProvider().GetRequiredService<DbContextOptions<InventoryDbContext>>()))
{
    db.Database.EnsureCreated();
    Console.WriteLine("Connected to the database successfully!");
}

builder.Services.AddScoped<OrderManager>();
builder.Services.AddScoped<ProductManager>();
builder.Services.AddScoped<CategoryManager>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<AuthHelpers>();
builder.Services.AddScoped<InventoryTransactionManager>();
builder.Services.AddScoped<CustomerManager>();
builder.Services.AddSingleton<IProductFactory, ProductFactory>();
builder.Services.AddScoped<IProductStockObserver, LowStockAlertObserver>();

builder.Services.AddScoped<RetailDataParser>();

// Uncomment the following code to parse and save the data from the CSV file into the database

// RetailDataParser retailDataParser = builder.Services.BuildServiceProvider().GetRequiredService<RetailDataParser>();

// retailDataParser.ParseAndSaveData(Path.Combine(Directory.GetCurrentDirectory(), "../data/synthetic_online_retail_data.csv"));


// Add these to your Program.cs
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // This is the default scheme that will be used to authenticate the user
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // This is the default scheme that will be used to challenge the user
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters // These are the parameters that will be used to validate the token
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured")))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(AllowSpecificOrigins);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
