using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaymentProcessingSystem.Abstractions;
using PaymentProcessingSystem.Abstractions.Models;
using PaymentProcessingSystem.Gateways;
using PaymentProcessingSystem.Gateways.Models;
using PaymentProcessingSystem.Models;
using PaymentProcessingSystem.Models.Events;
using PaymentProcessingSystem.Models.Response;
using PaymentProcessingSystem.Repositories;
using PaymentProcessingSystem.RequestHandlers;
using PaymentProcessingSystem.Requests;
using PaymentProcessingSystem.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppTests
{
    [TestClass]
    public class ProcessPaymentRequestHandlerTests
    {
        private Mock<IGuidGenerator> _mockGuidGenerator;
        private Mock<IPaymentRepository> _mockPaymentRepository;
        private Mock<ISystemClock> _mockSystemClock;
        private Mock<IPublishEndpoint> _mockPublishEndpoint;
        private Mock<IFraudDetectionService> _mockFraudDetectionService;
        private Mock<IPaymentGateway> _mockPaymentGateway;
        private Mock<ILogger<ProcessPaymentRequestHandler>> _mockLogger;
        private ProcessPaymentRequestHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockGuidGenerator = new Mock<IGuidGenerator>();
            _mockPaymentRepository = new Mock<IPaymentRepository>();
            _mockSystemClock = new Mock<ISystemClock>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _mockFraudDetectionService = new Mock<IFraudDetectionService>();
            _mockPaymentGateway = new Mock<IPaymentGateway>();
            _mockLogger = new Mock<ILogger<ProcessPaymentRequestHandler>>();

            _handler = new ProcessPaymentRequestHandler(
                _mockGuidGenerator.Object,
                _mockPaymentRepository.Object,
                _mockSystemClock.Object,
                _mockPublishEndpoint.Object,
                _mockFraudDetectionService.Object,
                _mockPaymentGateway.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_ShouldProcessPaymentSuccessfully()
        {
            var paymentId = Guid.NewGuid();
            var request = new ProcessPaymentRequest
            {
                UserId = Guid.NewGuid(),
                Amount = 100.00m,
                PaymentMethod = "CreditCard",
                Currency = Currency.USD
            };

            _mockGuidGenerator.Setup(x => x.NewGuid()).Returns(paymentId);
            _mockSystemClock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
            _mockFraudDetectionService.Setup(x => x.CheckForFraudAsync(request.UserId, request.Amount, request.PaymentMethod, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockPaymentGateway.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentGatewayProcessRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentGatewayResponse { IsSuccess = true });

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(paymentId, result.PaymentId);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Message);

            _mockPaymentRepository.Verify(x => x.SaveAsync(It.Is<Payment>(p => p.Id == paymentId), It.IsAny<CancellationToken>()), Times.Once);
            _mockPaymentRepository.Verify(x => x.UpdateAsync(It.Is<Payment>(p => p.Id == paymentId), It.IsAny<CancellationToken>()), Times.Once);
            _mockPublishEndpoint.Verify(x => x.Publish(It.Is<PaymentProcessedEvent>(e => e.PaymentId == paymentId && e.Status == PaymentStatus.Completed), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldMarkPaymentAsFraudWhenFraudDetected()
        {
            var paymentId = Guid.NewGuid();
            var request = new ProcessPaymentRequest
            {
                UserId = Guid.NewGuid(),
                Amount = 100.00m,
                PaymentMethod = "CreditCard",
                Currency = Currency.USD
            };

            _mockGuidGenerator.Setup(x => x.NewGuid()).Returns(paymentId);
            _mockSystemClock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            _mockFraudDetectionService
                .Setup(x => x.CheckForFraudAsync(request.UserId, request.Amount, request.PaymentMethod, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(paymentId, result.PaymentId);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"Fraud detected for Payment ID: {paymentId}. Payment rejected.", result.Message);

            _mockPaymentRepository.Verify(x => x.SaveAsync(It.Is<Payment>(p => p.Id == paymentId), It.IsAny<CancellationToken>()), Times.Once);
            _mockPaymentRepository.Verify(x => x.UpdateAsync(It.Is<Payment>(p => p.Id == paymentId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldMarkPaymentAsFailedWhenGatewayFails()
        {
            var paymentId = Guid.NewGuid();
            var request = new ProcessPaymentRequest
            {
                UserId = Guid.NewGuid(),
                Amount = 100.00m,
                PaymentMethod = "CreditCard",
                Currency = Currency.USD
            };

            _mockGuidGenerator.Setup(x => x.NewGuid()).Returns(paymentId);
            _mockSystemClock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

            _mockFraudDetectionService
                .Setup(x => x.CheckForFraudAsync(request.UserId, request.Amount, request.PaymentMethod, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mockPaymentGateway
                .Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentGatewayProcessRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PaymentGatewayResponse { IsSuccess = false, ErrorMessage = "Insufficient funds" });

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(paymentId, result.PaymentId);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"Payment failed for Payment ID: {paymentId}. Reason: Insufficient funds", result.Message);

            _mockPaymentRepository.Verify(x => x.SaveAsync(It.Is<Payment>(p => p.Id == paymentId), It.IsAny<CancellationToken>()), Times.Once);
            _mockPaymentRepository.Verify(x => x.UpdateAsync(It.Is<Payment>(p => p.Id == paymentId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}