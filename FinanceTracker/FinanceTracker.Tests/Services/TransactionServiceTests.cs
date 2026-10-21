using FinanceTracker.Exceptions;
using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Transactions;
using Moq;

namespace FinanceTracker.Tests.Services
{
    public class TransactionServiceTests
    {
        private const int UserId = 5;

        private readonly Mock<ITransactionRepository> _repo = new();
        private readonly TransactionService _sut;   // "system under test"

        public TransactionServiceTests()
        {
            _sut = new TransactionService(_repo.Object);
        }

        private static Category Food() =>
            new() { Id = 2, Name = "Food", Color = "#fff", Icon = "Cart" };

        // ── GetAllByUserAsync ────────────────────────────────────────

        [Fact]
        public async Task GetAllByUserAsync_ReturnsMappedDtosInRepositoryOrder()
        {
            var list = new List<Transaction>
            {
                new() { Id = 1, Amount = 10m, CategoryId = 2, Category = Food() },
                new() { Id = 2, Amount = 20m, CategoryId = 2, Category = Food() }
            };
            _repo.Setup(r => r.GetAllByUserAsync(UserId)).ReturnsAsync(list);

            var result = await _sut.GetAllByUserAsync(UserId);

            Assert.Equal(new[] { 1, 2 }, result.Select(t => t.Id));
        }

        [Fact]
        public async Task GetAllByUserAsync_WhenNone_ReturnsEmptyList()
        {
            _repo.Setup(r => r.GetAllByUserAsync(UserId)).ReturnsAsync(new List<Transaction>());

            var result = await _sut.GetAllByUserAsync(UserId);

            Assert.Empty(result);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Transaction?)null);

