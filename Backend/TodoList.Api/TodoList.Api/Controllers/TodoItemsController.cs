using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TodoList.Api.Repositories;

namespace TodoList.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly ILogger<TodoItemsController> _logger;
    private readonly ITodoRepository _todoRepository;

    private static readonly Contract.PutValidator putValidator = new();
    private static readonly Contract.PostTodoItemValidator postValidator = new();

    public TodoItemsController(ITodoRepository todoRepository, ILogger<TodoItemsController> logger)
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    // GET: api/TodoItems/...
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetTodoItem(Guid id)
    {
        var result = await _todoRepository.Find(id);

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
    /// // POST: api/TodoItems
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Contract.ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contract.TodoItem), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostTodoItem(Contract.PostTodoItem todoItem)
    {
        var result = postValidator.Validate(todoItem);

        if( result.IsValid == false)
        {
            return result.ToBadResult();
        }

        if (await _todoRepository.TodoItemDescriptionExists(todoItem.Description))
        {
            // TODO: Add ITodoRepository to TodoItemValidator... So format is the same
            // for all bad request responses.

            return BadRequest(new Contract.ErrorResponse()
            {
                Errors = new System.Collections.Generic.List<Contract.Error>()
                {
                    new Contract.Error()
                    {
                        PropertyName = nameof(Contract.PostTodoItem.Description),
                        ErrorMessage = "A Todo item with that name already exists"
                    }
                }
            });
        }

        var todoItemId = Guid.NewGuid();

        var entity = await _todoRepository.FindOrCreateAsync(todoItemId);

        entity.Description = todoItem.Description;
        entity.IsCompleted = false;
        entity.Id = todoItemId;

        await _todoRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItemId }, new Contract.TodoItem
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
    [ProducesResponseType(typeof(Contract.TodoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contract.ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutTodoItem(Guid id, Contract.TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest("Id in URL parameters does not match Id in Body.");
        }

        var result = putValidator.Validate(todoItem);

        if (result.IsValid == false)
        {
            return result.ToBadResult();
        }

        var entity = await _todoRepository.Find(id);

        if (entity == null)
        {
            return NotFound();
        }

        try
        {
            entity.IsCompleted = todoItem.IsCompleted;
            entity.Description = todoItem.Description;

            await _todoRepository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Ok(todoItem);
    }

    /// <summary>
    /// Post: api/TodoItems/{guid}/markascomplete
    /// </summary>
    [HttpPost("{id:Guid}/markascomplete")]
    [ProducesResponseType(typeof(Contract.TodoItem), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsComplete(Guid id)
    {
        var entity = await _todoRepository.Find(id);

        if(entity==null)
        {
            return NotFound();
        }

        try
        {
            entity.IsCompleted = true;

            await _todoRepository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Ok(new Contract.TodoItem {
            Id = entity.Id,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted
        });
    }
}
