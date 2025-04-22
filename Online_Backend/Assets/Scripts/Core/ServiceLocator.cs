using System;
using System.Collections.Generic;

public class ServiceLocator
{
    private static ServiceLocator instance;
    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static ServiceLocator Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ServiceLocator();
            }
            return instance;
        }
    }

    public void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            throw new Exception($"Service of type {type.Name} is already registered!");
        }
        services.Add(type, service);
    }

    public T Get<T>() where T : class
    {
        var type = typeof(T);
        if (!services.ContainsKey(type))
        {
            throw new Exception($"Service of type {type.Name} is not registered!");
        }
        return services[type] as T;
    }

    public void Unregister<T>() where T : class
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            services.Remove(type);
        }
    }
} 