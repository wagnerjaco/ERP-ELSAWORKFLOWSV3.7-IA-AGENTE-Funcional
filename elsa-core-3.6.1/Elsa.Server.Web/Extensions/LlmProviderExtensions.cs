using Elsa.Agents;
using Elsa.Server.Web.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Elsa.Server.Web.Extensions;

public static class LlmProviderExtensions
{
    public static IServiceCollection AddLlmProviders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var providers = configuration.GetSection("LlmProviders").Get<List<LlmProviderOptions>>() ?? [];

        foreach (var provider in providers)
        {
            services.AddSingleton(provider);
        }

        return services;
    }

    public static AgentsFeature AddLlmProviders(
        this AgentsFeature feature,
        IConfiguration configuration)
    {
        var providers = configuration.GetSection("LlmProviders").Get<List<LlmProviderOptions>>() ?? [];

        foreach (var provider in providers)
        {
            feature.AddLlmProvider(provider);
        }

        return feature;
    }

    public static AgentsFeature AddLlmProvider(
        this AgentsFeature feature,
        LlmProviderOptions options)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(options.BaseUrl) };

        return feature.AddServiceDescriptor(new Elsa.Agents.ServiceDescriptor
        {
            Name = options.Name,
            ConfigureKernel = builder => builder.AddOpenAIChatCompletion(
                options.DefaultModel,
                options.ApiKey,
                httpClient: httpClient)
        });
    }
}
