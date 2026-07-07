using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ExploraYa1.Destinos;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace ExploraYa1.Monitoreo;

public class ApiExternaLogDecorator : ICitySearchService
{
    private readonly ICitySearchService _inner;
    private readonly IRepository<ApiExternaLog, Guid> _logRepository;
    private readonly ILogger<ApiExternaLogDecorator> _logger;

    public ApiExternaLogDecorator(
        ICitySearchService inner,
        IRepository<ApiExternaLog, Guid> logRepository,
        ILogger<ApiExternaLogDecorator> logger)
    {
        _inner         = inner;
        _logRepository = logRepository;
        _logger        = logger;
    }

    /*public Task<CityInformationDto> GetCityDetailsAsync(int cityId)
    {
        throw new NotImplementedException();
    }*/

    public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
    {
        var sw = Stopwatch.StartNew();
        bool exitosa = true;
        int statusCode = 200;
        string? mensajeError = null;

        try
        {
            var result = await _inner.SearchCitiesAsync(request);
            sw.Stop();
            return result;
        }
        catch (HttpRequestException httpEx)
        {
            sw.Stop();
            exitosa      = false;
            statusCode   = (int)(httpEx.StatusCode ?? System.Net.HttpStatusCode.ServiceUnavailable);
            mensajeError = httpEx.Message;
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            exitosa      = false;
            statusCode   = 0;
            mensajeError = ex.Message;
            throw;
        }
        finally
        {
            await WriteLogAsync("Geo", "/v1/geo/cities", exitosa, statusCode,
                sw.Elapsed.TotalMilliseconds, mensajeError);
        }
    }

    public async Task<CityInformationDto> GetCityDetailsAsync(int cityId)
    {
        var sw = Stopwatch.StartNew();
        bool exitosa = true;
        int statusCode = 200;
        string? mensajeError = null;

        try
        {
            var result = await _inner.GetCityDetailsAsync(cityId);
            sw.Stop();
            return result;
        }
        catch (HttpRequestException httpEx)
        {
            sw.Stop();
            exitosa      = false;
            statusCode   = (int)(httpEx.StatusCode ?? System.Net.HttpStatusCode.ServiceUnavailable);
            mensajeError = httpEx.Message;
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            exitosa      = false;
            statusCode   = 0;
            mensajeError = ex.Message;
            throw;
        }
        finally
        {
            await WriteLogAsync("Geo", $"/v1/geo/cities/{cityId}", exitosa, statusCode,
                sw.Elapsed.TotalMilliseconds, mensajeError);
        }
    }

    private async Task WriteLogAsync(string nombreApi, string endpoint, bool exitosa,
        int codigoHttp, double tiempoMs, string? mensajeError)
    {
        try
        {
            var log = new ApiExternaLog(
                Guid.NewGuid(), nombreApi, endpoint, exitosa, codigoHttp, tiempoMs, mensajeError);
            await _logRepository.InsertAsync(log, autoSave: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing ApiExternaLog to database.");
        }
    }
}
