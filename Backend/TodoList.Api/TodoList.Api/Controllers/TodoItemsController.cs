using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TodoList.Api.Repositories;
using UuidExtensions;

namespace TodoList.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private static readonly OpenApiContracts.PostTodoItemValidator postValidator = new();
    private static readonly OpenApiContracts.PutValidator putValidator = new();
    private readonly ILogger<TodoItemsController> _logger;
    private readonly ITodoRepository _todoRepository;

    public TodoItemsController(ITodoRepository todoRepository, ILogger<TodoItemsController> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    // GET: api/TodoItems/...
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetTodoItem(Guid id)
    {
        var result = await _todoRepository.FindAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// GET: api/TodoItems
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTodoItems()
    {
        var results = await _todoRepository.GetAllAsync();

        return Ok(results);
    }

    /// <summary>
    /// Post: api/TodoItems/{guid}/markascomplete
    /// </summary>
    [HttpPost("{id:Guid}/markascomplete")]
    [ProducesResponseType(typeof(OpenApiContracts.TodoItem), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsComplete(Guid id)
    {
        var entity = await _todoRepository.FindAsync(id);

        if (entity == null)
        {
            return NotFound();
        }

        entity.IsCompleted = true;

        await _todoRepository.SaveChangesAsync();

        return Ok(new OpenApiContracts.TodoItem
        {
            Id = entity.Id,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted
        });
    }

    /// <summary>
    /// // POST: api/TodoItems
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OpenApiContracts.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OpenApiContracts.TodoItem), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostTodoItem(OpenApiContracts.PostTodoItem todoItem)
    {
        var result = postValidator.Validate(todoItem);

        if (result.IsValid == false)
        {
            return result.ToBadResult();
        }

        if (await _todoRepository.TodoItemDescriptionExistsAsync(todoItem.Description))
        {
            return BadRequest(new OpenApiContracts.ErrorResponse()
            {
                Errors = new System.Collections.Generic.List<OpenApiContracts.Error>()
                {
                    new OpenApiContracts.Error()
                    {
                        PropertyName = nameof(OpenApiContracts.PostTodoItem.Description),
                        ErrorMessage = "A Todo item with that name already exists"
                    }
                }
            });
        }

        // Was a bit disappointed to find the new UUID in dotnet 9 is not available in prerelease 9.6
        var todoItemId = Uuid7.Guid();

        var entity = await _todoRepository.FindOrCreateAsync(todoItemId);

        entity.Description = todoItem.Description;
        entity.IsCompleted = false;
        entity.Id = todoItemId;

        await _todoRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItemId }, new OpenApiContracts.TodoItem
        {
            Id = todoItemId,
            Description = todoItem.Description,
            IsCompleted = false
        });
    }

    /// <summary>
    /// PUT: api/TodoItems/...
    /// </summary>
    [HttpPut("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OpenApiContracts.TodoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OpenApiContracts.ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutTodoItem(Guid id, OpenApiContracts.TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            // Todo: This should be moved to a short method somewhere....
            return BadRequest(new OpenApiContracts.ErrorResponse()
            {
                Errors = new System.Collections.Generic.List<OpenApiContracts.Error>()
                {
                    new OpenApiContracts.Error()
                    {
                        PropertyName = nameof(OpenApiContracts.TodoItem.Id),
                        ErrorMessage = "Id in URL parameters does not match Id in Body."
                    }
                }
            });
        }

        var result = putValidator.Validate(todoItem);

        if (result.IsValid == false)
        {
            return result.ToBadResult();
        }

        var entity = await _todoRepository.FindAsync(id);

        if (entity == null)
        {
            return NotFound();
        }

        entity.IsCompleted = todoItem.IsCompleted;
        entity.Description = todoItem.Description;

        await _todoRepository.SaveChangesAsync();

        return Ok(todoItem);
    }
}