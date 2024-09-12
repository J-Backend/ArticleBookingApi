using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Models;
using api_layaway.Entities.Request;

namespace api_layaway.Interfaces
{
    public interface ITransactionService
    {
        Task<Reply<IEnumerable<TransactionRecord>>> GetTransactionsByLayawayId(TransactionParams paginatedParams);

        Task<Reply<TransactionRecord>> Create(TransactionRecord entity);

        Task<TransactionRecord> FindLatetesTransactionByLayawayId(int layawayId);
    }
}