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
    public class LayawayService : ILayawayService
    {
        protected readonly LayawayDbContext _dbContext;
        protected readonly DbSet<Layaway> _dbSet;
        private decimal total = 0;

        public LayawayService(LayawayDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Layaway>();
        }


         public async Task<Reply<IEnumerable<Layaway>>> GetLayawaysByCustomerId(LayawayParams paginatedParams)
        {
            var reply = new Reply<IEnumerable<Layaway>>();
            try
            {
                var query =  _dbSet
                    .Where(x => x.CustomerId == paginatedParams.CustomerId && x.Status == 1) 
                    .Include(x => x.Articles )
                    .Select(l => new Layaway 
                    {
                        LayawayId = l.LayawayId,
                        Opening = l.Opening,
                        Closing = l.Closing,
                        State = l.State,
                        CustomerId = l.CustomerId,
                        Total = l.Total,
                        Articles = l.Articles.Where(a => a.Status == 1).ToList()
                    }).AsQueryable();
                
                    DateTime filterDate;
                    bool isDateFilterValid = DateTime.TryParse(paginatedParams.Filter, out filterDate);

                    if (isDateFilterValid)
                    {
                        query = query.Where(e => e.Opening.Date == filterDate.Date);
                    }
                     

                    var totalRecords = await query.CountAsync();

                if (query.Count() > 0 && query != null)
                {
                     var entities = await query
                    .OrderBy(e => e.LayawayId) 
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


        public Reply<Layaway> Create(Layaway entity)
        {
             var reply = new Reply<Layaway>();
              if (entity == null)
                {
                    reply.Message = "Invalid entity";
                    reply.Status = 400;
                    return reply;
                }

           
            try
            {
                foreach (var item in entity.Articles)
                {
                    var subtotal = item.Price * item.Quantity;
                    item.Subtotal = subtotal;
                    total += subtotal;
                }
                entity.Total = total;
                _dbContext.Add(entity);
                

                if (entity.Articles != null && entity.Articles.Count > 0)
                {
                    foreach (var article in entity.Articles)
                    {
                        _dbContext.Set<Article>().Add(article);
                    }
                }
                _dbContext.SaveChanges();

                var firstTransaction = new TransactionRecord{
                    Date=DateTime.UtcNow,
                    LayawayId=entity.LayawayId,
                    Payment=0,
                    Balance=this.total,
                };
                 _dbContext.Add(firstTransaction);
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
    }
}
