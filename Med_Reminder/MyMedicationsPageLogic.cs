using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iText.Layout.Element;

namespace Med_Reminder
{
    public class MyMedicationsPageLogic
    {
        public List<string> ReadTextFile(string filePath)
        {
            List<string> fileLines = new List<string>();

            if (!File.Exists(filePath))
            {
                return fileLines;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    fileLines.Add(line);
                }
            }

            return fileLines;
        }

        public void SaveMedicationToList(string selectedMedication, string comment, List<string> medicationList)
        {
          
            string medicationWithComment = $"{selectedMedication}: {comment}";
            medicationList.Add(medicationWithComment);
        }
    }
}
