using System;
using System.Collections.Generic;
using System.Linq;
using OpenTextSummarizer;


public static class Summarizer
{
    public static String Summarize(String text) {

        var summaryArgs = new SummarizerArguments();

        summaryArgs.DictionaryLanguage = "en";
        //summaryArgs.DisplayLines = 3;
        summaryArgs.DisplayPercent = 50;
        summaryArgs.InputString = text;
        

        SummarizedDocument doc = OpenTextSummarizer.Summarizer.Summarize(summaryArgs);
        var output = doc.Sentences;
        var summary = string.Concat(output.ToArray());

        return summary;

    }

}
