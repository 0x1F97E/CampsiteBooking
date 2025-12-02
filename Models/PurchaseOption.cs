using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// PurchaseOption entity representing a catalog item for activities/services 
/// that can be purchased at a campsite or for a specific accommodation type.
/// This is a master list of available purchase options - distinct from PeripheralPurchase
/// which tracks actual purchases made within a booking.
/// </summary>
public class PurchaseOption : Entity<PurchaseOptionId>
{
    private CampsiteId _campsiteId = null!;
    private AccommodationTypeId? _accommodationTypeId;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private Money _price = null!;
    private string _category = "Activity";
    private bool _isActive = true;
    private DateTime _createdDate;
    private DateTime _updatedDate;

    public CampsiteId CampsiteId { get => _campsiteId; private set => _campsiteId = value; }
    public AccommodationTypeId? AccommodationTypeId { get => _accommodationTypeId; private set => _accommodationTypeId = value; }
    public string Name { get => _name; private set => _name = value; }
    public string Description { get => _description; private set => _description = value; }
    public Money Price { get => _price; private set => _price = value; }
    public string Category { get => _category; private set => _category = value; }
    public bool IsActive { get => _isActive; private set => _isActive = value; }
    public DateTime CreatedDate { get => _createdDate; private set => _createdDate = value; }
    public DateTime UpdatedDate { get => _updatedDate; private set => _updatedDate = value; }

    public int PurchaseOptionId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.PurchaseOptionId.Create(value) : ValueObjects.PurchaseOptionId.CreateNew();
    }

    /// <summary>
    /// Creates a campsite-level purchase option (available for all accommodation types)
    /// </summary>
    public static PurchaseOption CreateForCampsite(CampsiteId campsiteId, string name, string description, Money price, string category)
    {
        ValidateInputs(name, price, category);

        return new PurchaseOption
        {
            Id = ValueObjects.PurchaseOptionId.CreateNew(),
            _campsiteId = campsiteId,
            _accommodationTypeId = null,
            _name = name.Trim(),
            _description = description?.Trim() ?? string.Empty,
            _price = price,
            _category = category,
            _isActive = true,
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an accommodation-specific purchase option
    /// </summary>
    public static PurchaseOption CreateForAccommodationType(CampsiteId campsiteId, AccommodationTypeId accommodationTypeId, string name, string description, Money price, string category)
    {
        ValidateInputs(name, price, category);

        return new PurchaseOption
        {
            Id = ValueObjects.PurchaseOptionId.CreateNew(),
            _campsiteId = campsiteId,
            _accommodationTypeId = accommodationTypeId,
            _name = name.Trim(),
            _description = description?.Trim() ?? string.Empty,
            _price = price,
            _category = category,
            _isActive = true,
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow
        };
    }

    private static void ValidateInputs(string name, Money price, string category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");

        if (price.Amount < 0)
            throw new DomainException("Price cannot be negative");

        var validCategories = new[] { "Activity", "Service", "Equipment", "Food", "Other" };
        if (!validCategories.Contains(category))
            throw new DomainException("Category must be Activity, Service, Equipment, Food, or Other");
    }

    private PurchaseOption() { }

    public void Activate()
    {
        _isActive = true;
        _updatedDate = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        _isActive = false;
        _updatedDate = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, Money price, string category)
    {
        if (!string.IsNullOrWhiteSpace(name))
            _name = name.Trim();

        _description = description?.Trim() ?? string.Empty;

        if (price.Amount >= 0)
            _price = price;

        var validCategories = new[] { "Activity", "Service", "Equipment", "Food", "Other" };
        if (validCategories.Contains(category))
            _category = category;

        _updatedDate = DateTime.UtcNow;
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        _updatedDate = DateTime.UtcNow;
    }
}

