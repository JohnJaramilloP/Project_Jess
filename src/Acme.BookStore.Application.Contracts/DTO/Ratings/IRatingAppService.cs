using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;


namespace Acme.BookStore.DTO.Ratings
{
    public interface IRatingAppService : ICrudAppService<RatingDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateRatingDto>
    {

    }
}
