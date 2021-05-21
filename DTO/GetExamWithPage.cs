using System.Collections.Generic;

namespace DTO
{
    public class GetExamWithPage
    {
        public List<GetExamDTO> Exams { get; set; }
        public long Size { get; set; }
    }
}