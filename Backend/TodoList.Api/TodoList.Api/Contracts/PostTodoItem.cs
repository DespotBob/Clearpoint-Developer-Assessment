using Microsoft.Build.Framework;

namespace TodoList.Api.Contract;

public class PostTodoItem
{
    [Required]
    public string Description { get; set; }
}
