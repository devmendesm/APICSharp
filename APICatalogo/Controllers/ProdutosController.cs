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
        /* Poderia ser usado apenas a instância do repositorio especifico (IProdutoRepository),
         * sem a necessidade do repositorio genérico, pois o especifico implementa o genérico, 
         * logo ele tem acesso a todos os métodos do genérico.
         */
        private readonly IGenericRepository<Produto> _repository;
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosController(IGenericRepository<Produto> repository, IProdutoRepository produtoRepository)
        {
            _repository = repository;
            _produtoRepository = produtoRepository;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosCategoria(int id)
        {
            var produtos = _produtoRepository.GetProdutosPorCategoria(id);

            if (produtos is null) return NotFound("Produto não encontrado para esta categoria.");

            return Ok(produtos);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            // AsNoTracking serve para tornar essa consulta não rastreada, melhorando o desempenho
            //var produtos = await _context.Produtos.AsNoTracking().ToListAsync();

            var produtos = _repository.GetAll();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados...");
            }
            return Ok(produtos);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _repository.Get(c => c.ProdutoId == id);
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

            var produtoCriado = _repository.Create(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriado.ProdutoId }, produtoCriado);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }

            var produtoAtualizado = _repository.Update(produto);

            return Ok(produtoAtualizado);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _repository.Get(p => p.ProdutoId == id);

            if (produto is null) return NotFound("Produto não encontrado");

            var produtoDeletado = _repository.Delete(produto);

            return Ok(produtoDeletado);
        }


    }
}
