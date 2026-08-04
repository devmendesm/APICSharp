using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IGenericRepository<Categoria> _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(ICategoriaRepository repository, IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public string GetValores()
        {
            var valor1 = _configuration["chave1"];
            var valor2 = _configuration["chave2"];

            var secao1 = _configuration["secao1:chave2"];

            return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
        }


        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            _logger.LogInformation("============== GET api/categorias ==============");

            //throw new DataMisalignedException();
            // AsNoTracking serve para tornar essa consulta não rastreada, melhorando o desempenho
            var categorias = _repository.GetAll();
            return Ok(categorias);

        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            _logger.LogInformation($"============== GET api/categorias/id = {id} ==============");

            var categoria = _repository.Get(c => c.CategoriaId == id);
            if (categoria is null)
            {
                _logger.LogInformation($"============== GET api/categorias/id = {id} NOT FOUND ==============");
                return NotFound($"Categoria com o id {id} não encontrado...");
            }
            return categoria;
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
            {
                _logger.LogWarning("Dados inválidos...");
                return BadRequest("Dados Inválidos!");
            }

            var categoriaCriada = _repository.Create(categoria);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos...");
                return BadRequest("Dados Inválidos!");
            }

            var categoriaAtualizada = _repository.Update(categoria);

            return Ok(categoriaAtualizada);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _repository.Get(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogWarning($"Categoria com id {id} não encontrada...");
                return NotFound($"Categoria com id {id} não localizado...");
            }

            var categoriaDeletada = _repository.Delete(categoria);

            return Ok(categoriaDeletada);
        }
    }
}
