using Shouldly;
using Xunit;

namespace TodoList.Api.UnitTests;
public class DummyTestShouldly
{
    [Fact]
    public void TestShouldlyIsInstalled()
    {
        true.ShouldBe(true);
    }
}
