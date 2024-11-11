using ClientsWebApi.Entities;
using ClientsWebApi.Services;
using ClientsWebApi.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


var jwtSettings = new JwtSettings();
builder.Configuration.Bind("JwtSettings", jwtSettings);

var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);

AddAuthentication(builder.Services, jwtSettings);


var adminSettings = new AutoAdminSettings();
builder.Configuration.Bind("AutoAdminSettings", adminSettings);

var adminSection = builder.Configuration.GetSection("AutoAdminSettings");
builder.Services.Configure<AutoAdminSettings>(adminSection);



ConfigureServices(builder.Services);
//builder.Services.AddSingleton<AdminGeneratorService>();

var app = builder.Build();

await SeedAdminAsync(app.Services);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
 
async Task SeedAdminAsync(IServiceProvider appServices)
{
    using (var scope = appServices.CreateScope())
    {
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var adminSettings = services.GetRequiredService<IOptions<AutoAdminSettings>>().Value;

        var admin = await userManager.FindByEmailAsync(adminSettings.Email);
        if(admin == null)
        {
            admin = new IdentityUser()
            {
                Email = adminSettings.Email,
                UserName = adminSettings.Username,
            };

            await userManager.CreateAsync(admin, adminSettings.Password);
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}


void AddAuthentication(IServiceCollection services, JwtSettings jwtSettings)
{
    services.AddSingleton<JwtService>();

    services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.SigningKey ?? throw new InvalidOperationException())),
                ValidIssuer = jwtSettings.Issuer,
                ValidateIssuer = true,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };
            options.ClaimsIssuer = jwtSettings.Issuer;
        });


    services
        .AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;

            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddSignInManager()
        .AddEntityFrameworkStores<ApplicationDbContext>();
}


void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    services.AddAuthorization();

    AddSwagger(services);

    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer("Name=ConnectionStrings:ClientsDb");
    });
}


void AddSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Clients Web API."
        });

        options.EnableAnnotations();

        options.AddSecurityDefinition("Services Bearer", new OpenApiSecurityScheme()
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "Services",
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Services Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}
