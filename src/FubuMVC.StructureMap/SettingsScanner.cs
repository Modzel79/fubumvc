using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap
{
    public class SettingsScanner : IRegistrationConvention
    {
        public static readonly Func<Type, bool> DefaultFilter =
            type => type.Name.EndsWith("Settings") && !type.IsInterface;

        private readonly Func<Type, bool> _filter;

        public SettingsScanner()
            : this(DefaultFilter)
        {
        }

        public SettingsScanner(Func<Type, bool> filter)
        {
            _filter = filter;
        }

        public void Process(Type type, Registry graph)
        {
            if (!_filter(type)) return;

            var instanceType = typeof(SettingsInstance<>).MakeGenericType(type);
            var instance = Activator.CreateInstance(instanceType).As<Instance>();
            graph.For(type).Add(instance).Singleton();
        }
    }

    public class SettingsInstance<T> : LambdaInstance<T> where T : class, new()
    {
        public SettingsInstance() : base("Building {0} from application settings".ToFormat(typeof(T).FullName), c => c.GetInstance<ISettingsProvider>().SettingsFor<T>())
        {
            LifecycleIs<SingletonLifecycle>();
        }
    }
}