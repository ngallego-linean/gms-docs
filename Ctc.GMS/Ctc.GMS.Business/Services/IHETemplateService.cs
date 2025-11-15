using ClosedXML.Excel;

namespace GMS.Business.Services;

/// <summary>
/// Service for generating Excel templates for IHE bulk uploads
/// </summary>
public class IHETemplateService : IIHETemplateService
{
    /// <summary>
    /// Generates an Excel template file for student candidate bulk upload
    /// </summary>
    public byte[] GenerateStudentUploadTemplate()
    {
        using var workbook = new XLWorkbook();

        // Create worksheets
        CreateInstructionsSheet(workbook);
        CreateStudentDataSheet(workbook);
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
        ws.Cell("A2").Value = "IHE Candidate Bulk Upload Template - Instructions";
        ws.Cell("A2").Style.Font.Bold = true;
        ws.Cell("A2").Style.Font.FontSize = 14;

        int row = 4;

        // Purpose section
        ws.Cell($"A{row}").Value = "PURPOSE";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "This template allows Institutions of Higher Education (IHEs) to upload multiple student candidates for the Student Teacher Stipend Program.";
        ws.Range($"A{row}:F{row}").Merge();
        row += 2;

        // How to Use section
        ws.Cell($"A{row}").Value = "HOW TO USE THIS TEMPLATE";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "1. Go to the 'Student Data' tab";
        row++;
        ws.Cell($"A{row}").Value = "2. Fill in candidate information starting on row 5 (rows 2-4 contain sample data)";
        row++;
        ws.Cell($"A{row}").Value = "3. Use dropdown lists where provided (Credential Area, Race, Ethnicity, Gender)";
        row++;
        ws.Cell($"A{row}").Value = "4. Required fields are highlighted in light blue";
        row++;
        ws.Cell($"A{row}").Value = "5. Optional fields are highlighted in light gray";
        row++;
        ws.Cell($"A{row}").Value = "6. Save the file when complete";
        row++;
        ws.Cell($"A{row}").Value = "7. Upload the file through the IHE Portal bulk upload page";
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

        AddFieldDescription(ws, row++, "First Name", "Yes", "Text", "Candidate's legal first name");
        AddFieldDescription(ws, row++, "Last Name", "Yes", "Text", "Candidate's legal last name");
        AddFieldDescription(ws, row++, "Date of Birth", "Yes", "MM/DD/YYYY", "Candidate's date of birth");
        AddFieldDescription(ws, row++, "Last 4 SSN/ITIN", "Yes", "4 digits", "Last 4 digits of Social Security Number or ITIN");
        AddFieldDescription(ws, row++, "Credential Area", "Yes", "Dropdown", "Select from Reference Data tab");
        AddFieldDescription(ws, row++, "County CDS Code", "Yes", "1-2 digits", "California county code");
        AddFieldDescription(ws, row++, "LEA CDS Code", "Yes", "1-2 digits", "Local Educational Agency code");
        AddFieldDescription(ws, row++, "School CDS Code", "Yes", "1-2 digits", "School site code");
        AddFieldDescription(ws, row++, "LEA POC First Name", "Yes", "Text", "LEA point of contact first name");
        AddFieldDescription(ws, row++, "LEA POC Last Name", "Yes", "Text", "LEA point of contact last name");
        AddFieldDescription(ws, row++, "LEA POC Email", "Yes", "Email", "Valid email address for LEA contact");
        AddFieldDescription(ws, row++, "LEA POC Phone", "Yes", "XXX-XXX-XXXX", "Phone number for LEA contact");
        AddFieldDescription(ws, row++, "Race", "No", "Dropdown", "Federal race category (optional)");
        AddFieldDescription(ws, row++, "Ethnicity", "No", "Dropdown", "Hispanic/Latino or Not Hispanic/Latino");
        AddFieldDescription(ws, row++, "Gender", "No", "Dropdown", "Gender identity (optional)");

        row += 2;

        // Important Notes
        ws.Cell($"A{row}").Value = "IMPORTANT NOTES";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "• All required fields must be completed for successful upload";
        row++;
        ws.Cell($"A{row}").Value = "• Date format must be MM/DD/YYYY (e.g., 01/15/1995)";
        row++;
        ws.Cell($"A{row}").Value = "• Email addresses must be valid format (name@domain.com)";
        row++;
        ws.Cell($"A{row}").Value = "• Phone numbers must be in XXX-XXX-XXXX format";
        row++;
        ws.Cell($"A{row}").Value = "• CDS codes are numeric only, 1-2 digits";
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

    private void CreateStudentDataSheet(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Student Data");

        // Headers
        int col = 1;
        var headerRow = 1;

        // Required fields (light blue background)
        var requiredColor = XLColor.FromHtml("#D6EAF8");
        var optionalColor = XLColor.FromHtml("#E8E8E8");

        AddHeader(ws, headerRow, col++, "First Name", requiredColor);
        AddHeader(ws, headerRow, col++, "Last Name", requiredColor);
        AddHeader(ws, headerRow, col++, "Date of Birth", requiredColor);
        AddHeader(ws, headerRow, col++, "Last 4 SSN/ITIN", requiredColor);
        AddHeader(ws, headerRow, col++, "Credential Area", requiredColor);
        AddHeader(ws, headerRow, col++, "County CDS Code", requiredColor);
        AddHeader(ws, headerRow, col++, "LEA CDS Code", requiredColor);
        AddHeader(ws, headerRow, col++, "School CDS Code", requiredColor);
        AddHeader(ws, headerRow, col++, "LEA POC First Name", requiredColor);
        AddHeader(ws, headerRow, col++, "LEA POC Last Name", requiredColor);
        AddHeader(ws, headerRow, col++, "LEA POC Email", requiredColor);
        AddHeader(ws, headerRow, col++, "LEA POC Phone", requiredColor);

        // Optional fields (light gray background)
        AddHeader(ws, headerRow, col++, "Race", optionalColor);
        AddHeader(ws, headerRow, col++, "Ethnicity", optionalColor);
        AddHeader(ws, headerRow, col++, "Gender", optionalColor);

        // Sample data (3 rows)
        AddSampleRow(ws, 2, "John", "Smith", "01/15/1995", "1234", "Multiple Subject", "19", "64733", "0000000",
            "Jane", "Doe", "jane.doe@lausd.net", "213-555-0100", "White", "Not Hispanic/Latino", "Male");
        AddSampleRow(ws, 3, "Maria", "Garcia", "03/22/1996", "5678", "Single Subject - Mathematics", "37", "68338", "0000000",
            "Robert", "Martinez", "r.martinez@sdusd.net", "619-555-0200", "Hispanic", "Hispanic/Latino", "Female");
        AddSampleRow(ws, 4, "David", "Chen", "07/10/1994", "9012", "Education Specialist - Mild/Moderate", "10", "62281", "0000000",
            "Lisa", "Thompson", "l.thompson@fresnounified.org", "559-555-0300", "Asian", "Not Hispanic/Latino", "Male");

        // Add data validation for dropdown fields
        var credentialAreaRange = $"E5:E1000";
        var raceRange = $"M5:M1000";
        var ethnicityRange = $"N5:N1000";
        var genderRange = $"O5:O1000";

        ws.Range(credentialAreaRange).SetDataValidation().List("'Reference Data'!$A$2:$A$50", true);
        ws.Range(raceRange).SetDataValidation().List("'Reference Data'!$C$2:$C$10", true);
        ws.Range(ethnicityRange).SetDataValidation().List("'Reference Data'!$E$2:$E$5", true);
        ws.Range(genderRange).SetDataValidation().List("'Reference Data'!$G$2:$G$10", true);

        // Apply background colors to data entry area (rows 5-1000)
        for (int i = 5; i <= 1000; i++)
        {
            // Required fields
            ws.Range($"A{i}:L{i}").Style.Fill.BackgroundColor = requiredColor;
            // Optional fields
            ws.Range($"M{i}:O{i}").Style.Fill.BackgroundColor = optionalColor;
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

    private void CreateReferenceDataSheet(IXLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Reference Data");

        // Credential Areas
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

        // Race
        ws.Cell("C1").Value = "Race";
        ws.Cell("C1").Style.Font.Bold = true;
        ws.Cell("C1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"C{row++}").Value = "American Indian or Alaska Native";
        ws.Cell($"C{row++}").Value = "Asian";
        ws.Cell($"C{row++}").Value = "Black or African American";
        ws.Cell($"C{row++}").Value = "Hispanic";
        ws.Cell($"C{row++}").Value = "Native Hawaiian or Other Pacific Islander";
        ws.Cell($"C{row++}").Value = "White";
        ws.Cell($"C{row++}").Value = "Two or More Races";

        // Ethnicity
        ws.Cell("E1").Value = "Ethnicity";
        ws.Cell("E1").Style.Font.Bold = true;
        ws.Cell("E1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"E{row++}").Value = "Hispanic/Latino";
        ws.Cell($"E{row++}").Value = "Not Hispanic/Latino";

        // Gender
        ws.Cell("G1").Value = "Gender";
        ws.Cell("G1").Style.Font.Bold = true;
        ws.Cell("G1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"G{row++}").Value = "Male";
        ws.Cell($"G{row++}").Value = "Female";
        ws.Cell($"G{row++}").Value = "Non-binary";
        ws.Cell($"G{row++}").Value = "Prefer not to say";

        // Auto-fit columns
        ws.Columns().AdjustToContents();
    }
}
