using Desafio_ICI_Samuel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features;


[Route("news")]
[AutoValidateAntiforgeryToken]
public class NewsController : Controller
{
    private readonly AppDbContext _db;
    public NewsController(AppDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var rows = await _db.News
            .AsNoTracking()
            .OrderByDescending(n => n.Id)
            .Select(n => new Row(n.Id, n.Title, n.UserId))
            .ToListAsync();

        return View("Index", rows);
    }
    public record Row(int Id, string Title, int UserId);

    [HttpGet("create")]
    public async Task<IActionResult> Create()
        => PartialView("_CreateEdit", await BuildForm());

    [HttpPost("create")]
    public async Task<IActionResult> Create(NewsForm vm, [FromServices] CreateNewsHandler handler)
    {
        if (!ModelState.IsValid) return BadRequest(Errors());
        try
        {
            var id = await handler.Handle(vm);
            return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return BadRequest(Errors());
        }
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var n = await _db.News
            .Include(x => x.NewsTags)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (n is null) return NotFound();

        var vm = await BuildForm(n);
        return PartialView("_CreateEdit", vm);
    }

    [HttpPost("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, NewsForm vm, [FromServices] EditNewsHandler handler)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(Errors());

        try
        {
            await handler.Handle(vm);
            return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return BadRequest(Errors());
        }
    }

    [HttpPost("{id:int}/delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var n = await _db.News.FirstOrDefaultAsync(x => x.Id == id);
        if (n is null) return NotFound();

        _db.News.Remove(n);
        await _db.SaveChangesAsync();
        TempData["msg"] = "News deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
    private async Task<NewsForm> BuildForm(Domain.News? n = null)
    {
        var users = await _db.Users
            .OrderBy(u => u.Name)
            .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name })
            .ToListAsync();

        var tags = await _db.Tags
            .OrderBy(t => t.Nome) // Tag entity may still use Portuguese property
            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nome })
            .ToListAsync();

        return new NewsForm
        {
            Id = n?.Id,
            Title = n?.Title ?? "",
            Text = n?.Text ?? "",
            UserId = n?.UserId,
            TagIds = n?.NewsTags.Select(nt => nt.TagId).ToArray() ?? Array.Empty<int>(),
            AvailableUsers = users,
            AvailableTags = tags
        };
    }
    private object Errors() => ModelState.ToDictionary(
        k => k.Key,
        v => v.Value!.Errors.Select(e => e.ErrorMessage)
    );
}
