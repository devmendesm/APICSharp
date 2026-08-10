using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _ufo;

        public CategoriasController(IConfiguration configuration, ILogger<CategoriasController> logger, IUnitOfWork ufo)
        {
            _configuration = configuration;
            _logger = logger;
            _ufo = ufo;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public string GetValores()
        {
            var valor1 = _configuration["chave1"];
            var valor2 = _configuration["chave2"];

            var secao1 = _configuration["secao1:chave2"];

            return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
        }


        [HttpGet("pagination")]
        public ActionResult<IEnumerable<Categoria>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = _ufo.CategoriaRepository.GetCategorias(categoriasParameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            _logger.LogInformation("============== GET api/categorias ==============");

            var categorias = _ufo.CategoriaRepository.GetAll();

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            _logger.LogInformation($"============== GET api/categorias/id = {id} ==============");

            var categoria = _ufo.CategoriaRepository.Get(c => c.CategoriaId == id);
            if (categoria is null)
            {
                _logger.LogInformation($"============== GET api/categorias/id = {id} NOT FOUND ==============");
                return NotFound($"Categoria com o id {id} não encontrado...");
            }

            var categoriaDto = categoria.ToCategoriaDTO();

            return categoriaDto;
        }

        [HttpPost]
        public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
            {
                _logger.LogWarning("Dados inválidos...");
                return BadRequest("Dados Inválidos!");
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaCriada = _ufo.CategoriaRepository.Create(categoria);
            _ufo.Commit();

            var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

            return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
        }

        [HttpPut("{id:int}")]
        public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                _logger.LogWarning("Dados inválidos...");
                return BadRequest("Dados Inválidos!");
            }

            var categoria = categoriaDto.ToCategoria();

            var categoriaAtualizada = _ufo.CategoriaRepository.Update(categoria);
            _ufo.Commit();

            var categoriaAtualizadaDto = categoriaAtualizada.ToCategoriaDTO();

            return Ok(categoriaAtualizadaDto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _ufo.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogWarning($"Categoria com id {id} não encontrada...");
                return NotFound($"Categoria com id {id} não localizado...");
            }

            var categoriaDeletada = _ufo.CategoriaRepository.Delete(categoria);
            _ufo.Commit();

            var categoriaDeletadaDto = categoriaDeletada.ToCategoriaDTO();

            return Ok(categoriaDeletadaDto);
        }
    }
}
