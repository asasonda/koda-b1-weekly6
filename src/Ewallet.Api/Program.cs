using Ewallet.Api.Data;
using Ewallet.Api.Middlewares;
using Ewallet.Api.Repositories;
using Ewallet.Api.Seeding;
using Ewallet.Api.Services;


using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails(
    option =>
    {
        option.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Instance ??= context.HttpContext.Request.Path.Value;

            context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        };
    }
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddScoped<IWalletRepository, EfWalletRepository>();
builder.Services.AddScoped<IWalletServices, WalletServices>();

builder.Services.AddScoped<ITransactionRepository, EfTransactionRepository>();
builder.Services.AddScoped<IPaymentMethod, EfPaymentMethodRepository>();

builder.Services.AddScoped<ITopUpRequest, EfTopUpRequestRespository>();
builder.Services.AddScoped<ITopUpRequestServices, TopUpRequestService>();

builder.Services.AddScoped<IPaymentMethod, EfPaymentMethodRepository>();
builder.Services.AddScoped<IPaymentMethodServices, PaymentMethodServices>();

builder.Services.AddScoped<ITransferRepository, EfTransferRepository>();
builder.Services.AddScoped<ITransferService, TransferService>();

builder.Services.AddScoped<IWalletHistoryRepository, EfWalletHistoryRepository>();
builder.Services.AddScoped<IWalletHistoryService, WalletHistoryService>();



// builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
// builder.Services.AddScoped<IUserServices, UserServices>();
// builder.Services.AddScoped<IWalletRepository, SqlWalletRepository>();
// builder.Services.AddScoped<IWalletServices, WalletServices>();
// builder.Services.AddScoped<ITransactionRepository, SqlTransactionRepository>();


var connectionString = builder.Configuration.GetConnectionString("EwalletWeekly6") ?? throw new InvalidOperationException("connection belum diconfigurasi");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
        .UseSeeding((context, _) =>
        {
            PaymentMethodSeedData.Seed(context);
            UserSeedData.Seed(context);
        })
        .UseAsyncSeeding(async (context, _, ct) =>
        {
            await PaymentMethodSeedData.SeedAsync(context, ct);
            await UserSeedData.SeedAsync(context,ct);
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
