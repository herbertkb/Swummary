using System;
using NUnit.Framework;
using Swummary;
using System.Collections.Generic;

[TestFixture]
public class TestTextGenerator
{

    [TestCase]
    public void TestTextGenerationForSingleMethodCall() {

        // addUser( username, password);
        SUnit sunit = new SUnit(SUnitType.SingleMethodCall, 
                                "add", 
                                "user",
                                null, 
                                new List<string> { "username", "password" },
                                "void");

        var sentence = TextGenerator.GenerateText(sunit);

        Assert.AreEqual("Add user given username and password", sentence);
         
    }

}