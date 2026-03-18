using System;
using IQ_Api.Domain.Model;

namespace IQ_Api.Infrastructure.Repository.InMemory;

public class InMemoryUserRepository
{
    public List<User> Users { get; set; } = new()
    {
        new User { UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), Username = "alice", Password = "pass1", AvailableForWork = true },
        new User { UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), Username = "bob", Password = "pass2", AvailableForWork = false },
        new User { UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), Username = "carol", Password = "pass3", AvailableForWork = true },
        new User { UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), Username = "dave", Password = "pass4", AvailableForWork = false },
        new User { UserId = Guid.Parse("55555555-5555-5555-5555-555555555555"), Username = "eve", Password = "pass5", AvailableForWork = true }
    };
}
