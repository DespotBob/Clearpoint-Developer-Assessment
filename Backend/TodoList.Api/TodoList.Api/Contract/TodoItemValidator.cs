using FluentValidation;
using System;

namespace TodoList.Api.Contract;

public class TodoItemValidator : AbstractValidator<TodoItem>
{
    public TodoItemValidator()
    {
        
        RuleFor(todoItem => todoItem.Description).NotNull();
        RuleFor(todoItem => todoItem.Description).NotEmpty();
        RuleFor(todoItem => todoItem.Description).Must(d => d != null).WithMessage("Must be supplied.");
        RuleFor(todoItem => todoItem.Description).Must(d => !string.IsNullOrEmpty(d)).WithMessage("Must no be empty.");

        RuleFor(todoItem => todoItem.Id).NotNull().WithMessage("Must be supplied");
        RuleFor(todoItem => todoItem.Id).NotEmpty().WithMessage("Must be supplied");
        RuleFor(todoItem => todoItem.Id).Must(g =>  Guid.TryParse(g, out var _)).WithMessage("Must be a valid Guid");
    }
}