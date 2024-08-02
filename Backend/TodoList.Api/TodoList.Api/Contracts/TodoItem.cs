using System;

namespace TodoList.Api.Contract;

public class TodoItem
{
    public string Description { get; set; }
    public Guid Id { get; set; }
    public bool IsCompleted { get; set; }
}