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
    public class AccountService : IAccountService
    {
        protected readonly LayawayDbContext _dbContext;
        protected readonly DbSet<Account> _dbSet;

        public AccountService(LayawayDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Account>();
        }

        public async Task<Reply<decimal>> GetTotalByCustomerId(int id)
        {
            var reply = new Reply<decimal>();
            try
            {
                var layaways = await _dbContext
                    .Set<Layaway>()
                    .Where(x => x.CustomerId == id && x.Status == 1)
                    .ToListAsync();

                if (layaways == null || !layaways.Any())
                {
                    reply.Message = "No layaways found";
                    reply.Status = 404;
                    return reply;
                }

                decimal totalBalance = 0;

    
                foreach (var layaway in layaways)
                {
                
                    var latestTransaction = await _dbContext
                        .Set<TransactionRecord>()
                        .Where(x => x.LayawayId == layaway.LayawayId && x.Status == 1)
                        .OrderByDescending(x => x.Date)
                        .FirstOrDefaultAsync();

                    if (latestTransaction != null)
                    {
                        
                        totalBalance += latestTransaction.Balance;
                    }
                }

                reply.Data = totalBalance;
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
    }
}
