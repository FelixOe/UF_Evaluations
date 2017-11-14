using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UFEvaluations;
using UFEvaluations.Controllers;
using Newtonsoft.Json;

namespace UFEvaluations.Tests.Controllers
{
    [TestClass]
    public class SearchControllerTest
    {
        [TestMethod]
        public void SearchTest()
        {
            //Arrange
            SearchController controller = new SearchController();
            string testString = "a";

            try
            {
                //Act
                controller.Search(testString);
            }
            catch (Exception ex)
            {
                //Assert
                Assert.IsTrue(ex is FormatException);
            }
        }

        [TestMethod]
        public void AutoCompleteSearchTest()
        {
            //Arrange
            SearchController controller = new SearchController();
            string testString = "a";

            //Act
            var result = controller.AutoCompleteSearch(testString);

            //Assert
            Assert.IsInstanceOfType(result, typeof(System.Web.Mvc.JsonResult));
        }
    }
}
