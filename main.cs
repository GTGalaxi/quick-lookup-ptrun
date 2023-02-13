using ManagedCommon;
using System.Collections.Generic;
using Wox.Plugin;

namespace QuickLookup
{
    public class Main : IPlugin
    {
        private string ?IconPath { get; set; }
        private PluginInitContext ?Context { get; set; }
        public string Name => "Quick Lookup";
        public string Description => "Quick Lookup IP Addresses";
        public List<string> Tools => new List<string>() { "https://www.shodan.io/host/", "https://viz.greynoise.io/ip/", "https://www.virustotal.com/gui/ip-address/"};

        public List<Result> Query(Query query)
        {
            var QueryIn = query.Search;
            List<Result> results = new List<Result>();
            results.Add(new Result
            {
                Title = QueryIn,
                SubTitle = "Quick Lookup in Shodan, GreyNoise & VirusTotal",
                IcoPath = IconPath,
                Action = e =>
                {
                    foreach (var Tool in Tools)
                    {
                        System.Diagnostics.Process.Start("explorer", Tool+QueryIn);
                    }
                    return false;
                }
            });
            return results;
        }

        public void Init(PluginInitContext context)
        {
            Context = context;
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
        }

        private void UpdateIconPath(Theme theme)
        {
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                IconPath = "img/ql.light.png"; // Light Theme
            }
            else
            {
                IconPath = "img/ql.dark.png"; // Dark Theme
            }
        }

        private void OnThemeChanged(Theme currentTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }
    }
}