using System;

namespace TodoList.Api.OpenApiContracts;

public class TodoItem
{
    public string Description { get; set; }
    public Guid Id { get; set; }
    public bool IsCompleted { get; set; }
}