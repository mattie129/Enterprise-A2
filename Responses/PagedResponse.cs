using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Responses
{
    public class PagedResponse<T>
    {
        public List<T> Data { get; set; }
        public Dictionary<string, int> Meta { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>();

        public PagedResponse(List<T> data)
        {
            Data = data;
        }

    }
}