using System.Collections.Generic;
using NUnit.Framework;

namespace Swummary.Test
{
    [TestFixture]
    public class TestSUnit
    {

        [Test]
        public void TestSUnitNoReturnType()
        {
            // a.getPatterns(findString)
            var sunit = new SUnit(SUnitType.SingleMethodCall, 
                                    "get", 
                                    "patterns", 
                                    new List<string>(1){ "findString" }, 
                                    null );

            Assert.AreEqual(sunit.action, "get");
            Assert.AreEqual(sunit.theme, "patterns");
            Assert.Contains("findString", (System.Collections.ICollection) sunit.args );

            Assert.IsFalse(sunit.hasReturnType);
        }
        
        [Test]
        public void TestSUnitWithReturnType()
        {
            // string foob = FindAndBreakTwoVerbs("blahblahbreak");
            var sunit = new SUnit(  SUnitType.SingleMethodCall,
                                    "Find and Break",
                                    "Two Verbs",
                                    new List<string>(1) { "blahblahbreak" },
                                    "string"
                                    );

            Assert.AreEqual(sunit.action, "Find and Break");
            Assert.AreEqual(sunit.theme, "Two Verbs");
            Assert.Contains("blahblahbreak", (System.Collections.ICollection)sunit.args);

            Assert.IsTrue(sunit.hasReturnType);
            Assert.AreEqual(sunit.returnType, "string");
        }

    }
}
