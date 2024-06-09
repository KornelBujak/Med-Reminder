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
        string filePath = "C:\\Users\\korne\\OneDrive\\Pulpit\\Med-Reminder-main\\Med-Reminder-main\\Med_Reminder\\Lista_lekow.txt"; 
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
            await DisplayAlert("B³¹d", "Nie mo¿na za³adowaæ leków.", "OK");
        }
    }

    private async void SaveMedicationButton_Clicked(object sender, EventArgs e)
    {
        var selectedMedication = MedicationPicker.SelectedItem?.ToString();
        string comment = CommentEntry.Text;
        string dosage = DosageEntry.Text;
        if (string.IsNullOrWhiteSpace(selectedMedication) && string.IsNullOrEmpty(MedicationEntry.Text))
        {
            await DisplayAlert("B³¹d", "Wybierz lek albo wpisz w³asny.", "OK");
            return;
        }

        Lek newMedication = new Lek()
        {
            NazwaLeku = selectedMedication,
            DaneOsoboweId = App.CurrentUserId 
        };

        dbContext._leki.Add(newMedication);
        await dbContext.SaveChangesAsync();

        await DisplayAlert("Sukces", "Lek zosta³ zapisany.", "OK");

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
            MedicationEntry.Text = string.Empty; 
        }
    }

    private async void SaveMedicationAsPDFButton_Clicked(object sender, EventArgs e)
    {
        var selectedMedication = MedicationPicker.SelectedItem?.ToString();

        if (selectedMedication != null)
        {
            var currentUser = await dbContext._dane_osobowe.FindAsync(App.CurrentUserId);
            if (currentUser == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono u¿ytkownika.", "OK");
                return;
            }
            string userName = $"{currentUser.Imie} {currentUser.Nazwisko}";
            string userId = currentUser.Id.ToString();

            string comment = CommentEntry.Text;
            string dosage = DosageEntry.Text;
            string ownMedication = MedicationEntry.Text;

            List<string> selectedMedications = new List<string>();

            if (!string.IsNullOrWhiteSpace(ownMedication))
            {
                selectedMedications.Add(ownMedication);
            }
            selectedMedications.Add(selectedMedication);
            selectedMedications.Add("Komentarz: " + comment);
            selectedMedications.Add("Dawka: " + dosage);

            string fileName = "Lista_leków_" + DateTime.Now.ToString("yyyy_MM_dd") + ".pdf";

            string folderPath = "C:\\Users\\korne\\OneDrive\\Pulpit\\Med-Reminder-main\\Med-Reminder-main\\Med_Reminder";
            string filePath = Path.Combine(folderPath, fileName);

            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(filePath));
            Document doc = new Document(pdfDoc);
            Paragraph header = new Paragraph($"U¿ytkownik: {userName} (ID: {userId})")
                .SetBold()
                .SetFontSize(14);
            doc.Add(header);

            Paragraph subHeader = new Paragraph("Lista wybranych leków:")
                .SetBold()
                .SetFontSize(12);
            doc.Add(subHeader);

            foreach (var medication in selectedMedications)
            {
                Paragraph para = new Paragraph(medication);
                doc.Add(para);
            }

            doc.Close();

            await DisplayAlert("Sukces", "Lista leków zosta³a zapisana jako plik PDF.","OK");
        }
        else
        {
            await DisplayAlert("B³¹d", "Wybierz lek z listy.", "OK");
        }
    }
}
