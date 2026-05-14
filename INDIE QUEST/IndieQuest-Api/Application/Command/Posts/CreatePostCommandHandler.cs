using System;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Domain.ValueObject;
using IndieQuest_Api.Infrastructure;

namespace IndieQuest_Api.Application.Command.Posts;

public class CreatePostCommandHandler
{
    private readonly IPostRepository _postRepository;
    private readonly IndieQuestDbContext _context;

    public CreatePostCommandHandler(IPostRepository postRepository, IndieQuestDbContext context)
    {
        _postRepository = postRepository;
        _context = context;
    }

    public async Task Handle(CreatePostCommand command)
    {
        var post = new Post
        {
            Title = command.Title,
            MediaContent = command.MediaContent,
            Description = command.Description,
            CreationDate = DateTime.UtcNow
        };

        // Crear el post primero
        await _postRepository.CreatePostAsync(post);

        // Crear la relación UserPost (Makes_MadeBy)
        var userPost = new UserPost
        {
            UserId = command.UserId,
            PostId = post.PostId
        };
        _context.UserPosts.Add(userPost);

        // Agregar tags si es necesario
        if (command.TagIds != null && command.TagIds.Length > 0)
        {
            foreach (var tagId in command.TagIds)
            {
                var postTag = new PostTag
                {
                    PostId = post.PostId,
                    TagId = tagId
                };
                _context.PostTags.Add(postTag);
            }
        }

        await _context.SaveChangesAsync();
    }
}
