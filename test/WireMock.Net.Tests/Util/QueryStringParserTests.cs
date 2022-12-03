using FluentAssertions;
using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.Util;

public class QueryStringParserTests
{
    [Fact]
    public void Parse_WithNullString()
    {
        // Assign
        string? query = null;

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Should().Equal(new Dictionary<string, WireMockList<string>>());
    }

    [Fact]
    public void Parse_WithEmptyString()
    {
        // Assign
        string query = "";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Should().Equal(new Dictionary<string, WireMockList<string>>());
    }

    [Fact]
    public void Parse_WithQuestionMark()
    {
        // Assign
        string query = "?";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Should().Equal(new Dictionary<string, WireMockList<string>>());
    }

    [Fact]
    public void Parse_With1Param()
    {
        // Assign
        string query = "?key=bla/blub.xml";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("bla/blub.xml"));
    }

    [Fact]
    public void Parse_With2Params()
    {
        // Assign
        string query = "?x=1&y=2";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(2);
        result["x"].Should().Equal(new WireMockList<string>("1"));
        result["y"].Should().Equal(new WireMockList<string>("2"));
    }

    [Fact]
    public void Parse_With1ParamNoValue()
    {
        // Assign
        string query = "?empty";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["empty"].Should().Equal(new WireMockList<string>());
    }

    [Fact]
    public void Parse_With1ParamNoValueWithEqualSign()
    {
        // Assign
        string query = "?empty=";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["empty"].Should().Equal(new WireMockList<string>());
    }

    [Fact]
    public void Parse_With1ParamAndJustAndSign()
    {
        // Assign
        string query = "?key=1&";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("1"));
    }

    [Fact]
    public void Parse_With2ParamsAndWhereOneHasAQuestion()
    {
        // Assign
        string query = "?key=value?&b=c";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(2);
        result["key"].Should().Equal(new WireMockList<string>("value?"));
        result["b"].Should().Equal(new WireMockList<string>("c"));
    }

    [Fact]
    public void Parse_With1ParamWithEqualSign()
    {
        // Assign
        string query = "?key=value=what";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("value=what"));
    }

    [Fact]
    public void Parse_With1ParamWithTwoEqualSigns()
    {
        // Assign
        string query = "?key=value==what";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("value==what"));
    }

    [Fact]
    public void Parse_WithMultipleParamWithSameKeySeparatedBySemiColon()
    {
        // Assign
        string query = "?key=value;key=anotherValue";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("value", "anotherValue"));
    }

    [Fact]
    public void Parse_WithMultipleParamWithSameKeySeparatedByAmp()
    {
        // Assign
        string query = "?key=1&key=2";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("1", "2"));
    }

    [Fact]
    public void Parse_With1ParamContainingComma_When_SupportMultiValueUsingComma_Is_True()
    {
        // Assign
        string query = "?key=1,2,3";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("1", "2", "3"));
    }

    [Fact]
    public void Parse_With1ParamContainingCommaAndAmpCombined_When_SupportMultiValueUsingComma_Is_Comma()
    {
        // Assign
        string query = "?key=1,2&key=3";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("1", "2", "3"));
    }

    [Fact]
    public void Parse_With1ParamContainingComma_SupportMultiValueUsingComma_Is_AmpersandAndSemiColon()
    {
        // Assign
        string query = "?$filter=startswith(name,'testName')";

        // Act
        var result = QueryStringParser.Parse(query, QueryParameterMultipleValueSupport.AmpersandAndSemiColon);

        // Assert
        result.Count.Should().Be(1);
        result["$filter"].Should().Equal(new WireMockList<string>("startswith(name,'testName')"));
    }

    [Fact]
    public void Parse_With1ParamContainingEscapedAnd()
    {
        // Assign
        string query = "?winkel=C%26A";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["winkel"].Should().Equal(new WireMockList<string>("C&A"));
    }

    [Fact]
    public void Parse_With1ParamContainingParentheses()
    {
        // Assign
        string query = "?Transaction=(123)";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["Transaction"].Should().Equal(new WireMockList<string>("(123)"));
    }

    [Fact]
    public void Parse_WithMultipleParamWithSameKey()
    {
        // Assign
        string query = "?key=value&key=anotherValue";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["key"].Should().Equal(new WireMockList<string>("value", "anotherValue"));
    }

    [Fact]
    public void Parse_With1ParamContainingSpacesSingleQuoteAndEqualSign()
    {
        // Assign
        string query = "?q=SELECT Id from User where username='user@gmail.com'";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(1);
        result["q"].Should().Equal(new WireMockList<string>("SELECT Id from User where username='user@gmail.com'"));
    }

    // Issue #849
    [Fact]
    public void Parse_With1ParamContainingComma_Using_QueryParameterMultipleValueSupport_NoComma()
    {
        // Assign
        string query = "?query=SELECT id, value FROM table WHERE id = 1&test=42";

        // Act
        var result = QueryStringParser.Parse(query, QueryParameterMultipleValueSupport.NoComma);

        // Assert
        result.Count.Should().Be(2);
        result["query"].Should().Equal(new WireMockList<string>("SELECT id, value FROM table WHERE id = 1"));
        result["test"].Should().Equal(new WireMockList<string>("42"));
    }

    [Fact]
    public void Parse_WithComplex()
    {
        // Assign
        string query = "?q=energy+edge&rls=com.microsoft:en-au&ie=UTF-8&oe=UTF-8&startIndex=&startPage=1%22";

        // Act
        var result = QueryStringParser.Parse(query);

        // Assert
        result.Count.Should().Be(6);
        result["q"].Should().Equal(new WireMockList<string>("energy edge"));
        result["rls"].Should().Equal(new WireMockList<string>("com.microsoft:en-au"));
        result["ie"].Should().Equal(new WireMockList<string>("UTF-8"));
        result["oe"].Should().Equal(new WireMockList<string>("UTF-8"));
        result["startIndex"].Should().Equal(new WireMockList<string>());
        result["startPage"].Should().Equal(new WireMockList<string>("1\""));
    }
}