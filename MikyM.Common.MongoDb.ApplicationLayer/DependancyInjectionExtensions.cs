﻿using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Microsoft.Extensions.Options;
using MikyM.Common.ApplicationLayer;
using MikyM.Common.MongoDb.ApplicationLayer.Interfaces;
using MikyM.Common.MongoDb.ApplicationLayer.Services;

namespace MikyM.Common.MongoDb.ApplicationLayer;

/// <summary>
/// DI extensions for <see cref="ContainerBuilder"/>
/// </summary>
public static class DependancyInjectionExtensions
{
    /// <summary>
    /// Registers data services with the <see cref="ContainerBuilder"/>
    /// </summary>
    /// <param name="applicationConfiguration"></param>
    /// <param name="databases">List of database names if using more than one MongoDb (list also the default one) to register named data services for </param>
    /// <returns>Current <see cref="ApplicationConfiguration"/> instance</returns>
    public static ApplicationConfiguration AddMongoDbDataServices(
        this ApplicationConfiguration applicationConfiguration, IEnumerable<string> databases)
        => AddMongoDbDataServices(applicationConfiguration, null, databases);
    /// <summary>
    /// Registers data services with the <see cref="ContainerBuilder"/>
    /// </summary>
    /// <param name="applicationConfiguration"></param>
    /// <param name="options">Configuration action</param>
    /// <param name="databases">Lista nazw baz danych jeśli więcej niż 1 wraz z </param>
    /// <returns>Current <see cref="ApplicationConfiguration"/> instance</returns>
    public static ApplicationConfiguration AddMongoDbDataServices(this ApplicationConfiguration applicationConfiguration, Action<MongoDbServiceApplicationConfiguration>? options = null, IEnumerable<string>? databases = null)
    {
        if (applicationConfiguration.GetType().GetField("Builder", BindingFlags.Instance |
                                                                   BindingFlags.NonPublic |
                                                                   BindingFlags.Public)
                ?.GetValue(applicationConfiguration) is not ContainerBuilder builder)
            throw new InvalidOperationException();

        var config = new MongoDbServiceApplicationConfiguration(applicationConfiguration);
        options?.Invoke(config);

        builder.RegisterType<MongoDbDataServiceFactory>().As<IMongoDbDataServiceFactory>().InstancePerLifetimeScope();
        
        builder.Register(x => config).As<IOptions<MongoDbServiceApplicationConfiguration>>().SingleInstance();
        
        IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> registReadOnlyBuilder;
        IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> registCrudBuilder;

        switch (config.BaseGenericDataServiceLifetime)
        {
            case Lifetime.SingleInstance:
                registReadOnlyBuilder = builder.RegisterGeneric(typeof(ReadOnlyMongoDbDataService<>))
                    .As(typeof(IReadOnlyMongoDbDataService<>))
                    .SingleInstance();
                registCrudBuilder = builder.RegisterGeneric(typeof(CrudMongoDbDataService<>))
                    .As(typeof(ICrudMongoDbDataService<>))
                    .SingleInstance();
                break;
            case Lifetime.InstancePerRequest:
                registReadOnlyBuilder = builder.RegisterGeneric(typeof(ReadOnlyMongoDbDataService<>))
                    .As(typeof(IReadOnlyMongoDbDataService<>))
                    .InstancePerRequest();
                registCrudBuilder = builder.RegisterGeneric(typeof(CrudMongoDbDataService<>))
                    .As(typeof(ICrudMongoDbDataService<>))
                    .InstancePerRequest();
                break;
            case Lifetime.InstancePerLifetimeScope:
                registReadOnlyBuilder = builder.RegisterGeneric(typeof(ReadOnlyMongoDbDataService<>))
                    .As(typeof(IReadOnlyMongoDbDataService<>))
                    .InstancePerLifetimeScope();
                registCrudBuilder = builder.RegisterGeneric(typeof(CrudMongoDbDataService<>))
                    .As(typeof(ICrudMongoDbDataService<>))
                    .InstancePerLifetimeScope();
                break;
            case Lifetime.InstancePerMatchingLifetimeScope:
                registReadOnlyBuilder = builder.RegisterGeneric(typeof(ReadOnlyMongoDbDataService<>))
                    .As(typeof(IReadOnlyMongoDbDataService<>))
                    .InstancePerMatchingLifetimeScope();
                registCrudBuilder = builder.RegisterGeneric(typeof(CrudMongoDbDataService<>))
                    .As(typeof(ICrudMongoDbDataService<>))
                    .InstancePerMatchingLifetimeScope();
                break;
            case Lifetime.InstancePerDependancy:
                registReadOnlyBuilder = builder.RegisterGeneric(typeof(ReadOnlyMongoDbDataService<>))
                    .As(typeof(IReadOnlyMongoDbDataService<>))
                    .InstancePerDependency();
                registCrudBuilder = builder.RegisterGeneric(typeof(CrudMongoDbDataService<>))
                    .As(typeof(ICrudMongoDbDataService<>))
                    .InstancePerDependency();
                break;
            case Lifetime.InstancePerOwned:
                throw new NotSupportedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(config.BaseGenericDataServiceLifetime),
                    config.BaseGenericDataServiceLifetime, null);
        }

