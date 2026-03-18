using System;


namespace TodoApi.Domain.Patch;

public class TodoPatchDto
{
    public string? Name { get; set; }
    public bool? IsComplete { get; set; }
}