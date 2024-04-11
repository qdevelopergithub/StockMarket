    using Alpaca.Markets; 
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using StockMarket.Models;
    using System;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Add database context
    builder.Services.AddDbContext<appContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("Data")));

    // Configure Alpaca API client
    builder.Services.AddHttpClient<IAlpacaTradingClient, AlpacaTradingClient>(client =>
    {
        client.BaseAddress = new Uri("https://data.alpaca.markets/v2/");
        client.DefaultRequestHeaders.Add("APCA-API-KEY-ID", "PKPETELG3DY6Z5NUAO7P");
        client.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", "k0F5O8nzV1wIL0GbK8neSkkDRVeDtJxBTz8xK0UG");
    });


    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
