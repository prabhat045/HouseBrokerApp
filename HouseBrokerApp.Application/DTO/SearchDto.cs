using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBrokerApp.Application.DTO
{
    public class SearchDto
    {
        
         public string? Location { get; set; }
         public decimal? MinPrice { get; set; }
         public decimal? MaxPrice { get; set; }
         public string? PropertyType { get; set; }
    }
}
