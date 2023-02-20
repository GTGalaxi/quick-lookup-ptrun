using System;
using System.IO;
using ManagedCommon;
using System.Collections.Generic;
using Wox.Plugin;
using System.Text.RegularExpressions;
using System.Linq;

namespace QuickLookup
{
    public class Tool 
    {
        private string name;
        private string url;
        private bool enabled;

        public Tool(string name, string url, bool enabled) {
            this.name = name;
            this.url = url;
            this.enabled = enabled;
        }

        public string Name {
            get { return name;}
            set { name = value; }
        }

        public string URL {
            get { return url;}
            set { url = value; }
        }

        public bool Enabled {
            get { return enabled;}
            set { enabled = value; }
        }
        
    }
    public static class QLConfig 
    {
        public static List<Tool> ParseConfig() {
            string config = File.ReadAllText("modules/launcher/Plugins/QuickLookup/tools.conf");
            config = config.Replace("\r\n", "\n");
            List<Tool> Tools  = Regex.Matches(config, @"(?:\w*=([^\n]*)\n?){3}", RegexOptions.Multiline)
                                    .Cast<Match>()
                                    .Select(tool => new Tool(tool.Groups[1].Captures[0].Value, tool.Groups[1].Captures[1].Value, tool.Groups[1].Captures[2].Value == "1" ? true : false))
                                    .ToList();
            return Tools;
        }
        public static string UpdateSubTitle(List<Tool> Tools) {
            string subTitle = string.Join(", ", Tools.Where(t => t.Enabled).Select(t => t.Name));
            if (!string.IsNullOrEmpty(subTitle))
            {
                int place = subTitle.LastIndexOf(", ");
                if (place >= 0)
                {
                    subTitle = subTitle.Remove(place, 2).Insert(place, " & ");
                }
            }
            return subTitle;
        }
    }
    public class Main : IPlugin
    {
        private string ?IconPath { get; set; }
        private PluginInitContext ?Context { get; set; }
        public string Name => "Quick Lookup";
        public string Description => "Quick Lookup IP Addresses";
        public string SubTitle => "Quick Lookup IP Addresses";
        public List<Tool> Tools { get; set; }

        public List<Result> Query(Query query)
        {
            Tools = QLConfig.ParseConfig();
            string SubTitle = QLConfig.UpdateSubTitle(Tools);
            
            var QueryIn = query.Search;
            List<Result> results = new List<Result>();
            results.Add(new Result
            {
                Title = QueryIn,
                SubTitle = SubTitle,
                IcoPath = IconPath,
                Action = e =>
                {
                    foreach (var Tool in Tools)
                    {
                        if (Tool.Enabled) {
                            System.Diagnostics.Process.Start("explorer", string.Format(Tool.URL, QueryIn));
                        }
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
            Tools = QLConfig.ParseConfig();
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