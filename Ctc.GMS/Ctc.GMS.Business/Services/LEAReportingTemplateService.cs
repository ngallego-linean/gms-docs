using ClosedXML.Excel;

namespace GMS.Business.Services;

/// <summary>
/// Service for generating Excel templates for LEA bulk report uploads
/// </summary>
public class LEAReportingTemplateService : ILEAReportingTemplateService
{
    /// <summary>
    /// Generates an Excel template file for LEA bulk report upload
    /// </summary>
    public byte[] GenerateReportUploadTemplate()
    {
        using var workbook = new XLWorkbook();

        CreateInstructionsSheet(workbook);
        CreateReportDataSheet(workbook);
        CreateReferenceDataSheet(workbook);

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
        ws.Cell("A2").Value = "LEA Bulk Report Upload Template - Instructions";
        ws.Cell("A2").Style.Font.Bold = true;
        ws.Cell("A2").Style.Font.FontSize = 14;

        int row = 4;

        // Purpose section
        ws.Cell($"A{row}").Value = "PURPOSE";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "This template allows Local Education Agencies (LEAs) to submit post-payment outcome reports for multiple funded candidates at once.";
        ws.Range($"A{row}:F{row}").Merge();
        row += 2;

        // How to Use section
        ws.Cell($"A{row}").Value = "HOW TO USE THIS TEMPLATE";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;
        ws.Cell($"A{row}").Value = "1. Go to the 'Report Data' tab";
        row++;
        ws.Cell($"A{row}").Value = "2. Columns A-D (Candidate Name, SEID, Credential Area, IHE Partner) identify each candidate — fill these in to match your funded candidates";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "3. Fill in report data for each candidate starting on row 5 (rows 2-4 contain sample data)";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "4. Use dropdown lists where provided (Payment Category, Completion Status, Employment Status, etc.)";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "5. Required fields are highlighted in light blue";
        row++;
        ws.Cell($"A{row}").Value = "6. Optional fields are highlighted in light gray";
        row++;
        ws.Cell($"A{row}").Value = "7. Save the file when complete";
        row++;
        ws.Cell($"A{row}").Value = "8. Upload the file through the LEA Portal bulk upload page";
        row += 2;

        // Field Descriptions section
        ws.Cell($"A{row}").Value = "FIELD DESCRIPTIONS";
        ws.Cell($"A{row}").Style.Font.Bold = true;
        ws.Cell($"A{row}").Style.Font.FontSize = 12;
        row++;

        ws.Cell($"A{row}").Value = "Field Name";
        ws.Cell($"B{row}").Value = "Required";
        ws.Cell($"C{row}").Value = "Format";
        ws.Cell($"D{row}").Value = "Description";
        ws.Range($"A{row}:D{row}").Style.Font.Bold = true;
        ws.Range($"A{row}:D{row}").Style.Fill.BackgroundColor = XLColor.LightGray;
        row++;

        // Candidate Identification
        AddFieldDescription(ws, row++, "Candidate First Name", "Yes", "Text", "Candidate's first name (must match funded candidate record)");
        AddFieldDescription(ws, row++, "Candidate Last Name", "Yes", "Text", "Candidate's last name (must match funded candidate record)");
        AddFieldDescription(ws, row++, "SEID", "Yes", "Text", "Student Educator ID (must match funded candidate record)");
        AddFieldDescription(ws, row++, "Credential Area", "Yes", "Dropdown", "Credential area (must match funded candidate record)");
        AddFieldDescription(ws, row++, "IHE Partner", "Yes", "Text", "Partnering Institution of Higher Education name");

        // Payment Information
        AddFieldDescription(ws, row++, "Payment Category", "Yes", "Dropdown", "Stipend, Salary, Tuition Reimbursement, or Other");
        AddFieldDescription(ws, row++, "Payment Schedule", "Yes", "Dropdown", "Lump Sum, Monthly, Quarterly, or Semester");
        AddFieldDescription(ws, row++, "Actual Payment Amount", "Yes", "Currency", "Dollar amount paid to candidate (e.g. 5000.00)");
        AddFieldDescription(ws, row++, "First Payment Date", "No", "MM/DD/YYYY", "Date of first payment to candidate");
        AddFieldDescription(ws, row++, "Final Payment Date", "No", "MM/DD/YYYY", "Date of final payment to candidate");

        // Program Completion
        AddFieldDescription(ws, row++, "Program Completion Status", "Yes", "Dropdown", "Completed, In Progress, or Not Completed");
        AddFieldDescription(ws, row++, "Program Completion Date", "No", "MM/DD/YYYY", "Date of program completion (if Completed)");

        // Credential Information
        AddFieldDescription(ws, row++, "Credential Earned Status", "Yes", "Dropdown", "Credential Earned, In Progress, or Not Earned");
        AddFieldDescription(ws, row++, "Credential Issue Date", "No", "MM/DD/YYYY", "Date credential was issued (if earned)");

        // Employment Information
        AddFieldDescription(ws, row++, "Hired in District", "Yes", "Dropdown", "Yes or No — whether candidate was hired in the LEA district");
        AddFieldDescription(ws, row++, "Employment Status", "No", "Dropdown", "Full-Time Teacher, Part-Time Teacher, Seeking Employment, or Not Hired as Teacher");
        AddFieldDescription(ws, row++, "Employment Start Date", "No", "MM/DD/YYYY", "Date employment began");
        AddFieldDescription(ws, row++, "Employing LEA", "No", "Text", "Name of employing LEA/district");
        AddFieldDescription(ws, row++, "School Site", "No", "Text", "Name of school site");
        AddFieldDescription(ws, row++, "Grade Level", "No", "Text", "Grade level taught (e.g. K-2, 3-5, 9-12)");
        AddFieldDescription(ws, row++, "Subject Area", "No", "Text", "Subject area taught");
        AddFieldDescription(ws, row++, "Job Title", "No", "Text", "Job title (e.g. Elementary Teacher)");

        // Quality Metrics
        AddFieldDescription(ws, row++, "Placement Quality Rating", "No", "Number", "Rating 1-5 (1=Poor, 5=Excellent)");
        AddFieldDescription(ws, row++, "Mentor Teacher Name", "No", "Text", "Name of assigned mentor teacher");
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
        ws.Cell($"A{row}").Value = "• Candidate name, SEID, and Credential Area must match existing funded candidate records";
        ws.Range($"A{row}:F{row}").Merge();
        row++;
        ws.Cell($"A{row}").Value = "• Payment Amount must be a dollar value (e.g., 5000.00)";
        row++;
        ws.Cell($"A{row}").Value = "• Placement Quality Rating must be a whole number between 1 and 5";
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

        int col = 1;
        var headerRow = 1;
        var requiredColor = XLColor.FromHtml("#D6EAF8");
        var optionalColor = XLColor.FromHtml("#E8E8E8");

        // Candidate Identification (required)
        AddHeader(ws, headerRow, col++, "Candidate First Name", requiredColor);     // A
        AddHeader(ws, headerRow, col++, "Candidate Last Name", requiredColor);      // B
        AddHeader(ws, headerRow, col++, "SEID", requiredColor);                     // C
        AddHeader(ws, headerRow, col++, "Credential Area", requiredColor);          // D
        AddHeader(ws, headerRow, col++, "IHE Partner", requiredColor);              // E

        // Payment Information
        AddHeader(ws, headerRow, col++, "Payment Category", requiredColor);         // F
        AddHeader(ws, headerRow, col++, "Payment Schedule", requiredColor);         // G
        AddHeader(ws, headerRow, col++, "Actual Payment Amount", requiredColor);    // H
        AddHeader(ws, headerRow, col++, "First Payment Date", optionalColor);       // I
        AddHeader(ws, headerRow, col++, "Final Payment Date", optionalColor);       // J

        // Program Completion
        AddHeader(ws, headerRow, col++, "Program Completion Status", requiredColor); // K
        AddHeader(ws, headerRow, col++, "Program Completion Date", optionalColor);   // L

        // Credential Information
        AddHeader(ws, headerRow, col++, "Credential Earned Status", requiredColor);  // M
        AddHeader(ws, headerRow, col++, "Credential Issue Date", optionalColor);     // N

        // Employment Information
        AddHeader(ws, headerRow, col++, "Hired in District", requiredColor);         // O
        AddHeader(ws, headerRow, col++, "Employment Status", optionalColor);         // P
        AddHeader(ws, headerRow, col++, "Employment Start Date", optionalColor);     // Q
        AddHeader(ws, headerRow, col++, "Employing LEA", optionalColor);             // R
        AddHeader(ws, headerRow, col++, "School Site", optionalColor);               // S
        AddHeader(ws, headerRow, col++, "Grade Level", optionalColor);               // T
        AddHeader(ws, headerRow, col++, "Subject Area", optionalColor);              // U
        AddHeader(ws, headerRow, col++, "Job Title", optionalColor);                 // V

        // Quality Metrics
        AddHeader(ws, headerRow, col++, "Placement Quality Rating", optionalColor);  // W
        AddHeader(ws, headerRow, col++, "Mentor Teacher Name", optionalColor);       // X
        AddHeader(ws, headerRow, col++, "Additional Notes", optionalColor);          // Y

        // Sample data — 3 rows matching LEA report form scenarios
        AddSampleRow(ws, 2,
            "John", "Smith", "SE12345678", "Multiple Subject", "CSU Sacramento",
            "Stipend", "Lump Sum", "5000.00", "09/01/2025", "09/01/2025",
            "Completed", "12/15/2025",
            "Credential Earned", "01/10/2026",
            "Yes", "Full-Time Teacher", "01/15/2026", "Sacramento City USD", "Lincoln Elementary", "K-2", "General Education", "Elementary Teacher",
            "5", "Jane Doe", "");

        AddSampleRow(ws, 3,
            "Maria", "Garcia", "SE23456789", "Single Subject - Mathematics", "UCLA",
            "Monthly", "Monthly", "1500.00", "09/01/2025", "",
            "In Progress", "",
            "In Progress", "",
            "No", "Seeking Employment", "", "", "", "", "", "",
            "", "", "Expected to complete by June 2026");

        AddSampleRow(ws, 4,
            "David", "Chen", "SE34567890", "Education Specialist - Mild/Moderate", "San Jose State",
            "Tuition Reimbursement", "Semester", "3200.00", "08/15/2025", "12/20/2025",
            "Not Completed", "",
            "Not Earned", "",
            "No", "Not Hired as Teacher", "", "", "", "", "", "",
            "", "", "Withdrew from program due to personal reasons");

        // Data validation dropdowns
        ws.Range("D5:D1000").SetDataValidation().List("'Reference Data'!$A$2:$A$20", true);   // Credential Area
        ws.Range("F5:F1000").SetDataValidation().List("'Reference Data'!$C$2:$C$5", true);    // Payment Category
        ws.Range("G5:G1000").SetDataValidation().List("'Reference Data'!$E$2:$E$5", true);    // Payment Schedule
        ws.Range("K5:K1000").SetDataValidation().List("'Reference Data'!$G$2:$G$4", true);    // Program Completion Status
        ws.Range("M5:M1000").SetDataValidation().List("'Reference Data'!$I$2:$I$4", true);    // Credential Earned Status
        ws.Range("O5:O1000").SetDataValidation().List("'Reference Data'!$K$2:$K$3", true);    // Hired in District (Yes/No)
        ws.Range("P5:P1000").SetDataValidation().List("'Reference Data'!$M$2:$M$5", true);    // Employment Status

        // Apply background colors to data entry area (rows 5-1000)
        for (int i = 5; i <= 1000; i++)
        {
            // Required: A-H, K, M, O
            ws.Range($"A{i}:H{i}").Style.Fill.BackgroundColor = requiredColor;
            ws.Cell($"K{i}").Style.Fill.BackgroundColor = requiredColor;
            ws.Cell($"M{i}").Style.Fill.BackgroundColor = requiredColor;
            ws.Cell($"O{i}").Style.Fill.BackgroundColor = requiredColor;

            // Optional: I-J, L, N, P-Y
            ws.Range($"I{i}:J{i}").Style.Fill.BackgroundColor = optionalColor;
            ws.Cell($"L{i}").Style.Fill.BackgroundColor = optionalColor;
            ws.Cell($"N{i}").Style.Fill.BackgroundColor = optionalColor;
            ws.Range($"P{i}:Y{i}").Style.Fill.BackgroundColor = optionalColor;
        }

        ws.Columns().AdjustToContents();
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

        // Payment Category (column C)
        ws.Cell("C1").Value = "Payment Category";
        ws.Cell("C1").Style.Font.Bold = true;
        ws.Cell("C1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"C{row++}").Value = "Stipend";
        ws.Cell($"C{row++}").Value = "Salary";
        ws.Cell($"C{row++}").Value = "Tuition Reimbursement";
        ws.Cell($"C{row++}").Value = "Other";

        // Payment Schedule (column E)
        ws.Cell("E1").Value = "Payment Schedule";
        ws.Cell("E1").Style.Font.Bold = true;
        ws.Cell("E1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"E{row++}").Value = "Lump Sum";
        ws.Cell($"E{row++}").Value = "Monthly";
        ws.Cell($"E{row++}").Value = "Quarterly";
        ws.Cell($"E{row++}").Value = "Semester";

        // Program Completion Status (column G)
        ws.Cell("G1").Value = "Program Completion Status";
        ws.Cell("G1").Style.Font.Bold = true;
        ws.Cell("G1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"G{row++}").Value = "Completed";
        ws.Cell($"G{row++}").Value = "In Progress";
        ws.Cell($"G{row++}").Value = "Not Completed";

        // Credential Earned Status (column I)
        ws.Cell("I1").Value = "Credential Earned Status";
        ws.Cell("I1").Style.Font.Bold = true;
        ws.Cell("I1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"I{row++}").Value = "Credential Earned";
        ws.Cell($"I{row++}").Value = "In Progress";
        ws.Cell($"I{row++}").Value = "Not Earned";

        // Yes/No (column K)
        ws.Cell("K1").Value = "Yes/No";
        ws.Cell("K1").Style.Font.Bold = true;
        ws.Cell("K1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"K{row++}").Value = "Yes";
        ws.Cell($"K{row++}").Value = "No";

        // Employment Status (column M)
        ws.Cell("M1").Value = "Employment Status";
        ws.Cell("M1").Style.Font.Bold = true;
        ws.Cell("M1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        row = 2;
        ws.Cell($"M{row++}").Value = "Full-Time Teacher";
        ws.Cell($"M{row++}").Value = "Part-Time Teacher";
        ws.Cell($"M{row++}").Value = "Seeking Employment";
        ws.Cell($"M{row++}").Value = "Not Hired as Teacher";

        ws.Columns().AdjustToContents();
    }
}
