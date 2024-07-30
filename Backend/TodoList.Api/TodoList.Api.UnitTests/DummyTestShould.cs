using Shouldly;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class DummyTestShould
    {
        [Fact]
        public void Test_Shouldly_IsInstalled()
        {
            true.ShouldBe(true);
        }
    }
}
