using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Scripting.CSharp;

namespace RoslynRx.Tests
{
    [TestClass]
    public class BasicRoslyn
    {
        [TestMethod]
        public void HelloWorld()
        {
            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            Console.SetOut(writer);
            string expected = "Hello World!  I'm Roslyn!";

            StupidExecute(string.Format("System.Console.WriteLine(\"{0}\");", expected));
            sb.ToString().Should().Be(expected + Environment.NewLine);
        }

        private static void StupidExecute(string code)
        {
            new ScriptEngine().CreateSession().Execute(code);
        }
    }
}
