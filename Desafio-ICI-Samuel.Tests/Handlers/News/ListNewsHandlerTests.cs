using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.ListNews;
using Desafio_ICI_Samuel.Domain;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Desafio_ICI_Samuel.Models;

namespace Tests.Handlers;

public class ListNewsHandlerTests : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly AppDbContext _context;
    private readonly ListNewsHandler _handler;

    public ListNewsHandlerTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(_options);
        _handler = new ListNewsHandler(_context);
    }

    [Fact]
    public async Task Handle_WhenNoNews_ShouldReturnEmptyList()
    {
        var query = new ListNewsQuery { Page = 1, PageSize = 10 };

        var result = await _handler.Handle(query);

        result.Total.Should().Be(0);
        result.Rows.Should().BeEmpty();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedNewsWithTagsAndUserName()
    {
        var user = new User { Name = "Author" };
        _context.Users.Add(user);

        var tag1 = new Tag { Name = "Tag1" };
        var tag2 = new Tag { Name = "Tag2" };
        _context.Tags.AddRange(tag1, tag2);
        await _context.SaveChangesAsync();

        var news1 = new News { Title = "News 1", Text = "Text 1", UserId = user.Id };
        var news2 = new News { Title = "News 2", Text = "Text 2", UserId = user.Id };
        _context.News.AddRange(news1, news2);
        await _context.SaveChangesAsync();

        _context.NewsTags.AddRange(
            new NewsTag { NewsId = news1.Id, TagId = tag1.Id },
            new NewsTag { NewsId = news1.Id, TagId = tag2.Id },
            new NewsTag { NewsId = news2.Id, TagId = tag2.Id }
        );
        await _context.SaveChangesAsync();

        var query = new ListNewsQuery { Page = 1, PageSize = 10 };
        var result = await _handler.Handle(query);

        result.Total.Should().Be(2);
        result.Rows.Should().HaveCount(2);

        var first = result.Rows.First();
        first.Id.Should().Be(news2.Id);
        first.Title.Should().Be("News 2");
        first.UserName.Should().Be("Author");
        first.TagsName.Should().Be("Tag2");
        first.Text.Should().Be("Text 2");

        var second = result.Rows.Last();
        second.Id.Should().Be(news1.Id);
        second.Title.Should().Be("News 1");
        second.UserName.Should().Be("Author");
        second.TagsName.Should().Be("Tag2,Tag1");
        second.Text.Should().Be("Text 1");
    }

    [Fact]
    public async Task Handle_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        var query = new ListNewsQuery { Page = 1, PageSize = 10 };
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
