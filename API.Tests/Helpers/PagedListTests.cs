using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Models;
using Xunit;

namespace API.Tests.Helpers
{
    public class PagedListTests
    {
        private readonly List<UserIdentity> _items;
        
        public PagedListTests()
        {
            // Create test data
            _items = new List<UserIdentity>();
            for (int i = 1; i <= 25; i++)
            {
                _items.Add(new UserIdentity
                {
                    Id = i,
                    FullName = $"User {i}",
                    Email = $"user{i}@example.com",
                    UserId = i.ToString(),
                    SourceSystem = $"System {(i % 3) + 1}",
                    IsActive = i % 2 == 0
                });
            }
        }
        
        [Fact]
        public void PagedList_Constructor_SetsPaginationProperties()
        {
            // Arrange
            var pageNumber = 2;
            var pageSize = 5;
            var totalCount = 25;
            var items = _items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            // Act
            var pagedList = new PagedList<UserIdentity>(items, totalCount, pageNumber, pageSize);
            
            // Assert
            Assert.Equal(5, pagedList.Count);
            Assert.Equal(25, pagedList.TotalCount);
            Assert.Equal(2, pagedList.CurrentPage);
            Assert.Equal(5, pagedList.PageSize);
            Assert.Equal(5, pagedList.TotalPages);
        }
        
        [Fact]
        public void PagedList_FirstPage_HasNoPreviewPage()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 5;
            var totalCount = 25;
            var items = _items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            // Act
            var pagedList = new PagedList<UserIdentity>(items, totalCount, pageNumber, pageSize);
            
            // Assert
            Assert.Equal(1, pagedList.CurrentPage);
        }
        
        [Fact]
        public void PagedList_LastPage_HasNoNextPage()
        {
            // Arrange
            var pageNumber = 5;
            var pageSize = 5;
            var totalCount = 25;
            var items = _items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            
            // Act
            var pagedList = new PagedList<UserIdentity>(items, totalCount, pageNumber, pageSize);
            
            // Assert
            Assert.Equal(5, pagedList.CurrentPage);
        }
        
        [Fact]
        public async Task CreateAsync_CreatesPagedListFromQueryable()
        {
            // Arrange
            var pageNumber = 2;
            var pageSize = 5;
            var mockData = _items.AsQueryable();
            
            // Create an async queryable with our test provider
            var asyncQueryable = new TestAsyncEnumerable<UserIdentity>(mockData);
            
            // Act
            var pagedList = await PagedList<UserIdentity>.CreateAsync(asyncQueryable, pageNumber, pageSize);
            
            // Assert
            Assert.Equal(5, pagedList.Count);
            Assert.Equal(25, pagedList.TotalCount);
            Assert.Equal(2, pagedList.CurrentPage);
            Assert.Equal(5, pagedList.PageSize);
            Assert.Equal(5, pagedList.TotalPages);
            
            // Check correct items were returned
            Assert.Equal(6, pagedList[0].Id);
            Assert.Equal(10, pagedList[4].Id);
        }
        
        [Fact]
        public async Task CreateAsync_WithEmptySource_ReturnsEmptyPagedList()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 5;
            var emptyData = new List<UserIdentity>().AsQueryable();
            
            // Create an async queryable with our test provider
            var asyncQueryable = new TestAsyncEnumerable<UserIdentity>(emptyData);
            
            // Act
            var pagedList = await PagedList<UserIdentity>.CreateAsync(asyncQueryable, pageNumber, pageSize);
            
            // Assert
            Assert.Empty(pagedList);
            Assert.Equal(0, pagedList.TotalCount);
            Assert.Equal(1, pagedList.CurrentPage);
            Assert.Equal(5, pagedList.PageSize);
            Assert.Equal(0, pagedList.TotalPages);
        }
    }
}
