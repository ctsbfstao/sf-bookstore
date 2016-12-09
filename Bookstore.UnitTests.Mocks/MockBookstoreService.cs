
// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Bookstore.UnitTests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Bookstore.Service.Domain;

    public class MockBookstoreService : IBookstoreService
    {
        public void MockInventoryService()
        {
            this.AddBookAsyncFunc = (book) => Task.FromResult(result: true);
            this.GetBooksAsyncFunc = (ct) => Task.FromResult<IList<Book>>(new List<Book>());
            this.DeleteBookAsyncFunc = (book) => Task.FromResult(result: true);
        }

        public Func<Book, Task<bool>> AddBookAsyncFunc { get; set; }
        public Func<CancellationToken, Task<IList<Book>>> GetBooksAsyncFunc { get; set; }
        public Func<Book, Task<bool>> DeleteBookAsyncFunc { get; set; }


        public Task AddBookAsync(Book book)
        {
            return this.AddBookAsyncFunc(book);
        }

        public Task<IList<Book>> GetBooksAsync(CancellationToken ct)
        {
            return this.GetBooksAsyncFunc(ct);
        }

        public Task DeleteBookAsync(Book book)
        {
            return this.DeleteBookAsyncFunc(book);
        }

    }
}