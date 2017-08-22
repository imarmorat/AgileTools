using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgileTools.CommandLine.Commands;

namespace AgileTools.CommandLine.Tests
{
    [TestClass]
    public class CommandBaseTests
    {
        [TestMethod]
        public void CheckParsingOfParameterWorks()
        {
            var input = "${goodVar} ${bad var} {badVar} {bar var} ${goodVar2}";
            var expectedOutput = "replacement ${bad var} {badVar} {bar var} replacement2";

            var varManager = new VariableManager();
            varManager.Set("goodVar", "replacement");
            varManager.Set("goodVar2", "replacement2");

            var output = CommandManager.ParseParameter(input, varManager);

            Assert.AreEqual(expectedOutput, output);
            
        }
    }
}
