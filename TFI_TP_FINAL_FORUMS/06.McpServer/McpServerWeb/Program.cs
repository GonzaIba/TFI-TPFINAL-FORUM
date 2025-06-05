using McpServerWeb.Tools;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using IoC.Resolver;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using Core.Contracts.Services;
using System.Text.Json.Serialization;
using Core.Domain.Models;
using Core.Domain.Request;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithTools<EchoTool>()
    .WithTools<SampleLlmTool>()
    .WithTools<PublicationsTool>();

//builder.Services.Configure<JsonSerializerOptions>(options => {
//    options.TypeInfoResolver = new MyJsonContext();
//});

builder.Services.ConfigureIoC(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b.AddSource("*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(b => b.AddMeter("*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithLogging()
    .UseOtlpExporter();

var app = builder.Build();

app.MapMcp();

app.Run();