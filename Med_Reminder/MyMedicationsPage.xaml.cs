using iText.Kernel.Pdf;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using iText.Layout;
using iText.Layout.Element;
using System.IO;

namespace Med_Reminder;

public partial class MyMedicationsPage : ContentPage
{
    private List<string> medicationList = new List<string>();
    private MyMedicationsPageLogic pageLogic = new MyMedicationsPageLogic();
    private MyAppDbContext dbContext;

    public MyMedicationsPage()
    {
        InitializeComponent();
        dbContext = new MyAppDbContext();
        LoadMedications();
    }

    private async void LoadMedications()
    {
        string filePath = "C:\\Users\\korne\\OneDrive\\Pulpit\\Med-Reminder-main\\Med-Reminder-main\\Med_Reminder\\Lista_lekow.txt"; // Change to the correct path
        List<string> medications = pageLogic.ReadTextFile(filePath);

        if (medications != null && medications.Count > 0)
        {
            foreach (string medication in medications)
            {
                MedicationPicker.Items.Add(medication);
            }
        }
        else
        {
            await DisplayAlert("B��d", "Nie mo�na za�adowa� lek�w.", "OK");
        }
    }

    private async void SaveMedicationButton_Clicked(object sender, EventArgs e)
    {
        var selectedMedication = MedicationPicker.SelectedItem?.ToString();
        string comment = CommentEntry.Text;
        string dosage = DosageEntry.Text;
        if (string.IsNullOrWhiteSpace(selectedMedication) && string.IsNullOrEmpty(MedicationEntry.Text))
        {
            await DisplayAlert("B��d", "Wybierz lek albo wpisz w�asny.", "OK");
            return;
        }

        Lek newMedication = new Lek()
        {
            NazwaLeku = selectedMedication,
            DaneOsoboweId = App.CurrentUserId // Linking medication with the logged-in user
        };

        // Save medication to the database
        dbContext._leki.Add(newMedication);
        await dbContext.SaveChangesAsync();

        // Display success message
        await DisplayAlert("Sukces", "Lek zosta� zapisany.", "OK");

        // Refresh medication list
        RefreshMedicationList();
    }

    private void RefreshMedicationList()
    {
        MedicationList.ItemsSource = null;
        MedicationList.ItemsSource = medicationList;
    }

    private void MedicationEntry_Completed(object sender, EventArgs e)
    {
        string newMedication = MedicationEntry.Text;

        if (!string.IsNullOrWhiteSpace(newMedication))
        {
            MedicationPicker.Items.Add(newMedication);
            MedicationEntry.Text = string.Empty; // Clear the medication entry field after adding
        }
    }

    private async void SaveMedicationAsPDFButton_Clicked(object sender, EventArgs e)
    {
        var selectedMedication = MedicationPicker.SelectedItem?.ToString();

        if (selectedMedication != null)
        {
            // Fetch user's name and ID from the database
            var currentUser = await dbContext._dane_osobowe.FindAsync(App.CurrentUserId);
            if (currentUser == null)
            {
                await DisplayAlert("B��d", "Nie znaleziono u�ytkownika.", "OK");
                return;
            }
            string userName = $"{currentUser.Imie} {currentUser.Nazwisko}";
            string userId = currentUser.Id.ToString();

            // Get comment and dosage entered by the user
            string comment = CommentEntry.Text;
            string dosage = DosageEntry.Text;
            string ownMedication = MedicationEntry.Text;

            // Add selected medication to the list of selected medications
            List<string> selectedMedications = new List<string>();

            if (!string.IsNullOrWhiteSpace(ownMedication))
            {
                // Add own medication to the list of selected medications
                selectedMedications.Add(ownMedication);
            }
            selectedMedications.Add(selectedMedication);
            selectedMedications.Add("Komentarz: " + comment);
            selectedMedications.Add("Dawka: " + dosage);

            // Generate PDF file name based on the current date and time
            string fileName = "Lista_lek�w_" + DateTime.Now.ToString("yyyy_MM_dd") + ".pdf";

            // Path to the folder where the PDF file will be saved
            string folderPath = "C:\\Users\\korne\\OneDrive\\Pulpit\\Med-Reminder-main\\Med-Reminder-main\\Med_Reminder";
            string filePath = Path.Combine(folderPath, fileName);

            // Create PDF document
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filePath));
            Document doc = new Document(pdfDoc);

            // Add header with user's name and ID to the PDF document
            Paragraph header = new Paragraph($"U�ytkownik: {userName} (ID: {userId})")
                .SetBold()
                .SetFontSize(14);
            doc.Add(header);

            Paragraph subHeader = new Paragraph("Lista wybranych lek�w:")
                .SetBold()
                .SetFontSize(12);
            doc.Add(subHeader);

            foreach (var medication in selectedMedications)
            {
                Paragraph para = new Paragraph(medication);
                doc.Add(para);
            }

            // Finish editing the document and save it to the file
            doc.Close();

            // Display message after saving the PDF file
            await DisplayAlert("Sukces", "Lista lek�w zosta�a zapisana jako plik PDF.","OK");
        }
        else
        {
            // Display message if no medication is selected
            await DisplayAlert("B��d", "Wybierz lek z listy.", "OK");
        }
    }
}
