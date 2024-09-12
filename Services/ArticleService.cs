using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Entities.Request;
using api_layaway.Interfaces;
using api_layaway.Models;
using Microsoft.EntityFrameworkCore;

namespace api_layaway.Services
{
    public class ArticleService : IArticleService
    {
        protected readonly LayawayDbContext _dbContext;
        protected readonly DbSet<Article> _dbSet;
        protected readonly ITransactionService _serviceTransaction;

        public ArticleService(LayawayDbContext dbContext, ITransactionService serviceTransaction)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Article>();
            _serviceTransaction = serviceTransaction;

        }

        public async Task<Reply<IEnumerable<Article>>> GetArticlesByLayawayId(
            ArticleParams paginatedParams
        )
        {
            var reply = new Reply<IEnumerable<Article>>();
            try
            {
                var query = _dbSet
                    .Where(x => x.LayawayId == paginatedParams.LayawayId && x.Status == 1)
                    .Where(e =>
                        string.IsNullOrEmpty(paginatedParams.Filter)
                        || e.Description.Contains(paginatedParams.Filter)
                    )
                    .AsQueryable();

                var totalRecords = await query.CountAsync();

                if (query.Count() > 0 && query != null)
                {
                    var entities = await query
                        .OrderBy(e => e.ArticleId) 
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

        public async Task<Reply<Article>> GetById(int id)
        {
            var reply = new Reply<Article>();
            try
            {
                var entity = await _dbSet.FirstOrDefaultAsync(x =>
                    x.ArticleId == id && x.Status == 1
                );

                if (entity == null)
                {
                    reply.Message = "Invalid id or Article not active";
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

        public async Task<Reply<Article>> Update(Article entity)
        {
            var reply = new Reply<Article>();
            try
            {
                if (entity == null)
                {
                    reply.Message = "Invalid entity";
                    reply.Status = 400;
                    return reply;
                }
                var existingEntity = _dbContext.Set<Article>().Find(entity.ArticleId);

                if (existingEntity == null)
                {
                    reply.Message = "Entity not found";
                    reply.Status = 404;
                    return reply;
                }

                var layawayEditing = _dbContext.Set<Layaway>().Find(entity.LayawayId);

                if (layawayEditing == null)
                {
                    reply.Message = "Entity not found";
                    reply.Status = 404;
                    return reply;
                }
                var latestTotal = layawayEditing.Total;
                Console.WriteLine("total before ", latestTotal);

                var subtotalExisting = existingEntity.Quantity * existingEntity.Price;
                Console.WriteLine("subtotal existitng ", subtotalExisting);

                var subtotalUpdated = entity.Quantity * entity.Price;
                Console.WriteLine("subtotal updated ", subtotalUpdated);

                //Update Article with changes
                entity.Subtotal = subtotalUpdated;
                entity.ArticleId = existingEntity.ArticleId;
                entity.Status = 1;
        
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);   
                _dbContext.SaveChanges();

                var newTotal = layawayEditing.Total - subtotalExisting + subtotalUpdated;
                Console.WriteLine("total after ", newTotal);

                //Update Layaway with new Total value
                layawayEditing.Total = newTotal;
                _dbContext.Entry(layawayEditing).State = EntityState.Modified;
                _dbContext.SaveChanges();

               
                var errorPayment = newTotal - latestTotal;
                Console.WriteLine("errorPayment ", errorPayment);

                var latestTransaction = await _serviceTransaction.FindLatetesTransactionByLayawayId(entity.LayawayId);
                
                decimal latestBalance = 0;
                if (latestTransaction != null){
                    latestBalance = latestTransaction.Balance;
                } 
                
                var newBalance = latestBalance + errorPayment;

                var newTransaction = new TransactionRecord{
                    Date= DateTime.UtcNow,
                    Payment = errorPayment,
                    Balance = newBalance,
                    LayawayId = entity.LayawayId,
                };
                //Create new transaction and setting new Balance
                 _dbContext.Add(newTransaction);
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

        public async Task<Reply<Article>> Delete(int id)
        {
            var layawayId = 0;

            var reply = new Reply<Article>();
            try
            {
                var entity = _dbContext
                    .Set<Article>()
                    .FirstOrDefault(c => c.ArticleId == id && c.Status == 1);

                if (entity == null)
                {
                    reply.Message = "Invalid entity";
                    reply.Status = 400;
                    return reply;
                }
                var subtotal = entity.Subtotal;
                layawayId = entity.LayawayId;
                entity.Status = 3;
                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.SaveChanges();

                var layaway = _dbContext
                    .Set<Layaway>()
                    .Include(l => l.Articles)
                    .FirstOrDefault(c => c.LayawayId == layawayId && c.Status == 1);

                if (layaway != null)
                {
                    // Si el Layaway no tiene artículos activos (Status = 1)
                    if (!layaway.Articles.Any(a => a.Status == 1))
                    {
                        layaway.Status = 3;
                    }
                    else
                    {
                        layaway.Total -= subtotal;
                    }
                    _dbContext.Entry(layaway).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }

                var latestTransaction = await _serviceTransaction.FindLatetesTransactionByLayawayId(entity.LayawayId);
                
                decimal latestBalance = 0;
                if (latestTransaction != null){
                    latestBalance = latestTransaction.Balance;
                } 
                var newBalance = latestBalance - subtotal;

                var newTransaction = new TransactionRecord{
                    Date= DateTime.UtcNow,
                    Payment = subtotal,
                    Balance = newBalance,
                    LayawayId = entity.LayawayId,
                };
                //Create new transaction and setting new Balance
                 _dbContext.Add(newTransaction);
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
