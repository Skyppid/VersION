using DryIoc;
using Spectre.Console.Cli;

namespace VersION
{
    internal sealed class TypeResolver : ITypeResolver
    {
        private readonly IContainer _container;

        public TypeResolver(IContainer container)
        {
            _container = container;
        }

        public object? Resolve(Type? type)
        {
            if (type == null) return null;

            try
            {
                return _container.Resolve(type);
            }
            catch
            {
                return null;
            }
        }
    }
}
