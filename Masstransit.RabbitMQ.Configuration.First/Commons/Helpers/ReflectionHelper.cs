using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Masstransit.RabbitMQ.Configuration.First.Commons.Helpers
{
    public class ReflectionHelper
    {
        public static List<Type> GetClassesImplementingInterface(Assembly assembly, Type interfaceType)
        {
            if (!interfaceType.IsInterface) throw new ArgumentException("interfaceType must be an interface");

            return assembly.GetTypes().SelectMany(x => x.GetInterfaces(), (x, y) => new { x, y })
                .Where(z =>
                    z.y.IsGenericType && interfaceType.IsAssignableFrom(z.y.GetGenericTypeDefinition()) ||
                    interfaceType.IsAssignableFrom(z.x))
                .Select(t => t.x).ToList();
        }

        public static Type GetTypeByName(Assembly assembly, string name)
        {
            return assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Contains(name));
        }
    }
}