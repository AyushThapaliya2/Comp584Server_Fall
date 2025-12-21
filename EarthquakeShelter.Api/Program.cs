using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using EarthquakeShelter.Api;
using EarthquakeModel;

var builder = WebApplication.CreateBuilder(args);

//
// ======================= SERVICES =======================
//

// Controllers + global auth policy
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Earthquake Shelter API",
        Version = "v1",
        Description = "API for surfacing Los Angeles earthquake activity and matching nearby emergency shelters.",
        Contact = new OpenApiContact
        {
            Name = "Ayush Thapaliya",
            Email = "ayush.thapaliya.36@my.csun.edu"
        }
    });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

// Database
builder.Services.AddDbContext<ShelterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity
builder.Services.AddIdentity<ShelterUser, IdentityRole>()
    .AddEntityFrameworkStores<ShelterContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
builder.Services.AddAuthentication(options =>
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
        RequireExpirationTime = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["JwtSettings:SecurityKey"]
                ?? throw new InvalidOperationException("JWT key missing")
            )
        )
    };
});

// JWT helper
builder.Services.AddScoped<JwtHandler>();

// ❌ IMPORTANT: Do NOT enable ASP.NET CORS here
// Nginx is already adding CORS headers. If you enable both, you get duplicate headers and browsers block.
// builder.Services.AddCors(...);


//
// ======================= APP =======================
//

var app = builder.Build();

// Seed admin user (optional)
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ShelterUser>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var username = config["DefaultAdmin:UserName"];
    var email = config["DefaultAdmin:Email"];
    var password = config["DefaultAdmin:Password"];

    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
    {
        var existing = await userManager.FindByNameAsync(username);
        if (existing == null)
        {
            var user = new ShelterUser
            {
                UserName = username,
                Email = email
            };
            await userManager.CreateAsync(user, password);
        }
    }
}

// Swagger ONLY in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ❌ Do NOT call app.UseCors("Frontend") here either (nginx handles it)

// Auth pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();