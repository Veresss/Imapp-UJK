using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace EmptyMauiApp.Views;

public partial class HelperPage : BasePage
{
    private List<Prayer> prayers;
    private Random random = new Random();

    public HelperPage()
    {
        InitializeComponent();
        UpdateQuestion();
        LoadPrayers();
    }

    private void LoadPrayers()
    {
        prayers = new List<Prayer>
        {
            new Prayer("Elcsendesedés", 5, 10, new List<int> { 1 }),
            new Prayer("Napi Hálaadás", 10, 15, new List<int> { 2 }),
            new Prayer("Napi Evangélium & Ima Az Igével", 10, 15, new List<int> { 1, 2 }),
            new Prayer("Napi Olvasmány & Elmélkedés", 5, 10, new List<int> { 1, 2 }),
            new Prayer("Miatyánk", 1, 1, new List<int> { 1 }),
            new Prayer("Üdvözlégy", 1, 1, new List<int> { 3 }),
            new Prayer("Egy Tized Rózsafűzér", 5, 5, new List<int> { 1, 2, 3 }),
            new Prayer("Teljes Rózsafűzér", 20, 20, new List<int> { 1, 2, 3 }),
            new Prayer("Nyelvima", 10, 15, new List<int> { 2 }),
            new Prayer("Hitvallás", 2, 2, new List<int> { 1 }),
            new Prayer("Szent Mihály Arkangyal", 1, 1, new List<int> { 3 }),
            new Prayer("Az Úr Angyala", 5, 5, new List<int> { 3 }),
            new Prayer("Kötetlen Beszélgetés Istennel", 10, 20, new List<int> { 2 }),
            new Prayer("Szemlélődő Ima", 15, 20, new List<int> { 2 }),
            new Prayer("Az Úrral Való Egység Imája", 2, 2, new List<int> { 3 }),
            new Prayer("Dicsőség az Atyának", 1, 1, new List<int> { 3 }),
            new Prayer("Rövid Szentlélekhívás", 3, 3, new List<int> { 1 })
        };
    }

    private void UpdateQuestion()
    {
        string userName = Preferences.Get("UserName", "Vendég");

        List<string> _howMany = new List<string>();
        _howMany.Add("Hány percet");
        _howMany.Add("Mennyi időt");
        int c1 = random.Next(_howMany.Count);

        List<string> _toTake = new List<string>();
        _toTake.Add("szánsz");
        _toTake.Add("szánnál");
        int c2 = random.Next(_toTake.Count);

        List<string> _today = new List<string>();
        _today.Add(" ma");
        _today.Add(" most");
        _today.Add("");
        int c3 = random.Next(_today.Count);

        string how_Many = _howMany[c1];
        string to_Take = _toTake[c2];
        string today = _today[c3];

        HelpLabelQuestion.Text = $"{how_Many} {to_Take}{today} imára, {userName}?";
    }

