using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Dtos;
using api_layaway.Entities.Request;
using api_layaway.Entities.Reply;
using api_layaway.Interfaces;
using api_layaway.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace api_layaway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
         protected readonly IArticleService _service;
        protected readonly IHttpResult<Article> _httpResult;

        protected readonly IMapper _mapper;

         public ArticleController(IArticleService service, IHttpResult<Article> httpResult, IMapper mapper)
        {
            _service = service;
            _httpResult = httpResult;
             _mapper = mapper;
      
        }
        

         [HttpGet()]
        public async Task<IActionResult> GetArticlesByLayawayId([FromQuery]ArticleParams paginatedParams)
        {
            if (paginatedParams.PageNumber <= 0 || paginatedParams.PageSize <= 0  || paginatedParams.LayawayId<=0 )
                return BadRequest("The page number and page size must be greater than 0 and LayawayId is required.");

            var response = await _service.GetArticlesByLayawayId(paginatedParams);

            if (response.Data == null || !response.Data.Any())
            {
                return NotFound("No articles found.");
            }

            var list = new List<ArticleDto>();
            response.Data.ToList().ForEach(entity => list.Add(_mapper.Map<ArticleDto>(entity)));

            var result = new ReplyPaged<IEnumerable<ArticleDto>>();
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
            var handleResult = _httpResult.Handle<ArticleDto>(response);

            return handleResult;
        }

        [HttpPut]
        public virtual async Task<IActionResult> Update([FromBody] Request<ArticleDto> request)
        {
            

            if (request == null || request.Data == null)
                return BadRequest("Invalid request body");

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var entity = _mapper.Map<Article>(request.Data);
                var response = await _service.Update(entity);

                var handleResult = _httpResult.Handle<ArticleDto>(response);

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
        public virtual async Task<IActionResult> Delete(int id)
        {
            var response = await _service.Delete(id);
            return _httpResult.Handle<ArticleDto>(response);
        }


    }
}