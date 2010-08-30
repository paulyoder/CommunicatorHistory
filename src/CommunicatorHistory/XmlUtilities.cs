using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public static class XmlUtilities
    {
        public static string InnerText(this string xml, int openBracketIndex)
        {
            var innerText = new StringBuilder();
            var elementName = GetElementName(xml, openBracketIndex);
            var closeElementIndex = GetElementCloseIndex(xml, openBracketIndex, elementName);

            var inElement = false;
            var index = openBracketIndex;
            while (index < closeElementIndex)
            {
                var curChar = xml.Substring(index, 1);

                if (curChar == "<")
                    inElement = true;

                if (!inElement && !Char.IsControl(xml, index))
                    innerText.Append(curChar);

                if (curChar == ">")
                    inElement = false;

                index++;
            }
            var test = innerText.ToString();
            return innerText.ToString();
        }

        private static int GetElementCloseIndex(string xml, int startIndex, string elementName)
        {
            var closeBracket = string.Format("</{0}>", elementName);
            return xml.IndexOf(closeBracket, startIndex);
        }

        private static string GetElementName(string xml, int openBracketIndex)
        {
            var closeBracketIndex = xml.IndexOf(">", openBracketIndex);
            var spaceIndex = xml.IndexOf(" ", openBracketIndex);
            if (spaceIndex != -1 && spaceIndex < closeBracketIndex)
                return xml.Substring(openBracketIndex + 1, spaceIndex - openBracketIndex - 1);
            else
                return xml.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        }
    }
}
