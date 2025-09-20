using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    /// <summary>
    /// Controller para gerenciar as operações de Vendas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public SalesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria uma nova Venda.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
        {
            var command = _mapper.Map<CreateSaleCommand>(request);
            var result = await _mediator.Send(command);
            var response = _mapper.Map<CreateSaleResponse>(result);

            return CreatedAtAction(nameof(GetSaleById), new { id = response.SaleId }, response);
        }

        /// <summary>
        /// Busca uma Venda pelo seu ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetSaleByIdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSaleById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("O ID da venda é inválido.");
            }

            var query = new GetSaleByIdQuery { SaleId = id };
            try
            {
                var result = await _mediator.Send(query);
                var response = _mapper.Map<GetSaleByIdResponse>(result);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza uma Venda existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request)
        {
            var command = _mapper.Map<UpdateSaleCommand>(request);
            command.SaleId = id;

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Cancela uma Venda existente (soft delete).
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelSale(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("O ID da venda é inválido.");
            }

            var command = new CancelSaleCommand { SaleId = id };
            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Busca uma lista paginada de Vendas, com suporte para ordenação.
        /// </summary>
        /// <param name="request">Objeto contendo os parâmetros de consulta para paginação (_page, _size) e ordenação (_order).</param>
        /// <returns>Uma lista paginada com os registros de vendas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<GetSaleByIdResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSales([FromQuery] GetAllSalesRequest request)
        {
            var query = new GetAllSalesQuery
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _mediator.Send(query);

            var responseItems = _mapper.Map<IEnumerable<GetSaleByIdResponse>>(result.Sales);

            var paginatedResponse = new PaginatedResponse<GetSaleByIdResponse>
            {
                Success = true,
                Data = responseItems,
                CurrentPage = request.PageNumber,
                TotalCount = result.TotalCount,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)request.PageSize)
            };

            return Ok(paginatedResponse);
        }
    }
}