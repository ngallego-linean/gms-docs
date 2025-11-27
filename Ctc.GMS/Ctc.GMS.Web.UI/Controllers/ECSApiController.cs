using Microsoft.AspNetCore.Mvc;
using GMS.Business.Services;
using GMS.Data;

namespace Ctc.GMS.Web.UI.Controllers;

/// <summary>
/// API controller for ECS (Educator Credentialing System) integration.
/// Provides endpoints for organization hierarchy, contacts, and SEID lookup.
/// </summary>
[Route("api/ecs")]
public class ECSApiController : Controller
{
    private readonly IECSService _ecsService;
    private readonly ILogger<ECSApiController> _logger;

    public ECSApiController(IECSService ecsService, ILogger<ECSApiController> logger)
    {
        _ecsService = ecsService;
        _logger = logger;
    }

    // Organization Hierarchy Endpoints

    /// <summary>
    /// Get all California counties for dropdown
    /// </summary>
    [HttpGet("counties")]
    public IActionResult GetCounties()
    {
        var counties = _ecsService.GetCounties()
            .Select(c => new { c.Code, c.Name })
            .ToList();

        return Json(counties);
    }

    /// <summary>
    /// Get school districts, optionally filtered by county
    /// </summary>
    [HttpGet("districts")]
    public IActionResult GetDistricts(string? countyCode = null)
    {
        var districts = _ecsService.GetDistricts(countyCode)
            .Select(d => new
            {
                d.CdsCode,
                d.Name,
                d.CountyCode,
                CountyName = d.CountyName
            })
            .ToList();

        return Json(districts);
    }

    /// <summary>
    /// Get schools, optionally filtered by district CDS code
    /// </summary>
    [HttpGet("schools")]
    public IActionResult GetSchools(string? districtCdsCode = null)
    {
        var schools = _ecsService.GetSchools(districtCdsCode)
            .Select(s => new
            {
                s.CdsCode,
                s.Name,
                s.DistrictName,
                s.Address,
                s.City
            })
            .ToList();

        return Json(schools);
    }

    /// <summary>
    /// Get a specific school by CDS code
    /// </summary>
    [HttpGet("schools/{cdsCode}")]
    public IActionResult GetSchool(string cdsCode)
    {
        var school = _ecsService.GetSchoolByCdsCode(cdsCode);

        if (school == null)
        {
            return NotFound(new { error = "School not found" });
        }

        return Json(new
        {
            school.CdsCode,
            school.Name,
            school.DistrictName,
            school.Address,
            school.City,
            school.GradeSpan
        });
    }

    /// <summary>
    /// Resolve school from county, district, and school name (for bulk upload validation)
    /// </summary>
    [HttpPost("schools/resolve")]
    public IActionResult ResolveSchool([FromBody] ResolveSchoolRequest request)
    {
        if (string.IsNullOrEmpty(request.CountyName) ||
            string.IsNullOrEmpty(request.DistrictName) ||
            string.IsNullOrEmpty(request.SchoolName))
        {
            return BadRequest(new { error = "County, district, and school name are required" });
        }

        var school = _ecsService.ResolveSchool(request.CountyName, request.DistrictName, request.SchoolName);

        if (school == null)
        {
            return NotFound(new
            {
                error = "School not found",
                suggestion = "Please verify the county, district, and school names are spelled correctly"
            });
        }

        return Json(new
        {
            school.CdsCode,
            school.Name,
            school.DistrictName,
            resolved = true
        });
    }

    // Contact Endpoints

