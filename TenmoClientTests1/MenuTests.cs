using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoClient;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClientTests1.TestUser;

namespace TenmoClient.Tests
{
    [TestClass()]
    public class MenuTests
    {
        public Menu menu;
        public TestUser user;
        [TestInitialize]
        public void Setup()
        {
            user = new TestUser();
            menu = new Menu(user, new TestInput("1"));
        }
        [TestMethod()]
        public void MenuSelect1Test()
        {
            Assert.AreEqual(2, menu.MenuSelect1(user.UserId).Count);
        }

        [TestMethod()]
        public void MenuSelect3Test()
        {
            Assert.Fail();
        }
    }
}