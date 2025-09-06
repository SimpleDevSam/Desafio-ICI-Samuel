using Desafio_ICI_Samuel.Data;
using Desafio_ICI_Samuel.Features.Tags;
using Desafio_ICI_Samuel.Features.Tags.Create;
using Desafio_ICI_Samuel.Features.Tags.Delete;
using Desafio_ICI_Samuel.Features.Tags.Edit;
using Desafio_ICI_Samuel.Features.Tags.Get;
using Desafio_ICI_Samuel.Features.Tags.List;
using Microsoft.AspNetCore.Mvc;

namespace Desafio_ICI_Samuel.Controllers;

[Route("tags")]

[AutoValidateAntiforgeryToken]
public class TagsController : Controller
{
    private readonly ICreateTagHandler _createHandler;
    private readonly IDeleteTagHandler _deleteHandler;
    private readonly IEditTagHandler _editHandler;
    private readonly IGetTagHandler _getHadndler;
    private readonly IListTagsHandler _listHandler;
    public TagsController(
    IListTagsHandler listHandler,
    ICreateTagHandler createHandler,
    IDeleteTagHandler deleteHandler,
    IEditTagHandler editHandler,
    IGetTagHandler getHandler)
    {
        _listHandler = listHandler;
        _createHandler = createHandler;
        _deleteHandler = deleteHandler;
        _editHandler = editHandler;
        _getHadndler = getHandler;
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            var vm = await _getHadndler.Handle(id);
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
        var tag = await _getHadndler.Handle(id);

        if (tag is null)
        {
            return NotFound();
        }

        var vm = new EditTagForm { Id = tag.Id, Name = tag.Name };
        return View("Edit", vm);
    }

    [HttpPost("{id:int}/edit")]
    public async Task<IActionResult> Edit( int id, EditTagForm vm)
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
            await _editHandler.Handle(vm);
            TempData["msg"] = "Tag atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException dup)
        {
            ModelState.AddModelError(nameof(vm.Name), dup.Message);
            return View("Edit", vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] int page = 1, [FromQuery] string? search = null)
    {
        var vm = await _listHandler.Handle(new ListTagsQuery(page, 5, search));
        return View("Index", vm);
    }

    [HttpGet("create")]
    public IActionResult Create()
        => View("Create", new CreateTagForm());

    [HttpPost("create")]
    public async Task<IActionResult> Create( CreateTagForm vm)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", vm);
        }
            
        try
        {
            await _createHandler.Handle(vm);
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
    public async Task<IActionResult> Delete( int id, DeleteTagCommand vm)
    {
        if (id != vm.Id) return BadRequest();

        try
        {
            await _deleteHandler.Handle(id);
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
