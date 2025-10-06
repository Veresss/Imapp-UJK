using System.Reflection;
using System.Diagnostics;

namespace EmptyMauiApp.Views;

public class Verse
{
    public string Sentence { get; set; }  //idezet
    public string Reference { get; set; } //hivatkozas
}

public partial class StartPage : BasePage
{
    //csak olvasando bibliai verslista
    private readonly List<Verse> _verses = new List <Verse>();
    public StartPage()
    {
        InitializeComponent();  //xaml inicializalashoz
        LoadVerses();
        DisplayTitle();
        DisplayRandomVerse();
        LoadImages();
    }

    private async void GoToMenuPage(object sender, EventArgs e) 
    {
        if (!Preferences.ContainsKey("UserName")) //vizsgalat, hogy van-e mar felhasznalonev
        {
            await Navigation.PushAsync(new NamePage());
        }
        else
        {
            await Navigation.PushAsync(new MenuPage());
        }
    }

    private void LoadImages()
    {
        TopImage.Source = ImageSource.FromFile("logo.png");
        MiddleImage.Source = ImageSource.FromFile("arrow.png");
    }

    private async void LoadVerses()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("verses.txt");
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (!string.IsNullOrWhiteSpace(line))
                {
                    var verse = ParseVerseLine(line);
                    if (verse != null)
                        _verses.Add(verse);
                }
            }
        }
        catch (Exception ex)
        {
            VerseLabel.Text = "Verse Loading Error"; //xml-ben meghivatkozva
            Debug.WriteLine($"HIBA: {ex}"); //system.diagnostics
        }
    }

    private Verse ParseVerseLine(string line)
    {
        var parts = line.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            return new Verse
            {
                Sentence = parts[0].Trim(),
                Reference = parts[1].Trim()
            };
        }
        return null;
    }

    private void DisplayRandomVerse()
    {
        if (_verses.Count == 0)
            return;

        var random = new Random();
        Verse verse = _verses[random.Next(0, _verses.Count)];

        VerseLabel.Text = verse.Sentence;
        ReferenceLabel.Text = $"— {verse.Reference}";
    }

    private void DisplayTitle()
    {
        TitleLabel.Text = "ISTEN MELLETT DÖNTÖK!";
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
        this.ForceLayout();
    }
}