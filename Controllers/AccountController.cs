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
    public class AccountController : ControllerBase
    {
        protected readonly IAccountService _service;
        protected readonly IHttpResult<Account> _httpResult;
        protected readonly IMapper _mapper;

        public AccountController(
            IAccountService service,
            IHttpResult<Account> httpResult,
            IMapper mapper
        )
        {
            _service = service;
            _httpResult = httpResult;
            _mapper = mapper;
        }


         [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            if (id == 0)
                return BadRequest("Invalid id");

            var response = await _service.GetTotalByCustomerId(id);
 
            return Ok(response);
      
        }

    }
}