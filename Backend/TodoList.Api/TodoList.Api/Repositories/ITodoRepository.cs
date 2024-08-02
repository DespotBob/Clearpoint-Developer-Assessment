using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api.Entities;

namespace TodoList.Api.Repositories;

public interface ITodoRepository
{
    /// <summary>
    /// No tracking query to check for a duplicate Description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    /// 
    Task<bool> TodoItemDescriptionExists(string description);

    /// <summary>
    /// No Tracking query to find a TodoItem by Id
    /// </summary>
    Task<bool> TodoItemIdExists(Guid id);

    /// <summary>
    /// Find a TodoItem by Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TodoItem> FindOrCreateAsync(Guid id);

    /// <summary>
    /// No tracking query to get all TodoItems
    /// </summary>
    Task<List<TodoItem>> GetAllAsync();


    /// <summary>
    /// No tracking query to find a TodoItem by Id
    /// </summary>
    public Task<TodoItem> Find(Guid id);


    Task SaveChangesAsync();
}