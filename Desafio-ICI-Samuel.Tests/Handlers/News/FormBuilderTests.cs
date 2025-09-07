using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.Form;
using Desafio_ICI_Samuel.Domain;
using System.Linq;
using Desafio_ICI_Samuel.Models;

namespace Tests.Handlers;

public class FormBuilderTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly FormBuilder _builder;

    public FormBuilderTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _builder = new FormBuilder(_context);
    }

    [Fact]
    public async Task Build_WhenNoNewsProvided_ShouldReturnEmptyFormWithUsersAndTags()
    {
        var user1 = new User { Name = "Alice" };
        var user2 = new User { Name = "Bob" };
        _context.Users.AddRange(user1, user2);

        var tag1 = new Tag { Name = "TagA" };
        var tag2 = new Tag { Name = "TagB" };
        _context.Tags.AddRange(tag1, tag2);

        await _context.SaveChangesAsync();

        var result = await _builder.Build();

        result.Id.Should().BeNull();
        result.Title.Should().BeEmpty();
        result.Text.Should().BeEmpty();
        result.UserId.Should().BeNull();
        result.TagIds.Should().BeEmpty();

        result.AvailableUsers.Should().HaveCount(2);
        result.AvailableUsers.Select(u => u.Text).Should().ContainInOrder("Alice", "Bob");

        result.AvailableTags.Should().HaveCount(2);
        result.AvailableTags.Select(t => t.Text).Should().ContainInOrder("TagA", "TagB");
    }

    [Fact]
    public async Task Build_WhenNewsProvided_ShouldPopulateFormFields()
    {
        var user = new User { Name = "Alice" };
        _context.Users.Add(user);

        var tag1 = new Tag { Name = "Tag1" };
        var tag2 = new Tag { Name = "Tag2" };
        _context.Tags.AddRange(tag1, tag2);

        await _context.SaveChangesAsync();

        var news = new News
        {
            Title = "My News",
            Text = "Some text",
            UserId = user.Id,
            NewsTags = new[]
            {
                new NewsTag { TagId = tag1.Id },
                new NewsTag { TagId = tag2.Id }
            }
        };
        _context.News.Add(news);
        await _context.SaveChangesAsync();

        var result = await _builder.Build(news);

        result.Id.Should().Be(news.Id);
        result.Title.Should().Be("My News");
        result.Text.Should().Be("Some text");
        result.UserId.Should().Be(user.Id);
        result.TagIds.Should().BeEquivalentTo(new[] { tag1.Id, tag2.Id });

        result.AvailableUsers.Select(u => u.Text).Should().Contain("Alice");
        result.AvailableTags.Select(t => t.Text).Should().Contain("Tag1", "Tag2");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
