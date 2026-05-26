using ERP.Domain;
using ERP.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FornecedorController : ControllerBase
    {
        private readonly FornecedoresService service;
        public FornecedorController(FornecedoresService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var fornecedor = await service.GetById(id);
            if (fornecedor == null) return NotFound();
            return Ok(fornecedor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Fornecedor fornecedor)
            => CreatedAtAction(nameof(GetById), new { id = fornecedor.Id }, await service.Create(fornecedor));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Fornecedor input)
        {
            var fornecedor = await service.Update(id, input);
            if (fornecedor == null) return NotFound();
            return Ok(fornecedor);
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