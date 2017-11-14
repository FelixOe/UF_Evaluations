using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UFEvaluations.Tests
{
    [TestClass]
    public class GlobalFunctionsTests
    {
        [TestMethod]
        public void escapeQuerystringElementTest()
        {
            //Arrange
            string testString = "? &,'?, '";

            //Act
            string result = GlobalFunctions.escapeQuerystringElement(testString);

            //Assert
            Assert.AreEqual(result, String.Empty);
        }

        [TestMethod]
        public void createMultipleChartScriptTest()
        {
            //Arrange
            InstructorGraph testGraph = new InstructorGraph();
            testGraph.data = new List<string>();
            List<string> testLabels = new List<string>();

            //Act
            string result = GlobalFunctions.createMultipleChartScript(testGraph,testLabels);

            //Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void createAutoCompleteScriptTest()
        {
            //Arrange

            //Act
            string result = GlobalFunctions.createAutoCompleteScript();

            //Assert
            Assert.AreNotEqual(result, String.Empty);
        }
    }
}