            var result = await _sut.GetByIdAsync(1, UserId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_MapsAllFields()
        {
            var transaction = new Transaction
            {
                Id = 1,
                Amount = 50m,
                Description = "Groceries",
                Date = new DateTime(2026, 10, 1),
                IsExpense = true,
                Notes = "Weekly shopping",
                CategoryId = 2,
                Category = Food()
            };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(transaction);

            var result = await _sut.GetByIdAsync(1, UserId);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(50m, result.Amount);
            Assert.Equal("Groceries", result.Description);
            Assert.Equal(new DateTime(2026, 10, 1), result.Date);
            Assert.True(result.IsExpense);
            Assert.Equal("Weekly shopping", result.Notes);
            Assert.Equal(2, result.CategoryId);
            Assert.Equal("Food", result.CategoryName);
            Assert.Equal("#fff", result.CategoryColor);
            Assert.Equal("Cart", result.CategoryIcon);
        }

        // ── CreateAsync ──────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WithValidCategory_MapsAllFieldsAndSavesForCurrentUser()
        {
            var category = Food();
            _repo.Setup(r => r.GetCategoryByIdAsync(2, UserId)).ReturnsAsync(category);
            _repo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);   // echo: result reflects what the service built

            var dto = new SaveTransactionDto
            {
                Amount = 10m,
                Description = "Groceries",
                Date = new DateTime(2026, 10, 1),
                IsExpense = true,
                Notes = "Weekly shopping",
                CategoryId = 2
            };

            var result = await _sut.CreateAsync(dto, UserId);

            Assert.Equal(10m, result.Amount);
            Assert.Equal("Groceries", result.Description);
            Assert.Equal(new DateTime(2026, 10, 1), result.Date);
            Assert.True(result.IsExpense);
            Assert.Equal("Weekly shopping", result.Notes);
            Assert.Equal(2, result.CategoryId);
            Assert.Equal("Food", result.CategoryName);

            _repo.Verify(r => r.GetOrCreateUncategorizedAsync(It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.CreateAsync(It.Is<Transaction>(t =>
                t.UserId == UserId &&
                t.CategoryId == 2 &&
                t.Category == category)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithUnknownCategory_FallsBackToUncategorized()
        {
            var fallback = new Category { Id = 99, Name = "Other", IsSystemDefault = true };

            _repo.Setup(r => r.GetCategoryByIdAsync(123, UserId)).ReturnsAsync((Category?)null);
            _repo.Setup(r => r.GetOrCreateUncategorizedAsync(UserId)).ReturnsAsync(fallback);
            _repo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);

            var dto = new SaveTransactionDto { Amount = 10m, CategoryId = 123 };

            var result = await _sut.CreateAsync(dto, UserId);

            Assert.Equal(99, result.CategoryId);
            Assert.Equal("Other", result.CategoryName);

            _repo.Verify(r => r.GetOrCreateUncategorizedAsync(UserId), Times.Once);
            _repo.Verify(r => r.CreateAsync(It.Is<Transaction>(t =>
                t.CategoryId == 99 &&
                t.Category == fallback &&
                t.UserId == UserId)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNullAmount_ThrowsBeforeTouchingTheRepository()
        {
            var dto = new SaveTransactionDto { Amount = null, CategoryId = 2 };

            await Assert.ThrowsAsync<ValidationFailedException>(() => _sut.CreateAsync(dto, UserId));

            // Validation runs first: no category lookup, no fallback creation, no save
            _repo.Verify(r => r.GetCategoryByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.GetOrCreateUncategorizedAsync(It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Never);
        }

        // ── UpdateAsync ──────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_WithNullAmount_ThrowsBeforeTouchingTheRepository()
        {
            var dto = new SaveTransactionDto { Amount = null, CategoryId = 2 };

            await Assert.ThrowsAsync<ValidationFailedException>(() => _sut.UpdateAsync(1, dto, UserId));

            _repo.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenTransactionNotFound_ThrowsKeyNotFound()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Transaction?)null);

            // Amount must be valid, or the amount check would throw first
            var dto = new SaveTransactionDto { Amount = 10m, CategoryId = 2 };

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateAsync(1, dto, UserId));

            _repo.Verify(r => r.GetCategoryByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenNewCategoryNotFound_ThrowsKeyNotFoundAndDoesNotSave()
        {
            var existing = new Transaction
            {
                Id = 1,
                UserId = UserId,
                CategoryId = 2,
                Category = Food()
            };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(existing);
            _repo.Setup(r => r.GetCategoryByIdAsync(777, UserId)).ReturnsAsync((Category?)null);

            var dto = new SaveTransactionDto { Amount = 10m, CategoryId = 777 };

            await Assert.ThrowsAsync<ValidationFailedException>(() => _sut.UpdateAsync(1, dto, UserId));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryChanged_UpdatesFieldsAndCategory()
        {
            var existing = new Transaction
            {
                Id = 1,
                UserId = UserId,
                CategoryId = 2,
                Category = Food()
            };
            var transport = new Category { Id = 3, Name = "Transport", Color = "#000", Icon = "Bus" };

            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(existing);
            _repo.Setup(r => r.GetCategoryByIdAsync(3, UserId)).ReturnsAsync(transport);
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

            var result = await _sut.UpdateAsync(1, dto, UserId);

            Assert.Equal(25m, result.Amount);
            Assert.Equal("Taxi", result.Description);
            Assert.Equal(new DateTime(2026, 10, 2), result.Date);
            Assert.True(result.IsExpense);
            Assert.Equal("Late night", result.Notes);
            Assert.Equal(3, result.CategoryId);
            Assert.Equal("Transport", result.CategoryName);

            // The SAME fetched object was modified and saved, and its identity is intact
            _repo.Verify(r => r.UpdateAsync(existing), Times.Once);
            Assert.Equal(1, existing.Id);
            Assert.Equal(UserId, existing.UserId);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryUnchanged_DoesNotLookUpCategory()
        {
            var existing = new Transaction
            {
                Id = 1,
                UserId = UserId,
                CategoryId = 2,
                Category = Food()
            };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(existing);
            _repo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);

            var dto = new SaveTransactionDto { Amount = 25m, Description = "Updated", CategoryId = 2 };

            var result = await _sut.UpdateAsync(1, dto, UserId);

            Assert.Equal(25m, result.Amount);
            Assert.Equal("Updated", result.Description);
            Assert.Equal("Food", result.CategoryName);   // category kept as-is
            _repo.Verify(r => r.GetCategoryByIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        // ── DeleteAsync ──────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsAndDoesNotDelete()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Transaction?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteAsync(1, UserId));

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Transaction>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_DeletesThatTransaction()
        {
            var existing = new Transaction { Id = 1, UserId = UserId };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(existing);

            await _sut.DeleteAsync(1, UserId);

            _repo.Verify(r => r.DeleteAsync(existing), Times.Once);
        }
    }
}