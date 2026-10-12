using FinanceTracker.Controllers;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FinanceTracker.Tests.Controller
{
    public class TransactionsControllerTests
    {
        private readonly Mock<ITransactionService> _service = new();
        private readonly TransactionsController _sut;

        public TransactionsControllerTests()
        {
            _sut = new TransactionsController(_service.Object);

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "5") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task GetById_WhenServiceReturnsNull_ReturnsNotFound()
        {
            _service.Setup(s => s.GetByIdAsync(1, 5)).ReturnsAsync((TransactionDto?)null);

            var result = await _sut.GetById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_WhenFound_ReturnsOkWithDto()
        {
            var dto = new TransactionDto { Id = 1 };
            _service.Setup(s => s.GetByIdAsync(1, 5)).ReturnsAsync(dto);

            var result = await _sut.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(dto, ok.Value);
        }
    }
}
