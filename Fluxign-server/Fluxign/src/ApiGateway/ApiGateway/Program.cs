using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var isDevelopment = builder.Environment.IsDevelopment();

var (routes, clusters) = ReverseProxyConfig.GetProxyConfig(isDevelopment);

if (routes == null || clusters == null)
{
    throw new Exception("Routes or Clusters is null!");
}

builder.Services.AddReverseProxy()
    .LoadFromMemory(routes, clusters);

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.MapReverseProxy();

app.Run();
