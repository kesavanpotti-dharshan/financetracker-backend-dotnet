using System.Text;
using FinanceTracker.Application.Accounts;
using FinanceTracker.Application.Auth;
using FinanceTracker.Application.Institutions;
using FinanceTracker.Application.Interfaces;
using FinanceTracker.Application.Statements;
using FinanceTracker.Application.Transactions;
using FinanceTracker.Infrastructure.Ai;
using FinanceTracker.Infrastructure.Auth;
using FinanceTracker.Infrastructure.Persistence;
using FinanceTracker.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// auth DI
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RefreshHandler>();
builder.Services.AddScoped<LogoutHandler>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<AccountHandlers>();

builder.Services.AddHttpClient<IStatementParser, GeminiStatementParser>();
builder.Services.AddScoped<IFileStorage, AzureBlobStorage>();
builder.Services.AddScoped<IStatementRepository, StatementRepository>();
builder.Services.AddScoped<StatementHandlers>();

builder.Services.AddScoped<IInstitutionRepository, InstitutionRepository>();
builder.Services.AddScoped<InstitutionHandlers>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<TransactionHandlers>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!))
        };
    });

builder.Services.AddAuthorization();

// CORS — required since frontend (Vercel) and backend (Railway) are different origins, and cookies need credentials allowed
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173", "https://<your-vercel-domain>.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // required for cookies to be sent cross-origin
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();