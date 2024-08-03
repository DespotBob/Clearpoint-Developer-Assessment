using Shouldly;
using System;
using TodoList.Api.OpenApiContracts;
using Xunit;

namespace TodoList.Api.UnitTests;

public partial class ContractValidation
{
    public class PutValidatorTests
    {
        public static PutValidator uut = new PutValidator();

        [Fact]
        public void TodoListValidator_Check01()
        {
            var t = new TodoItem()
            {
                Description = null,
                Id = Guid.NewGuid(),
                IsCompleted = true
            };

            var results = uut.Validate(t);

            results.ShouldSatisfyAllConditions(
                r => r.IsValid.ShouldBe(false),
                r => r.Errors.ShouldContain(e => e.PropertyName == "Description"));
        }

        [Fact]
        public void TodoListValidator_Check02()
        {
            var t = new TodoItem()
            {
                Description = null,
                Id = Guid.Empty,
                IsCompleted = false
            };

            uut.Validate(t).IsValid.ShouldBe(false);
        }
    }
}
