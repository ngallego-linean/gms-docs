namespace GMS.Business.Services;

/// <summary>
/// Service for generating Excel templates for LEA bulk report uploads
/// </summary>
public interface ILEAReportingTemplateService
{
    /// <summary>
    /// Generates an Excel template file for LEA bulk report upload
    /// </summary>
    /// <returns>Byte array containing the Excel file</returns>
    byte[] GenerateReportUploadTemplate();
}
