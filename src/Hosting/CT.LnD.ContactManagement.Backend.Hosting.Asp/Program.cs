using CT.LnD.ContactManagement.Backend.Business.Services.Implementations;
using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Configurations;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Implementations;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Interfaces;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
//using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDbContext<ContactManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<CT.LnD.ContactManagement.Backend.Business.Utility.Interfaces.IEmailService, CT.LnD.ContactManagement.Backend.Business.Utility.EmailService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{

    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));


    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Contact Management",
        Description = "Contact Management Application APIs",

    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    options.ExampleFilters();


});


builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt")["Key"])),
            ValidateAudience = false,
            ValidateIssuer = false,
        };

        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                string result = JsonSerializer.Serialize(new
                {
                    status = 401,
                    error = "Unauthorized",
                    message = "Access denied",
                    timeStamp = DateTime.UtcNow,
                });

                return context.Response.WriteAsync(result);
            },

            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                string result = JsonSerializer.Serialize(new
                {
                    status = 403,
                    error = "Forbidden",
                    message = "You do not have permission to access this route",
                    timeStamp = DateTime.UtcNow,
                });

                return context.Response.WriteAsync(result);
            }


        };

    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    policy.RequireRole("Admin"));
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ActiveUserOnly", policy =>
    policy.RequireRole("User").RequireClaim("Status", "Active"));
});


WebApplication app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Contact Management API");
    options.RoutePrefix = string.Empty;
});


/// <summary>
/// Get Server Status
/// </summary>
/// <returns>Server Status</returns>
/// <response code="200">Server is Running</response>

app.MapGet("/health", () => Results.Ok(new
{
    success = true,
    message = "Server is running."
}))
.WithName("HealthCheck")
.WithTags("Health")
.Produces(StatusCodes.Status200OK, typeof(object))
.WithSummary("Get Server Status")
.WithDescription("Returns a 200 OK response to indicate that the server is up and running.");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
