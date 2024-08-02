using Shouldly;
using Xunit;

namespace TodoList.Api.UnitTests;

public partial class ContractValidation
{
    public class PostValidatorTests
    {
        public static Contract.PostTodoItemValidator uut = new TodoList.Api.Contract.PostTodoItemValidator();

        [Fact]
        public void TodoListValidator_Check01()
        {
            var t = new Contract.PostTodoItem()
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
            var t = new Contract.PostTodoItem()
            {
                Description = "",
            };

            uut.Validate(t).IsValid.ShouldBe(false);
        }
    }
}
