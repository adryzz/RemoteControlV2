using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using RemoteControlV2.Logging;
using System.IO;

namespace RemoteControlV2.Plugins
{
    public class PluginManager
    {
        public List<Plugin> Plugins = new List<Plugin>();

        public void LoadAllPlugins()
        {
            foreach (Plugin p in Plugins)
            {
                if (!p.Loaded)
                {
                    LoadPlugin(p);
                }
            }
        }

        private void LoadPlugin(Plugin p)
        {
            try
            {
                p.Assembly = Assembly.LoadFile(Utils.GetAbsolutePath(Program.PluginFolder, p.AssemblyName));
                foreach (Type t in p.Assembly.GetExportedTypes())
                {
                    if (Utils.IsPluginType(t))
                    {
                        p.PluginType = t;
                    }
                    else
                    {
                        Program.Logger.Log(LogType.Runtime, LogSeverity.Debug, $"Type {t.FullName} does not implement IPlugin interface");
                    }
                }
                if (p.PluginType == null)
                {
                    Program.Logger.Log(LogType.Runtime, LogSeverity.Error, $"No plugin code detected. The plugin will not get loaded.");
                    return;
                }
                p.Instance = (IPlugin)Activator.CreateInstance(p.PluginType);
                p.Loaded = true;
            }
            catch (Exception ex)
            {
                Program.Logger.Log(LogType.Runtime, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                Program.Logger.Log(LogType.Runtime, LogSeverity.Error, "Could not load plugin.");
            }
        }

        public void OnCommand(ICommand command, string arguments)
        {
            Program.Logger.Log(LogType.Runtime, LogSeverity.Trace, "OnCommand");
            if (Program.Config.ForceCommandsOnNewThread)
            {
                if (command.Enabled)
                {
                    new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            command.Execute(arguments);
                        }
                        catch (Exception ex)
                        {
                            Program.Logger.Log(LogType.Commands, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                            Program.Logger.Log(LogType.Commands, LogSeverity.Error, "Unhandled exception.");
                                //ReloadPlugin(p);
                        }
                    })).Start();
                }
            }
            else
            {
                try
                {
                    if (command.Enabled)
                    {
                        command.Execute(arguments);
                    }
                }
                catch (Exception ex)
                {
                    Program.Logger.Log(LogType.Commands, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                    Program.Logger.Log(LogType.Commands, LogSeverity.Error, "Unhandled exception.");
                    //ReloadPlugin(p);
                }
            }
        }

        public void ReloadPlugin(Plugin p)
        {
            Program.Logger.Log(LogType.Runtime, LogSeverity.Info, "Reloading plugin...");
            p.Instance.Dispose();
            p.Instance = null;
            p.Instance = (IPlugin)Activator.CreateInstance(p.PluginType);
            try
            {
                p.Instance.Initialize();
            }
            catch (Exception ex)
            {
                Program.Logger.Log(LogType.Commands, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                Program.Logger.Log(LogType.Commands, LogSeverity.Error, "Could not load plugin.");

                p.UnhandledExceptions++;
                if (Program.Config.RestartOnUnhandledException && p.UnhandledExceptions < Program.Config.MaxUnhandledExceptions)
                {
                    ReloadPlugin(p);
                }
                else
                {
                    Program.Logger.Log(LogType.Runtime, LogSeverity.Warning, "The plugin will not be loaded.");
                    p.Instance.Dispose();
                    p.Loaded = false;
                }
            }
        }
    }
}
