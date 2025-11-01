using Yarp.ReverseProxy.Configuration;
using System.Collections.Generic;

public static class ReverseProxyConfig
{
    public static (IReadOnlyList<RouteConfig> Routes, IReadOnlyList<ClusterConfig> Clusters) GetProxyConfig(bool isDevelopment)
    {
        var userServiceAddress = isDevelopment
            ? "http://localhost:5085/"
            : "https://your-production-url/";

        var documentServiceAddress = isDevelopment
            ? "http://localhost:5298/"
            : "https://your-production-url/";

        var requestServiceAddress = isDevelopment
            ? "http://localhost:5167/"
            : "https://your-production-url/";

        var notificationServiceAddress = isDevelopment
            ? "http://localhost:5211/"
            : "https://your-production-url/";

        var routes = new List<RouteConfig>
        {
            new RouteConfig
            {
                RouteId = "user-route",
                ClusterId = "user-service-cluster",
                Match = new RouteMatch
                {
                    Path = "/api/users/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "health-route",
                ClusterId = "user-service-cluster",
                Match = new RouteMatch
                {
                    Path = "/api/health/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "document-route",
                ClusterId = "document-service-cluster",
                Match = new RouteMatch
                {
                    Path = "/api/document/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "request-route",
                ClusterId = "request-service-cluster",
                Match = new RouteMatch
                {
                    Path = "/api/signingrequest/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "request-hub-route",
                ClusterId = "request-service-cluster",
                Match = new RouteMatch
                {
                    Path = "/api/signingrequest/hubs/{**catch-all}"
                }
            },
            new RouteConfig
            {
                RouteId = "notification-route",
                ClusterId = "notification-service-cluster",
                Match = new RouteMatch
                {
                    Path = "/api/notification/{**catch-all}"
                }
            }
        };

        var clusters = new List<ClusterConfig>
        {
            new ClusterConfig
            {
                ClusterId = "user-service-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "user-service-destination", new DestinationConfig { Address = userServiceAddress } }
                }
            },
             new ClusterConfig
            {
                ClusterId = "document-service-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "document-service-destination", new DestinationConfig { Address = documentServiceAddress } }
                }
            },
            new ClusterConfig
            {
                ClusterId = "request-service-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "request-service-destination", new DestinationConfig { Address = requestServiceAddress } }
                }
            },
            new ClusterConfig
            {
                ClusterId = "request-hub-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "request-service-destination", new DestinationConfig { Address = requestServiceAddress } }
                }
            },
            new ClusterConfig
            {
                ClusterId = "notification-service-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "notification-service-destination", new DestinationConfig { Address = notificationServiceAddress } }
                }
            }
        };

        return (routes, clusters);
    }
}
