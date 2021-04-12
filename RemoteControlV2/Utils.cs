using RemoteControlV2.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2
{
    static class Utils
    {
        public static string GetAbsolutePath(params string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    {
                        throw new ArgumentException("The specified arguments aren't valid");
                    }
                case 1:
                    {
                        if (args[0].Equals(""))
                        {
                            return Environment.CurrentDirectory;
                        }
                        return args[0];
                    }
                default:
                    {
                        args = args.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        if (args.Length == 0)
                        {
                            return Environment.CurrentDirectory;
                        }

                        if (args[0].Equals(""))
                        {
                            return Path.GetFullPath(Path.Combine(args.Skip(1).ToArray()));
                        }
                        return Path.GetFullPath(Path.Combine(args));
                    }
            }
        }

        public static bool IsPluginType(Type type)
        {
            return ((!type.IsAbstract) && (!type.IsInterface)) &&
                (typeof(IPlugin).IsAssignableFrom(type) ||
                    type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IPlugin)));
        }
    }
}
