using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Budgets;
using Moq;

namespace FinanceTracker.Tests.Services
{
    public class BudgetServiceTests
    {
        private const int UserId = 5;

        private readonly Mock<IBudgetRepository> _repo = new();
        private readonly BudgetService _sut;

        public BudgetServiceTests()
        {
            _sut = new BudgetService(_repo.Object);
        }

        private static Category Food() =>
            new() { Id = 2, Name = "Food", Color = "#fff", Icon = "Cart" };

        // ── GetAllByUserAsync ────────────────────────────────────────

        [Fact]
        public async Task GetAllByUserAsync_ReturnsMappedDtos_ForRequestedMonthAndYear()
        {
            var list = new List<Budget>
            {
                new() { Id = 1, LimitAmount = 100m, Month = 10, Year = 2026, CategoryId = 2, Category = Food() },
                new() { Id = 2, LimitAmount = 200m, Month = 10, Year = 2026, CategoryId = 2, Category = Food() }
            };
            _repo.Setup(r => r.GetAllByUserAsync(UserId, 10, 2026)).ReturnsAsync(list);

            var result = await _sut.GetAllByUserAsync(UserId, 10, 2026);

            Assert.Equal(new[] { 1, 2 }, result.Select(b => b.Id));
        }

        [Fact]
        public async Task GetAllByUserAsync_WhenNoBudgets_ReturnsEmptyList()
        {
            _repo.Setup(r => r.GetAllByUserAsync(UserId, 10, 2026)).ReturnsAsync(new List<Budget>());

            var result = await _sut.GetAllByUserAsync(UserId, 10, 2026);

            Assert.Empty(result);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Budget?)null);

            var result = await _sut.GetByIdAsync(1, UserId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_MapsAllFields()
        {
            var budget = new Budget
            {
                Id = 1,
                LimitAmount = 500m,
                Month = 10,
                Year = 2026,
                CategoryId = 2,
                Category = Food(),
                AmountSpent = 120m
            };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(budget);

            var result = await _sut.GetByIdAsync(1, UserId);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(500m, result.LimitAmount);
            Assert.Equal(10, result.Month);
            Assert.Equal(2026, result.Year);
            Assert.Equal(2, result.CategoryId);
            Assert.Equal("Food", result.CategoryName);
            Assert.Equal("#fff", result.CategoryColor);
            Assert.Equal("Cart", result.CategoryIcon);
            Assert.Equal(120m, result.AmountSpent);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryIsNull_UsesEmptyNameAndNullStyling()
        {
            var budget = new Budget { Id = 1, CategoryId = 2, Category = null! };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(budget);

            var result = await _sut.GetByIdAsync(1, UserId);

            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.CategoryName);
            Assert.Null(result.CategoryColor);
            Assert.Null(result.CategoryIcon);
        }

        // ── CreateAsync ──────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WithValidCategory_SavesBudgetForCurrentUser()
        {
            var category = Food();
            _repo.Setup(r => r.GetCategoryByIdAsync(2, UserId)).ReturnsAsync(category);
            _repo.Setup(r => r.CreateAsync(It.IsAny<Budget>()))
                 .ReturnsAsync((Budget b) => b);

            var dto = new SaveBudgetDto { LimitAmount = 300m, Month = 10, Year = 2026, CategoryId = 2 };

            var result = await _sut.CreateAsync(dto, UserId);

            Assert.Equal(300m, result.LimitAmount);
            Assert.Equal(10, result.Month);
            Assert.Equal(2026, result.Year);
            Assert.Equal(2, result.CategoryId);
            Assert.Equal("Food", result.CategoryName);

            _repo.Verify(r => r.CreateAsync(It.Is<Budget>(b =>
                b.UserId == UserId &&
                b.CategoryId == 2 &&
                b.Category == category)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenCategoryNotFound_ThrowsAndDoesNotSave()
        {
            _repo.Setup(r => r.GetCategoryByIdAsync(777, UserId)).ReturnsAsync((Category?)null);

            var dto = new SaveBudgetDto { LimitAmount = 300m, CategoryId = 777 };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.CreateAsync(dto, UserId));

            _repo.Verify(r => r.CreateAsync(It.IsAny<Budget>()), Times.Never);
        }

        // Documents current behavior: a null limit is stored as 0
        [Fact]
        public async Task CreateAsync_WithNullLimit_StoresZero()
        {
            _repo.Setup(r => r.GetCategoryByIdAsync(2, UserId)).ReturnsAsync(Food());
            _repo.Setup(r => r.CreateAsync(It.IsAny<Budget>()))
                 .ReturnsAsync((Budget b) => b);

            var dto = new SaveBudgetDto { LimitAmount = null, CategoryId = 2 };

            var result = await _sut.CreateAsync(dto, UserId);

            Assert.Equal(0m, result.LimitAmount);
        }

        // ── UpdateAsync ──────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsAndDoesNotSave()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Budget?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(1, new SaveBudgetDto(), UserId));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Budget>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_UpdatesFieldsAndSaves()
        {
            var existing = new Budget
            {
                Id = 1,
                UserId = UserId,
                LimitAmount = 100m,
                Month = 9,
                Year = 2026,
                CategoryId = 2,
                Category = Food()
            };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(existing);
            _repo.Setup(r => r.UpdateAsync(It.IsAny<Budget>()))
                 .ReturnsAsync((Budget b) => b);

            var dto = new SaveBudgetDto { LimitAmount = 450m, Month = 11, Year = 2027, CategoryId = 2 };

            var result = await _sut.UpdateAsync(1, dto, UserId);

            Assert.Equal(450m, result.LimitAmount);
            Assert.Equal(11, result.Month);
            Assert.Equal(2027, result.Year);
            _repo.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        // ── DeleteAsync ──────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsAndDoesNotDelete()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Budget?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteAsync(1, UserId));

            _repo.Verify(r => r.DeleteAsync(It.IsAny<Budget>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenFound_DeletesThatBudget()
        {
            var existing = new Budget { Id = 1, UserId = UserId };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(existing);

            await _sut.DeleteAsync(1, UserId);

            _repo.Verify(r => r.DeleteAsync(existing), Times.Once);
        }
    }
}