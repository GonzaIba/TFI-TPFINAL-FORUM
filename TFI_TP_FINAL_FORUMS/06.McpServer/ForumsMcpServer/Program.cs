using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using ModelContextProtocol.Server;
using Microsoft.AspNetCore.Builder; // Add this namespace for MapMcp extension method  

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.AddConsole(options =>
//{
//    options.LogToStandardErrorThreshold = LogLevel.Trace;
//});

builder.Services.AddMvc().AddJsonOptions(options => {
    options.JsonSerializerOptions.MaxDepth = 1;  // or however deep you need
});

builder.Services
  .AddMcpServer()
  .WithHttpTransport(opt =>
  {
      //opt.MaxIdleSessionCount = 1;
  })
  .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp(); // Ensure the extension method is defined in the added namespace

await app.RunAsync("http://localhost:5001");
