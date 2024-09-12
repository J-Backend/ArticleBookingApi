using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using Microsoft.AspNetCore.Mvc;

namespace api_layaway.Interfaces
{
    public interface IHttpResult<T> where T : class
    {
    
        IActionResult Handle<D>(Reply<T> reply);
        IActionResult Handle<D>(Reply<IEnumerable<T>> reply);
        D MapToDto<T,D>(T entity);
        IEnumerable<D> MapToDto<T, D>(IEnumerable<T> entities);
    }
}