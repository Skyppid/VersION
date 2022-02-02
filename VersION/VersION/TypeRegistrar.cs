using DryIoc;
using Spectre.Console.Cli;

namespace VersION
{
    internal sealed class TypeRegistrar : ITypeRegistrar
    {
        private readonly IContainer _container;

        public TypeRegistrar(IContainer container)
        {
            _container = container;
        }

        public void Register(Type service, Type implementation)
        {
            _container.Register(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            _container.UseInstance(service, implementation);
        }

        public void RegisterLazy(Type service, Func<object> factory)
        {
            _container.Register(service, new DelegateFactory(context => factory(), new SingletonReuse()));
        }

        public ITypeResolver Build()
        {
            return new TypeResolver(_container.WithNoMoreRegistrationAllowed());
        }
    }
}
