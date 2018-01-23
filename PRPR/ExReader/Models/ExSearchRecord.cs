using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.ExReader.Models
{
    public class ExSearchRecord
    {
        public int Id { get; set; }


        public static ExSearchRecord Create(string keyword)
        {
            return new ExSearchRecord()
            {
                DateCreated = DateTime.UtcNow,
                Keyword = keyword
            };
        }



        public string Keyword { get; set; }

        public DateTime DateCreated { get; set; }


        
    }
}
