using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.Delete;
using Desafio_ICI_Samuel.Domain;
using System.Threading;

namespace Tests.Handlers;

public class DeleteNewsHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly DeleteNewsHandler _handler;

    public DeleteNewsHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new DeleteNewsHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ShouldThrowException()
    {
        var act = async () => await _handler.Handle(999);

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Error when deleting news, News was not found");
    }

    [Fact]
    public async Task Handle_WhenNewsExists_ShouldDeleteSuccessfully()
    {
        var news = new News { Title = "Test News", Text = "Some text", UserId = 1 };
        _context.News.Add(news);
        await _context.SaveChangesAsync();

        await _handler.Handle(news.Id);

        var deleted = await _context.News.FindAsync(news.Id);
        deleted.Should().BeNull();
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
