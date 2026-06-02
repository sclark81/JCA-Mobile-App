using JCA.Mobile.Models;

namespace JCA.Mobile.Services;

public class ThemeService
{
    public static readonly Dictionary<string, SchoolTheme> Themes = new()
    {
        {
            "classic", new SchoolTheme
            {
                Key = "classic",
                Name = "Classic",
                SchoolName = "Joshua Christian Academy",
                Primary = "#1B3A5C",
                Secondary = "#C9A84C",
                Accent = "#E8DCC8",
                Bg = "#F7F5F0",
                LogoEmoji = "📖",
                Tagline = "Equipping Students for Excellence"
            }
        },
        {
            "modern", new SchoolTheme
            {
                Key = "modern",
                Name = "Modern",
                SchoolName = "Joshua Christian Academy",
                Primary = "#0A4D3C",
                Secondary = "#FFD700",
                Accent = "#F0F9F6",
                Bg = "#FAFBFE",
                LogoEmoji = "✨",
                Tagline = "Faith and Learning in Harmony"
            }
        }
    };

    private SchoolTheme _currentTheme = Themes["classic"];
    public SchoolTheme CurrentTheme 
    {
        get => _currentTheme;
        private set => _currentTheme = value;
    }

    public void SetTheme(string key)
    {
        if (Themes.TryGetValue(key, out var theme))
        {
            CurrentTheme = theme;
            UpdateApplicationResources(theme);
        }
    }

    private void UpdateApplicationResources(SchoolTheme theme)
    {
        // In MAUI, this would update the App.Current.Resources
    }
}
