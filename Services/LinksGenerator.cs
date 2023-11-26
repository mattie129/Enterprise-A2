using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeopleAPI.Services
{
    public static class LinksGenerator
    {
        public static Dictionary<string, string> GenerateLinks(string baseURL,
                                            int currentPage,
                                            int totalRecords,
                                            int pageSize
                                            )
        {
            var result = new Dictionary<string, string>();
            int totalPages = (int)Math.Ceiling((decimal)totalRecords / (decimal)pageSize);
            result.Add("First", $"{baseURL}?pageNumber=1&pagesize={pageSize}"); // /api/people?pageNumber=1&pagesize=10
            result.Add("Next", currentPage == totalPages ? "" : $"{baseURL}?pageNumber={currentPage + 1}&pagesize={pageSize}"); // /api/people?pageNumber=1&pagesize=10
            return result;
        }
    }
}