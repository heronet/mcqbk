using System.Collections.Generic;

namespace DTO
{
    public class GetResponseWithPage<T>
    {
        public List<T> Data { get; set; }
        public long Size { get; set; }
    }
}