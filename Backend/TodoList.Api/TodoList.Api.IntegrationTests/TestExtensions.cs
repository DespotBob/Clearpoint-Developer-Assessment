using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace TodoList.Api.IntegrationTests;

public static class TestExtensions
{
    /// <summary>
    /// https://www.damirscorner.com/blog/posts/20220520-ComparingJsonStringsInUnitTests.html
    /// </summary>   
    public static void ShouldBeJsonEquivalent<T>(this string actualJson, T expectedShape, ITestOutputHelper? testOutputHelper = null) where T : class
    {

        var expectedJson = Encoding.UTF8.GetString(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(expectedShape));
        var expectedTokens = JToken.Parse(expectedJson);

        // Deserialized JSON to Object  

        var actualJToken = JToken.Parse(actualJson);
        

        testOutputHelper?.WriteLine("response:");
        testOutputHelper?.WriteLine("");
        testOutputHelper?.WriteLine(actualJson);

        //var expanded = JsonConvert.DeserializeObject<ExpandoObject>(actualJson);
        //dynamic dyn = expanded;


        // Using fluent assertions to compare the objects as it is just better at this sort of thing.
        actualJToken.Should().ContainSubtree(expectedTokens);

    }
}