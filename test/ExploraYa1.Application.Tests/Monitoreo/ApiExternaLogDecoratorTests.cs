using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ExploraYa1.Destinos;
using ExploraYa1.Monitoreo;
using Microsoft.Extensions.Logging;
using Moq;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace ExploraYa1.Tests.Monitoreo;

public class ApiExternaLogDecoratorTests
{
    private readonly Mock<ICitySearchService> _innerMock;
    private readonly Mock<IRepository<ApiExternaLog, Guid>> _repoMock;
    private readonly Mock<ILogger<ApiExternaLogDecorator>> _loggerMock;
    private readonly ApiExternaLogDecorator _decorator;

    public ApiExternaLogDecoratorTests()
    {
        _innerMock  = new Mock<ICitySearchService>();
        _repoMock   = new Mock<IRepository<ApiExternaLog, Guid>>();
        _loggerMock = new Mock<ILogger<ApiExternaLogDecorator>>();

        _decorator = new ApiExternaLogDecorator(
            _innerMock.Object,
            _repoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SuccessfulCall_InsertsLogWithExitosaTrue()
    {
        var request  = new CitySearchRequestDto { PartialName = "Buenos" };
        var response = new CitySearchResultDto();

        _innerMock
            .Setup(s => s.SearchCitiesAsync(request))
            .ReturnsAsync(response);

        ApiExternaLog? captured = null;
        _repoMock
            .Setup(r => r.InsertAsync(
                It.IsAny<ApiExternaLog>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .Callback<ApiExternaLog, bool, System.Threading.CancellationToken>(
                (log, _, __) => captured = log)
            .ReturnsAsync((ApiExternaLog l, bool _, System.Threading.CancellationToken __) => l);

        var result = await _decorator.SearchCitiesAsync(request);

        Assert.Same(response, result);
        Assert.NotNull(captured);
        Assert.True(captured!.Exitosa);
        Assert.True(captured.TiempoMs >= 0);
        Assert.Equal("Geo", captured.NombreApi);
        Assert.Null(captured.MensajeError);
    }

    [Fact]
    public async Task FailedCall_InsertsLogWithExitosaFalseAndRethrows()
    {
        var request = new CitySearchRequestDto { PartialName = "X" };
        var httpEx  = new HttpRequestException("Connection refused", null, HttpStatusCode.ServiceUnavailable);

        _innerMock
            .Setup(s => s.SearchCitiesAsync(request))
            .ThrowsAsync(httpEx);

        ApiExternaLog? captured = null;
        _repoMock
            .Setup(r => r.InsertAsync(
                It.IsAny<ApiExternaLog>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .Callback<ApiExternaLog, bool, System.Threading.CancellationToken>(
                (log, _, __) => captured = log)
            .ReturnsAsync((ApiExternaLog l, bool _, System.Threading.CancellationToken __) => l);

        var thrown = await Assert.ThrowsAsync<HttpRequestException>(
            () => _decorator.SearchCitiesAsync(request));

        Assert.Same(httpEx, thrown);
        Assert.NotNull(captured);
        Assert.False(captured!.Exitosa);
        Assert.NotNull(captured.MensajeError);
        Assert.Equal("Connection refused", captured.MensajeError);
    }

    [Fact]
    public async Task DbWriteFailure_LoggingErrorSwallowed_OriginalExceptionPropagates()
    {
        var request = new CitySearchRequestDto { PartialName = "X" };
        var innerEx = new InvalidOperationException("API down");
        var dbEx    = new Exception("DB unavailable");

        _innerMock
            .Setup(s => s.SearchCitiesAsync(request))
            .ThrowsAsync(innerEx);

        _repoMock
            .Setup(r => r.InsertAsync(
                It.IsAny<ApiExternaLog>(),
                It.IsAny<bool>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ThrowsAsync(dbEx);

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _decorator.SearchCitiesAsync(request));

        Assert.Same(innerEx, thrown);

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<Exception>(e => e == dbEx),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
