using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Models;
using api_layaway.Entities.Request;

namespace api_layaway.Interfaces
{
    public interface ILayawayService
    {

        Task<Reply<IEnumerable<Layaway>>> GetLayawaysByCustomerId(LayawayParams paginatedParams);

        Reply<Layaway> Create(Layaway entity);
    }
}