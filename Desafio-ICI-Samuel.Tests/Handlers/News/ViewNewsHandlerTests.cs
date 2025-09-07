using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.ListNews;
using Desafio_ICI_Samuel.Features.News.View;
using Desafio_ICI_Samuel.Domain;
using System.Linq;
using System.Threading;
using Desafio_ICI_Samuel.Models;

namespace Tests.Handlers;

public class ViewNewsHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly ViewNewsHandler _handler;

    public ViewNewsHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new ViewNewsHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ShouldReturnNull()
    {
        var result = await _handler.Handle(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenNewsExists_ShouldReturnNewsRowWithTagsAndUserName()
    {
        var user = new User { Name = "Author" };
        _context.Users.Add(user);

        var tag1 = new Tag { Name = "Tag1" };
        var tag2 = new Tag { Name = "Tag2" };
        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var news = new News
        {
            Title = "Test News",
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

        var result = await _handler.Handle(news.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(news.Id);
        result.Title.Should().Be("Test News");
        result.UserName.Should().Be("Author");
        result.TagsName.Should().Be("Tag2,Tag1");
        result.Text.Should().Be("Some text");
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        var user = new User { Name = "Author" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var news = new News { Title = "Cancelable News", Text = "Text", UserId = user.Id };
        _context.News.Add(news);
        await _context.SaveChangesAsync();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await _handler.Handle(news.Id, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
