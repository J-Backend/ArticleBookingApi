using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Interfaces;
using api_layaway.Models;
using Microsoft.EntityFrameworkCore;
using api_layaway.Entities.Request;
namespace api_layaway.Services
{
    public class CustomerService : ICustomerService
    {
        protected readonly LayawayDbContext _dbContext;
        protected readonly DbSet<Customer> _dbSet;

        public CustomerService(LayawayDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Customer>();
        }


        public async Task<Reply<IEnumerable<Customer>>> GetAll(PaginationParams paginatedParams)
        {
            var reply = new Reply<IEnumerable<Customer>>();

            try
            {
                 var query = _dbSet.Where(e => e.Status == 1)
                .Where(e =>
                    string.IsNullOrEmpty(paginatedParams.Filter) || e.Name.Contains(paginatedParams.Filter)
                )
                .AsQueryable(); 

                var totalRecords = await query.CountAsync();

                if (query.Count() > 0 && query != null)
                {
                     var entities = await query
                    .OrderBy(e => e.CustomerId) // Asegúrate de ordenar por un campo único
                    .Skip((paginatedParams.PageNumber - 1) * paginatedParams.PageSize)
                    .Take(paginatedParams.PageSize)
                    .ToListAsync();

                    reply.Data = entities;
                    reply.Status = 200;
                    reply.Method = "GETALL";
                    reply.TotalRecords = totalRecords;
                }
                else
                {
                    reply.Message = "No entities found";
                    reply.Status = 404;
                }
            }
            catch (Exception e)
            {
                reply.Status = 500;
                reply.Message = e.Message;
            }

            return reply;
        }





        public async Task<Reply<Customer>> GetById(int id)
        {
            var reply = new Reply<Customer>();
            try
            {
                var entity = await _dbSet.FirstOrDefaultAsync(x =>
                    x.CustomerId == id && x.Status == 1
                );

                if (entity == null)
                {
                    reply.Message = "Invalid id or customer not active";
                    reply.Status = 404;
                    return reply;
                }

                reply.Data = entity;
                reply.Status = 200;
                reply.Method = "GET";
            }
            catch (Exception e)
            {
                reply.Status = 500;
                reply.Message = e.Message;
            }
            return reply;
        }

        public Reply<Customer> Create(Customer entity)
        {
            var reply = new Reply<Customer>();
            try
            {
                if (entity == null)
                {
                    reply.Message = "Invalid entity";
                    reply.Status = 400;
                    return reply;
                }
                _dbContext.Add(entity);
                _dbContext.SaveChanges();
                reply.Method = "POST";
                reply.Status = 200;
            }
            catch (Exception e)
            {
                reply.Status = 500;
                reply.Message = e.Message;
            }
            return reply;
        }

        public Reply<Customer> Update(Customer entity)
        {
            var reply = new Reply<Customer>();
            try
            {
                if (entity == null)
                {
                    reply.Message = "Invalid entity";
                    reply.Status = 400;
                    return reply;
                }
                var existingEntity = _dbContext.Set<Customer>().Find(entity.CustomerId);

                if (existingEntity == null)
                {
                    reply.Message = "Entity not found";
                    reply.Status = 404;
                    return reply;
                }
                entity.Status = 1;
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                _dbContext.SaveChanges();

                reply.Method = "PUT";
                reply.Status = 200;
            }
            catch (Exception e)
            {
                reply.Status = 500;
                reply.Message = e.Message;
            }
            return reply;
        }

        public Reply<Customer> Delete(int id)
        {
            var reply = new Reply<Customer>();
            try
            {
                var entity = _dbContext
                    .Set<Customer>()
                    .FirstOrDefault(c => c.CustomerId == id && c.Status == 1);
                if (entity == null)
                {
                    reply.Message = "Invalid entity";
                    reply.Status = 400;
                    return reply;
                }
                var layaways = _dbContext
                    .Set<Layaway>()
                    .Where(x => x.CustomerId == id && x.Status == 1)
                    .ToList();

                if (layaways.Any(a => a.Status == 1))
                {
                    reply.Message = "Customer still have active layaways";
                    reply.Status = 400;
                    return reply;
                }
                entity.Status = 3;
                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.SaveChanges();

                reply.Method = "DELETE";
                reply.Status = 200;
            }
            catch (Exception e)
            {
                reply.Status = 500;
                reply.Message = e.Message;
            }
            return reply;
        }
    }
}
