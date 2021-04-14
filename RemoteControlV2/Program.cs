using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemoteControlV2.Commands;
using RemoteControlV2.Connection;
using RemoteControlV2.Logging;
using RemoteControlV2.Plugins;
using RemoteControlV2.Properties;

namespace RemoteControlV2
{
    static class Program
    {
        public static Logger Logger;
        //***** PROGRAM *****//
        public static string PluginFolder = "Plugins";
        public static PluginManager Manager { get; set; } = new PluginManager();
        public static Configuration Config { get; set; } = new Configuration();

        //***** UI *****//
        public static NotifyIcon Icon;
        public static ContextMenuStrip SubMenu;
        public static ToolStripMenuItem MainItem;
        public static ToolStripMenuItem ExitItem;

        //***** COMMANDS *****//
        public static List<ICommand> Commands { get; private set; } = new List<ICommand>();

        public static IConnectionMethod Connection;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger = Logger.AllocateLogger();
            Logger.Initialize();
            Logger.Log(LogType.Runtime, LogSeverity.Info, "Application Started");
            Logger.Log(LogType.Runtime, LogSeverity.Debug, "Loading configuration...");
            if (File.Exists("config.json"))
            {
                try
                {
                    Logger.Log(LogType.Runtime, LogSeverity.Debug, "Found configuration file. Loading...");
                    Config = Configuration.FromFile("config.json");
                }
                catch (Exception ex)
                {
                    Logger.Log(LogType.Runtime, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                    Logger.Log(LogType.Runtime, LogSeverity.Trace, ex.StackTrace);
                    Logger.Log(LogType.Runtime, LogSeverity.Fatal, "Could not load configuration file.");
                    Exit(-1);
                }
            }
            else
            {
                try
                {
                    Logger.Log(LogType.Runtime, LogSeverity.Debug, "No configuration file found. Creating it...");
                    Config.Save("config.json");
                }
                catch (Exception ex)
                {
                    Logger.Log(LogType.Runtime, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                    Logger.Log(LogType.Runtime, LogSeverity.Trace, ex.StackTrace);
                    Logger.Log(LogType.Runtime, LogSeverity.Fatal, "Could not create configuration file.");
                    Exit(-1);
                }
            }
            AddStandardCommands();
            Connection = new SerialConnectionMethod(Config.Port, Config.BaudRate);

            Manager.Plugins = EnumeratePlugins();
            Manager.LoadAllPlugins();
            InitializePlugins();
            Logger.Log(LogType.Runtime, LogSeverity.Info, "Plugins loaded and initialized.");

            Connection.Initialize();
            Connection.OnCommandReceived += Connection_OnCommandReceived;
            Logger.Log(LogType.Runtime, LogSeverity.Debug, "Adding notification tray icon...");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Icon = new NotifyIcon();
            Icon.Icon = Resources.Icon;
            Icon.Text = "RemoteControlV2";
            Icon.Visible = Config.ShowIcon;
            SubMenu = new ContextMenuStrip();
            MainItem = new ToolStripMenuItem("Main Window");
            ExitItem = new ToolStripMenuItem("Exit");
            SubMenu.Items.AddRange(new ToolStripMenuItem[] { MainItem, ExitItem });
            Icon.ContextMenuStrip = SubMenu;
            MainItem.Click += MainItem_Click;
            ExitItem.Click += (o, e) => Exit(0);
            Logger.Log(LogType.Runtime, LogSeverity.Debug, "Notification tray icon added.");
            Logger.Log(LogType.Runtime, LogSeverity.Debug, "Starting message loop.");
            Application.Run();
        }

        private static void AddStandardCommands()
        {
            Commands.Add(new SetCommand());
            Commands.Add(new VolumeCommand());
            Commands.Add(new MuteCommand());
        }

        private static void Connection_OnCommandReceived(object sender, CommandEventArgs e)
        {
            Logger.Log(LogType.Commands, LogSeverity.Debug, e.Command);
            try
            {
                string name = e.Command;
                string args = "";
                if (e.Command.Contains(" "))
                {
                    name = e.Command.Remove(e.Command.IndexOf(' ')).Replace(" ", "");
                    args = e.Command.Substring(e.Command.IndexOf(' ') + 1);
                }

                foreach (ICommand c in Commands)
                {
                    if (c.Name.Equals(name))
                    {
                        Manager.OnCommand(c, args.Replace("\n", "").Replace("\r", "").Replace("\t", ""));
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Log(LogType.Commands, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                Logger.Log(LogType.Commands, LogSeverity.Trace, ex.StackTrace);
                Logger.Log(LogType.Commands, LogSeverity.Error, "Error while parsing command.");
            }
        }

        private static void MainItem_Click(object sender, EventArgs e)
        {
            //show main setup window
        }

        /// <summary>
        /// Exits flushing all buffers and resources
        /// </summary>
        /// <param name="code">the exit code</param>
        public static void Exit(int code)
        {
            Logger.Dispose();
            Environment.Exit(code);
        }

        private static List<Plugin> EnumeratePlugins()
        {
            List<Plugin> plugins = new List<Plugin>();

            Logger.Log(LogType.Runtime, LogSeverity.Info, "Loading plugins...");
            if (!Directory.Exists(Utils.GetAbsolutePath(PluginFolder)))
            {
                Logger.Log(LogType.Runtime, LogSeverity.Warning, "The plugin folder isn't present.");
                Logger.Log(LogType.Runtime, LogSeverity.Info, "Creating new plugin folder...");
                try
                {
                    Directory.CreateDirectory(Utils.GetAbsolutePath(PluginFolder));
                    Logger.Log(LogType.Runtime, LogSeverity.Info, "Successfully created plugin directory.");
                }
                catch (Exception ex)
                {
                    Logger.Log(LogType.Runtime, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                    Logger.Log(LogType.Runtime, LogSeverity.Trace, ex.StackTrace);
                    Logger.Log(LogType.Runtime, LogSeverity.Fatal, "Could not create plugin directory.");
                    Exit(-1);
                }
            }

            try
            {
                //check for .json files
                List<string> json = Directory.EnumerateFiles(Utils.GetAbsolutePath(PluginFolder), "*.json").ToList();
                Logger.Log(LogType.Runtime, LogSeverity.Info, $"Found {json.Count} plugin(s).");
                if (json.Count > 0)
                {
                    foreach (string s in json)
                    {
                        Plugin p = Newtonsoft.Json.JsonConvert.DeserializeObject<Plugin>(File.ReadAllText(s));
                        if (p == null)
                        {
                            Logger.Log(LogType.Runtime, LogSeverity.Warning, $"Plugin {Path.GetFileName(s)} does not exist.");
                        }
                        else if (!File.Exists(Utils.GetAbsolutePath(PluginFolder, p.AssemblyName)))
                        {
                            Logger.Log(LogType.Runtime, LogSeverity.Warning, $"Plugin {Path.GetFileName(s)} does not exist.");
                        }
                        else
                        {
                            plugins.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Runtime, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                Logger.Log(LogType.Runtime, LogSeverity.Trace, ex.StackTrace);
                Logger.Log(LogType.Runtime, LogSeverity.Fatal, "Could not load plugins.");
                Exit(-1);
            }
            return plugins;
        }

        private static void InitializePlugins()
        {
            foreach (Plugin p in Manager.Plugins)
            {
                if (p.Loaded && p.Enabled)
                {
                    Logger.Log(LogType.Runtime, LogSeverity.Debug, "Initializing plugin...");
                    try
                    {
                        p.Instance.Initialize();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogType.Runtime, LogSeverity.Debug, $"Exception of type {ex.GetType()}: {ex.Message}");
                        Logger.Log(LogType.Runtime, LogSeverity.Trace, ex.StackTrace);
                        Logger.Log(LogType.Runtime, LogSeverity.Error, "Could not load plugin.");
                        Manager.ReloadPlugin(p);
                    }
                    Logger.Log(LogType.Runtime, LogSeverity.Debug, "Plugin Initialized.");
                }
                else
                {
                    Logger.Log(LogType.Runtime, LogSeverity.Info, "Plugin will not be loaded.");
                }
            }
        }
    }
}
