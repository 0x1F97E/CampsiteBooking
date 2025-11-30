using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CampsiteBooking.Controllers;

/// <summary>
/// REST API Controller for Campsite read operations.
/// Implements REST maturity level 2 (HTTP verbs + status codes).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CampsitesController : ControllerBase
{
    private readonly ICampsiteRepository _campsiteRepository;
    private readonly ILogger<CampsitesController> _logger;

    public CampsitesController(
        ICampsiteRepository campsiteRepository,
        ILogger<CampsitesController> logger)
    {
        _campsiteRepository = campsiteRepository ?? throw new ArgumentNullException(nameof(campsiteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// GET /api/campsites - Get all campsites
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CampsiteDto>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all campsites");
        var campsites = await _campsiteRepository.GetAllAsync(cancellationToken);
        var dtos = campsites.Select(MapToDto);
        return Ok(dtos);
    }

    /// <summary>
    /// GET /api/campsites/{id} - Get campsite by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CampsiteDto>> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting campsite {CampsiteId}", id);
        
        var campsiteId = CampsiteId.Create(id);
        var campsite = await _campsiteRepository.GetByIdAsync(campsiteId, cancellationToken);

        if (campsite == null)
        {
            _logger.LogWarning("Campsite {CampsiteId} not found", id);
            return NotFound(new { message = $"Campsite with ID {id} not found" });
        }

        return Ok(MapToDto(campsite));
    }

    /// <summary>
    /// Map Campsite entity to DTO
    /// </summary>
    private static CampsiteDto MapToDto(Models.Campsite campsite)
    {
        return new CampsiteDto
        {
            CampsiteId = campsite.Id.Value,
            Name = campsite.Name,
            StreetAddress = campsite.StreetAddress,
            City = campsite.City,
            PostalCode = campsite.PostalCode,
            Description = campsite.Description,
            Latitude = campsite.Latitude,
            Longitude = campsite.Longitude,
            IsActive = campsite.IsActive
        };
    }
}

