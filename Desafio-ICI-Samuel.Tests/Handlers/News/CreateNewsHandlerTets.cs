using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Models;
using Desafio_ICI_Samuel.Features;
using Desafio_ICI_Samuel.Features.News.Create;

namespace Tests.Handlers;

public class CreateNewsHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly CreateNewsHandler _handler;

    public CreateNewsHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new CreateNewsHandler(_context);
    }

    [Fact]
    public async Task Handle_WithValidNews_ShouldCreateNewsAndTags()
    {
        var tag1 = new Tag { Name = "Tag1" };
        var tag2 = new Tag { Name = "Tag2" };
        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var form = new NewsForm
        {
            Title = "  My News  ",
            Text = "  Some text  ",
            UserId = 1,
            TagIds = new int[] { tag1.Id, tag2.Id }
        };

        var result = await _handler.Handle(form);

        result.Should().BeGreaterThan(0);

        var createdNews = await _context.News.FindAsync(result);
        createdNews.Should().NotBeNull();
        createdNews!.Title.Should().Be("My News");
        createdNews.Text.Should().Be("Some text");
        createdNews.UserId.Should().Be(1);

        var newsTags = _context.NewsTags.Where(nt => nt.NewsId == createdNews.Id).ToList();
        newsTags.Should().HaveCount(2);
        newsTags.Select(nt => nt.TagId).Should().Contain(new[] { tag1.Id, tag2.Id });
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        var tag = new Tag { Name = "Tag" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var form = new NewsForm
        {
            Title = "News",
            Text = "Text",
            UserId = 1,
            TagIds = new int[] { tag.Id}
        };

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await _handler.Handle(form, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
