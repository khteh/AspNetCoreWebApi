using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
namespace Web.Api.Core.UnitTests;

public class RegexTests
{
    private readonly ITestOutputHelper _output;
    public RegexTests(ITestOutputHelper output) => _output = output;
    [Theory]
    [InlineData("123", true)]
    [InlineData("Hello World", false)]
    [InlineData("123 ", false)]
    [InlineData(" 123", false)]
    [InlineData("123.", false)]
    [InlineData("123-", false)]
    [InlineData("-123", false)]
    public void NumberRegexTests(string input, bool expected)
    {
        Regex r = new Regex(@"^(\d)+$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("1234567", false)]
    [InlineData("12345678", true)]
    [InlineData("123456789", true)]
    [InlineData("1234567890", true)]
    [InlineData("12345678901", false)]
    public void NumericLengthTests(string input, bool expected)
    {
        Regex r = new Regex(@"^(\d{8,10})$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("Hello World", true)]
    [InlineData("Hello-World", true)]
    [InlineData("Hello_World", true)]
    [InlineData("Hello World 123", true)]
    [InlineData("Hello World!!!", false)]
    [InlineData("Hello World ~!@#$%^&*()_+", false)]
    public void StringRegexTests(string input, bool expected)
    {
        Regex r = new Regex(@"^([\w\d\-_\s])+$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("Hello-Worl", true)]
    [InlineData("HelloWorl8", true)]
    [InlineData("Helo", false)]
    [InlineData("HelloWorl89", false)]
    [InlineData("Hello World ~!@#$%^&*()_+", false)]
    public void StringLengthRegexTests(string input, bool expected)
    {
        Regex r = new Regex(@"^([\w\d\-_\s]){5,10}$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("HelloWorld", true)]
    [InlineData("HelloWorl0", false)] // Should NOT match due to number
    [InlineData("Hello Worl", false)] // Should NOT match due to space
    [InlineData("Hello World", false)]// Should NOT match due to length
    public void LettersRegexTests(string input, bool expected)
    {
        Regex r = new Regex(@"^([a-zA-Z]){0,10}$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("", false)]
    [InlineData(".@.", false)]
    [InlineData("-@-", false)]
    [InlineData("a@", false)]
    [InlineData("ab@de", false)]
    [InlineData("abc@def", false)]
    [InlineData("a@b.c", false)]
    [InlineData("~!@#$%^&*()_+@b.c", false)]
    [InlineData("~!#$%^&*()_+@b.c", false)]
    [InlineData("kokhow.teh@b.c", false)]
    [InlineData("kokhow teh@b.c", false)]
    [InlineData("kok-how_teh@b.c", false)]
    [InlineData("kok-how_teh@b.c.d", false)]
    [InlineData("123@456", false)]
    [InlineData("123@ntu.edu.sg", true)]
    [InlineData("me@gmail.com", true)]
    [InlineData("tan_k_h@gmail.com", true)]
    [InlineData("tan-k.h@gmail.com", true)]
    [InlineData("tan-k.h@gmail.co.uk", true)]
    [InlineData("tan k h@gmail.com", false)]
    public void EmailRegexTest(string input, bool expected)
    {
        Regex r = new Regex(@"^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("1234567", false)] // Invalid due to length < 8
    [InlineData("12345678", true)]
    [InlineData("1234567890", true)]
    [InlineData("12345678901", false)] // Invalid due to length > 10
    [InlineData("+6512345678", true)]
    [InlineData("+65-12345678", true)]
    [InlineData("+ab-91234567", false)]
    [InlineData("+65 91234567", false)] // Invalid due to space
    [InlineData("+123-1234567", false)] // Invalid due to length < 8
    [InlineData("+123-12345678", true)]
    [InlineData("+123-1234567890", true)]
    [InlineData("+123-12345678901", false)] // Invalid due to length > 10
    [InlineData("+-1234567890", false)] // Invalid due to country code < 1
    [InlineData("-1234567890", false)] // Invalid due to country code < 1
    [InlineData("+1234-1234567890", false)] // Invalid due to country code > 3
    [InlineData("+123-HelloWorld", false)]
    [InlineData("+123-", false)]
    public void PhoneNumberRegexTests(string input, bool expected)
    {
        Regex r = new Regex(@"^(\+\d{1,3}\-?)*(\d{8,10})$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("C++", true)]
    [InlineData("C#", true)]
    [InlineData("C+", false)]
    [InlineData("C##", false)]
    [InlineData("C", true)]
    public void C_CPP_CSharpStringRegexTests(string input, bool expected)
    {
        /*
         * https://stackoverflow.com/questions/79435236/how-to-match-c-c-or-c
           The ?: inside the group (?:\+\+|#) just make the group non capturing. The (?<!S) and (?!\S) are called lookarounds, and assert that either whitespace or the start/end precedes/follows the match
           matches to be the entire input string
           matches perhaps as part of a larger string, with the matches surrounded by whitespace
           cpp_csharp_regex = r"\bC(?:\+\+|#)?(?!\S)"
         */
        Regex r = new Regex(@"^C(?:\+\+|#)?$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("is 1 hour.", true)]
    [InlineData("is 2 hours.", true)]
    [InlineData("is 1 minute.", true)]
    [InlineData("is 45 minutes.", true)]
    [InlineData("is 1 hour and 1 minute.", true)]
    [InlineData("is 1 hours and 45 minutes.", true)]
    [InlineData("is 2 hours and 1 minute.", true)]
    [InlineData("is 10 hours and 45 minute.", true)]
    public void TimeStringRegexTests(string input, bool expected)
    {
        Regex r = new Regex(@"\b(\d+)(\s)+(hours?|minutes?)+");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("www.google.com", false)]
    [InlineData("http://www.google.com", true)]
    [InlineData("https://www.google.com", true)]
    [InlineData("127.0.0.1:8080", false)]
    [InlineData("http://127.0.0.1:8080", true)]
    [InlineData("https://127.0.0.1:4433", true)]
    [InlineData("localhost:8080", false)]
    [InlineData("http://localhost:8080", true)]
    [InlineData("https://localhost:4433", true)]
    [InlineData("[::1]:8080", false)]
    [InlineData("http://[::1]:8080", true)]
    [InlineData("https://[::1]:4433", true)]
    public void UriRegexTests(string input, bool expected)
    {
        bool result = Uri.TryCreate(input, UriKind.Absolute, out Uri uri);
        Assert.Equal(expected, result);
        if (result)
        {
            var parameters = QueryHelpers.ParseQuery(uri.Query);
            _output.WriteLine("");
            _output.WriteLine(@$"
                Hostname Type:   {uri.HostNameType}
                Full URL:        {uri.ToString()}
                Host and Port:   {uri.Authority}
                Scheme:          {uri.Scheme}
                Host:            {uri.Host}
                Port:            {uri.Port}
                Query:           {uri.Query}");
            foreach (var param in parameters)
                _output.WriteLine($"Query param: {param.Key}={param.Value}");
        }
    }
    [Theory]
    [InlineData("$", false)]
    [InlineData("$0.50", true)]
    [InlineData("$123.50", true)]
    [InlineData("$123", true)]
    [InlineData("$123,456", true)]
    [InlineData("$123,456,789", true)]
    [InlineData("$123,456,789.50", true)]
    [InlineData("$123.", true)]
    [InlineData("$123,456,789.", true)]
    public void MoneyStringRegexTests(string input, bool expected)
    {
        Regex r = new Regex(@"^\$?(\d*\.\d{1,2})$|^\$?(\d+)$|^\$?(\d+\.?)$|^\$?(\d+)(,\d{3})*$|^\$?(\d+)(,\d{3})*\.?$|^\$?(\d+)(,\d{3})*.\d{1,2}$");
        Assert.Equal(expected, r.IsMatch(input));
    }
    [Theory]
    [InlineData("billed amount of $", false)]
    [InlineData("billed amount of $0.50", true)]
    [InlineData("billed amount of $123.50", true)]
    [InlineData("billed amount of $123", true)]
    [InlineData("billed amount of $123,456", true)]
    [InlineData("billed amount of $123,456,789", true)]
    [InlineData("billed amount of $123,456,789.50", true)]
    [InlineData("billed amount of $123.", true)]
    [InlineData("billed amount of $123,456,789.", true)]
    public void StringContainsMoneyRegexTests(string input, bool expected)
    {
        string regex = @"\b\$?(\d*\.\d{1,2})|\b\$?(\d+)|\b\$?(\d+\.?)|\b\$?(\d{1,3},\d{3})*|\b\$?(\d{1,3},\d{3})*\.?|\b\$?(\d{1,3},\d{3})*\.\d{1,2}";
        MatchCollection matches = Regex.Matches(input, regex);
        bool found = false;
        foreach (Match match in matches)
        {
            _output.WriteLine($"Match '{match.Value}' @ Position {match.Index}");
            found |= !string.IsNullOrEmpty(match.Value);
        }
        Assert.Equal(expected, found);
    }
    [Theory]
    [InlineData("Sky is blue.", new string[] { "Sky", "is", "blue", "." })]
    [InlineData("Leaves are green.", new string[] { "Leaves", "are", "green", "." })]
    [InlineData("Roses are red.", new string[] { "Roses", "are", "red", "." })]
    [InlineData("Very very long sentence...", new string[] { "Very", "very", "long", "sentence", "..." })]
    [InlineData("Last sentence .", new string[] { "Last", "sentence", "." })]
    public void StringPunctuationRegexTests(string input, string[] expected)
    {
        string regex = @"\b(\w+)\b|([\p{P}\p{S}]*)";
        List<string> result = new List<string>();
        MatchCollection matches = Regex.Matches(input, regex);
        foreach (Match match in matches)
            if (!string.IsNullOrEmpty(match.Value) && !result.Contains(match.Value))
            {
                _output.WriteLine($"Match '{match.Value}' @ Position {match.Index}");
                result.Add(match.Value);
            }
        Assert.Equal(expected.ToList<string>(), result);
    }
    [Fact]
    public void LogFormatRegexTest()
    {
        string text = "2025-04-04 05:44:55";
        Assert.Matches(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})?$", text);

        text = " INFO ";
        Assert.Matches(@"^(\s)+INFO(\s)+?$", text);

        text = " Running app...";
        Assert.Matches(@"^(\s)+([\w\d\-_\.\s])+?$", text);

        text = " INFO Running app...";
        Assert.Matches(@"^(\s)+INFO(\s)+([\w\d\-_\.\s])+?$", text);

        text = "2025-04-04 05:44:55 INFO Running app...";
        string regex = @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})(\s)+INFO(\s)+([\w\d\-_\.\s])+?$";
        List<string> result = new List<string>();
        MatchCollection matches = Regex.Matches(text, regex);
        Assert.NotEmpty(matches);
        Assert.Single(matches);
        Assert.Equal(text, matches[0].Value);
    }
}
