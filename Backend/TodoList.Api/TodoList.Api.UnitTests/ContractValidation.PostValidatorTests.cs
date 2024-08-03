using Shouldly;
using Xunit;

using TodoList.Api.OpenApiContracts;

namespace TodoList.Api.UnitTests;

public partial class ContractValidation
{
    public class PostValidatorTests
    {
        public static PostTodoItemValidator uut = new PostTodoItemValidator();

        [Fact]
        public void TodoListValidator_Check01()
        {
            var t = new PostTodoItem()
            {
                Description = null,
            };

            var results = uut.Validate(t);

            results.ShouldSatisfyAllConditions(
                r => r.IsValid.ShouldBe(false),
                r => r.Errors.ShouldContain(e => e.PropertyName == "Description"));
        }

        [Fact]
        public void TodoListValidator_Check02()
        {
            var t = new PostTodoItem()
            {
                Description = "",
            };

            uut.Validate(t).IsValid.ShouldBe(false);
        }
    }
}
