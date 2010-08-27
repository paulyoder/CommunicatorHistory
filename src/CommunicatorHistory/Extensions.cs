using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public static class Extensions
    {
        public static string Char(this string value, int index)
        {
            return value.Substring(index, 1);
        }
    }
}
