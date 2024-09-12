using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Models;
using api_layaway.Entities.Request;
namespace api_layaway.Interfaces
{
    public interface ICustomerService
    {

        Task<Reply<IEnumerable<Customer>>> GetAll(PaginationParams paginatedParams);
        Task<Reply<Customer>> GetById(int id);
        Reply<Customer> Create(Customer entity);

        Reply<Customer> Update(Customer entity);

        Reply<Customer> Delete(int id);
    }
}