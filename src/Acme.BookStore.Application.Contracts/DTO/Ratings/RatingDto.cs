using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Acme.BookStore.DTO.Ratings
{
    public class RatingDto : AuditedEntityDto<Guid>
    {
        public string qar { get; set; }
        public string record_type { get; set; }
        public string qa_analyst { get; set; }
        public string session_type { get; set; }
        public string date { get; set; }
        public DateTime date_time { get; set; }
        public string overall_rating { get; set; }
        public string score_string { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
}
