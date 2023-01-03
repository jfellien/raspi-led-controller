using System.Device.Spi;
using LedStripControllerApi.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<ILedStrip, LedStrip>(x => LedStrip.ForSimulation());
    Console.WriteLine("LED Strip is in simulation mode");
}
else
{
    int numberOfLeds = 60;
    var spiSettings = new SpiConnectionSettings(0, 1)
    {
        ClockFrequency = 2_400_000,
        Mode = SpiMode.Mode0,
        DataBitLength = 8
    };

    builder.Services.AddSingleton<ILedStrip, LedStrip>(x => new LedStrip(spiSettings, numberOfLeds));
    Console.WriteLine("LED Strip is in production mode");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment() == false)
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthorization();

app.MapControllers();

app.Run();
