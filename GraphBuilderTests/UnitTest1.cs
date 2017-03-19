using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dopple;
using Mono.Cecil;
using System.Linq;
using System.Collections.Generic;
using Dopple.InstructionNodes;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace GraphBuilderTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSimpleAdd()
        {
            HelperFuncs.TestFunction("SimpleAdd");
        }


        [TestMethod]
        public void TestSimpleForLoop()
        {
            HelperFuncs.TestFunction("SimpleForLoop");
        }


        [TestMethod]
        public void TestSimpleWhileLoop()
        {
            HelperFuncs.TestFunction("SimpleWhile");
        }

        [TestMethod]
        public void TestConditionsSimple()
        {
            HelperFuncs.TestFunction("ConpoundConditions");
        }
    }
}
