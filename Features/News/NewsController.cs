using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.News.Delete;
using Desafio_ICI_Samuel.Features.News.Edit;
using Desafio_ICI_Samuel.Features.News.Form;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features;


[Route("news")]
[AutoValidateAntiforgeryToken]
public class NewsController : Controller
{
    private readonly AppDbContext _db;
    private readonly ICreateNewsHandler _createHandler;
    private readonly IEditNewsHandler _editHandler;
    private readonly IDeleteNewsHandler _deleteHandler;
    private readonly IFormBuilder _formBuilder;
    public NewsController(AppDbContext db, ICreateNewsHandler createHandler, IEditNewsHandler editHandler, IDeleteNewsHandler deleteHandler, IFormBuilder formBuilder)
    {
        _db = db;
        _createHandler = createHandler;
        _editHandler = editHandler;
        _deleteHandler = deleteHandler;
        _formBuilder = formBuilder;
    }

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
        => PartialView("_CreateEdit", await _formBuilder.Build());

    [HttpPost("create")]
    public async Task<IActionResult> Create(NewsForm vm)
    {
        if (!ModelState.IsValid) return BadRequest(Errors());
        try
        {
            await _createHandler.Handle(vm);
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

        var vm = await _formBuilder.Build(n);
        return PartialView("_CreateEdit", vm);
    }

    [HttpPost("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, NewsForm vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(Errors());

        try
        {
            await _editHandler.Handle(vm);
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
        try
        {
          await _deleteHandler.Handle(id);
          TempData["msg"] = "News deleted successfully.";
        }
        catch (Exception ex)
        {
          TempData["err"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
    private object Errors() => ModelState.ToDictionary(
        k => k.Key,
        v => v.Value!.Errors.Select(e => e.ErrorMessage)
    );
}
