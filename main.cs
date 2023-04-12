using ManagedCommon;
using Wox.Plugin;
using System.Text.RegularExpressions;

namespace QuickLookup
{
    public class Tool 
    {
        private string name;
        private string url;
        private string[] categories;
        private bool enabled;

        public Tool(string name, string url, string[] categories, bool enabled) {
            this.name = name;
            this.url = url;
            this.categories = categories;
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

        public string[] Categories {
            get { return categories;}
            set { categories = value; }
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
            List<Tool> Tools  = Regex.Matches(config, @"(?:\w*=([^\n]*)\n?){4}", RegexOptions.Multiline)
                                    .Cast<Match>()
                                    .Select(tool => new Tool(tool.Groups[1].Captures[0].Value, tool.Groups[1].Captures[1].Value, tool.Groups[1].Captures[2].Value.ToLower().Split(","), tool.Groups[1].Captures[3].Value == "1" ? true : false))
                                    .ToList();
            return Tools;
        }
        public static string UpdateSubTitle(List<Tool> Tools, string Category) {
            string subTitle = string.Join(", ", Tools.Where(t => t.Categories.Contains(Category)).Where(t => t.Enabled).Select(t => t.Name));
            if (subTitle == "") {
                return "No tools found in category!";
            }
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
        public static string FilterCategories(List<Tool> Tools, string Category) {
            string subTitle = string.Join(", ", GetCategories(Tools).Where(c => (c.Contains(Category.ToLower()) | Category == ""))).ToUpper();
            if (subTitle == "") {
                return "No matching Categories!";
            }
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
        
        public static string[] GetCategories(List<Tool> Tools) {
            List<string> categories = new List<string>();
            string[][] t = Tools.Select(t => t.Categories).ToArray();
            foreach (var cats in t) {
                foreach (var cat in cats) {
                    if (!categories.Contains(cat)) {
                        categories.Add(cat);
                    }
                }
            };
            return categories.ToArray();
        }
    }
    public class Main : IPlugin
    {
        private string ?IconPath { get; set; }
        private PluginInitContext ?Context { get; set; }
        public string Name => "Quick Lookup";
        public string Description => "Quick Lookup";
        public string SubTitle = "Quick Lookup";
        public List<Tool> Tools { get; set; } = new List<Tool>();

        public List<Result> Query(Query query)
        {
            Tools = QLConfig.ParseConfig();

            List<Result> results = new List<Result>();
            
            var QueryIn = query.Search;
            var QuerySplit = QueryIn.Split(" ");
            if (QuerySplit.Length >= 2) {
                var Category = QuerySplit[0].ToLower();
                var Input = QuerySplit[1];

                SubTitle = "Quick Lookup "+ Category.ToUpper() + " using: " + QLConfig.UpdateSubTitle(Tools, Category);
                results.Add(new Result
                {
                    Title = Input,
                    SubTitle = SubTitle,
                    IcoPath = IconPath,
                    Action = e =>
                    {
                        foreach (var Tool in Tools.Where(t => t.Categories.Contains(Category)))
                        {
                            if (Tool.Enabled) {
                                System.Diagnostics.Process.Start("explorer", string.Format(Tool.URL, Input));
                            }
                        }
                        return false;
                    }
                });
            }
            else if (QuerySplit.Length <= 1) {
                SubTitle = "Categories: " + QLConfig.FilterCategories(Tools, QueryIn);
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
            }
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