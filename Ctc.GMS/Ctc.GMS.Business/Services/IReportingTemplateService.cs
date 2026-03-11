namespace GMS.Business.Services;

/// <summary>
/// Service for generating Excel templates for IHE bulk report uploads
/// </summary>
public interface IReportingTemplateService
{
    /// <summary>
    /// Generates an Excel template file for IHE bulk report upload
    /// </summary>
    /// <returns>Byte array containing the Excel file</returns>
    byte[] GenerateReportUploadTemplate();
}
