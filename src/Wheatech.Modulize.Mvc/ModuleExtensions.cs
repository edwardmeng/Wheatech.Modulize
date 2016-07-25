using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wheatech.Modulize.Mvc
{
    internal static class ModuleExtensions
    {
        private static bool TypeIsPublicClass(Type type)
        {
            return type != null && type.IsPublic && type.IsClass && !type.IsAbstract;
        }

        internal static IEnumerable<Type> FilterTypesInModule(this ModuleDescriptor module, Predicate<Type> predicate)
        {
            IEnumerable<Type> types = Type.EmptyTypes;
            foreach (var assembly in module.Assemblies)
            {
                Type[] assemblyTypes;
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    assemblyTypes = ex.Types;
                }
                types = types.Concat(assemblyTypes);
            }
            return from type in types
                   where TypeIsPublicClass(type) && predicate(type)
                   select type;
        }
    }
}
