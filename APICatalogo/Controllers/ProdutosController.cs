using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _ufo;

        public ProdutosController(IUnitOfWork ufo)
        {
            _ufo = ufo;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosCategoria(int id)
        {
            var produtos = _ufo.ProdutoRepository.GetProdutosPorCategoria(id);

            if (produtos is null) return NotFound("Produto não encontrado para esta categoria.");

            return Ok(produtos);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            // AsNoTracking serve para tornar essa consulta não rastreada, melhorando o desempenho
            //var produtos = await _context.Produtos.AsNoTracking().ToListAsync();

            var produtos = _ufo.ProdutoRepository.GetAll();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }
            return Ok(produtos);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _ufo.ProdutoRepository.Get(c => c.ProdutoId == id);
            if (produto is null)
            {
                return NotFound($"Produto com o id {id} não encontrado...");
            }
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
            {
                return BadRequest();
            }

            var produtoCriado = _ufo.ProdutoRepository.Create(produto);
            _ufo.Commit();

            return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriado.ProdutoId }, produtoCriado);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }

            var produtoAtualizado = _ufo.ProdutoRepository.Update(produto);
            _ufo.Commit();

            return Ok(produtoAtualizado);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _ufo.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null) return NotFound("Produto não encontrado");

            var produtoDeletado = _ufo.ProdutoRepository.Delete(produto);
            _ufo.Commit();

            return Ok(produtoDeletado);
        }


    }
}
