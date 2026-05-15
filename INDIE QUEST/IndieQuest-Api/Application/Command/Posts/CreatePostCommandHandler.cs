using System;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Domain.ValueObject;
using IndieQuest_Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Post> Handle(CreatePostCommand command)
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

        // Si el post tiene contenido multimedia, crear su carpeta y actualizar la ruta
        if (!string.IsNullOrEmpty(post.MediaContent))
        {
            var postFolder = $"IndieQuest-LocalData/postdata/{post.PostId}";
            Directory.CreateDirectory(postFolder);
            post.MediaContent = $"{postFolder}/{post.MediaContent}";
            _context.Posts.Update(post);
        }

        // Crear la relación UserPost (Makes_MadeBy)
        var userPost = new UserPost
        {
            UserId = command.UserId,
            PostId = post.PostId
        };
        _context.UserPosts.Add(userPost);

        // Agregar tags por ID si se proporcionan
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

        // Agregar tags por nombre (find-or-create)
        if (command.TagNames != null && command.TagNames.Length > 0)
        {
            foreach (var tagName in command.TagNames)
            {
                var trimmed = tagName.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                var existingTag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.tagName.ToLower() == trimmed.ToLower());

                if (existingTag == null)
                {
                    existingTag = new Tag { tagName = trimmed };
                    _context.Tags.Add(existingTag);
                    await _context.SaveChangesAsync();
                }

                _context.PostTags.Add(new PostTag
                {
                    PostId = post.PostId,
                    TagId = existingTag.tagId
                });
            }
        }

        await _context.SaveChangesAsync();

        return post;
    }
}
