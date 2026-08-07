using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        // Implementação de mapeamento diferente da categorias, com IMapper
        private readonly IUnitOfWork _ufo;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork ufo, IMapper mapper)
        {
            _ufo = ufo;
            _mapper = mapper;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosCategoria(int id)
        {
            var produtos = _ufo.ProdutoRepository.GetProdutosPorCategoria(id);

            if (produtos is null)
                return NotFound("Produto não encontrado para esta categoria.");

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            // AsNoTracking serve para tornar essa consulta não rastreada, melhorando o desempenho
            //var produtos = await _context.Produtos.AsNoTracking().ToListAsync();

            var produtos = _ufo.ProdutoRepository.GetAll();

            if (produtos is null)
                return NotFound("Produtos não encontrados...");

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            var produto = _ufo.ProdutoRepository.Get(c => c.ProdutoId == id);
            if (produto is null)
                return NotFound($"Produto com o id {id} não encontrado...");

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }

        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoCriado = _ufo.ProdutoRepository.Create(produto);
            _ufo.Commit();

            var produtoCriadoDto = _mapper.Map<ProdutoDTO>(produtoCriado);

            return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriadoDto.ProdutoId }, produtoCriadoDto);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProdutoDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if (patchProdutoDTO is null || id <= 0)
                return BadRequest();

            var produto = _ufo.ProdutoRepository.Get(c => c.ProdutoId == id);

            if (produto is null)
                return NotFound();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            var modelValido = ModelState.IsValid;
            var tryValido = TryValidateModel(produtoUpdateRequest);

            Console.WriteLine($"ModelState: {modelValido}");
            Console.WriteLine($"TryValidateModel: {tryValido}");

            // TryValidateModel vai validar o modelo com base nas regras que configuramos
            if (!ModelState.IsValid || !TryValidateModel(produtoUpdateRequest))
                return BadRequest(ModelState);

            // Mapeia de volta para produto
            _mapper.Map(produtoUpdateRequest, produto);
            _ufo.ProdutoRepository.Update(produto);
            _ufo.Commit();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAtualizado = _ufo.ProdutoRepository.Update(produto);
            _ufo.Commit();

            var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

            return Ok(produtoAtualizadoDto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _ufo.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado");

            var produtoDeletado = _ufo.ProdutoRepository.Delete(produto);
            _ufo.Commit();

            var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDto);
        }


    }
}
