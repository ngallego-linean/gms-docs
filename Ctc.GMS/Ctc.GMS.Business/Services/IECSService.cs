using GMS.Data;

namespace GMS.Business.Services;

/// <summary>
/// ECS (Educator Credentialing System) Service interface for GMS integration.
/// In production, this will call real ECS APIs. For mockup purposes, uses MockECSData.
/// </summary>
public interface IECSService
{
    // Organization Hierarchy Lookup

    /// <summary>
    /// Get all California counties
    /// </summary>
    List<ECSCounty> GetCounties();

    /// <summary>
    /// Get school districts, optionally filtered by county code
    /// </summary>
    List<ECSDistrict> GetDistricts(string? countyCode = null);

    /// <summary>
    /// Get schools, optionally filtered by district CDS code
    /// </summary>
    List<ECSSchool> GetSchools(string? districtCdsCode = null);

    /// <summary>
    /// Get a specific school by its full CDS code
    /// </summary>
    ECSSchool? GetSchoolByCdsCode(string cdsCode);

    /// <summary>
    /// Resolve school from county, district, and school name (for bulk upload)
    /// </summary>
    ECSSchool? ResolveSchool(string countyName, string districtName, string schoolName);

    // Contact Lookup

    /// <summary>
    /// Get contacts for an organization (LEA/District), filtered for GMS-appropriate roles
    /// </summary>
    List<ECSContact> GetContacts(string districtCdsCode, string? roleFilter = null);

    /// <summary>
    /// Get a specific contact by ID
    /// </summary>
    ECSContact? GetContact(int contactId);

    /// <summary>
    /// Write a new contact back to ECS (or mock storage)
    /// </summary>
    ECSContact CreateContact(ECSContact contact);

    // SEID Lookup

    /// <summary>
    /// Lookup SEID by student identifying information.
    /// Returns single match, multiple potential matches, or no match.
    /// </summary>
    SEIDLookupResult LookupSEID(string firstName, string lastName, DateTime dateOfBirth, string last4SSN);

    /// <summary>
    /// Validate if a SEID exists in ECS
    /// </summary>
    bool ValidateSEID(string seid);

    // User-Organization Association (Authentication Support)

    /// <summary>
    /// Lookup organization association for a user email address.
    /// Returns district CDS code if found, null if not found.
    /// </summary>
    string? GetOrganizationByEmail(string email);

    /// <summary>
    /// Check if email domain is associated with any organization
    /// </summary>
    bool IsValidOrganizationDomain(string emailDomain);
}
