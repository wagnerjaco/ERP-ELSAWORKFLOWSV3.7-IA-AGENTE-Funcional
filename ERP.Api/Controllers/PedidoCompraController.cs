using ERP.Domain;
using ERP.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class PedidoCompraController : ControllerBase
    {
        private readonly PedidosComprasService service;

        public PedidoCompraController(PedidosComprasService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await service.GetAll());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var pedido = await service.GetById(id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PedidoCompra pedido)
            => CreatedAtAction(nameof(GetById), new { id = pedido.Id }, await service.Create(pedido));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PedidoCompra input)
        {
            var pedido = await service.Update(id, input);
            if (pedido == null) return NotFound();
            if (pedido.Aprovado == true) return Ok(new { message = "Pedido Já foi aprovado" });
            return Ok(pedido);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.Delete(id);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("pendentes")]
        public async Task<IActionResult> GetPendentes()
            => Ok(await service.GetPendentes());

        //[HttpGet("aprovar/{id}/{email}")]
        //public async Task<IActionResult> Aprovar(string id, string email)
        //{
        //    try
        //    {
        //        var result = await service.Aprovar(id, email);
        //        if (!result)
        //            return BadRequest(new { message = "Email de aprovação inválido" });
        //        return Ok(new { message = "Pedido aprovado com sucesso" });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}
        [HttpGet("aprovar/{id}/{email}")]
        public async Task<IActionResult> Aprovar(string id, string email)
        {
            try
            {
                var result = await service.Aprovar(id, email);

                if (!result)
                {
                    var htmlErro = @"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <title>Erro</title>
                <style>
                    body{
                        font-family: Arial;
                        background:#f5f5f5;
                        display:flex;
                        justify-content:center;
                        align-items:center;
                        height:100vh;
                    }

                    .box{
                        background:white;
                        padding:40px;
                        border-radius:10px;
                        box-shadow:0 0 10px rgba(0,0,0,0.1);
                        text-align:center;
                    }

                    h1{
                        color:#dc3545;
                    }
                </style>
            </head>
            <body>
                <div class='box'>
                    <h1>Email inválido</h1>
                    <p>O link de aprovação não é válido.</p>
                </div>
            </body>
            </html>";

                    return Content(htmlErro, "text/html");
                }

                var htmlSucesso = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <title>Pedido Aprovado</title>
            <style>
                body{
                    font-family: Arial;
                    background:#f5f5f5;
                    display:flex;
                    justify-content:center;
                    align-items:center;
                    height:100vh;
                }

                .box{
                    background:white;
                    padding:40px;
                    border-radius:10px;
                    box-shadow:0 0 10px rgba(0,0,0,0.1);
                    text-align:center;
                }

                h1{
                    color:#28a745;
                }
            </style>
        </head>
        <body>
            <div class='box'>
                <h1>Pedido aprovado</h1>
                <p>O pedido foi aprovado com sucesso.</p>
            </div>
        </body>
        </html>";

                return Content(htmlSucesso, "text/html");
            }
            catch (ArgumentException ex)
            {
                var htmlErro = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <title>Erro</title>
        </head>
        <body>
            <h1>Erro</h1>
            <p>{ex.Message}</p>
        </body>
        </html>";

                return Content(htmlErro, "text/html");
            }
        }
    }

    public class AprovarInput
    {
        public string Email { get; set; } = string.Empty;
    }
}