using Elsa.Studio.Contracts;
using Elsa.Studio.Extensions;
using Elsa.Studio.Models;
using Elsa.Studio.Secrets.Client;
using Elsa.Studio.Security;
using Elsa.Studio.Security.Menu;
using Elsa.Studio.Security.Pages;
using Elsa.Studio.Security.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Security;
namespace Elsa.Studio.Services
{
    public static class ExtencionSecurity
    {
        public static IServiceCollection AddSecurityModuleMod(this IServiceCollection services, BackendApiConfig backendApiConfig)
        {
            return services
                .AddScoped<IFeature, Feature>()
                .AddScoped<IMenuProvider, SecurityMenu>()
    
                ;
        }
    }
}

