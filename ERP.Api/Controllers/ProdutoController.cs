using ERP.Domain;
using ERP.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutosService service;
        public ProdutoController(ProdutosService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var produto = await service.GetById(id);
            if (produto == null) return NotFound();
            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Produto produto)
            => CreatedAtAction(nameof(GetById), new { id = produto.Id }, await service.Create(produto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Produto input)
        {
            var produto = await service.Update(id, input);
            if (produto == null) return NotFound();
            return Ok(produto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.Delete(id);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("estoque-baixo")]
        public async Task<IActionResult> GetEstoqueBaixo()
            => Ok(await service.GetEstoqueBaixo());

        [HttpGet("{id}/para-pedido")]
        public async Task<IActionResult> GetParaPedido(Guid id)
        {
            var produto = await service.GetParaPedido(id);
            if (produto == null) return NotFound();
            return Ok(produto);
        }

        [HttpPost("{id}/encaminhar")]
        public async Task<IActionResult> Encaminhar(Guid id, [FromBody] SituacaoInput input)
        {
            try
            {
                var produto = await service.Encaminhar(id, input.Situacao);
                return Ok(produto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class SituacaoInput
    {
        public SituacaoProduto Situacao { get; set; }
    }
}