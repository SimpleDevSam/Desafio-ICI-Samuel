using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags;
using Desafio_ICI_Samuel.Features.Tags.Create;
using Desafio_ICI_Samuel.Features.Tags.Delete;
using Desafio_ICI_Samuel.Features.Tags.Edit;
using Desafio_ICI_Samuel.Features.Tags.Get;
using Desafio_ICI_Samuel.Features.Tags.List;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desafio_ICI_Samuel.Controllers;

[Route("tags")]
[AutoValidateAntiforgeryToken]
public class TagsController : Controller
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, [FromServices] ViewTagHandler handler)
    {
        try
        {
            var vm = await handler.Handle(id);
            return View("View", vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id:int}/edit")]
    public async Task<IActionResult> Edit(int id, [FromServices] AppDbContext db)
    {
        var t = await db.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (t is null)
        {
            return NotFound();
        }

        var vm = new EditTagForm { Id = t.Id, Nome = t.Nome };
        return View("Edit", vm);
    }

    [HttpPost("{id:int}/edit")]
    public async Task<IActionResult> Edit( int id, EditTagForm vm,  [FromServices] EditTagHandler handler)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Edit", vm);
        }

        try
        {
            await handler.Handle(vm);
            TempData["msg"] = "Tag atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException dup)
        {
            ModelState.AddModelError(nameof(vm.Nome), dup.Message);
            return View("Edit", vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> Index( [FromServices] ListTagsHandler handler, [FromQuery] int page = 1, [FromQuery] string? search = null)
    {
        var vm = await handler.Handle(new ListTagsQuery(page, 5, search));
        return View("Index", vm);
    }

    [HttpGet("create")]
    public IActionResult Create()
        => View("Create", new CreateTagForm());

    [HttpPost("create")]
    public async Task<IActionResult> Create( CreateTagForm vm, [FromServices] CreateTagHandler handler)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", vm);
        }
            
        try
        {
            await handler.Handle(vm);
            TempData["msg"] = "Tag criada com sucesso!";
            return RedirectToAction(nameof(Create));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(vm.Nome), ex.Message);
            return View("Create", vm);
        }
    }

    [HttpDelete("{id:int}/delete")]
    public async Task<IActionResult> Delete( int id, DeleteTagCommand vm, [FromServices] DeleteTagHandler handler)
    {
        if (id != vm.Id) return BadRequest();

        try
        {
            await handler.Handle(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Delete", vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
