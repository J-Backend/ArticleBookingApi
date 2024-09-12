using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api_layaway.Helpers
{
   public class HttpResult<T> : ControllerBase, IHttpResult<T>
        where T : class
    {
        private readonly IMapper mapper;

        public HttpResult(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IActionResult Handle<D>(Reply<T> response)
        {
            return ClasifyCode<D>(
                response.Status,
                response.Message,
                response.Data,
                response.Method
            );
        }

        public IActionResult Handle<D>(Reply<IEnumerable<T>> response)
        {
            return ClasifyCode<D>(
                response.Status,
                response.Message,
                response.Data,
                response.Method
            );
        }

        private IActionResult ClasifyCode<D>(int Status, string Message, object Data, string method)
        {
            switch (Status)
            {
                case 200:
                    // if (method.Equals("GETALL")){
                    //     var list = MapToDto<T,D>(Data as IEnumerable<T>);
                    //     return Ok(list);
                    // }else  if (method.Equals("GET")){
                    //     var entity = MapToDto<T,D>(Data as T);
                    //     return Ok(entity);
                    // }
                    // return Ok();
                    return Handle200GetResult<D>(Data, method);
                case 400:
                    return BadRequest(Message);
                case 401:
                    return Unauthorized(Message);
                case 403:
                    return Forbid(Message);
                case 404:
                    return NotFound(Message);
                case 500:
                    return StatusCode(500, Message);
            }
            return StatusCode(500, "Unknown server error");
        }

        public IEnumerable<D> MapToDto< T,D>(IEnumerable<T> entities)
        {
            var list = new List<D>();
            entities.ToList().ForEach(entity => list.Add(this.mapper.Map<D>(entity)));

            return list;
        }

        public D MapToDto<T, D>(T entity)
        {
            var entityDto = this.mapper.Map<D>(entity);
            return entityDto;
        }

        public IActionResult Handle200GetResult<D>(object Data, string method){
            try
            {
                 if (method.Equals("GETALL")){
                        var list = MapToDto<T,D>(Data as IEnumerable<T>);
                        return Ok(list);
                    }else  if (method.Equals("GET")){
                        var entity = MapToDto<T,D>(Data as T);
                        return Ok(entity);
                    }
                    return Ok();
            }
            catch (System.Exception e)
            {
                
                return StatusCode(500, e.Message);
            }
        }
    }
}