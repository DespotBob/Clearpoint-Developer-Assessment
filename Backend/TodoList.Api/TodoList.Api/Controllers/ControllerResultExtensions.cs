using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TodoList.Api.Controllers;

public static class ControllerResultExtensions
{
    public static BadRequestObjectResult ToBadResult(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .Select(e => new { e.PropertyName, e.ErrorMessage })
            .ToList();

        return new BadRequestObjectResult(errors);
    }
    
}