    /// <summary>
    /// Get contacts for a district (for autocomplete selection)
    /// </summary>
    [HttpGet("contacts")]
    public IActionResult GetContacts(string districtCdsCode, string? role = null)
    {
        if (string.IsNullOrEmpty(districtCdsCode))
        {
            return BadRequest(new { error = "District CDS code is required" });
        }

        var contacts = _ecsService.GetContacts(districtCdsCode, role)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Email,
                c.Phone,
                c.Title,
                c.Role
            })
            .ToList();

        return Json(contacts);
    }

    /// <summary>
    /// Get a specific contact
    /// </summary>
    [HttpGet("contacts/{id}")]
    public IActionResult GetContact(int id)
    {
        var contact = _ecsService.GetContact(id);

        if (contact == null)
        {
            return NotFound(new { error = "Contact not found" });
        }

        return Json(new
        {
            contact.Id,
            contact.Name,
            contact.Email,
            contact.Phone,
            contact.Title,
            contact.Role
        });
    }

    /// <summary>
    /// Create a new contact (write-back to ECS)
    /// </summary>
    [HttpPost("contacts")]
    public IActionResult CreateContact([FromBody] CreateContactRequest request)
    {
        if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email))
        {
            return BadRequest(new { error = "Name and email are required" });
        }

        var contact = new ECSContact
        {
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Title = request.Title,
            Role = request.Role ?? "GMS_CONTACT",
            DistrictCdsCode = request.DistrictCdsCode
        };

        var created = _ecsService.CreateContact(contact);

        return Json(new
        {
            created.Id,
            created.Name,
            created.Email,
            created.Phone,
            created.Title,
            created.Role,
            success = true
        });
    }

    // SEID Lookup Endpoints

    /// <summary>
    /// Lookup SEID by student identifying information.
    /// Note: In production, this would only be accessible from LEA portal (not IHE).
    /// </summary>
    [HttpPost("seid/lookup")]
    public IActionResult LookupSEID([FromBody] SEIDLookupRequest request)
    {
        if (string.IsNullOrEmpty(request.FirstName) ||
            string.IsNullOrEmpty(request.LastName) ||
            string.IsNullOrEmpty(request.Last4SSN))
        {
            return BadRequest(new { error = "First name, last name, date of birth, and last 4 SSN are required" });
        }

        var result = _ecsService.LookupSEID(
            request.FirstName,
            request.LastName,
            request.DateOfBirth,
            request.Last4SSN);

        // Transform result for API response
        var response = new
        {
            status = result.Status.ToString(),
            matchCount = result.Records.Count,
            records = result.Records.Select(r => new
            {
                r.SEID,
                r.FirstName,
                r.LastName,
                DateOfBirth = r.DateOfBirth.ToString("MM/dd/yyyy"),
                // Don't expose full SSN info in response
                HasCredential = r.HasActiveCredential
            }).ToList(),
            // Provide user-friendly message
            message = result.Status switch
            {
                SEIDLookupStatus.SingleMatch => "SEID found",
                SEIDLookupStatus.MultipleMatches => $"Multiple matches found ({result.Records.Count}). Manual resolution required.",
                SEIDLookupStatus.NotFound => "No matching SEID found. Please verify the information entered.",
                _ => "Unknown status"
            }
        };

        return Json(response);
    }

    /// <summary>
    /// Validate if a SEID exists
    /// </summary>
    [HttpGet("seid/validate/{seid}")]
    public IActionResult ValidateSEID(string seid)
    {
        var isValid = _ecsService.ValidateSEID(seid);

        return Json(new
        {
            seid,
            isValid,
            message = isValid ? "SEID is valid" : "SEID not found in ECS"
        });
    }

    // User-Organization Association (for authentication support)

    /// <summary>
    /// Check if an email domain is associated with a known organization
    /// </summary>
    [HttpGet("organization/by-email")]
    public IActionResult GetOrganizationByEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new { error = "Email is required" });
        }

        var orgCdsCode = _ecsService.GetOrganizationByEmail(email);

        if (orgCdsCode == null)
        {
            return Json(new
            {
                found = false,
                message = "No organization association found for this email domain"
            });
        }

        // Get the district info
        var districts = _ecsService.GetDistricts();
        var district = districts.FirstOrDefault(d => d.CdsCode == orgCdsCode);

        return Json(new
        {
            found = true,
            cdsCode = orgCdsCode,
            organizationName = district?.Name ?? "Unknown",
            countyName = district?.CountyName ?? "Unknown"
        });
    }
}

// Request DTOs

public class ResolveSchoolRequest
{
    public string CountyName { get; set; } = "";
    public string DistrictName { get; set; } = "";
    public string SchoolName { get; set; } = "";
}

public class CreateContactRequest
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public string? Title { get; set; }
    public string? Role { get; set; }
    public string DistrictCdsCode { get; set; } = "";
}

public class SEIDLookupRequest
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Last4SSN { get; set; } = "";
}
