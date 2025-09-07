using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags.List;
using Desafio_ICI_Samuel.Models;
using Desafio_ICI_Samuel.Domain;
using Desafio_ICI_Samuel.Features.Tags;

namespace Tests.Handlers.Tags;

public class ListTagsHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly ListTagsHandler _handler;

    public ListTagsHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new ListTagsHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenNoTagsExist_ShouldReturnEmptyList()
    {
        var query = new ListTagsQuery { Page = 1, PageSize = 10 };

        var result = await _handler.Handle(query);

        result.Total.Should().Be(0);
        result.Rows.Should().BeEmpty();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WhenTagsExist_ShouldReturnPaginatedList()
    {
        for (int i = 1; i <= 15; i++)
        {
            _context.Tags.Add(new Tag { Name = $"Tag{i}" });
        }
        await _context.SaveChangesAsync();

        var query = new ListTagsQuery { Page = 2, PageSize = 5 };

        var result = await _handler.Handle(query);

        result.Total.Should().Be(15);
        result.Rows.Should().HaveCount(5);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);

        result.Rows.Select(r => r.Name).Should().Contain("Tag6");
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        _context.Tags.Add(new Tag { Name = "CancelableTag" });
        await _context.SaveChangesAsync();

        var query = new ListTagsQuery { Page = 1, PageSize = 10 };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await _handler.Handle(query, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
