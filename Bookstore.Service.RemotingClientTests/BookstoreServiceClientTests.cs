using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore.Service.Domain;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;
using System.Threading;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Service.RemotingClientTests
{
    [TestClass]
    public class BookstoreServiceClientTests
    {
        private ICodePackageActivationContext ActivationContext { get { return FabricRuntime.GetActivationContext(); } }

        [TestMethod]
        public async Task TestAddBookRemotingClient()
        {
            //arrange
            IBookstoreService bookService = ServiceProxy.Create<IBookstoreService>(new Uri("fabric:/Bookstore/Service"), new ServicePartitionKey((long) 1));
            string expectedBookName = "Introduction to Service Fabric";
            Guid expectedId = Guid.NewGuid();
            string expectedPrice = "$1.95";

            //act
            await bookService.AddBookAsync(new Book() { BookName = expectedBookName, ID = expectedId, Price = expectedPrice });

            IList<Book> books = await bookService.GetBooksAsync(CancellationToken.None);

            //assert
            Book actualBook = books.First(b => b.ID == expectedId);
            Assert.AreEqual(actualBook.BookName, expectedBookName);
            Assert.AreEqual(actualBook.Price, expectedPrice);
        }
    }
}
