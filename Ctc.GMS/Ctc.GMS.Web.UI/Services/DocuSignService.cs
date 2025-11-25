using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using Ctc.GMS.Web.UI.Configuration;
using Microsoft.Extensions.Options;

namespace Ctc.GMS.Web.UI.Services;

public interface IDocuSignService
{
    Task<string> SendGAAForSigningAsync(GAAEnvelopeRequest request);
}

public class GAAEnvelopeRequest
{
    public int GroupId { get; set; }
    public string LEAName { get; set; } = string.Empty;
    public string GrantNumber { get; set; } = string.Empty;
    public string AgreementTermStart { get; set; } = "July 1, 2024";
    public string AgreementTermEnd { get; set; } = "June 30, 2025";
    public string SubmissionMonth { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int StudentCount { get; set; }
    public List<GAAStudentInfo> Students { get; set; } = new();
    public string SignerEmail { get; set; } = string.Empty;
    public string SignerName { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}

public class GAAStudentInfo
{
    public string StudentName { get; set; } = string.Empty;
    public string SEID { get; set; } = string.Empty;
    public string IHEName { get; set; } = string.Empty;
    public string CredentialArea { get; set; } = string.Empty;
    public decimal AwardAmount { get; set; }
}

public class DocuSignService : IDocuSignService
{
    private readonly DocuSignConfig _config;
    private readonly ILogger<DocuSignService> _logger;
    private readonly IWebHostEnvironment _environment;

    public DocuSignService(
        IOptions<DocuSignConfig> config,
        ILogger<DocuSignService> logger,
        IWebHostEnvironment environment)
    {
        _config = config.Value;
        _logger = logger;
        _environment = environment;
    }

    public async Task<string> SendGAAForSigningAsync(GAAEnvelopeRequest request)
    {
        try
        {
            // Get access token using JWT
            var accessToken = await GetAccessTokenAsync();

            // Create API client
            var apiClient = new ApiClient(_config.BasePath);
            apiClient.Configuration.DefaultHeader.Add("Authorization", "Bearer " + accessToken);

            // Create envelope
            var envelopeDefinition = CreateEnvelopeDefinition(request);

            // Send envelope
            var envelopesApi = new EnvelopesApi(apiClient);
            var envelopeSummary = await envelopesApi.CreateEnvelopeAsync(_config.AccountId, envelopeDefinition);

            _logger.LogInformation("Envelope created with ID: {EnvelopeId}", envelopeSummary.EnvelopeId);

            // Get embedded signing URL
            var signingUrl = await GetEmbeddedSigningUrlAsync(
                envelopesApi,
                envelopeSummary.EnvelopeId,
                request.SignerEmail,
                request.SignerName,
                request.ReturnUrl);

            return signingUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending GAA for signing");
            throw;
        }
    }

    private async Task<string> GetAccessTokenAsync()
    {
        // Clean up private key - remove extra whitespace from .env formatting
        var privateKey = _config.PrivateKey
            .Replace("\\n", "\n")
            .Trim();

        // If the key has extra indentation, clean it up
        var lines = privateKey.Split('\n');
        privateKey = string.Join("\n", lines.Select(l => l.Trim()));

        var scopes = new List<string> { "signature", "impersonation" };

        var apiClient = new ApiClient();
        var tokenInfo = apiClient.RequestJWTUserToken(
            _config.IntegrationKey,
            _config.UserId,
            _config.AuthServer,
            System.Text.Encoding.UTF8.GetBytes(privateKey),
            1, // Token valid for 1 hour
            scopes);

        return await Task.FromResult(tokenInfo.access_token);
    }

    private EnvelopeDefinition CreateEnvelopeDefinition(GAAEnvelopeRequest request)
    {
        // Read the GAA document template - check both Web.UI and parent Ctc.GMS directories
        var documentPath = Path.Combine(_environment.ContentRootPath, _config.GAADocumentPath);
        var parentDocumentPath = Path.Combine(_environment.ContentRootPath, "..", _config.GAADocumentPath);

        byte[] documentBytes;
        string fileExtension;

        if (File.Exists(documentPath))
        {
            documentBytes = File.ReadAllBytes(documentPath);
            fileExtension = "pdf";
            _logger.LogInformation("Using GAA template from {Path}", documentPath);
        }
        else if (File.Exists(parentDocumentPath))
        {
            documentBytes = File.ReadAllBytes(parentDocumentPath);
            fileExtension = "pdf";
            _logger.LogInformation("Using GAA template from {Path}", parentDocumentPath);
        }
        else
        {
            // Create a simple placeholder document if template doesn't exist
            _logger.LogWarning("GAA template not found at {Path} or {ParentPath}, using HTML placeholder", documentPath, parentDocumentPath);
            documentBytes = CreatePlaceholderDocument(request);
            fileExtension = "html";  // HTML placeholder needs html extension
        }

        var document = new Document
        {
            DocumentBase64 = Convert.ToBase64String(documentBytes),
            Name = $"Grant Award Agreement - {request.LEAName}",
            FileExtension = fileExtension,
            DocumentId = "1"
        };

        // Define signer
        var signer = new Signer
        {
            Email = request.SignerEmail,
            Name = request.SignerName,
            RecipientId = "1",
            RoutingOrder = "1",
            ClientUserId = "1001" // Required for embedded signing
        };

        // Extract year digits from agreement terms (e.g., "July 1, 2024" -> "4")
        var startYearDigit = request.AgreementTermStart.Length >= 4
            ? request.AgreementTermStart[^1].ToString() : "4";
        var endYearDigit = request.AgreementTermEnd.Length >= 4
            ? request.AgreementTermEnd[^1].ToString() : "5";

        // Create text tabs for populating document fields using anchor strings
        // XOffset is from START of anchor string, so must account for anchor width
        // YOffset negative moves UP, positive moves DOWN
        var textTabs = new List<Text>
        {
            // Grantee Name: __________________________________
            new Text
            {
                AnchorString = "Grantee Name:",
                AnchorUnits = "pixels",
                AnchorXOffset = "85",
                AnchorYOffset = "-7",  // Move up to align with baseline
                Font = "helvetica",
                FontSize = "size10",
                Value = request.LEAName,
                Locked = "true",
                TabLabel = "GranteeName"
            },
            // Grant Number: __________________________________
            new Text
            {
                AnchorString = "Grant Number:",
                AnchorUnits = "pixels",
                AnchorXOffset = "85",
                AnchorYOffset = "-7",
                Font = "helvetica",
                FontSize = "size10",
                Value = request.GrantNumber,
                Locked = "true",
                TabLabel = "GrantNumber"
            },
            // Agreement Term: July 1, 202__ (start year digit)
            new Text
            {
                AnchorString = "July 1, 202",
                AnchorUnits = "pixels",
                AnchorXOffset = "52",  // Moved left
                AnchorYOffset = "-10", // Moved up
                Font = "helvetica",
                FontSize = "size10",
                Value = startYearDigit,
                Locked = "true",
                TabLabel = "AgreementTermStart"
            },
            // Agreement Term: June 30, 202__ (end year digit)
            new Text
            {
                AnchorString = "June 30, 202",
                AnchorUnits = "pixels",
                AnchorXOffset = "60",  // Moved left
                AnchorYOffset = "-10", // Moved up
                Font = "helvetica",
                FontSize = "size10",
                Value = endYearDigit,
                Locked = "true",
                TabLabel = "AgreementTermEnd"
            },
            // THIS AGREEMENT is between _______(Grantee)
            new Text
            {
                AnchorString = "THIS AGREEMENT is between",
                AnchorUnits = "pixels",
                AnchorXOffset = "155",
                AnchorYOffset = "-7",
                Font = "helvetica",
                FontSize = "size10",
                Value = request.LEAName,
                Locked = "true",
                TabLabel = "GranteeNameParagraph"
            },
            // Grantee will support _______ of student teachers
            new Text
            {
                AnchorString = "Grantee will support",
                AnchorUnits = "pixels",
                AnchorXOffset = "105",
                AnchorYOffset = "-7",
                Font = "helvetica",
                FontSize = "size10",
                Value = request.StudentCount.ToString(),
                Locked = "true",
                TabLabel = "StudentCount"
            },
            // Original Amount: $_______________
            new Text
            {
                AnchorString = "Original Amount: $",
                AnchorUnits = "pixels",
                AnchorXOffset = "100",
                AnchorYOffset = "-7",
                Font = "helvetica",
                FontSize = "size10",
                Value = request.TotalAmount.ToString("N2"),
                Locked = "true",
                TabLabel = "OriginalAmount"
            }
        };

        // Signature tab - positioned on underscores after "Grantee Authorized"
        var signHereTabs = new List<SignHere>
        {
            new SignHere
            {
                AnchorString = "Grantee Authorized",
                AnchorUnits = "pixels",
                AnchorXOffset = "115",  // Width of "Grantee Authorized" + space
                AnchorYOffset = "-5",
                TabLabel = "GranteeSignature"
            }
        };

        var tabs = new Tabs
        {
            TextTabs = textTabs,
            SignHereTabs = signHereTabs
        };

        signer.Tabs = tabs;

        var recipients = new Recipients
        {
            Signers = new List<Signer> { signer }
        };

        return new EnvelopeDefinition
        {
            EmailSubject = $"Grant Award Agreement - {request.LEAName} ({request.SubmissionMonth})",
            EmailBlurb = $"Please review and sign the Grant Award Agreement for {request.StudentCount} student(s) totaling {request.TotalAmount:C0}.",
            Documents = new List<Document> { document },
            Recipients = recipients,
            Status = "sent" // Send immediately
        };
    }

    private byte[] CreatePlaceholderDocument(GAAEnvelopeRequest request)
    {
        // Create a simple HTML document that can be converted
        var html = $@"
            <html>
            <head><title>Grant Award Agreement</title></head>
            <body style='font-family: Arial, sans-serif; padding: 40px;'>
                <h1 style='text-align: center;'>California Commission on Teacher Credentialing</h1>
                <h2 style='text-align: center;'>Grant Award Agreement</h2>
                <h3 style='text-align: center;'>Student Teacher Stipend Program</h3>

                <hr/>

                <p><strong>LEA District:</strong> {request.LEAName}</p>
                <p><strong>Submission Month:</strong> {request.SubmissionMonth}</p>
                <p><strong>Number of Students:</strong> {request.StudentCount}</p>
                <p><strong>Total Award Amount:</strong> {request.TotalAmount:C0}</p>

                <h3>Students Covered by this Agreement:</h3>
                <table border='1' cellpadding='8' cellspacing='0' style='width: 100%; border-collapse: collapse;'>
                    <tr style='background-color: #f0f0f0;'>
                        <th>Student Name</th>
                        <th>SEID</th>
                        <th>IHE</th>
                        <th>Credential Area</th>
                        <th>Amount</th>
                    </tr>
                    {string.Join("", request.Students.Select(s => $@"
                    <tr>
                        <td>{s.StudentName}</td>
                        <td>{s.SEID}</td>
                        <td>{s.IHEName}</td>
                        <td>{s.CredentialArea}</td>
                        <td>{s.AwardAmount:C0}</td>
                    </tr>"))}
                </table>

                <br/><br/>

                <p>By signing below, you agree to the terms and conditions of the Student Teacher Stipend Program Grant Award Agreement.</p>

                <br/><br/>

                <p><strong>Authorized Signature:</strong> _______________________________</p>
                <p><strong>Date:</strong> _______________________________</p>
            </body>
            </html>";

        return System.Text.Encoding.UTF8.GetBytes(html);
    }

    private async Task<string> GetEmbeddedSigningUrlAsync(
        EnvelopesApi envelopesApi,
        string envelopeId,
        string signerEmail,
        string signerName,
        string returnUrl)
    {
        var viewRequest = new RecipientViewRequest
        {
            ReturnUrl = returnUrl,
            AuthenticationMethod = "none",
            Email = signerEmail,
            UserName = signerName,
            ClientUserId = "1001" // Must match the ClientUserId set on the signer
        };

        var viewUrl = await envelopesApi.CreateRecipientViewAsync(
            _config.AccountId,
            envelopeId,
            viewRequest);

        return viewUrl.Url;
    }
}
