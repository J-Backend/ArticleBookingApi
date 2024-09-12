using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Dtos;
using api_layaway.Entities.DtosNew;
using api_layaway.Entities.Request;
using api_layaway.Interfaces;
using api_layaway.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using api_layaway.Entities.Reply;
using Microsoft.AspNetCore.Authorization;

namespace api_layaway.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LayawayController : ControllerBase
    {
        private readonly ILayawayService _service;
        protected readonly IHttpResult<Layaway> _httpResult;

        protected readonly IMapper _mapper;

        public LayawayController(ILayawayService service, IHttpResult<Layaway> httpResult, IMapper mapper)
        {
            _service = service;
            _httpResult = httpResult;
            _mapper = mapper;

        }



        [HttpGet()]
        public async Task<IActionResult> GetLayawaysByCustomerId([FromQuery]LayawayParams paginatedParams)
        {
            if (paginatedParams.PageNumber <= 0 || paginatedParams.PageSize <= 0  || paginatedParams.CustomerId<=0 )
                return BadRequest("The page number and page size must be greater than 0 and CustomerId is required.");

            var response = await _service.GetLayawaysByCustomerId(paginatedParams);

            if (response.Data == null || !response.Data.Any())
            {
                return NotFound("No layaways found.");
            }

            var list = new List<LayawayDto>();
            response.Data.ToList().ForEach(entity => list.Add(_mapper.Map<LayawayDto>(entity)));

            var result = new ReplyPaged<IEnumerable<LayawayDto>>();
            result.Data = list;
            result.TotalRecords = response.TotalRecords;

            return Ok(result);
        }


        [HttpPost]
        public virtual IActionResult Create([FromBody] Request<LayawayDtoNew> request)
        {
            if (request == null || request.Data == null)
                return BadRequest("Invalid request body");

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = _mapper.Map<Layaway>(request.Data);
                var response = _service.Create(entity);
                var handleResult = _httpResult.Handle<LayawayDtoNew>(response);

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