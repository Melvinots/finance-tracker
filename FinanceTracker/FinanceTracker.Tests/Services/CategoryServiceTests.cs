using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Categories;
using Moq;

namespace FinanceTracker.Tests.Services
{
    public class CategoryServiceTests
    {
        private const int UserId = 5;

        private readonly Mock<ICategoryRepository> _repo = new();
        private readonly CategoryService _sut;

        public CategoryServiceTests()
        {
            _sut = new CategoryService(_repo.Object);
        }

        // ── GetAllByUserAsync ────────────────────────────────────────

        [Fact]
        public async Task GetAllByUserAsync_ReturnsMappedDtosInOrder()
        {
            var list = new List<Category>
            {
                new() { Id = 1, Name = "Food", Color = "#fff", Icon = "Cart" },
                new() { Id = 2, Name = "Rent", Color = "#000", Icon = "Home" }
            };
            _repo.Setup(r => r.GetAllByUserAsync(UserId)).ReturnsAsync(list);

            var result = await _sut.GetAllByUserAsync(UserId);

            Assert.Equal(new[] { 1, 2 }, result.Select(c => c.Id));
            Assert.Equal(new[] { "Food", "Rent" }, result.Select(c => c.Name));
        }

        [Fact]
        public async Task GetAllByUserAsync_WhenNone_ReturnsEmptyList()
        {
            _repo.Setup(r => r.GetAllByUserAsync(UserId)).ReturnsAsync(new List<Category>());

            var result = await _sut.GetAllByUserAsync(UserId);

            Assert.Empty(result);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Category?)null);

            var result = await _sut.GetByIdAsync(1, UserId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_MapsAllFields()
        {
            var category = new Category
            {
                Id = 1,
                Name = "Food",
                Color = "#fff",
                Icon = "Cart",
                IsSystemDefault = true
            };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(category);

            var result = await _sut.GetByIdAsync(1, UserId);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Food", result.Name);
            Assert.Equal("#fff", result.Color);
            Assert.Equal("Cart", result.Icon);
            Assert.True(result.IsSystemDefault);
        }

        // ── GetTransactionCountAsync ─────────────────────────────────

        [Fact]
        public async Task GetTransactionCountAsync_ReturnsCountFromRepository()
        {
            _repo.Setup(r => r.GetTransactionCountAsync(2, UserId)).ReturnsAsync(7);

            var result = await _sut.GetTransactionCountAsync(2, UserId);

            Assert.Equal(7, result);
        }

        // ── CreateAsync ──────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_SavesCategoryForCurrentUser_AndReturnsDto()
        {
            _repo.Setup(r => r.CreateAsync(It.IsAny<Category>()))
                 .ReturnsAsync((Category c) => c);   // echo: result reflects what the service built

            var dto = new SaveCategoryDto { Name = "Food", Color = "#fff", Icon = "Cart" };

            var result = await _sut.CreateAsync(dto, UserId);

            Assert.Equal("Food", result.Name);
            Assert.Equal("#fff", result.Color);
            Assert.Equal("Cart", result.Icon);
            Assert.False(result.IsSystemDefault);

            // What was actually saved
            _repo.Verify(r => r.CreateAsync(It.Is<Category>(c =>
                c.UserId == UserId &&
                c.Name == "Food" &&
                c.Color == "#fff" &&
                c.Icon == "Cart" &&
                !c.IsSystemDefault)), Times.Once);
        }

        // ── UpdateAsync ──────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_WhenNotFound_ThrowsAndDoesNotSave()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Category?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateAsync(1, new SaveCategoryDto(), UserId));

            _repo.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenFound_UpdatesFieldsOnFetchedCategoryAndSaves()
        {
            var existing = new Category
            {
                Id = 1,
                UserId = UserId,
                Name = "Old",
                Color = "#000",
                Icon = "OldIcon"
            };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(existing);
            _repo.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                 .ReturnsAsync((Category c) => c);

            var dto = new SaveCategoryDto { Name = "New", Color = "#fff", Icon = "Cart" };

            var result = await _sut.UpdateAsync(1, dto, UserId);

            Assert.Equal("New", result.Name);
            Assert.Equal("#fff", result.Color);
            Assert.Equal("Cart", result.Icon);

            // The SAME fetched object must be saved (not a new one), and its identity must not change
            _repo.Verify(r => r.UpdateAsync(existing), Times.Once);
            Assert.Equal(1, existing.Id);
            Assert.Equal(UserId, existing.UserId);
        }

        // ── DeleteAsync ──────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ThrowsAndDoesNotDelete()
        {
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync((Category?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteAsync(1, UserId));

            _repo.Verify(r => r.DeleteWithReassignmentAsync(
                It.IsAny<Category>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenSystemDefault_ThrowsAndDoesNotDelete()
        {
            var systemCategory = new Category { Id = 1, UserId = UserId, IsSystemDefault = true };
            _repo.Setup(r => r.GetByIdAsync(1, UserId)).ReturnsAsync(systemCategory);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteAsync(1, UserId));

            // The guard runs first: no fallback lookup, no delete
            _repo.Verify(r => r.GetSystemDefaultAsync(It.IsAny<int>()), Times.Never);
            _repo.Verify(r => r.DeleteWithReassignmentAsync(
                It.IsAny<Category>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenFallbackMissing_ThrowsAndDoesNotDelete()
        {
            var category = new Category { Id = 2, UserId = UserId };
            _repo.Setup(r => r.GetByIdAsync(2, UserId)).ReturnsAsync(category);
            _repo.Setup(r => r.GetSystemDefaultAsync(UserId)).ReturnsAsync((Category?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DeleteAsync(2, UserId));

            _repo.Verify(r => r.DeleteWithReassignmentAsync(
                It.IsAny<Category>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenValid_DeletesAndReassignsToSystemDefault()
        {
            var category = new Category { Id = 2, UserId = UserId };
            var fallback = new Category { Id = 99, UserId = UserId, IsSystemDefault = true };
            _repo.Setup(r => r.GetByIdAsync(2, UserId)).ReturnsAsync(category);
            _repo.Setup(r => r.GetSystemDefaultAsync(UserId)).ReturnsAsync(fallback);

            await _sut.DeleteAsync(2, UserId);

            // Transactions must move to the fallback (99), for this user
            _repo.Verify(r => r.DeleteWithReassignmentAsync(category, 99, UserId), Times.Once);
        }
    }
}