using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Masstransit.RabbitMQ.Configuration.First.Commons.Helpers
{
    public class TypeHelper
    {
        private static readonly ConcurrentDictionary<string, Type> Cache = new ConcurrentDictionary<string, Type>();

        public static void AddType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Cache.TryAdd(type.Name, type);
        }

        public static Type GetType(Assembly assembly, string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Cache.GetOrAdd(name, ReflectionHelper.GetTypeByName(assembly, name));
        }
    }
}