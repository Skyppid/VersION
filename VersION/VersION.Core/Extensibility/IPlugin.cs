using DryIoc;

namespace VersION.Core.Extensibility
{
    /// <summary>   Interface for plugins extending VersIONs base functionality.    </summary>
    public interface IPlugin
    {
        string Name { get; }

        Task Initialize(IContainer container);
    }
}
