using GMS.Data;

namespace GMS.Business.Services;

/// <summary>
/// ECS Service implementation using MockECSData.
/// In production, this would call real ECS APIs.
/// </summary>
public class ECSService : IECSService
{
    // In-memory contact storage for write-back simulation
    private static List<ECSContact> _additionalContacts = new();
    private static int _nextContactId = 100;

    // Organization Hierarchy Lookup

    public List<ECSCounty> GetCounties()
    {
        return MockECSData.Counties.OrderBy(c => c.Name).ToList();
    }

    public List<ECSDistrict> GetDistricts(string? countyCode = null)
    {
        var query = MockECSData.Districts.AsEnumerable();

        if (!string.IsNullOrEmpty(countyCode))
        {
            query = query.Where(d => d.CountyCode == countyCode);
        }

        return query.OrderBy(d => d.Name).ToList();
    }

    public List<ECSSchool> GetSchools(string? districtCdsCode = null)
    {
        var query = MockECSData.Schools.AsEnumerable();

        if (!string.IsNullOrEmpty(districtCdsCode))
        {
            // Match on the county-district portion (first 7 digits)
            query = query.Where(s => s.CdsCode.StartsWith(districtCdsCode));
        }

        return query.OrderBy(s => s.Name).ToList();
    }

    public ECSSchool? GetSchoolByCdsCode(string cdsCode)
    {
        return MockECSData.Schools.FirstOrDefault(s => s.CdsCode == cdsCode);
    }

    public ECSSchool? ResolveSchool(string countyName, string districtName, string schoolName)
    {
        // Find county
        var county = MockECSData.Counties.FirstOrDefault(c =>
            c.Name.Equals(countyName, StringComparison.OrdinalIgnoreCase));

        if (county == null) return null;

        // Find district in that county
        var district = MockECSData.Districts.FirstOrDefault(d =>
            d.CountyCode == county.Code &&
            d.Name.Equals(districtName, StringComparison.OrdinalIgnoreCase));

        if (district == null) return null;

        // Find school in that district
        return MockECSData.Schools.FirstOrDefault(s =>
            s.CdsCode.StartsWith(district.CdsCode) &&
            s.Name.Equals(schoolName, StringComparison.OrdinalIgnoreCase));
    }

    // Contact Lookup

    public List<ECSContact> GetContacts(string districtCdsCode, string? roleFilter = null)
    {
        var allContacts = MockECSData.Contacts
            .Concat(_additionalContacts)
            .Where(c => c.DistrictCdsCode == districtCdsCode);

        if (!string.IsNullOrEmpty(roleFilter))
        {
            allContacts = allContacts.Where(c => c.Role == roleFilter);
        }

        // Filter for GMS-appropriate roles (exclude principals, superintendents)
        allContacts = allContacts.Where(c =>
            c.Role == "GMS_CONTACT" ||
            c.Role == "FISCAL" ||
            c.Role == "HR" ||
            c.Role == "COORDINATOR");

        return allContacts.OrderBy(c => c.Name).ToList();
    }

    public ECSContact? GetContact(int contactId)
    {
        return MockECSData.Contacts
            .Concat(_additionalContacts)
            .FirstOrDefault(c => c.Id == contactId);
    }

    public ECSContact CreateContact(ECSContact contact)
    {
        // Simulate write-back to ECS
        var newContact = new ECSContact
        {
            Id = _nextContactId++,
            Name = contact.Name,
            Email = contact.Email,
            Phone = contact.Phone,
            Title = contact.Title,
            Role = contact.Role,
            DistrictCdsCode = contact.DistrictCdsCode
        };

        _additionalContacts.Add(newContact);
        return newContact;
    }

    // SEID Lookup

    public SEIDLookupResult LookupSEID(string firstName, string lastName, DateTime dateOfBirth, string last4SSN)
    {
        var matches = MockECSData.SEIDRecords
            .Where(r =>
                r.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
                r.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase) &&
                r.DateOfBirth.Date == dateOfBirth.Date &&
                r.Last4SSN == last4SSN)
            .ToList();

        if (matches.Count == 0)
        {
            return new SEIDLookupResult
            {
                Status = SEIDLookupStatus.NotFound,
                Records = new List<ECSSEIDRecord>()
            };
        }

        if (matches.Count == 1)
        {
            return new SEIDLookupResult
            {
                Status = SEIDLookupStatus.SingleMatch,
                Records = matches
            };
        }

        // Multiple matches - this represents the ~0.04% duplicate scenario
        return new SEIDLookupResult
        {
            Status = SEIDLookupStatus.MultipleMatches,
            Records = matches
        };
    }

    public bool ValidateSEID(string seid)
    {
        return MockECSData.SEIDRecords.Any(r => r.SEID == seid);
    }

    // User-Organization Association

    public string? GetOrganizationByEmail(string email)
    {
        // Simulate email domain to organization mapping
        // In production, this would call ECS to get the user's organization association

        var domain = email.Split('@').LastOrDefault()?.ToLowerInvariant();
        if (string.IsNullOrEmpty(domain)) return null;

        // Mock domain mappings (would come from ECS in production)
        var domainMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Los Angeles Unified
            { "lausd.net", "1964733" },
            { "lausd.org", "1964733" },

            // San Diego Unified
            { "sandi.net", "3768338" },

            // Fresno Unified
            { "fresnounified.org", "1062166" },
            { "fresno.k12.ca.us", "1062166" },

            // Long Beach Unified
            { "lbusd.k12.ca.us", "1964709" },

            // Sacramento City Unified
            { "scusd.edu", "3467439" },

            // San Francisco Unified
            { "sfusd.edu", "3868478" },

            // Oakland Unified
            { "ousd.org", "0161259" },

            // San Jose Unified
            { "sjusd.org", "4369450" },

            // Santa Ana Unified
            { "sausd.us", "3030393" }
        };

        return domainMappings.TryGetValue(domain, out var cdsCode) ? cdsCode : null;
    }

    public bool IsValidOrganizationDomain(string emailDomain)
    {
        // Check if domain matches any known organization
        // In production, this would validate against ECS domain registry

        var knownDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "lausd.net", "lausd.org", "sandi.net", "fresnounified.org",
            "fresno.k12.ca.us", "lbusd.k12.ca.us", "scusd.edu", "sfusd.edu",
            "ousd.org", "sjusd.org", "sausd.us", "pomona.k12.ca.us",
            "rialto.k12.ca.us", "murrieta.k12.ca.us", "kern.org"
        };

        return knownDomains.Contains(emailDomain.ToLowerInvariant());
    }
}
