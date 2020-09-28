using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fan;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NuGet.Frameworks;

namespace Fan.Tests
{
    [TestClass()]
    public class FanOutputTests
    {
        private FanOutput _test;

        [TestInitialize]
        public void beforeTest()
        {
            _test = new FanOutput(1, "ja", 25, 60);
        }

        [TestMethod()]
        public void nameTest()
        {
            Assert.AreEqual("ja", _test.Name);

            try
            {
                _test.Name = "j";
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Name should be at least 2 characters", e.Message);
            }

        }

        [TestMethod()]
        public void CheckTempTest()
        {
            Assert.AreEqual(25, _test.Temp);

            try
            {
                _test.Temp = 10;
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Temperature should be between 15 and 25", e.Message);
            }
        }

        [TestMethod()]
        public void CheckFugtTest()
        {
            Assert.AreEqual(60, _test.Fugt);

            try
            {
                _test.Fugt = 10;
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Humidity should be between 30 and 80", e.Message);
            }
        }
    }
}