using System;
using TodoApi.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Infrastructure.Repository.Database;

class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }

    public DbSet<Todo> Todos => Set<Todo>();
}
