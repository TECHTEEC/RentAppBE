using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RentAppBE.DataContext;
using RentAppBE.Models;
using RentAppBE.Repositories.OtpService;
using RentAppBE.Repositories.SenderService.EmailService;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//-------------services--------------
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IUserOtpService, UserOtpService>();

//-----------------------------------

// Sql server Db Connections
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(setup =>
{

    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});
// Read CORS origins from config
var allowedOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();

// Define a CORS policy (e.g., allow all or restrict to specific origins)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCorsPolicy", policy =>
    {
        if (allowedOrigins!.Length == 1 && allowedOrigins[0] == "*")
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();





var app = builder.Build();

//Seed Messages

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!db.UserMessages.Any())
    {
        db.UserMessages.AddRange(new[]
        {
            new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "تم إرسال الرمز",
                EnglisMsg = "OTP has been sent"
            },
            new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "تم التسجيل بنجاح",
                EnglisMsg = "Registration successful"
            },
            new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "تم الوصول إلى الحد الأقصى لمحاولات إعادة الإرسال. يرجى الانتظار حتى انتهاء صلاحيته",
                EnglisMsg = "Maximum OTP resend attempts reached. Please wait until it expires."
            },
             new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = ":كود التغعيل الخاص بك",
                EnglisMsg = "Your OTP is:"
            },


        });

        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DynamicCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
