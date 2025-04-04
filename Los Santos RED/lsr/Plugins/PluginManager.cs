using Rage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using File = System.IO.File;

namespace LosSantosRED.lsr.Plugins
{
    public static class PluginManager
    {
        public static void SetupPlugins(List<PluginTask> plugins, ModController controller)
        {
            string[] files = Directory.GetFiles("./Plugins/LosSantosRED/Plugins/", "*.dll");
            foreach (var file in files)
            {
                ILSRPlugin plugin = LoadPlugin(Path.Combine(Environment.CurrentDirectory, file));
                if(plugin != null)
                {
                    plugins.Add(new PluginTask(plugin.Interval, plugin.DebugName, plugin.EntryPoint, controller));
                    Game.Console.Print($"Plugin {file} loaded");
                }
                else
                {
                    Game.Console.Print($"Plugin {file} not loaded");
                }
            }
        }
        public static ILSRPlugin LoadPlugin(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            Assembly asm;
            try
            {
                asm = Assembly.LoadFile(file);
            }
            catch (Exception)
            {
                return null;
            }
            Type pluginInfo = null;
            try
            {
                Type[] types = asm.GetTypes();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Game.Console.Print(assembly.GetName().Name);
                }
                Type type = typeof(ILSRPlugin);
                foreach (var t in types)
                    if (type.IsAssignableFrom((Type)t))
                    {
                        Game.Console.Print(t.Name);
                        pluginInfo = t;
                        break;
                    }
                if (pluginInfo != null)
                {
                    object o = Activator.CreateInstance(pluginInfo);
                    ILSRPlugin plugin = (ILSRPlugin)o;
                    return plugin;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
    }
}
