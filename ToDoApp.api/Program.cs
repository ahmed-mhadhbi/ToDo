using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using TodoApp.Domain.Entities;
using ToDoApp.Application.IRepo;
using ToDoApp.Application.Services.ToDos;
using ToDoApp.Application.Services.Users;
using ToDoApp.Infrastructure.Data;
using ToDoApp.Infrastructure.Identity;
using ToDoApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

#region SERILOG
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/fichier.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        shared: true)
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

#region CONTROLLERS
builder.Services.AddControllers();
#endregion

#region JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
#endregion

#region AUTHORIZATION
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser",
        policy => policy.RequireAuthenticatedUser());

    options.AddPolicy("RequireAdminRole",
        policy => policy.RequireRole("Admin"));

    options.AddPolicy("RequireFullName",
        policy => policy.RequireClaim("FullName"));

    options.AddPolicy("RequireAdminOrFullName",
        policy => policy.RequireRole("Admin")
                         .RequireClaim("FullName"));
});
#endregion



#region DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>();
#endregion

#region IDENTITY (JWT ONLY — NO COOKIES)
builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region DI
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
#endregion

#region SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TodoApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();   
app.UseAuthorization();

app.MapControllers();
app.Run();
