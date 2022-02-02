using System.Runtime.Loader;
using VersION.Core.Extensibility;

namespace VersION
{
    internal static class PluginOrchestrator
    {
        private static readonly AssemblyLoadContext Context = new("PluginContext");

        internal static async IAsyncEnumerable<Type> SearchPlugins(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            if (!di.Exists)
                throw new DirectoryNotFoundException($"Directory '{directory}' does not exist.");

            var files = await Task.Run(() => di.GetFiles("VersION.Plugin.*.dll", SearchOption.AllDirectories));
            foreach (var pluginFile in files)
            {
                var assembly = await Task.Run(() => Context.LoadFromAssemblyPath(pluginFile.FullName));
                foreach (var type in assembly.GetExportedTypes().Where(t => t.IsAssignableTo(typeof(IPlugin))))
                {
                    yield return type;
                }
            }
        }
    }
}
