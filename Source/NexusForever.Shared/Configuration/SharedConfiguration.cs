using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using NLog;

namespace NexusForever.Shared.Configuration
{
    public class SharedConfiguration : Singleton<SharedConfiguration>, ISharedConfiguration
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration configuration;

        private ImmutableDictionary<Type, string> binds;

        public SharedConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Initialise<T>()
        {
            InitialiseBindSections<T>();
        }

        /// <summary>
        /// Create binds for a configuration file model <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// Takes a configuration model and recursively finds any child configuration models with the <see cref="ConfigurationBindAttribute"/> attribute.<br/>
        /// Any type with this attribute has a bind created where the type is paired with breadcrumbs based on the property name,
        /// this is used when calling <see cref="Get{T}"/> to map the configuration model to a <see cref="IConfigurationSection"/>.
        /// </remarks>
        private void InitialiseBindSections<T>()
        {
            log.Info("Initialising configuration binds...");

            var builder = ImmutableDictionary.CreateBuilder<Type, string>();

            // Create binds on root configuration
            foreach (PropertyInfo info in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => p.PropertyType.GetCustomAttribute<ConfigurationBindAttribute>() != null))
            {
                // Initialize with a StringBuilder instead of string
                StringBuilder breadcrumbs = new StringBuilder(info.Name);
                InitialiseBindSection(info, breadcrumbs, builder);
            }

            binds = builder.ToImmutable();

            log.Trace($"Initialized {binds.Count} configuration bind(s)...");
        }

        private void InitialiseBindSection(PropertyInfo info, StringBuilder breadcrumbs, ImmutableDictionary<Type, string>.Builder builder)
        {
            builder.Add(info.PropertyType, breadcrumbs.ToString()); // Convert to string only when adding to builder

            // Create binds on child configurations
            int length = breadcrumbs.Length;
            foreach (PropertyInfo child in info.PropertyType
                         .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                         .Where(p => p.PropertyType.GetCustomAttribute<ConfigurationBindAttribute>() != null))
            {
                breadcrumbs.Append(":").Append(child.Name);
                InitialiseBindSection(child, breadcrumbs, builder);
                breadcrumbs.Length = length; // Reset the StringBuilder to its original state before they append
            }
        }

        /// <summary>
        /// Return configuration model <typeparamref name="T"/>.
        /// </summary>
        public T Get<T>()
        {
            if (!binds.TryGetValue(typeof(T), out string key))
                return default;

            return configuration.GetSection(key).Get<T>();
        }
    }
}
