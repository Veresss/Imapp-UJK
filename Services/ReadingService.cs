using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmptyMauiApp.Models;
using System.IO;
using System.Reflection;

namespace EmptyMauiApp.Services
{
    public static class ReadingService
    {
        private static List<DailyReading> _readings;

        public static void LoadReadings()
        {
            if (_readings != null) return;

            _readings = new List<DailyReading>();
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "EmptyMauiApp.Resources.Raw.Cyear2025.csv"; // Projekt namespace + Resources/Raw mappa
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) return;

                using var reader = new StreamReader(stream);
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (values.Length >= 3 &&
                        DateTime.TryParse(values[0].Trim('"'), out var date))
                    {
                        _readings.Add(new DailyReading
                        {
                            Date = date,
                            FirstReading = values[1].Trim('"').Replace("Bármely olvasmány a 668. számú alkalomból vagy a Halottakért tartott miseből (1011-1016)", "Halottak napja olvasmány"),
                            Gospel = values[2].Trim('"').Replace("Bármely evangélium a 668. számú alkalomból vagy a Halottakért tartott miseből (1011-1016)", "Halottak napja evangélium")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a CSV beolvasásakor: {ex.Message}");
            }
        }

        public static DailyReading GetReadingForDate(DateTime date)
        {
            LoadReadings();
            return _readings?.FirstOrDefault(r => r.Date.Date == date.Date) ?? new DailyReading
            {
                Date = date,
                FirstReading = "Nincs adat",
                Gospel = "Nincs adat"
            };
        }
    }
}