    private void OnTimeEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        GenerateButton.IsEnabled = int.TryParse(TimeEntry.Text, out int time) && time > 0 && time < 61;
    }

    private async void OnGeneratePlanClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(TimeEntry.Text, out int totalTime))
        {
            await DisplayAlert("Hiba", "Érvénytelen időtartam.", "OK");
            return;
        }

        // Tisztítás és Border elrejtése
        PlanLayout.Children.Clear();
        PlanBorder.IsVisible = false;
        PlanBorder.HeightRequest = 40;
        PlanBorder.ScaleX = 0.1;

        var plan = GeneratePrayerPlan(totalTime);

        if (plan.Count == 0)
        {
            var noPlanLabel = new Label
            {
                Text = "Nem sikerült tervet generálni a megadott időhöz. Próbálj nagyobb időt megadni.",
                TextColor = Color.FromArgb("#FF0000"),
                FontSize = 18,
                FontFamily = "AptosExtrabold",
                FontAttributes = FontAttributes.Italic,
                Opacity = 0
            };

            PlanLayout.Children.Add(noPlanLabel);

            // Mérjük a magasságot
            PlanBorder.IsVisible = true;
            PlanBorder.HeightRequest = -1;
            await Task.Delay(50);
            double _fullHeight = PlanBorder.Height;
            PlanBorder.HeightRequest = 40;

            // Border animációk
            await PlanBorder.ScaleXTo(1, 300, Easing.SinInOut);
            new Animation(v => PlanBorder.HeightRequest = v, 40, _fullHeight)
                .Commit(PlanBorder, "ExpandHeight", 16, 500, Easing.SinOut);

            noPlanLabel.FadeTo(1, 200);
            return;
        }

        int estimatedTime = plan.Sum(p => random.Next(p.MinTime, p.MaxTime + 1));
        var titleLabel = new Label
        {
            Text = $"Generált Ima Terv: \n (kb. {estimatedTime} perc)",
            FontSize = 26,
            FontFamily = "AptosExtrabold",
            LineBreakMode = LineBreakMode.WordWrap,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(20, 0, 20, 20),
            Opacity = 0
        };

        titleLabel.SetDynamicResource(Label.TextColorProperty, "PrimaryColor");

        var prayerLabels = new List<Label>();

        foreach (var prayer in plan)
        {
            var label = new Label
            {
                Text = $"- {prayer.Name}",
                FontSize = 18,
                FontFamily = "AptosExtrabold",
                LineBreakMode = LineBreakMode.WordWrap,
                Padding = new Thickness(10, 5),
                Margin = new Thickness(20, 0, 20, 0),
                Opacity = 0
            };

            label.SetDynamicResource(Label.TextColorProperty, "PrimaryColor");

            prayerLabels.Add(label);
        }

        PlanLayout.Children.Add(titleLabel);
        foreach (var label in prayerLabels)
        {
            PlanLayout.Children.Add(label);
        }

        PlanBorder.IsVisible = true;
        PlanBorder.HeightRequest = -1;
        await Task.Delay(50);
        double fullHeight = PlanBorder.Height;
        PlanBorder.HeightRequest = 40;

        // Border animációk
        await PlanBorder.ScaleXTo(1, 300, Easing.SinInOut);
        new Animation(v => PlanBorder.HeightRequest = v, 40, fullHeight)
            .Commit(PlanBorder, "ExpandHeight", 16, 500, Easing.SinOut);

        // Cím és imák fade-in
        titleLabel.FadeTo(1, 200);
        await Task.Delay(100);
        foreach (var label in prayerLabels)
        {
            label.FadeTo(1, 200);
            await Task.Delay(100);
        }

        // Auto magasság visszaállítása
        await Task.Delay(500);
        PlanBorder.HeightRequest = -1;
    }

    private List<Prayer> GeneratePrayerPlan(int totalTime)
    {
        var section1Prayers = prayers.Where(p => p.Sections.Contains(1)).ToList();
        var section2Prayers = prayers.Where(p => p.Sections.Contains(2)).ToList();
        var section3Prayers = prayers.Where(p => p.Sections.Contains(3)).ToList();

        var plan = new List<Prayer>();
        var usedNames = new HashSet<string>();
        int remainingTime = totalTime;

        // Kizárjuk a "Miatyánk" és "Üdvözlégy" imákat a nem keret szekciókból
        var miatyank = prayers.FirstOrDefault(p => p.Name == "Miatyánk");
        var udvozlegy = prayers.FirstOrDefault(p => p.Name == "Üdvözlégy");

        // Kezdeti szűrők a rózsafűzérekre
        var rozsafuzerek = new List<string> { "Egy Tized Rózsafűzér", "Teljes Rózsafűzér" };
        bool hasRozsafuzer = false;
        string? selectedRozsafuzer = null;

        // Section 1 kiválasztása (kötelező 1 db)
        Prayer? selectedSection1 = null;
        if (!section1Prayers.Any())
        {
            return new List<Prayer>();
        }

        // Döntés: Miatyánk-Üdvözlégy keret vagy normál ima
        bool useMiatyankFrame = random.Next(2) == 0 && miatyank != null && udvozlegy != null;
        if (useMiatyankFrame)
        {
            selectedSection1 = miatyank;
            int duration1 = random.Next(selectedSection1.MinTime, selectedSection1.MaxTime + 1);
            remainingTime -= duration1;
            usedNames.Add(selectedSection1.Name);
            plan.Add(selectedSection1);
        }
        else
        {
            var fittingSection1 = section1Prayers
                .Where(p => p.Name != "Miatyánk" && !usedNames.Contains(p.Name))
                .OrderBy(p => p.MinTime)
                .ToList();
            if (!fittingSection1.Any())
            {
                return new List<Prayer>();
            }
            selectedSection1 = fittingSection1[random.Next(fittingSection1.Count)];
            int duration1 = random.Next(selectedSection1.MinTime, selectedSection1.MaxTime + 1);
            remainingTime -= duration1;
            usedNames.Add(selectedSection1.Name);
            plan.Add(selectedSection1);
        }

        // Section 2-k hozzáadása (több is lehet, amíg belefér az idő)
        while (remainingTime > 0)
        {
            var fittingSection2 = section2Prayers
                .Where(p => !usedNames.Contains(p.Name) && p.MaxTime <= remainingTime)
                .ToList();

            // Rózsafűzér szabály: csak az egyik szerepelhet
            if (hasRozsafuzer)
            {
                fittingSection2 = fittingSection2.Where(p => !rozsafuzerek.Contains(p.Name)).ToList();
            }
            else
            {
                fittingSection2 = fittingSection2.Where(p => p.Name != "Miatyánk" && p.Name != "Üdvözlégy").ToList();
            }

            if (!fittingSection2.Any())
            {
                break;
            }

            var selectedSection2 = fittingSection2[random.Next(fittingSection2.Count)];
            if (rozsafuzerek.Contains(selectedSection2.Name))
            {
                hasRozsafuzer = true;
                selectedRozsafuzer = selectedSection2.Name;
            }

            int duration2 = random.Next(selectedSection2.MinTime, selectedSection2.MaxTime + 1);
            plan.Add(selectedSection2);
            remainingTime -= duration2;
            usedNames.Add(selectedSection2.Name);
        }

        // Section 3 kiválasztása (kötelező 1 db)
        Prayer? selectedSection3 = null;
        if (useMiatyankFrame)
        {
            selectedSection3 = udvozlegy;
            int duration3 = random.Next(selectedSection3.MinTime, selectedSection3.MaxTime + 1);
            usedNames.Add(selectedSection3.Name);
            plan.Add(selectedSection3);
        }
        else
        {
            var fittingSection3 = section3Prayers
                .Where(p => p.Name != "Üdvözlégy" && !usedNames.Contains(p.Name))
                .OrderBy(p => p.MinTime)
                .ToList();

            if (hasRozsafuzer)
            {
                fittingSection3 = fittingSection3.Where(p => !rozsafuzerek.Contains(p.Name)).ToList();
            }

            if (!fittingSection3.Any())
            {
                return new List<Prayer>();
            }
            selectedSection3 = fittingSection3[random.Next(fittingSection3.Count)];
            int duration3 = random.Next(selectedSection3.MinTime, selectedSection3.MaxTime + 1);
            usedNames.Add(selectedSection3.Name);
            plan.Add(selectedSection3);
        }

        return plan;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
        this.ForceLayout();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

public class Prayer
{
    public string Name { get; set; }
    public int MinTime { get; set; }
    public int MaxTime { get; set; }
    public List<int> Sections { get; set; }

    public Prayer(string name, int minTime, int maxTime, List<int> sections)
    {
        Name = name;
        MinTime = minTime;
        MaxTime = maxTime;
        Sections = sections;
    }
}