using Acme.BookStore.DTO.Ratings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Acme.BookStore.DTO.Base
{
    public interface IBaseAppService : IApplicationService
    {
        Task<List<RatingDto>> GetListRatingsDto();
        Task<Guid> CrearRating(RatingDto data);
        Task editarRating(RatingDto data);
        Task eliminarRating(Guid Id);
        Task ProcesarIncentivos(string pathFile);
    }
}
