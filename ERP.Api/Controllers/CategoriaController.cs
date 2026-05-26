using ERP.Domain;
using ERP.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriasService service;
        public CategoriaController(CategoriasService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var categoria = await service.GetById(id);
            if (categoria == null) return NotFound();
            return Ok(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Categoria categoria)
            => CreatedAtAction(nameof(GetById), new { id = categoria.Id }, await service.Create(categoria));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Categoria input)
        {
            var categoria = await service.Update(id, input);
            if (categoria == null) return NotFound();
            return Ok(categoria);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.Delete(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
}