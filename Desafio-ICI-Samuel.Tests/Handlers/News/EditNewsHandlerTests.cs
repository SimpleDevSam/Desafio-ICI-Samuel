using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.Edit;
using Desafio_ICI_Samuel.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Desafio_ICI_Samuel.Features;
using Desafio_ICI_Samuel.Models;

namespace Tests.Handlers;

public class EditNewsHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly EditNewsHandler _handler;

    public EditNewsHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new EditNewsHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var form = new NewsForm { Id = 999, Title = "Title", Text = "Text", UserId = 1, TagIds = new int[0] };

        var act = async () => await _handler.Handle(form);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("News not found.");
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var news = new News { Title = "Old Title", Text = "Old Text", UserId = 1 };
        _context.News.Add(news);
        await _context.SaveChangesAsync();

        var form = new NewsForm { Id = news.Id, Title = "New Title", Text = "New Text", UserId = 999, TagIds = new int[0] };

        var act = async () => await _handler.Handle(form);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Author not found.");
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldUpdateNewsAndTags()
    {
        var user = new User { Name = "Author" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var tag1 = new Tag { Name = "Tag1" };
        var tag2 = new Tag { Name = "Tag2" };
        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var news = new News { Title = "Old Title", Text = "Old Text", UserId = user.Id };
        _context.News.Add(news);
        await _context.SaveChangesAsync();

        var form = new NewsForm
        {
            Id = news.Id,
            Title = "  New Title  ",
            Text = "  New Text  ",
            UserId = user.Id,
            TagIds = new int[] { tag1.Id, tag2.Id }
        };

        await _handler.Handle(form);

        var updatedNews = await _context.News.Include(n => n.NewsTags).FirstOrDefaultAsync(n => n.Id == news.Id);
        updatedNews.Should().NotBeNull();
        updatedNews!.Title.Should().Be("New Title");
        updatedNews.Text.Should().Be("New Text");
        updatedNews.UserId.Should().Be(user.Id);
        updatedNews.NewsTags.Should().HaveCount(2);
        updatedNews.NewsTags.Select(nt => nt.TagId).Should().Contain(new[] { tag1.Id, tag2.Id });
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        var user = new User { Name = "Author" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var news = new News { Title = "Old Title", Text = "Old Text", UserId = user.Id };
        _context.News.Add(news);
        await _context.SaveChangesAsync();

        var form = new NewsForm { Id = news.Id, Title = "New Title", Text = "New Text", UserId = user.Id, TagIds = new int[0] };
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
