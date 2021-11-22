using DMSTask.Data;
using DMSTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSTask.Repositories
{
    public interface ICustomerRepository : IRepository<Customer, int>
    {
    }

    public class CustomerRepository : Repository<Customer, int>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
