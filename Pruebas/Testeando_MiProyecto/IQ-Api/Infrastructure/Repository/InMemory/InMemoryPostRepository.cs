using System;
using IQ_Api.Domain.Model;
namespace IQ_Api.Infrastructure.Repository.InMemory;

public class InMemoryPostRepository
{
    public List<Post> Posts { get; set; } = new()
    {
        new Post { PostId = Guid.Parse("aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), Title = "Alice's Post 1", Content = "Content 1" },
        new Post { PostId = Guid.Parse("aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"), Title = "Alice's Post 2", Content = "Content 2" },
        new Post { PostId = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), Title = "Bob's Post 1", Content = "Content 3" },
        new Post { PostId = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"), Title = "Bob's Post 2", Content = "Content 4" },
        new Post { PostId = Guid.Parse("ccccccc1-cccc-cccc-cccc-cccccccccccc"), UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), Title = "Carol's Post 1", Content = "Content 5" },
        new Post { PostId = Guid.Parse("ccccccc2-cccc-cccc-cccc-cccccccccccc"), UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"), Title = "Carol's Post 2", Content = "Content 6" },
        new Post { PostId = Guid.Parse("ddddddd1-dddd-dddd-dddd-dddddddddddd"), UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), Title = "Dave's Post 1", Content = "Content 7" },
        new Post { PostId = Guid.Parse("ddddddd2-dddd-dddd-dddd-dddddddddddd"), UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), Title = "Dave's Post 2", Content = "Content 8" },
        new Post { PostId = Guid.Parse("eeeeeee1-eeee-eeee-eeee-eeeeeeeeeeee"), UserId = Guid.Parse("55555555-5555-5555-5555-555555555555"), Title = "Eve's Post 1", Content = "Content 9" },
        new Post { PostId = Guid.Parse("eeeeeee2-eeee-eeee-eeee-eeeeeeeeeeee"), UserId = Guid.Parse("55555555-5555-5555-5555-555555555555"), Title = "Eve's Post 2", Content = "Content 10" }
    };
}
