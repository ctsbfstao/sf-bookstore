using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bookstore.Service.Domain
{
    public interface IBookstoreService :IService
    {
        Task AddBookAsync(Book book);
        Task<IList<Book>> GetBooksAsync(CancellationToken ct);
        Task DeleteBookAsync(Book book);
    }
}


