using System;
using IndieQuest_Api.Application.Command.Posts;
using IndieQuest_Api.Domain.Model;
using IndieQuest_Api.Domain.Repository;
using IndieQuest_Api.Domain.ValueObject;
using IndieQuest_Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IndieQuest_Api.Application.Command.Posts;

public class UpdatePostCommandHandler
{
    private readonly IPostRepository _postRepository;
    private readonly IndieQuestDbContext _context;

    public UpdatePostCommandHandler(IPostRepository postRepository, IndieQuestDbContext context)
    {
        _postRepository = postRepository;
        _context = context;
    }

    public async Task<Post?> Handle(UpdatePostCommand command)
    {
        var existingPost = await _postRepository.GetPostByIdAsync(command.PostId);
        if (existingPost == null)
        {
            return null;
        }

        existingPost.Title = command.Title;
        existingPost.MediaContent = command.MediaContent;
        existingPost.Description = command.Description;

        await _postRepository.UpdatePostAsync(existingPost);

        // Actualizar tags si es necesario
        if (command.TagIds != null)
        {
            var existingTags = await _context.PostTags
                .Where(pt => pt.PostId == command.PostId)
                .ToListAsync();

            _context.PostTags.RemoveRange(existingTags);

            foreach (var tagId in command.TagIds)
            {
                var postTag = new PostTag
                {
                    PostId = command.PostId,
                    TagId = tagId
                };
                _context.PostTags.Add(postTag);
            }

            await _context.SaveChangesAsync();
        }

        return existingPost;
    }
}
