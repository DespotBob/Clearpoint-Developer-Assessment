using FluentValidation;
using System;

namespace TodoList.Api.Contract;

public class PostTodoItemValidator : AbstractValidator<PostTodoItem>
{
    public PostTodoItemValidator() : base()
    {
        RuleFor(todoItem => todoItem.Description).NotNull();
      
        When(TodoItem => TodoItem.Description != null, () =>
        {
            RuleFor(todoItem => todoItem.Description).NotEmpty();
        });

    }
}