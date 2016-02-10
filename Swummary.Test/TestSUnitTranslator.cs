using System;
using NUnit.Framework;

[TestFixture]
public class TestSUnitTranslator
{

    // create dummy XElement objects of several C# or C++ statements


    [TestCase]
    public void TranslateSameActionSUnit() { Assert.AreEqual(1, 2, "Not implemented."); }

    [TestCase]
    public void TranslateVoidReturnSUnit() { Assert.AreEqual(1, 2, "Not implemented."); }

    [TestCase]
    public void TranslateEndingSUnit() { Assert.AreEqual(1, 2, "Not implemented."); }

}