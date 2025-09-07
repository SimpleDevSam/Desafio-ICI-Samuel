using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.Get;
using Desafio_ICI_Samuel.Domain;
using Desafio_ICI_Samuel.Models;

namespace Tests.Handlers;

public class GetNewsHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly GetNewsHandler _handler;

    public GetNewsHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new GetNewsHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ShouldReturnNull()
    {
        var result = await _handler.Handle(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenNewsExists_ShouldReturnNewsWithTags()
    {
        var tag1 = new Tag { Name = "Tag1" };
        var tag2 = new Tag { Name = "Tag2" };
        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var news = new News
        {
            Title = "Test News",
            Text = "Some text",
            UserId = 1,
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
        result.Text.Should().Be("Some text");
        result.NewsTags.Should().HaveCount(2);
        result.NewsTags.Select(nt => nt.TagId).Should().Contain(new[] { tag1.Id, tag2.Id });
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        var news = new News { Title = "Cancelable News", Text = "Text", UserId = 1 };
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
