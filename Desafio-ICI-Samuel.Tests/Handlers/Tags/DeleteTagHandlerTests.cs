using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags.Delete;
using Desafio_ICI_Samuel.Models;
using Desafio_ICI_Samuel.Domain;

namespace Tests.Handlers.Tags;

public class DeleteTagHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly DeleteTagHandler _handler;

    public DeleteTagHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new DeleteTagHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var act = async () => await _handler.Handle(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Tag não encontrada.");
    }

    [Fact]
    public async Task Handle_WhenTagIsUsedByNews_ShouldThrowInvalidOperationException()
    {
        var tag = new Tag { Name = "TagInUse" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var news = new News { Title = "Some news" };
        _context.News.Add(news);
        await _context.SaveChangesAsync();

        var newsTag = new NewsTag { NewsId = news.Id, TagId = tag.Id };
        _context.NewsTags.Add(newsTag);
        await _context.SaveChangesAsync();

        var act = async () => await _handler.Handle(tag.Id);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WhenTagExistsAndNotUsed_ShouldDeleteSuccessfully()
    {
        var tag = new Tag { Name = "DeletableTag" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        await _handler.Handle(tag.Id);

        var deleted = await _context.Tags.FindAsync(tag.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        var tag = new Tag { Name = "CancelableTag" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await _handler.Handle(tag.Id, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
