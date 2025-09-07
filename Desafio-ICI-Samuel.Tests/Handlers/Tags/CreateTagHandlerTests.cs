using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags.Create;
using Desafio_ICI_Samuel.Models;
using Desafio_ICI_Samuel.Data.Interfaces;

namespace Tests.Handlers.Tags;

public class CreateTagHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly CreateTagHandler _handler;

    public CreateTagHandlerTests()
    {
        // Use in-memory database for testing
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new CreateTagHandler(_context);
    }

    [Fact]
    public async Task Handle_WithValidName_ShouldCreateTagAndReturnId()
    {
        // Arrange
        var form = new CreateTagForm { Nome = "TestTag" };

        // Act
        var result = await _handler.Handle(form);

        // Assert
        result.Should().BeGreaterThan(0);

        var createdTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == "TestTag");
        createdTag.Should().NotBeNull();
        createdTag.Name.Should().Be("TestTag");
        createdTag.Id.Should().Be(result);
    }

    [Fact]
    public async Task Handle_WithNameContainingSpaces_ShouldTrimSpaces()
    {
        // Arrange
        var form = new CreateTagForm { Nome = "  TestTag  " };

        // Act
        var result = await _handler.Handle(form);

        // Assert
        var createdTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == result);
        createdTag.Should().NotBeNull();
        createdTag.Name.Should().Be("TestTag");
    }

    [Fact]
    public async Task Handle_WithNullName_ShouldUseEmptyString()
    {
        // Arrange
        var form = new CreateTagForm { Nome = null };

        // Act
        var result = await _handler.Handle(form);

        // Assert
        var createdTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == result);
        createdTag.Should().NotBeNull();
        createdTag.Name.Should().Be(string.Empty);
    }

    [Fact]
    public async Task Handle_WithExistingTag_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingTag = new Tag { Name = "ExistingTag" };
        _context.Tags.Add(existingTag);
        await _context.SaveChangesAsync();

        var form = new CreateTagForm { Nome = "ExistingTag" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(form));
    }

    [Fact]
    public async Task Handle_WithExistingTagDifferentCase_ShouldThrowInvalidOperationException()
    {
        var existingTag = new Tag { Name = "ExistingTag" };
        _context.Tags.Add(existingTag);
        await _context.SaveChangesAsync();

        var form = new CreateTagForm { Nome = "EXISTINGTAG" };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(form));

        exception.Message.Should().Be("Já existe uma tag com esse nome.");
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToDatabase()
    {
        var form = new CreateTagForm { Nome = "TestTag" };
        var cts = new CancellationTokenSource();

        var result = await _handler.Handle(form, cts.Token);

        result.Should().BeGreaterThan(0);

        var createdTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == result);
        createdTag.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCancelledException()
    {
        var form = new CreateTagForm { Nome = "TestTag" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _handler.Handle(form, cts.Token));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ValidTag")]
    [InlineData("Tag With Spaces")]
    [InlineData("Tag123")]
    public async Task Handle_WithVariousValidNames_ShouldCreateSuccessfully(string tagName)
    {
        var form = new CreateTagForm { Nome = tagName };

        var result = await _handler.Handle(form);

        result.Should().BeGreaterThan(0);

        var expectedName = (tagName ?? string.Empty).Trim();
        var createdTag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == result);
        createdTag.Should().NotBeNull();
        createdTag.Name.Should().Be(expectedName);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}