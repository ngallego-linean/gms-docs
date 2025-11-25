namespace Ctc.GMS.Web.UI.Configuration;

public class DocuSignConfig
{
    public string IntegrationKey { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string AuthServer { get; set; } = "account-d.docusign.com"; // Demo server
    public string BasePath { get; set; } = "https://demo.docusign.net/restapi";
    public string PrivateKey { get; set; } = string.Empty;

    // Path to GAA template document
    public string GAADocumentPath { get; set; } = "docs/Student_Teacher_GAA.pdf";
}
