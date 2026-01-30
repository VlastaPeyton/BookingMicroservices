using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.MinimalApis
{   
    public static class JsonEnumConverterExtensions
    {
        public static IServiceCollection AddJsonEnumConverter(this IServiceCollection services)
        {
            // Minimal API kad primi RequestDto koji sadrzi Enum polje, da osiguram da ce iz stringa u http request da prevede u Enum automatski
            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            return services;
        }
    }
}
