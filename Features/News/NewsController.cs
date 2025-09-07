using System.Runtime.CompilerServices;
using Desafio_ICI_Samuel.Data.Interfaces;
using Desafio_ICI_Samuel.Features.News.Delete;
using Desafio_ICI_Samuel.Features.News.Edit;
using Desafio_ICI_Samuel.Features.News.Form;
using Desafio_ICI_Samuel.Features.News.Get;
using Desafio_ICI_Samuel.Features.News.ListNews;
using Desafio_ICI_Samuel.Features.News.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Features.News;


[Route("news")]
[AutoValidateAntiforgeryToken]
public class NewsController : Controller
{
    private readonly IAppDbContext _db;
    private readonly ICreateNewsHandler _createHandler;
    private readonly IEditNewsHandler _editHandler;
    private readonly IDeleteNewsHandler _deleteHandler;
    private readonly IFormBuilder _formBuilder;
    private readonly IGetNewsHandler _getHandler;
    private readonly IListNewsHandler _listHandler;
    private readonly IViewNewsHandler _viewHandler;
    public NewsController(
        IAppDbContext db,
        ICreateNewsHandler createHandler,
        IEditNewsHandler editHandler,
        IDeleteNewsHandler deleteHandler,
        IFormBuilder formBuilder,
        IGetNewsHandler getHandler,
        IListNewsHandler listHandler,
        IViewNewsHandler viewHandler)
    {
        _db = db;
        _createHandler = createHandler;
        _editHandler = editHandler;
        _deleteHandler = deleteHandler;
        _formBuilder = formBuilder;
        _getHandler = getHandler;
        _listHandler = listHandler;
        _viewHandler = viewHandler;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] int page = 1)
    {
        return View("Index", await _listHandler.Handle(new ListNewsQuery(page, 5)));
    }

    [HttpGet("{id}/view")]
    public async Task<IActionResult> View(int id)
    {
        var news = await _viewHandler.Handle(id);

        if (news == null)
        {
            return NotFound();
        }
           
        return PartialView("_View", news);
    }

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
        var news = await _getHandler.Handle(id);

        if (news is null)
        {
            return NotFound();
        }

        var vm = await _formBuilder.Build(news);
        return PartialView("_CreateEdit", vm);
    }

    [HttpPost("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, NewsForm vm)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(Errors());
        }

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
          TempData["msg"] = "Notícia deletada com sucesso.";
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
