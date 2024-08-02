using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace TodoList.Api;

public static class ControllerResultExtensions
{
    public static BadRequestObjectResult ToBadResult(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors
            .Select(e => new Contract.Error { PropertyName = e.PropertyName, ErrorMessage= e.ErrorMessage})
            .ToList();

        return new BadRequestObjectResult(new Contract.ErrorResponse()
        {
            Errors = errors
        });
    }

    
}