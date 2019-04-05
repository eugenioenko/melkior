using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MelkiorTests
{
    [TestClass]
    public class BinaryExpressions
    {
        [TestMethod]
        public void FirstTest()
        {
            var source = "10 == 10;";
           var result = Melkior.Melkior.Execute(source);
            Assert.AreEqual(1, 1);
        }
    }
}
