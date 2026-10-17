using FinanceTracker.Controllers;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace FinanceTracker.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private const int UserId = 5;

        private readonly Mock<ICategoryService> _service = new();
        private readonly CategoriesController _sut;

        public CategoriesControllerTests()
        {
            _sut = new CategoriesController(_service.Object);
            SetUser(UserId);
        }

        private void SetUser(int userId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        // ── GetAll ───────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOkWithCategoriesForCurrentUser()
        {
            var list = new List<CategoryDto> { new() { Id = 1 }, new() { Id = 2 } };
            _service.Setup(s => s.GetAllByUserAsync(UserId)).ReturnsAsync(list);

            var result = await _sut.GetAll();

            // ActionResult<T>: the response is in .Result
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(list, ok.Value);
        }

        // ── GetById ──────────────────────────────────────────────────

        [Fact]
        public async Task GetById_WhenServiceReturnsNull_ReturnsNotFound()
        {
            _service.Setup(s => s.GetByIdAsync(1, UserId)).ReturnsAsync((CategoryDto?)null);

            var result = await _sut.GetById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_WhenFound_ReturnsOkWithDto()
        {
            var dto = new CategoryDto { Id = 1 };
            _service.Setup(s => s.GetByIdAsync(1, UserId)).ReturnsAsync(dto);

            var result = await _sut.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(dto, ok.Value);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(42)]
        public async Task GetById_UsesUserIdFromClaims(int userId)
        {
            SetUser(userId);
            _service.Setup(s => s.GetByIdAsync(1, userId)).ReturnsAsync(new CategoryDto { Id = 1 });

            await _sut.GetById(1);

            _service.Verify(s => s.GetByIdAsync(1, userId), Times.Once);
        }

        // ── GetTransactionCount ──────────────────────────────────────

        [Fact]
        public async Task GetTransactionCount_ReturnsOkWithCount()
        {
            _service.Setup(s => s.GetTransactionCountAsync(2, UserId)).ReturnsAsync(7);

            var result = await _sut.GetTransactionCount(2);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(7, ok.Value);
        }

        // ── Create ───────────────────────────────────────────────────

        [Fact]
        public async Task Create_ReturnsCreatedAtGetByIdWithNewId()
        {
            var input = new SaveCategoryDto { Name = "Food" };
            var created = new CategoryDto { Id = 10 };
            _service.Setup(s => s.CreateAsync(input, UserId)).ReturnsAsync(created);

            var result = await _sut.Create(input);

            var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal("GetCategoryById", createdResult.RouteName);
            Assert.Equal(10, createdResult.RouteValues!["id"]);
            Assert.Same(created, createdResult.Value);
        }

        // ── Update ───────────────────────────────────────────────────

        [Fact]
        public async Task Update_ReturnsOkWithUpdatedDto()
        {
            var input = new SaveCategoryDto { Name = "Renamed" };
            var updated = new CategoryDto { Id = 1, Name = "Renamed" };
            _service.Setup(s => s.UpdateAsync(1, input, UserId)).ReturnsAsync(updated);

            var result = await _sut.Update(1, input);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(updated, ok.Value);
        }

        // ── Delete ───────────────────────────────────────────────────

        [Fact]
        public async Task Delete_ReturnsNoContentAndDeletesForCurrentUser()
        {
            var result = await _sut.Delete(1);

            Assert.IsType<NoContentResult>(result);
            _service.Verify(s => s.DeleteAsync(1, UserId), Times.Once);
        }

        // ── Security ─────────────────────────────────────────────────

        [Fact]
        public void Controller_RequiresAuthorization()
        {
            var attribute = typeof(CategoriesController)
                .GetCustomAttribute<AuthorizeAttribute>(inherit: true);

            Assert.NotNull(attribute);
        }
    }
}