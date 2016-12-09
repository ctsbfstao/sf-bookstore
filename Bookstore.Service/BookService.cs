using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Bookstore.Service.Domain;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace Bookstore.Service
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class BookService : StatefulService, IBookstoreService
    {
        private const string BooksDictionaryName = "booksInStore";

        public BookService(StatefulServiceContext serviceContext)
            : this(serviceContext, (new ReliableStateManager(serviceContext)))
        { }

        public BookService(StatefulServiceContext context, IReliableStateManagerReplica stateManagerReplica) : 
            base(context, stateManagerReplica)
        {
        }



        public async Task AddBookAsync(Book book)
        {
            IReliableDictionary<Guid, Book> books =
                            await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Book>>(BooksDictionaryName);

            ServiceEventSource.Current.ServiceMessage(this, message: "Received add book request. ID: {0}. Name: {1}.", args: new object[] { book.ID, book.BookName });

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                bool exists = await books.ContainsKeyAsync(tx, book.ID);

                if (!exists)
                    await books.AddAsync(tx, book.ID, book);

                await tx.CommitAsync();

                ServiceEventSource.Current.ServiceMessage(
                    this, message: "Received add book request. Item: {0}. Name: {1}.", args: new object[] { book.ID, book.BookName });
            }

        }

        public async Task DeleteBookAsync(Book book)
        {
            IReliableDictionary<Guid, Book> books =
                            await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Book>>(BooksDictionaryName);

            ServiceEventSource.Current.ServiceMessage(this, message: "Received delete book request. ID: {0}. Name: {1}.", args: new object[] { book.ID, book.BookName });

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                await books.TryRemoveAsync(tx, book.ID);
                await tx.CommitAsync();

                ServiceEventSource.Current.ServiceMessage(
                    this, message: "Succesfully deleted book. ID: {0}. Name: {1}.", args: new object[] { book.ID, book.BookName });
            }
        }

        public async Task<IList<Book>> GetBooksAsync(CancellationToken ct)
        {
            IReliableDictionary<Guid, Book> books =
                            await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Book>>(BooksDictionaryName);

            ServiceEventSource.Current.Message(message: "Called GetBooksAsync to return books");

            IList<Book> results = new List<Book>();

            using (ITransaction tx = this.StateManager.CreateTransaction())
            {
                ServiceEventSource.Current.Message(message: "Generating item views for {0} items", args: new object[] { await books.GetCountAsync(tx) });

                IAsyncEnumerator<KeyValuePair<Guid, Book>> enumerator =
                    (await books.CreateEnumerableAsync(tx)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(ct))
                {
                    results.Add(enumerator.Current.Value);
                }
            }


            return results;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context))
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                ServiceEventSource.Current.ServiceMessage(this, message: "inside RunAsync for Book Service");
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceMessage(this, message: "RunAsync Failed, {0}", args: new object[] { e });
                throw;
            }
        }
    }
}
