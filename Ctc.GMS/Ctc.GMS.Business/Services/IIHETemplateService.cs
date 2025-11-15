namespace GMS.Business.Services;

/// <summary>
/// Service for generating Excel templates for IHE bulk uploads
/// </summary>
public interface IIHETemplateService
{
    /// <summary>
    /// Generates an Excel template file for student candidate bulk upload
    /// </summary>
    /// <returns>Byte array containing the Excel file</returns>
    byte[] GenerateStudentUploadTemplate();
}
