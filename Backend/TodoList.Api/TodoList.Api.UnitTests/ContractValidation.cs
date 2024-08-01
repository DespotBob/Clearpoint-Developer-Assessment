using Shouldly;
using System;
using Xunit;

namespace TodoList.Api.UnitTests;
public class ContractValidation
{
    public static Contract.TodoItemValidator uut = new TodoList.Api.Contract.TodoItemValidator();

    [Fact]
    public void TodoListValidator_Check01()
    {
        var t = new Contract.TodoItem()
        {
            Description = null,
            Id = Guid.NewGuid().ToString(),
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
        var t = new Contract.TodoItem()
        {
            Description = null,
            Id = null,
            IsCompleted = null
        };

        uut.Validate(t).IsValid.ShouldBe(false);
    }
}