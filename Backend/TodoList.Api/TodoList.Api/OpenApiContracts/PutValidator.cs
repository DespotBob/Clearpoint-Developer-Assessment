using FluentValidation;

namespace TodoList.Api.OpenApiContracts;

public class PutValidator : AbstractValidator<TodoItem>
{
    public PutValidator()
    {
        RuleFor(todoItem => todoItem.Id).NotNull();
        RuleFor(todoItem => todoItem.Id).NotEmpty();
        RuleFor(todoItem => todoItem.Id).NotNull();

        RuleFor(todoItem => todoItem.Description).NotNull();
        RuleFor(todoItem => todoItem.Description).NotEmpty();
        RuleFor(todoItem => todoItem.Description).NotNull();
        RuleFor(todoItem => todoItem.Description).NotEmpty();
    }
}
