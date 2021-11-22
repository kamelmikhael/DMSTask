using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSTask.Models
{
    public class BaseEntity<TKey>
    {
        public TKey Id
        {
            get;
            set;
        }
        public DateTime AddedDate
        {
            get;
            set;
        }
        public DateTime ModifiedDate
        {
            get;
            set;
        }
        public string IPAddress
        {
            get;
            set;
        }
    }
}
