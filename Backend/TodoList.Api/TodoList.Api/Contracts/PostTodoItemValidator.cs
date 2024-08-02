using FluentValidation;
using System;

namespace TodoList.Api.Contract;

public class PostTodoItemValidator : AbstractValidator<PostTodoItem>
{
    public PostTodoItemValidator() : base()
    {
        RuleFor(todoItem => todoItem.Description).NotNull();
        RuleFor(todoItem => todoItem.Description).NotEmpty();
        RuleFor(todoItem => todoItem.Description).Must(d => d != null).WithMessage("Must be supplied.");
        RuleFor(todoItem => todoItem.Description).Must(d => !string.IsNullOrEmpty(d)).WithMessage("Must no be empty.");
    }
}