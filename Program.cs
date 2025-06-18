using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RentAppBE.DataContext;
using RentAppBE.Models;
using RentAppBE.Repositories.OtpService;
using RentAppBE.Repositories.SenderService.EmailService;
using RentAppBE.Repositories.TokenService;
using RentAppBE.Shared.Services.ValidationService;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//-------------services--------------
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IUserOtpService, UserOtpService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IValidationService, ValidationService>();


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

// add token JWT

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();




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
             new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "عليك ادخال رقم الهاتف للتحقق",
                EnglisMsg = "You should send OTP to phone number"
            },
              new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "رمز التحقق غير صحيح او منتهي الصلاحية",
                EnglisMsg = "Invalid or expired OTP"
            },
                new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "رمز التحقق غير صحيح او منتهي الصلاحية",
                EnglisMsg = "Invalid or expired OTP"
            },
                new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "فشل بانشاء مستخدم",
                EnglisMsg = "Failed to create user"
            },
                new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "تمت العملية بنجاح",
                EnglisMsg = "The operation was completed successfully"
            },
                 new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "الايميل مطلوب",
                EnglisMsg = "Email is required"
            },
                  new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "شكل الايميل خاطئ",
                EnglisMsg = "Invalid email format"
            },

                  new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "الهاتف مطلوب",
                EnglisMsg = "Phone number is required"
            },
                   new UserMessage
            {
                Id = Guid.NewGuid(),
                ArabicMsg = "يجب أن يبدأ رقم الهاتف بـ +9639 ويتبعه 8 أرقام",
                EnglisMsg = "Phone number must start with +9639 and be followed by 8 digits"
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
