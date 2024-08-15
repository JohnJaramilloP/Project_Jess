using Acme.BookStore.DTO.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Acme.BookStore.Ratings
{
    public class RatingAppService : CrudAppService<Rating, RatingDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateRatingDto>, IRatingAppService
    {
        public RatingAppService(IRepository<Rating, Guid> repository) : base(repository)
        {
        }
    }
}
