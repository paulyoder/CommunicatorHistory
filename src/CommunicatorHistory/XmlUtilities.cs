using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicatorHistory
{
    public class XmlUtilities : IXmlUtilities
    {
        public string InnerText(string xml, int openBracketIndex)
        {
            var innerText = new StringBuilder();
            var elementName = GetElementName(xml, openBracketIndex);
            var closeElementIndex = GetElementCloseIndex(xml, openBracketIndex, elementName);

            var inElement = false;
            var index = openBracketIndex;
            while (index < closeElementIndex)
            {
                if (xml.Char(index) == "<")
                    inElement = true;

                if (!inElement)
                    innerText.Append(xml.Substring(index, 1));

                if (xml.Char(index) == ">")
                    inElement = false;

                index++;
            }

            return innerText.ToString();
        }

        private int GetElementCloseIndex(string xml, int startIndex, string elementName)
        {
            var closeBracket = string.Format("</{0}>", elementName);
            return xml.IndexOf(closeBracket, startIndex);
        }

        private string GetElementName(string xml, int openBracketIndex)
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
