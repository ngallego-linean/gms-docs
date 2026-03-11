using ClosedXML.Excel;

namespace GMS.Business.Services;

/// <summary>
/// Service for generating Excel templates for IHE bulk report uploads
/// </summary>
public class ReportingTemplateService : IReportingTemplateService
{
    /// <summary>
    /// Generates an Excel template file for IHE bulk report upload
    /// </summary>
    public byte[] GenerateReportUploadTemplate()
    {
        using var workbook = new XLWorkbook();

        // Create worksheets
        CreateInstructionsSheet(workbook);
        CreateReportDataSheet(workbook);
        CreateReferenceDataSheet(workbook);

        // Save to memory stream and return bytes
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private void CreateInstructionsSheet(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Instructions");

        // Title
        ws.Cell("A1").Value = "CTC Student Teacher Stipend Program";
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 16;
        ws.Cell("A2").Value = "IHE Bulk Report Upload Template - Instructions";
        ws.Cell("A2").Style.Font.Bold = true;
        ws.Cell("A2").Style.Font.FontSize = 14;

        int row = 4;

        // Purpose section
        ws.Cell($"A{row}").Value = "PURPOSE";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "This template allows Institutions of Higher Education (IHEs) to submit completion reports for multiple funded candidates at once.";
        ws.Range($"A{row}:F{row}").Merge();
        row += 2;

        // How to Use section
        ws.Cell($"A{row}").Value = "HOW TO USE THIS TEMPLATE";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "1. Go to the 'Report Data' tab";
        row++;
        ws.Cell($"A{row}").Value = "2. Columns A-D (Student First Name, Last Name, SEID, Credential Area) identify each candidate — fill these in to match your funded candidates";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "3. Fill in report data for each candidate starting on row 5 (rows 2-4 contain sample data)";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "4. Use dropdown lists where provided (Completion Status, Employment Status, Yes/No fields)";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "5. Required fields are highlighted in light blue";
        row++;
        ws.Cell($"A{row}").Value = "6. Optional fields are highlighted in light gray";
        row++;
        ws.Cell($"A{row}").Value = "7. Save the file when complete";
        row++;
        ws.Cell($"A{row}").Value = "8. Upload the file through the IHE Portal bulk upload page";
        row += 2;

        // Field Descriptions section
        ws.Cell($"A{row}").Value = "FIELD DESCRIPTIONS";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;

        // Create field description table
        ws.Cell($"A{row}").Value = "Field Name";
        ws.Cell($"B{row}").Value = "Required";
        ws.Cell($"C{row}").Value = "Format";
        ws.Cell($"D{row}").Value = "Description";
        ws.Range($"A{row}:D{row}").Style.Font.Bold = true;
        ws.Range($"A{row}:D{row}").Style.Fill.BackgroundColor = XLColor.LightGray;
        row++;

        AddFieldDescription(ws, row++, "Student First Name", "Yes", "Text", "Candidate's first name (must match funded candidate record)");
        AddFieldDescription(ws, row++, "Student Last Name", "Yes", "Text", "Candidate's last name (must match funded candidate record)");
        AddFieldDescription(ws, row++, "SEID", "Yes", "Text", "Student Educator ID (must match funded candidate record)");
        AddFieldDescription(ws, row++, "Credential Area", "Yes", "Dropdown", "Credential area (must match funded candidate record)");
        AddFieldDescription(ws, row++, "Completion Status", "Yes", "Dropdown", "In Progress, Completed, or Denied");
        AddFieldDescription(ws, row++, "Completion Date", "No", "MM/DD/YYYY", "Date of program completion (if Completed)");
        AddFieldDescription(ws, row++, "Denial Reason", "No", "Text", "Reason for denial (if Denied)");
        AddFieldDescription(ws, row++, "Switched to Intern", "No", "Dropdown", "Yes or No — whether candidate switched to intern pathway");
        AddFieldDescription(ws, row++, "Grant Program Hours", "Yes", "Number", "Hours completed in grant program (0-1000)");
        AddFieldDescription(ws, row++, "Credential Program Hours", "Yes", "Number", "Hours completed in credential program (0-1000)");
        AddFieldDescription(ws, row++, "Credential Earned", "No", "Dropdown", "Yes or No — whether teaching credential was earned");
        AddFieldDescription(ws, row++, "Credential Issue Date", "No", "MM/DD/YYYY", "Date credential was issued (if earned)");
        AddFieldDescription(ws, row++, "Employment Status", "Yes", "Dropdown", "Not Employed, Seeking, or Employed");
        AddFieldDescription(ws, row++, "Employed in Partner District", "No", "Dropdown", "Yes or No — employed in partner LEA district");
        AddFieldDescription(ws, row++, "Employer Name", "No", "Text", "Name of employer (if employed)");
        AddFieldDescription(ws, row++, "School Site", "No", "Text", "Name of school site (if employed)");
        AddFieldDescription(ws, row++, "Grade Level", "No", "Text", "Grade level taught (e.g. K-2, 3-5, 9-12)");
        AddFieldDescription(ws, row++, "Subject Area", "No", "Text", "Subject area taught");
        AddFieldDescription(ws, row++, "Additional Notes", "No", "Text", "Any additional notes or comments");

        row += 2;

        // Important Notes
        ws.Cell($"A{row}").Value = "IMPORTANT NOTES";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "• All required fields must be completed for successful upload";
        row++;
        ws.Cell($"A{row}").Value = "• Date format must be MM/DD/YYYY (e.g., 01/15/2026)";
        row++;
        ws.Cell($"A{row}").Value = "• Student First Name, Last Name, SEID, and Credential Area must match existing funded candidate records";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "• Completion Status values: In Progress, Completed, Denied";
        row++;
        ws.Cell($"A{row}").Value = "• Employment Status values: Not Employed, Seeking, Employed";
        row++;
        ws.Cell($"A{row}").Value = "• Hours must be whole numbers between 0 and 1000";
        row++;
        ws.Cell($"A{row}").Value = "• Maximum file size: 10MB";
        row++;
        ws.Cell($"A{row}").Value = "• Supported formats: .xlsx, .xls, .csv";
        row += 2;

        // Contact section
        ws.Cell($"A{row}").Value = "NEED HELP?";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "Contact the CTC Grants Team: grants@ctc.ca.gov";
        row++;
        ws.Cell($"A{row}").Value = "Visit: https://www.ctc.ca.gov";

        // Auto-fit columns
        ws.Columns().AdjustToContents();
    }

    private void AddFieldDescription(IXLWorksheet ws, int row, string fieldName, string required, string format, string description)
    {
        ws.Cell($"A{row}").Value = fieldName;
        ws.Cell($"B{row}").Value = required;
        ws.Cell($"C{row}").Value = format;
        ws.Cell($"D{row}").Value = description;
    }

    private void CreateReportDataSheet(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Report Data");

        // Headers
        int col = 1;
        var headerRow = 1;

        // Color coding
        var requiredColor = XLColor.FromHtml("#D6EAF8");
        var optionalColor = XLColor.FromHtml("#E8E8E8");

        // Required fields (blue)
        AddHeader(ws, headerRow, col++, "Student First Name", requiredColor);       // A
        AddHeader(ws, headerRow, col++, "Student Last Name", requiredColor);        // B
        AddHeader(ws, headerRow, col++, "SEID", requiredColor);                     // C
        AddHeader(ws, headerRow, col++, "Credential Area", requiredColor);          // D
        AddHeader(ws, headerRow, col++, "Completion Status", requiredColor);        // E
        // Optional fields (gray)
        AddHeader(ws, headerRow, col++, "Completion Date", optionalColor);          // F
        AddHeader(ws, headerRow, col++, "Denial Reason", optionalColor);            // G
        AddHeader(ws, headerRow, col++, "Switched to Intern", optionalColor);       // H
        // Required fields (blue)
        AddHeader(ws, headerRow, col++, "Grant Program Hours", requiredColor);      // I
        AddHeader(ws, headerRow, col++, "Credential Program Hours", requiredColor); // J
        // Optional fields (gray)
        AddHeader(ws, headerRow, col++, "Credential Earned", optionalColor);        // K
        AddHeader(ws, headerRow, col++, "Credential Issue Date", optionalColor);    // L
        // Required field (blue)
        AddHeader(ws, headerRow, col++, "Employment Status", requiredColor);        // M
        // Optional fields (gray)
        AddHeader(ws, headerRow, col++, "Employed in Partner District", optionalColor); // N
        AddHeader(ws, headerRow, col++, "Employer Name", optionalColor);            // O
        AddHeader(ws, headerRow, col++, "School Site", optionalColor);              // P
        AddHeader(ws, headerRow, col++, "Grade Level", optionalColor);              // Q
        AddHeader(ws, headerRow, col++, "Subject Area", optionalColor);             // R
        AddHeader(ws, headerRow, col++, "Additional Notes", optionalColor);         // S

        // Sample data (3 rows)
        AddSampleRow(ws, 2,
            "John", "Smith", "SE12345678", "Multiple Subject",
            "Completed", "12/15/2025", "", "No",
            "520", "640",
            "Yes", "01/10/2026",
            "Employed", "Yes", "Los Angeles Unified", "Lincoln Elementary", "K-2", "General Education", "");

        AddSampleRow(ws, 3,
            "Maria", "Garcia", "SE23456789", "Single Subject - Mathematics",
            "In Progress", "", "", "No",
            "350", "420",
            "No", "",
            "Seeking", "No", "", "", "", "", "Expected to complete by June 2026");

        AddSampleRow(ws, 4,
            "David", "Chen", "SE34567890", "Education Specialist - Mild/Moderate",
            "Denied", "", "Withdrew from program due to personal reasons", "No",
            "120", "150",
            "No", "",
            "Not Employed", "No", "", "", "", "", "");

        // Data validation dropdowns
        ws.Range("D5:D1000").SetDataValidation().List("'Reference Data'!$A$2:$A$20", true);   // Credential Area
        ws.Range("E5:E1000").SetDataValidation().List("'Reference Data'!$C$2:$C$5", true);    // Completion Status
        ws.Range("H5:H1000").SetDataValidation().List("'Reference Data'!$G$2:$G$3", true);    // Switched to Intern (Yes/No)
        ws.Range("K5:K1000").SetDataValidation().List("'Reference Data'!$G$2:$G$3", true);    // Credential Earned (Yes/No)
        ws.Range("M5:M1000").SetDataValidation().List("'Reference Data'!$E$2:$E$5", true);    // Employment Status
        ws.Range("N5:N1000").SetDataValidation().List("'Reference Data'!$G$2:$G$3", true);    // Employed in Partner District (Yes/No)

        // Apply background colors to data entry area (rows 5-1000)
        for (int i = 5; i <= 1000; i++)
        {
            // Required fields: A-E, I-J, M
            ws.Range($"A{i}:E{i}").Style.Fill.BackgroundColor = requiredColor;
            ws.Range($"I{i}:J{i}").Style.Fill.BackgroundColor = requiredColor;
            ws.Cell($"M{i}").Style.Fill.BackgroundColor = requiredColor;

            // Optional fields: F-H, K-L, N-S
            ws.Range($"F{i}:H{i}").Style.Fill.BackgroundColor = optionalColor;
            ws.Range($"K{i}:L{i}").Style.Fill.BackgroundColor = optionalColor;
            ws.Range($"N{i}:S{i}").Style.Fill.BackgroundColor = optionalColor;
        }

        // Auto-fit columns
        ws.Columns().AdjustToContents();

        // Freeze header row
        ws.SheetView.FreezeRows(1);
    }

    private void AddHeader(IXLWorksheet ws, int row, int col, string text, XLColor bgColor)
    {
        var cell = ws.Cell(row, col);
        cell.Value = text;
        cell.Style.Font.Bold = true;
        cell.Style.Fill.BackgroundColor = bgColor;
        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cell.Style.Alignment.WrapText = true;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    private void AddSampleRow(IXLWorksheet ws, int row, params string[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            ws.Cell(row, i + 1).Value = values[i];
        }
    }

    private void CreateReferenceDataSheet(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Reference Data");

        // Credential Areas (column A)
        ws.Cell("A1").Value = "Credential Areas";
        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        int row = 2;
        ws.Cell($"A{row++}").Value = "Multiple Subject";
        ws.Cell($"A{row++}").Value = "Single Subject - English";
        ws.Cell($"A{row++}").Value = "Single Subject - Mathematics";
        ws.Cell($"A{row++}").Value = "Single Subject - Science: Biological Sciences";
        ws.Cell($"A{row++}").Value = "Single Subject - Science: Chemistry";
        ws.Cell($"A{row++}").Value = "Single Subject - Science: Geosciences";
        ws.Cell($"A{row++}").Value = "Single Subject - Science: Physics";
        ws.Cell($"A{row++}").Value = "Single Subject - Social Science";
        ws.Cell($"A{row++}").Value = "Single Subject - World Languages: Spanish";
        ws.Cell($"A{row++}").Value = "Single Subject - World Languages: French";
        ws.Cell($"A{row++}").Value = "Single Subject - World Languages: Other";
        ws.Cell($"A{row++}").Value = "Single Subject - Art";
        ws.Cell($"A{row++}").Value = "Single Subject - Music";
        ws.Cell($"A{row++}").Value = "Single Subject - Physical Education";
        ws.Cell($"A{row++}").Value = "Education Specialist - Mild/Moderate";
        ws.Cell($"A{row++}").Value = "Education Specialist - Moderate/Severe";
        ws.Cell($"A{row++}").Value = "Education Specialist - Deaf and Hard of Hearing";
        ws.Cell($"A{row++}").Value = "Education Specialist - Visual Impairments";
        ws.Cell($"A{row++}").Value = "Education Specialist - Early Childhood Special Education";

        // Completion Status (column C)
        ws.Cell("C1").Value = "Completion Status";
        ws.Cell("C1").Style.Font.Bold = true;
        ws.Cell("C1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"C{row++}").Value = "In Progress";
        ws.Cell($"C{row++}").Value = "Completed";
        ws.Cell($"C{row++}").Value = "Denied";

        // Employment Status (column E)
        ws.Cell("E1").Value = "Employment Status";
        ws.Cell("E1").Style.Font.Bold = true;
        ws.Cell("E1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"E{row++}").Value = "Not Employed";
        ws.Cell($"E{row++}").Value = "Seeking";
        ws.Cell($"E{row++}").Value = "Employed";

        // Yes/No (column G)
        ws.Cell("G1").Value = "Yes/No";
        ws.Cell("G1").Style.Font.Bold = true;
        ws.Cell("G1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"G{row++}").Value = "Yes";
        ws.Cell($"G{row++}").Value = "No";

        // Auto-fit columns
        ws.Columns().AdjustToContents();
    }
}
