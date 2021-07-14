using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoClient;
using System;
using System.Collections.Generic;
using System.Text;


namespace TenmoClientTests
{
    [TestClass]
    public class MyTestClass
    {
        public Menu menu = new Menu();
        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(9)]
        [DataRow(-50)]
        [DataRow(8)]
        [DataRow(50000000)]
        public void MenuSelectInvalidNumber(int selection)
        {
            Assert.AreEqual("Invalid selection.", menu.MenuSelect(selection));
        }
    }
}