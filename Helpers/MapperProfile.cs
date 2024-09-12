using api_layaway.Entities.Dtos;
using api_layaway.Entities.DtosNew;
using api_layaway.Models;

using AutoMapper;


namespace api_layaway.Helpers
{
    public class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<Article, ArticleDto>().ReverseMap();
            CreateMap<ArticleDtoNew, Article>();

            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<CustomerDtoNew, Customer>();

            CreateMap<Layaway, LayawayDto>().ReverseMap();
            CreateMap<LayawayDtoNew, Layaway>();

            CreateMap<Account, AccountDto>().ReverseMap();
           

             CreateMap<TransactionRecord, TransactionRecordDto>().ReverseMap();
             CreateMap<TransactionRecordDtoNew, TransactionRecord>();
        }
    }
}