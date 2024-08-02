﻿using Shouldly;
using System;
using Xunit;

namespace TodoList.Api.UnitTests;

public partial class ContractValidation
{
    public class PutValidatorTests
    {
        public static Contract.PutValidator uut = new TodoList.Api.Contract.PutValidator();

        [Fact]
        public void TodoListValidator_Check01()
        {
            var t = new Contract.TodoItem()
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
            var t = new Contract.TodoItem()
            {
                Description = null,
                Id = Guid.Empty,
                IsCompleted = false
            };

            uut.Validate(t).IsValid.ShouldBe(false);
        }
    }
}
