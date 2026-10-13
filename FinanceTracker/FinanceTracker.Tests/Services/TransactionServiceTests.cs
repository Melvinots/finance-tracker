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

        // ── GetAll ───────────────────────────────────────────────────

        [Fact]
        public async Task GetAllByUserAsync_ReturnsMappedDtos()
        {
            var category = new Category { Id = 2, Name = "Food", Color = "#fff", Icon = "Cart" };
            var list = new List<Transaction>
                {
                    new() { Id = 1, Amount = 10m, CategoryId = 2, Category = category },
                    new() { Id = 2, Amount = 20m, CategoryId = 2, Category = category }
                };
            
            _repo.Setup(r => r.GetAllByUserAsync(5)).ReturnsAsync(list);

            var result = await _sut.GetAllByUserAsync(5);

            Assert.Equal(2, result.Count);
            Assert.Equal(new[] { 1, 2 }, result.Select(r => r.Id));
        }

        [Fact]
        public async Task GetAllByUserAsync_WhenNoTransactions_ReturnsEmptyList()
        {
            _repo.Setup(r => r.GetAllByUserAsync(5)).ReturnsAsync(new List<Transaction>());

            var result = await _sut.GetAllByUserAsync(5);

            Assert.Empty(result);
        }

        // ── GetById ──────────────────────────────────────────────────

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
                Date = new DateTime(2026, 10, 1),
                IsExpense = true,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Food", Color = "#fff", Icon = "Cart" }
            };
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync(transaction);

            var result = await _sut.GetByIdAsync(1, 5);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(50m, result.Amount);
            Assert.Equal("Groceries", result.Description);
            Assert.Equal(new DateTime(2026, 10, 1), result.Date);
            Assert.True(result.IsExpense);
            Assert.Equal(2, result.CategoryId);
            Assert.Equal("Food", result.CategoryName);
            Assert.Equal("Cart", result.CategoryIcon);
            Assert.Equal("#fff", result.CategoryColor);
        }

        // ── Create ───────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WithValidCategory_UsesThatCategory()
        {
            var category = new Category { Id = 2, Name = "Food", Color = "#fff", Icon = "Cart" };

            _repo.Setup(r => r.GetCategoryByIdAsync(2, 5)).ReturnsAsync(category);
            _repo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);   // return what was passed in

            var dto = new SaveTransactionDto
            {
                Amount = 10m,
                Description = "Groceries",
                Date = new DateTime(2026, 10, 1),
                IsExpense = true,
                Notes = "Weekly shopping",
                CategoryId = 2
            };

            var result = await _sut.CreateAsync(dto, 5);

            Assert.Equal(10m, result.Amount);
            Assert.Equal("Groceries", result.Description);
            Assert.Equal(new DateTime(2026, 10, 1), result.Date);
            Assert.True(result.IsExpense);
            Assert.Equal("Weekly shopping", result.Notes);
            Assert.Equal(2, result.CategoryId);

            _repo.Verify(r => r.GetOrCreateUncategorizedAsync(It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.CreateAsync(It.Is<Transaction>(t => t.UserId == 5)), Times.Once);
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

            _repo.Verify(r => r.GetOrCreateUncategorizedAsync(5), Times.Once);
            _repo.Verify(r => r.CreateAsync(It.Is<Transaction>(t =>
                t.CategoryId == 99 &&
                t.Category == fallback &&
                t.UserId == 5)), Times.Once);
        }

        // ── Update ───────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_WhenTransactionNotFound_ThrowsKeyNotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync((Transaction?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(1, new SaveTransactionDto(), 5));

            _repo.Verify(r => r.GetCategoryByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenNewCategoryNotFound_ThrowsKeyNotFound()
        {
            var existing = new Transaction
            {
                Id = 1,
                UserId = 5,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Food" }
            };
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync(existing);
            _repo.Setup(r => r.GetCategoryByIdAsync(777, 5)).ReturnsAsync((Category?)null);

            var dto = new SaveTransactionDto { Amount = 10m, CategoryId = 777 };

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(1, dto, 5));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryUnchanged_DoesNotLookUpCategory()
        {
            var existing = new Transaction
            {
                Id = 1,
                UserId = 5,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Food", Color = "#fff", Icon = "Cart" }
            };
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync(existing);
            _repo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);

            var dto = new SaveTransactionDto { Amount = 25m, Description = "Updated", CategoryId = 2 };

            var result = await _sut.UpdateAsync(1, dto, 5);

            Assert.Equal(25m, result.Amount);
            _repo.Verify(r => r.GetCategoryByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryChanged_UpdatesFieldsAndCategory()
        {
            var existing = new Transaction
            {
                Id = 1,
                UserId = 5,
                CategoryId = 2,
                Category = new Category { Id = 2, Name = "Food" }
            };
            var newCategory = new Category { Id = 3, Name = "Transport", Color = "#000", Icon = "Bus" };

            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync(existing);
            _repo.Setup(r => r.GetCategoryByIdAsync(3, 5)).ReturnsAsync(newCategory);
            _repo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);

            var dto = new SaveTransactionDto
            {
                Amount = 25m,
                Description = "Taxi",
                Date = new DateTime(2026, 10, 2),
                IsExpense = true,
                Notes = "Late night",
                CategoryId = 3
            };

            var result = await _sut.UpdateAsync(1, dto, 5);

            Assert.Equal(25m, result.Amount);
            Assert.Equal("Taxi", result.Description);
            Assert.Equal("Late night", result.Notes);
            Assert.Equal(3, result.CategoryId);
            Assert.Equal("Transport", result.CategoryName);
            _repo.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        // ── Delete ───────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsAndDoesNotDelete()
        {
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync((Transaction?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteAsync(1, 5));

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_DeletesThatTransaction()
        {
            var existing = new Transaction { Id = 1, UserId = 5 };
            _repo.Setup(r => r.GetByIdAsync(1, 5)).ReturnsAsync(existing);

            await _sut.DeleteAsync(1, 5);

            _repo.Verify(r => r.DeleteAsync(existing), Times.Once);
        }
    }
}
