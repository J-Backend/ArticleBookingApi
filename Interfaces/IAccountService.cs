using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Models;

namespace api_layaway.Interfaces
{
    public interface IAccountService
    {
        Task<Reply<decimal>> GetTotalByCustomerId(int id);
    }
}