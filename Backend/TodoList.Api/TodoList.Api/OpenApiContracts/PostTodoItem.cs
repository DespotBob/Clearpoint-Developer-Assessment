using Microsoft.Build.Framework;

namespace TodoList.Api.OpenApiContracts;

public class PostTodoItem
{
    [Required]
    public string Description { get; set; }
}
