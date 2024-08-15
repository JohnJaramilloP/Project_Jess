using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Acme.BookStore.Ratings
{
    public class Rating : AuditedAggregateRoot<Guid>
    {
        public Guid Id { get; set; }
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
