using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags.Edit;
using Desafio_ICI_Samuel.Models;

namespace Tests.Handlers.Tags;

public class EditTagHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly EditTagHandler _handler;

    public EditTagHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new EditTagHandler(_context);
    }

    [Fact]
    public async Task Handle_WithValidUpdate_ShouldChangeTagName()
    {
        var tag = new Tag { Name = "OldName" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var form = new EditTagForm { Id = tag.Id, Name = "NewName" };

        await _handler.Handle(form);

        var updated = await _context.Tags.FindAsync(tag.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("NewName");
    }

    [Fact]
    public async Task Handle_WithSpacesInName_ShouldTrimBeforeSaving()
    {
        var tag = new Tag { Name = "OldName" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var form = new EditTagForm { Id = tag.Id, Name = "   TrimmedName   " };

        await _handler.Handle(form);

        var updated = await _context.Tags.FindAsync(tag.Id);
        updated!.Name.Should().Be("TrimmedName");
    }

    [Fact]
    public async Task Handle_WithNullName_ShouldSaveEmptyString()
    {
        var tag = new Tag { Name = "OldName" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var form = new EditTagForm { Id = tag.Id, Name = null };

        await _handler.Handle(form);

        var updated = await _context.Tags.FindAsync(tag.Id);
        updated!.Name.Should().Be(string.Empty);
    }

    [Fact]
    public async Task Handle_WhenTagNotFound_ShouldThrowKeyNotFoundException()
    {
        var form = new EditTagForm { Id = 9999, Name = "NewName" };

        var act = async () => await _handler.Handle(form);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Tag não encontrada.");
    }

    [Fact]
    public async Task Handle_WhenDuplicateName_ShouldThrowInvalidOperationException()
    {
        var tag1 = new Tag { Name = "Original" };
        var tag2 = new Tag { Name = "Other" };
        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var form = new EditTagForm { Id = tag2.Id, Name = "Original" };

        var act = async () => await _handler.Handle(form);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe uma tag com esse nome.");
    }

    [Fact]
    public async Task Handle_WhenDuplicateNameDifferentCase_ShouldThrowInvalidOperationException()
    {
        var tag1 = new Tag { Name = "Original" };
        var tag2 = new Tag { Name = "Other" };
        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var form = new EditTagForm { Id = tag2.Id, Name = "ORIGINAL" };

        var act = async () => await _handler.Handle(form);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        var tag = new Tag { Name = "OldName" };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        var form = new EditTagForm { Id = tag.Id, Name = "NewName" };

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
