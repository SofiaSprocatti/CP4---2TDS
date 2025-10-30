using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Models;

namespace UserManagement.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger _logger;

    protected BaseController(ILogger logger)
    {
        _logger = logger;
    }

    protected async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> operation, string operationName)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Starting operation: {OperationName}", operationName);
            
            var result = await operation();
            stopwatch.Stop();
            
            _logger.LogInformation("Operation {OperationName} completed successfully in {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            
            var response = ApiResponse<T>.Success(result, stopwatch.ElapsedMilliseconds);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Validation error in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse<T>.Failure(ex.Message, null, stopwatch.ElapsedMilliseconds);
            return BadRequest(response);
        }
        catch (InvalidOperationException ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Business rule violation in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse<T>.Failure(ex.Message, null, stopwatch.ElapsedMilliseconds);
            return BadRequest(response);
        }
        catch (KeyNotFoundException ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Resource not found in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse<T>.Failure(ex.Message, null, stopwatch.ElapsedMilliseconds);
            return NotFound(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse<T>.Failure(
                "An unexpected error occurred", 
                ex.StackTrace, 
                stopwatch.ElapsedMilliseconds);
            return StatusCode(500, response);
        }
    }

    protected async Task<IActionResult> ExecuteAsync(Func<Task> operation, string operationName)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Starting operation: {OperationName}", operationName);
            
            await operation();
            stopwatch.Stop();
            
            _logger.LogInformation("Operation {OperationName} completed successfully in {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            
            var response = ApiResponse.Success(stopwatch.ElapsedMilliseconds);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Validation error in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse.Failure(ex.Message, null, stopwatch.ElapsedMilliseconds);
            return BadRequest(response);
        }
        catch (InvalidOperationException ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Business rule violation in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse.Failure(ex.Message, null, stopwatch.ElapsedMilliseconds);
            return BadRequest(response);
        }
        catch (KeyNotFoundException ex)
        {
            stopwatch.Stop();
            _logger.LogWarning(ex, "Resource not found in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse.Failure(ex.Message, null, stopwatch.ElapsedMilliseconds);
            return NotFound(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error in operation {OperationName}: {Message}", 
                operationName, ex.Message);
            
            var response = ApiResponse.Failure(
                "An unexpected error occurred", 
                ex.StackTrace, 
                stopwatch.ElapsedMilliseconds);
            return StatusCode(500, response);
        }
    }
}