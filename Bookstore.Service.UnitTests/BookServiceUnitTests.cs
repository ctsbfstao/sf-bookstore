using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bookstore.UnitTests.Mocks;
using Bookstore.Service.Domain;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace Bookstore.Service.UnitTests
{
    [TestClass]
    public class BookServiceUnitTests
    {
        private static readonly ICodePackageActivationContext codePackageContext = new MockCodePackageActivationContext(
    "fabric:/someapp",
    "SomeAppType",
    "Code",
    "1.0.0.0",
    Guid.NewGuid().ToString(),
    @"C:\Log",
    @"C:\Temp",
    @"C:\Work",
    "ServiceManifest",
    "1.0.0.0"
    );


        readonly StatefulServiceContext statefulServiceContext = new StatefulServiceContext(
    new NodeContext("Node0", new NodeId(0, 1), 0, "NodeType1", "TEST.MACHINE"),
    codePackageContext,
    "ServiceType",
    new Uri("fabric:/someapp/someservice"),
    null,
    Guid.NewGuid(),
    long.MaxValue
    );

        [TestMethod]
        public async Task TestAddBook()
        {
            var target = new BookService(statefulServiceContext, new MockReliableStateManager());
            var expected = new Book { BookName = "Azure", ID = Guid.NewGuid(), Price = "34.95", IconName = "Azure" };

            //add book
            await target.AddBookAsync(expected);

            //verify book exists
            System.Collections.Generic.IList<Book> books = await target.GetBooksAsync(CancellationToken.None);
            bool resultTrue = books.Contains(expected);

            Assert.IsTrue(resultTrue);
        }

        [TestMethod]
        public async Task TestDeleteBook()
        {
            var target = new BookService(statefulServiceContext, new MockReliableStateManager());
            var expected = new Book { BookName = "Azure", ID = Guid.NewGuid(), Price = "34.95", IconName = "Azure" };

            //add book
            await target.AddBookAsync(expected);

            //delete book
            await target.DeleteBookAsync(expected);
            System.Collections.Generic.IList<Book> books = await target.GetBooksAsync(CancellationToken.None);

            //verify book is deleted
            Assert.IsTrue(books.Count == 0);
        }
    }
}
