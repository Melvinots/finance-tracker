using FinanceTracker.Controllers;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace FinanceTracker.Tests.Controllers
{
    public class TransactionsControllerTests
    {
        private const int UserId = 5;

        private readonly Mock<ITransactionService> _service = new();
        private readonly TransactionsController _sut;

        public TransactionsControllerTests()
        {
            _sut = new TransactionsController(_service.Object);
            SetUser(UserId);
        }

        // Fakes a logged-in user with the given ID in the claims
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
        public async Task GetAll_ReturnsOkWithTransactionsForCurrentUser()
        {
            var list = new List<TransactionDto> { new() { Id = 1 }, new() { Id = 2 } };
            _service.Setup(s => s.GetAllByUserAsync(UserId)).ReturnsAsync(list);

            var result = await _sut.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(list, ok.Value);
        }

        // ── GetById ──────────────────────────────────────────────────

        [Fact]
        public async Task GetById_WhenServiceReturnsNull_ReturnsNotFound()
        {
            _service.Setup(s => s.GetByIdAsync(1, UserId)).ReturnsAsync((TransactionDto?)null);

            var result = await _sut.GetById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_WhenFound_ReturnsOkWithDto()
        {
            var dto = new TransactionDto { Id = 1 };
            _service.Setup(s => s.GetByIdAsync(1, UserId)).ReturnsAsync(dto);

            var result = await _sut.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(dto, ok.Value);
        }

        // Proves the user ID comes from the claim, not a hard-coded value
        [Theory]
        [InlineData(5)]
        [InlineData(42)]
        public async Task GetById_UsesUserIdFromClaims(int userId)
        {
            SetUser(userId);
            _service.Setup(s => s.GetByIdAsync(1, userId))
                    .ReturnsAsync(new TransactionDto { Id = 1 });

            await _sut.GetById(1);

            _service.Verify(s => s.GetByIdAsync(1, userId), Times.Once);
        }

        // ── Create ───────────────────────────────────────────────────

        [Fact]
        public async Task Create_ReturnsCreatedAtGetByIdWithNewId()
        {
            var input = new SaveTransactionDto { Amount = 10m, CategoryId = 2 };
            var created = new TransactionDto { Id = 10 };
            _service.Setup(s => s.CreateAsync(input, UserId)).ReturnsAsync(created);

            var result = await _sut.Create(input);

            var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal("GetTransactionById", createdResult.RouteName);
            Assert.Equal(10, createdResult.RouteValues!["id"]);
            Assert.Same(created, createdResult.Value);
        }

        // ── Update ───────────────────────────────────────────────────

        [Fact]
        public async Task Update_ReturnsOkWithUpdatedDto()
        {
            var input = new SaveTransactionDto { Amount = 25m, CategoryId = 2 };
            var updated = new TransactionDto { Id = 1, Amount = 25m };
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
            var attribute = typeof(TransactionsController)
                .GetCustomAttribute<AuthorizeAttribute>(inherit: true);

            Assert.NotNull(attribute);
        }
    }
}