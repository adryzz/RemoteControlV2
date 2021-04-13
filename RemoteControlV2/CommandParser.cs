using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteControlV2
{
    static class CommandParser
    {

        public static bool? BooleanParser(string argument)
        {
            switch (argument)
            {
                case "0":
                case "off":
                case "false":
                    {
                        return false;
                    }
                case "1":
                case "on":
                case "true":
                    {
                        return true;
                    }
            }
            return null;
        }

        public static byte? ByteParser(string v)
        {
            byte b = 0x00;
            if (byte.TryParse(v, out b))
            {
                return b;
            }
            return null;
        }

        public static int? Int32Parser(string v)
        {
            int b = 0;
            if (int.TryParse(v, out b))
            {
                return b;
            }
            return null;
        }

        public static long? Int64Parser(string v)
        {
            long b = 0;
            if (long.TryParse(v, out b))
            {
                return b;
            }
            return null;
        }

        public static float? SingleParser(string v)
        {
            float b = 0;
            if (float.TryParse(v, out b))
            {
                return b;
            }
            return null;
        }

        public static double? DoubleParser(string v)
        {
            double b = 0;
            if (double.TryParse(v, out b))
            {
                return b;
            }
            return null;
        }
    }
}
