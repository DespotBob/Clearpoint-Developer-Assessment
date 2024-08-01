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

    private static readonly Contract.TodoItemValidator validationRules = new();

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

    // GET: api/TodoItems
    [HttpGet]
    public async Task<IActionResult> GetTodoItems()
    {
        var results = await _todoRepository.GetAllAsync();

        return Ok(results);
    }

    // POST: api/TodoItems
    [HttpPost]
    public async Task<IActionResult> PostTodoItem(Contract.TodoItem todoItem)
    {
        var result = validationRules.Validate(todoItem);

        if( result.IsValid == false)
        {
            return result.ToBadResult();
        }

        if (await _todoRepository.TodoItemDescriptionExists(todoItem.Description))
        {
            // TODO: Add ITodoRepository to TodoItemValidator... So format is the same
            // for all bad request reponses.
            return BadRequest("Description already exists");
        }

         Guid.TryParse(todoItem.Id, out var todoItemId);

        var entity = await _todoRepository.FindOrCreateAsync(todoItemId);

        entity.Description = todoItem.Description;
        entity.IsCompleted = (bool) todoItem.IsCompleted;
        entity.Id = todoItemId;

        await _todoRepository.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }

    // PUT: api/TodoItems/...
    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> PutTodoItem(Guid id, Contract.TodoItem todoItem)
    {
        Guid.TryParse(todoItem.Id, out var todoItemId);

        if (id != todoItemId)
        {
            return BadRequest("Id in URL parameters does not match Id in Body.");
        }

        var result = validationRules.Validate(todoItem);

        if (result.IsValid == false)
        {
            return result.ToBadResult();
        }

        var entity = await _todoRepository.FindOrCreateAsync(id);

        try
        {
            entity.Description = todoItem.Description;
            entity.IsCompleted = (bool)todoItem.IsCompleted;

            await _todoRepository.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return Ok(todoItem);
    }
}
