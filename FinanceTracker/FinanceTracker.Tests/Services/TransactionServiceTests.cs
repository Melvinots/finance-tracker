using FinanceTracker.Models;
using FinanceTracker.Services;
using FinanceTracker.Repositories;
using FinanceTracker.Shared.DTOs.Transactions;
using Moq;

namespace FinanceTracker.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionRepository> _repo = new();
        private readonly TransactionService _sut;   // "system under test"

        public TransactionServiceTests()
        {
            _sut = new TransactionService(_repo.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange: repo says "nothing found"
            _repo.Setup(r => r.GetByIdAsync(1, 5))
                 .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _sut.GetByIdAsync(1, 5);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_MapsCategoryFieldsToDto()
        {
            var transaction = new Transaction
            {
                Id = 1,
                Amount = 50m,
                Description = "Groceries",
                IsExpense = true,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Food", Color = "#fff", Icon = "Cart" }
            };
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync(transaction);

            var result = await _sut.GetByIdAsync(1, 5);

            Assert.NotNull(result);
            Assert.Equal(50m, result!.Amount);
            Assert.Equal("Food", result.CategoryName);
            Assert.Equal("Cart", result.CategoryIcon);
        }

        [Fact]
        public async Task CreateAsync_WithValidCategory_UsesThatCategory()
        {
            var category = new Category { Id = 2, Name = "Food" };
            _repo.Setup(r => r.GetCategoryByIdAsync(2, 5)).ReturnsAsync(category);
            _repo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);   // return what was passed in

            var dto = new SaveTransactionDto { Amount = 10m, CategoryId = 2 };

            var result = await _sut.CreateAsync(dto, 5);

            Assert.Equal(2, result.CategoryId);
            _repo.Verify(r => r.GetOrCreateUncategorizedAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithUnknownCategory_FallsBackToUncategorized()
        {
            var fallback = new Category { Id = 99, Name = "Other", IsSystemDefault = true };
            _repo.Setup(r => r.GetCategoryByIdAsync(123, 5)).ReturnsAsync((Category?)null);
            _repo.Setup(r => r.GetOrCreateUncategorizedAsync(5)).ReturnsAsync(fallback);
            _repo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);

            var dto = new SaveTransactionDto { Amount = 10m, CategoryId = 123 };

            var result = await _sut.CreateAsync(dto, 5);

            Assert.Equal(99, result.CategoryId);
            Assert.Equal("Other", result.CategoryName);
        }

        [Fact]
        public async Task UpdateAsync_WhenTransactionNotFound_ThrowsKeyNotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync((Transaction?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(1, new SaveTransactionDto(), 5));
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsAndDoesNotDelete()
        {
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync((Transaction?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAsync(1, 5));

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Transaction>()), Times.Never);
        }
    }
}
