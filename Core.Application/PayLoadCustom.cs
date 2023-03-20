using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application
{
    public class PayloadCustom<T>
    {
        public T Entity { get; set; }

      
        public List<T> EntityList { get; set; }

        [DefaultValue(0)]
        public int TotalRecords { get; set; }

        public int Status { get; set; }

        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; }
    }
}
