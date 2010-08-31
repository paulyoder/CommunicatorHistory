using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicatorHistory;

namespace CommuncatorHistory.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class XmlUtilitiesTest
    {

        [TestMethod]
        public void InnerText_Basic()
        {
            var xml = "<DIV>hello</DIV>";

            var expected = "hello";
            var actual = xml.InnerText(0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InnerText_Attributes()
        {
            var xml = "<DIV style=\"margin:5px;\" id=231kd>hello</DIV>";

            var expected = "hello";
            var actual = xml.InnerText(0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InnerText_ChildElements()
        {
            var xml = "<DIV style=\"margin:5px;\" id=231kd>hello <span id=23>world</span>!</DIV>";

            var expected = "hello world!";
            var actual = xml.InnerText(0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void InnerText_RemovesControlCharacters()
        {
            var xml = "<DIV style=\"margin:5px;\" id=231kd>\r\nhello <span id=23>world</span>!</DIV>";

            var expected = "hello world!";
            var actual = xml.InnerText(0);

            Assert.AreEqual(expected, actual);
        }
    }
}
