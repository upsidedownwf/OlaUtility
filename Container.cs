using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OlamideIOCContainer
{
    public sealed class Container
    {
        private readonly Dictionary<Type, Type> types = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, Type> instances = new Dictionary<Type, Type>();

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            types[typeof(TInterface)] = typeof(TImplementation);
        }
        //public void RegisterSingleton<TService>(Func<TService> instanceCreator)
        //{
        //    var lazy = new Lazy<TService>(instanceCreator);
        //    this.Register<TService>(() => lazy.Value);
        //}
        public TInterface Create<TInterface>()
        {
            var result = Create(typeof(TInterface));
            return (TInterface)result;
        }

        private object Create(Type type)
        {
            //Find a default constructor using reflection
            var concreteType = types[type];
            ConstructorInfo defaultConstructor = concreteType.GetConstructors()[0];
            //Verify if the default constructor requires params
            var defaultParams = defaultConstructor.GetParameters();
            //Instantiate all constructor parameters using recursion
            var parameters = defaultParams.Select(param => Create(param.ParameterType)).ToArray();

            return defaultConstructor.Invoke(parameters);
        }

    }
}