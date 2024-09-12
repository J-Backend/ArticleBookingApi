using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Dtos;
using api_layaway.Entities.DtosNew;
using api_layaway.Entities.Reply;
using api_layaway.Entities.Request;
using api_layaway.Interfaces;
using api_layaway.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace api_layaway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        protected readonly ITransactionService _service;
        protected readonly IHttpResult<TransactionRecord> _httpResult;
        protected readonly IMapper _mapper;

        public TransactionController(
            ITransactionService service,
            IHttpResult<TransactionRecord> httpResult,
            IMapper mapper
        )
        {
            _service = service;
            _httpResult = httpResult;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<IActionResult> GetTransactionsByLayawayId([FromQuery]TransactionParams paginatedParams)
        {
            if (paginatedParams.PageNumber <= 0 || paginatedParams.PageSize <= 0  || paginatedParams.LayawayId<=0 )
                return BadRequest("The page number and page size must be greater than 0 and LayawayId is required.");

            var response = await _service.GetTransactionsByLayawayId(paginatedParams);

            if (response.Data == null || !response.Data.Any())
            {
                return NotFound("No transactions found");
            }

            var list = new List<TransactionRecordDto>();
            response.Data.ToList().ForEach(entity => list.Add(_mapper.Map<TransactionRecordDto>(entity)));

            var result = new ReplyPaged<IEnumerable<TransactionRecordDto>>();
            result.Data = list;
            result.TotalRecords = response.TotalRecords;

            return Ok(result);
        }

         [HttpPost]
        public async Task<IActionResult> Create([FromBody] Request<TransactionRecordDtoNew> request)
        {
            if (request == null || request.Data == null)
                return BadRequest("Invalid request body");

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = _mapper.Map<TransactionRecord>(request.Data);
                var response = await _service.Create(entity);
                var handleResult = _httpResult.Handle<TransactionRecordDtoNew>(response);

                if (handleResult is OkResult { StatusCode: 200 })
                {
                    return Ok(request.Data);
                }
                return handleResult;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}