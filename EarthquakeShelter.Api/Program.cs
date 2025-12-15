using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using EarthquakeShelter.Api;
using EarthquakeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() {
        Contact = new() {
            Email = "ayush.thapaliya.36@my.csun.edu",
            Name = "Ayush Thapaliya",
            Url = new("https://canvas.csun.edu/courses/128137")
        },
        Description = "API for surfacing Los Angeles earthquake activity and matching nearby emergency shelters.",
        Title = "Earthquake Shelter API",
        Version = "V1"
    });
    OpenApiSecurityScheme jwtSecurityScheme = new() {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Please enter *only* JWT token",
        Reference = new()
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
    c.AddSecurityRequirement(new()
    {
        { jwtSecurityScheme, [] }
    });
});

builder.Services.AddDbContext<ShelterContext>(optionsBuilder =>
    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ShelterUser, IdentityRole>()
    .AddEntityFrameworkStores<ShelterContext>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        RequireExpirationTime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            builder.Configuration["JwtSettings:SecurityKey"] ?? throw new InvalidOperationException()))
    };
});
builder.Services.AddScoped<JwtHandler>();

WebApplication app = builder.Build();

// Seed a default admin user if configured
using (IServiceScope scope = app.Services.CreateScope())
{
    UserManager<ShelterUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelterUser>>();
    IConfiguration config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    string? adminUser = config["DefaultAdmin:UserName"];
    string? adminEmail = config["DefaultAdmin:Email"];
    string? adminPassword = config["DefaultAdmin:Password"];

    if (!string.IsNullOrWhiteSpace(adminUser) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        ShelterUser? existing = await userManager.FindByNameAsync(adminUser);
        if (existing == null)
        {
            ShelterUser newUser = new()
            {
                UserName = adminUser,
                Email = adminEmail
            };
            await userManager.CreateAsync(newUser, adminPassword);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
