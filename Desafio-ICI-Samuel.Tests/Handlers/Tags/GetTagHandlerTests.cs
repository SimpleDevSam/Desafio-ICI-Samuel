using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags.Get;
using Desafio_ICI_Samuel.Models;

namespace Tests.Handlers.Tags;

public class GetTagHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly GetTagHandler _handler;

    public GetTagHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new GetTagHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var act = async () => await _handler.Handle(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Tag not found.");
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldReturnTagVm()
    {
        var tag = new Tag { Name = "ExistingTag" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var result = await _handler.Handle(tag.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(tag.Id);
        result.Name.Should().Be("ExistingTag");
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
