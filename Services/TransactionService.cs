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
    public class TransactionService : ITransactionService
    {
        protected readonly LayawayDbContext _dbContext;
        protected readonly DbSet<TransactionRecord> _dbSet;

        public TransactionService(LayawayDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TransactionRecord>();
        }

        public async Task<Reply<IEnumerable<TransactionRecord>>> GetTransactionsByLayawayId(
            TransactionParams paginatedParams
        )
        {
            var reply = new Reply<IEnumerable<TransactionRecord>>();
            try
            {


                var transactions = await _dbSet
                        .Where(x => x.LayawayId == paginatedParams.LayawayId && x.Status == 1)
                        .ToListAsync();

                 if (transactions == null || !transactions.Any())
                {
                    reply.Message = "No transactions found";
                    reply.Status = 404;
                    return reply;
                }

                DateTime filterDate;
                bool isDateFilterValid = DateTime.TryParse(paginatedParams.Filter, out filterDate);

                if (isDateFilterValid)
                {
                    transactions = transactions.Where(e => e.Date.Date == filterDate.Date).ToList();
                }

                var totalRecords = transactions.Count();

                if (totalRecords > 0)
                {
                    var entities = transactions
                        .OrderByDescending(e => e.Date)
                        .ThenBy(e => e.TransactionRecordId)
                        .Skip((paginatedParams.PageNumber - 1) * paginatedParams.PageSize)
                        .Take(paginatedParams.PageSize)
                        .ToList();

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

        public async Task<Reply<TransactionRecord>> Create(TransactionRecord entity)
        {
            var reply = new Reply<TransactionRecord>();  
            try
            {
   
                var latestTransaction = await FindLatetesTransactionByLayawayId(entity.LayawayId);

                if (latestTransaction == null)
                {
                    reply.Message = "No recent transaction found";
                    reply.Status = 404;
                    return reply;
                }

    
                var latestBalance = latestTransaction.Balance;
                var newBalance = latestBalance - entity.Payment;

                entity.Balance = newBalance;
                entity.Date = DateTime.UtcNow;

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

        public async Task<TransactionRecord> FindLatetesTransactionByLayawayId(int layawayId)
        {
           
                var transactions = await _dbSet
                    .Where(x => x.LayawayId == layawayId && x.Status == 1)
                    .ToListAsync();

                if (transactions == null || !transactions.Any())
                {
                    return null;
                }

                var latestTransaction = transactions
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();

                if (latestTransaction == null)
                {
                    return null;
                }

                return latestTransaction;

        }   
    
    }
}
