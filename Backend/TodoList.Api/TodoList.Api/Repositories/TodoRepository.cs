using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Entities;

namespace TodoList.Api.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TodoContext _context;

    public TodoRepository(TodoContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<TodoItem>> GetAllAsync()
        => await _context.TodoItems.Where(x => !x.IsCompleted)
            .AsNoTracking()
            .ToListAsync();


    /// <inheritdoc/>
    public Task<bool> TodoItemIdExists(Guid id)
        => _context.TodoItems
            .AsNoTracking()
            .AnyAsync(x => x.Id == id);

    /// <inheritdoc/>
    public Task<bool> TodoItemDescriptionExists(string description)
        => _context.TodoItems
            .AsNoTracking()
            .AnyAsync(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);

    /// <inheritdoc/>
    public Task<TodoItem> Find(Guid id)
        =>_context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == id);

    /// <inheritdoc/>
    public async Task<TodoItem> FindOrCreateAsync(Guid id)
    {
        var entity = await _context.TodoItems.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
        {
            entity = new TodoItem();
            _context.TodoItems.Add(entity);

            entity.Id = id;
        }

        return entity;
    }

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
    
}
