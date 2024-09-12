using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_layaway.Entities.Reply;
using api_layaway.Models;
using api_layaway.Entities.Request;

namespace api_layaway.Interfaces
{
    public interface IArticleService
    {
        Task<Reply<IEnumerable<Article>>> GetArticlesByLayawayId(ArticleParams paginatedParams);
        Task<Reply<Article>> GetById(int id);

        Task<Reply<Article>> Update(Article entity);
  
        Task<Reply<Article>> Delete(int id);
    }
}