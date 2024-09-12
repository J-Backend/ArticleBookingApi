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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace api_layaway.controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        protected readonly ICustomerService _service;
        protected readonly IHttpResult<Customer> _httpResult;
        protected readonly IMapper _mapper;

        public CustomerController(
            ICustomerService service,
            IHttpResult<Customer> httpResult,
            IMapper mapper
        )
        {
            _service = service;
            _httpResult = httpResult;
            _mapper = mapper;
        }


         
        [HttpGet]
        public virtual async Task<IActionResult> GetAll( [FromQuery] CustomerParams paginatedParams )
        {
            if (paginatedParams.PageNumber <= 0 || paginatedParams.PageSize <= 0 )
            {
                return BadRequest("The page number and page size must be greater than 0.");
            }
            var response = await _service.GetAll(paginatedParams);
            if (response.Data == null || !response.Data.Any())
            {
                return NotFound("No customers found.");
            }

            var list = new List<CustomerDto>();
            response.Data.ToList().ForEach(entity => list.Add(_mapper.Map<CustomerDto>(entity)));

            var result = new ReplyPaged<IEnumerable<CustomerDto>>();
            result.Data = list;
            result.TotalRecords = response.TotalRecords;

            return Ok(result);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            if (id == 0)
                return BadRequest("Invalid id");

            var response = await _service.GetById(id);
            var handleResult = _httpResult.Handle<CustomerDto>(response);

            return handleResult;
        }

        [HttpPost]
        public virtual IActionResult Create([FromBody] Request<CustomerDtoNew> request)
        {
            if (request == null || request.Data == null)
                return BadRequest("Invalid request body");

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = _mapper.Map<Customer>(request.Data);
                var response = _service.Create(entity);
                var handleResult = _httpResult.Handle<CustomerDtoNew>(response);

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

        [HttpPut]
        public virtual IActionResult Update([FromBody] Request<CustomerDto> request)
        {
            if (request == null || request.Data == null)
                return BadRequest("Invalid request body");

            try
            {
   
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var entity = _mapper.Map<Customer>(request.Data);
                var response = _service.Update(entity);

                var handleResult = _httpResult.Handle<CustomerDto>(response);

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

        [HttpDelete("{id}")]
        public virtual IActionResult Delete(int id)
        {
            var response = _service.Delete(id);
            return _httpResult.Handle<CustomerDto>(response);
        }
    }
}