        // base data interceptors
        bool crudEnabled = false;
        bool readEnabled = false;
        foreach (var (interceptorType, dataConfig) in config.DataInterceptors)
        {
            switch (dataConfig)
            {
                case DataInterceptorConfiguration.CrudAndReadOnly:
                    registCrudBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                        ? registCrudBuilder.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                        : registCrudBuilder.InterceptedBy(interceptorType);
                    registReadOnlyBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                        ? registReadOnlyBuilder.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                        : registReadOnlyBuilder.InterceptedBy(interceptorType);

                    if (!crudEnabled)
                    {
                        registCrudBuilder = registCrudBuilder.EnableInterfaceInterceptors();
                        crudEnabled = true;
                    }

                    if (!readEnabled)
                    {
                        registReadOnlyBuilder = registCrudBuilder.EnableInterfaceInterceptors();
                        readEnabled = true;
                    }

                    break;
                case DataInterceptorConfiguration.Crud:
                    registCrudBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                        ? registCrudBuilder.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                        : registCrudBuilder.InterceptedBy(interceptorType);
                    if (!crudEnabled)
                    {
                        registCrudBuilder = registCrudBuilder.EnableInterfaceInterceptors();
                        crudEnabled = true;
                    }

                    break;
                case DataInterceptorConfiguration.ReadOnly:
                    registReadOnlyBuilder = interceptorType.GetInterfaces().Any(x => x == typeof(IAsyncInterceptor))
                        ? registReadOnlyBuilder.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(interceptorType))
                        : registReadOnlyBuilder.InterceptedBy(interceptorType);
                    if (!readEnabled)
                    {
                        registReadOnlyBuilder = registCrudBuilder.EnableInterfaceInterceptors();
                        readEnabled = true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataConfig));
            }
        }

        var excluded = new[] { typeof(MongoDbDataServiceBase), typeof(CrudMongoDbDataService<>), typeof(ReadOnlyMongoDbDataService<>) };

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var dataSubSet = assembly.GetTypes()
                .Where(x => x.GetInterfaces()
                                .Any(y => y.IsGenericType &&
                                          y.GetGenericTypeDefinition() == typeof(IMongoDbDataServiceBase)) &&
                            x.IsClass && !x.IsAbstract)
                .ToList();
            
            dataSubSet.RemoveAll(x => excluded.Any(y => y == x));

            // handle data services
            foreach (var dataType in dataSubSet)
            {
                var scopeOverrideAttr = dataType.GetCustomAttribute<LifetimeAttribute>(false);
                var intrAttrs = dataType.GetCustomAttributes<InterceptedByAttribute>(false).ToList();
                var asAttr = dataType.GetCustomAttributes<RegisterAsAttribute>(false).ToList();
                var intrEnableAttr = dataType.GetCustomAttribute<EnableInterceptionAttribute>(false);

                var scope = scopeOverrideAttr?.Scope ?? config.DataServiceLifetime;

                var registerAsTypes = asAttr.Where(x => x.RegisterAsType is not null)
                    .Select(x => x.RegisterAsType)
                    .Distinct()
                    .ToList();
                var shouldAsSelf = asAttr.Any(x => x.RegisterAsOption == RegisterAs.Self) &&
                                   asAttr.All(x => x.RegisterAsType != dataType);
                var shouldAsInterfaces =
                    !asAttr.Any() || asAttr.Any(x => x.RegisterAsOption == RegisterAs.ImplementedInterfaces);

                IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>?
                    registrationGenericBuilder = null;
                IRegistrationBuilder<object, ReflectionActivatorData, SingleRegistrationStyle>? registrationBuilder =
                    null;

                if (dataType.IsGenericType && dataType.IsGenericTypeDefinition)
                {
                    if (intrEnableAttr is not null)
                        registrationGenericBuilder = shouldAsInterfaces
                            ? builder.RegisterGeneric(dataType).AsImplementedInterfaces().EnableInterfaceInterceptors()
                            : builder.RegisterGeneric(dataType).EnableInterfaceInterceptors();
                    else
                        registrationGenericBuilder = shouldAsInterfaces
                            ? builder.RegisterGeneric(dataType).AsImplementedInterfaces()
                            : builder.RegisterGeneric(dataType);
                }
                else
                {
                    if (intrEnableAttr is not null)
                    {
                        registrationBuilder = intrEnableAttr.Intercept switch
                        {
                            Intercept.InterfaceAndClass => shouldAsInterfaces
                                ? builder.RegisterType(dataType)
                                    .AsImplementedInterfaces()
                                    .EnableClassInterceptors()
                                    .EnableInterfaceInterceptors()
                                : builder.RegisterType(dataType)
                                    .EnableClassInterceptors()
                                    .EnableInterfaceInterceptors(),
                            Intercept.Interface => shouldAsInterfaces
                                ? builder.RegisterType(dataType).AsImplementedInterfaces().EnableInterfaceInterceptors()
                                : builder.RegisterType(dataType).EnableInterfaceInterceptors(),
                            Intercept.Class => shouldAsInterfaces
                                ? builder.RegisterType(dataType).AsImplementedInterfaces().EnableClassInterceptors()
                                : builder.RegisterType(dataType).EnableClassInterceptors(),
                            _ => throw new ArgumentOutOfRangeException(nameof(intrEnableAttr.Intercept))
                        };
                    }
                    else
                    {
                        registrationBuilder = shouldAsInterfaces
                            ? builder.RegisterType(dataType).AsImplementedInterfaces()
                            : builder.RegisterType(dataType);
                    }
                }

                if (shouldAsSelf)
                {
                    registrationBuilder = registrationBuilder?.As(dataType);
                    registrationGenericBuilder = registrationGenericBuilder?.AsSelf();
                }

                foreach (var asType in registerAsTypes)
                {
                    if (asType is null) throw new InvalidOperationException("Type was null during registration");

                    registrationBuilder = registrationBuilder?.As(asType);
                    registrationGenericBuilder = registrationGenericBuilder?.As(asType);
                }

                switch (scope)
                {
                    case Lifetime.SingleInstance:
                        registrationBuilder = registrationBuilder?.SingleInstance();
                        registrationGenericBuilder = registrationGenericBuilder?.SingleInstance();
                        break;
                    case Lifetime.InstancePerRequest:
                        registrationBuilder = registrationBuilder?.InstancePerRequest();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerRequest();
                        break;
                    case Lifetime.InstancePerLifetimeScope:
                        registrationBuilder = registrationBuilder?.InstancePerLifetimeScope();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerLifetimeScope();
                        break;
                    case Lifetime.InstancePerDependancy:
                        registrationBuilder = registrationBuilder?.InstancePerDependency();
                        registrationGenericBuilder = registrationGenericBuilder?.InstancePerDependency();
                        break;
                    case Lifetime.InstancePerMatchingLifetimeScope:
                        registrationBuilder =
                            registrationBuilder?.InstancePerMatchingLifetimeScope(scopeOverrideAttr?.Tags.ToArray() ??
                                Array.Empty<object>());
                        registrationGenericBuilder =
                            registrationGenericBuilder?.InstancePerMatchingLifetimeScope(
                                scopeOverrideAttr?.Tags.ToArray() ?? Array.Empty<object>());
                        break;
                    case Lifetime.InstancePerOwned:
                        if (scopeOverrideAttr?.Owned is null)
                            throw new InvalidOperationException("Owned type was null");

                        registrationBuilder = registrationBuilder?.InstancePerOwned(scopeOverrideAttr.Owned);
                        registrationGenericBuilder =
                            registrationGenericBuilder?.InstancePerOwned(scopeOverrideAttr.Owned);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(scope));
                }

                foreach (var attr in intrAttrs)
                {
                    registrationBuilder = attr.IsAsync
                        ? registrationBuilder?.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(attr.Interceptor))
                        : registrationBuilder?.InterceptedBy(attr.Interceptor);
                    registrationGenericBuilder = attr.IsAsync
                        ? registrationGenericBuilder?.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(attr.Interceptor))
                        : registrationGenericBuilder?.InterceptedBy(attr.Interceptor);
                }
            }
        }

        return applicationConfiguration;
    }
}