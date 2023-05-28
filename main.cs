using ManagedCommon;
using Wox.Plugin;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Diagnostics;

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
        public string Name { get { return name;} set { name = value; } }
        public string URL { get { return url;} set { url = value; } } 
        public string[] Categories { get { return categories;} set { categories = value; } } 
        public bool Enabled { get { return enabled;} set { enabled = value; } }
        
    }
    public class Tools
    {
        private List<Tool> tools { get; set; }
        public Tools() { this.tools = QLConfig.ParseConfig(); }
        public Tools(List<Tool> tools) { this.tools = tools; }
        public Tools Update() { return this.tools = QLConfig.ParseConfig(); }
        public IEnumerable<Tool> GetEnumerator() { return this.tools; }
        public static implicit operator List<Tool>(Tools t) => t.tools;
        public static implicit operator Tools(List<Tool> t) => new Tools(t);
        public static Tools DefaultTools() {
            return new List<Tool> {
                new Tool("Shodan","https://www.shodan.io/host/{0}",new string[] {"ip"},true),
                new Tool("VirusTotal","https://www.virustotal.com/gui/search/{0}",new string[] {"ip","domain","hash"},true)
            };
        }
    }
    public static class QLConfig 
    {
        public static Tools ParseConfig(bool force = false) {
            Tools tools = Tools.DefaultTools();
            string pluginDir = Main.Context.CurrentPluginMetadata.PluginDirectory;
            try {
                if (File.Exists(pluginDir+"\\tools.json") && force == false) {
                        string config = File.ReadAllText(pluginDir+"\\tools.json");
                        tools = JsonSerializer.Deserialize<List<Tool>>(config) ?? Tools.DefaultTools();
                } else if (File.Exists(pluginDir+"\\tools.conf")) {
                        string config = File.ReadAllText(pluginDir+"\\tools.conf");
                        config = config.Replace("\r\n", "\n");
                        tools  = (Tools)Regex.Matches(config, @"(?:\w*=([^\n]*)\n?){4}", RegexOptions.Multiline).Cast<Match>()
                            .Select(tool => {var cTool = tool.Groups[1].Captures; return new Tool(cTool[0].Value, cTool[1].Value.Replace("\"", ""), cTool[2].Value.ToLower().Split(","), cTool[3].Value == "1" ? true : false);});
                }
            } catch(Exception e) { Wox.Plugin.Logger.Log.Exception(e.Message, e, typeof(QLConfig), "ParseConfig"); tools = Tools.DefaultTools(); }
            return tools;
        }
        public static bool ConvertConfigToJSON(this Tools Tools, bool force = false) {
            string pluginDir = Main.Context.CurrentPluginMetadata.PluginDirectory;
            if (File.Exists(pluginDir+"\\tools.json") && force == false) { return true; }
            try {
                string jsonString = JsonSerializer.Serialize(Tools.GetEnumerator(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(pluginDir+"\\tools.json", Regex.Unescape(jsonString));
                return true;
            } catch { return false; }
        }
        public static string[] FilterTools(List<Tool> Tools, string Category) {
            return Tools.Where(t => t.Categories.Contains(Category)).Where(t => t.Enabled).Select(t => t.Name).ToArray();
        }
        public static string[] FilterCategories(List<Tool> Tools, string PartialCategory) {
            return Tools.SelectMany(t => t.Categories).Distinct().Where(c => (c.Contains(PartialCategory.ToLower()) | PartialCategory == "")).ToArray();
        }
        public static string Subtitle(this string[] items) {
            return items.Length >= 1 ? (items.Length >= 2 ? string.Join(", ", items[..(items.Length-1)]) + " & " + items[(items.Length-1)] : items[0]) : "No matching Categories or no Tools found in Category!";
        }
        public static (string, int) Subtitle(this string[] items, bool includeCount = true) {
            return (items.Subtitle(), items.Count());
        }
    }
    public class Main : IPlugin
    {
        private string IconPath { get; set; } = "img/ql.dark.png";
        public static PluginInitContext Context { get; set; } = null!;
        public string Name => "Quick Lookup";
        public string Description => "Quick Lookup";
        public string SubTitle = "Quick Lookup";
        public Tools ToolList { get; set; } = null!;

        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();
            ToolList.Update();
            if (Context is null) {
                results.Add(new Result {
                    Title = "ERROR",
                    SubTitle = "Error: null reference to Context",
                    IcoPath = IconPath,
                    Action = e => { return false; }
                });
                return results;
            }
            if (ToolList is null) {
                results.Add(new Result
                {
                    Title = "ERROR",
                    SubTitle = "Error Parsing Tools or no Tools found! Press ENTER to open Plugin folder",
                    IcoPath = IconPath,
                    Action = e => { Process.Start("explorer", Context.CurrentPluginMetadata.PluginDirectory); return false; }
                });
                return results;
            }
            var QuerySplit = query.Search.Split(" ");
            string category = QuerySplit[0].ToLower();
            (string categories, int count) = QLConfig.FilterCategories(ToolList, category).Subtitle(true);
            if (count == 1 && category == categories.ToLower()) {
                string input;
                if (QuerySplit.Length <= 1) {
                    input = "";
                } else {
                    input = QuerySplit[1];
                }
                SubTitle = "Quick Lookup "+ category.ToLower() + " using: " + QLConfig.FilterTools(ToolList, category).Subtitle();
                results.Add(new Result
                {
                    Title = input,
                    SubTitle = SubTitle,
                    IcoPath = IconPath,
                    Action = e =>
                    {
                        foreach (var Tool in ToolList.GetEnumerator().Where(t => t.Categories.Contains(category)))
                        {
                            if (Tool.Enabled) {
                                Process.Start(new ProcessStartInfo(string.Format(Tool.URL, input)) { UseShellExecute = true });
                            }
                        }
                        return false;
                    }
                });
            }
            else if (QuerySplit.Length <= 1) {
                SubTitle = "Categories: " + categories;
                results.Add(new Result
                {
                    Title = count == 1 ? categories.ToLower() : query.Search,
                    SubTitle = SubTitle,
                    IcoPath = IconPath,
                    Action = e =>
                    {
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
            ToolList = new Tools();
            ToolList.ConvertConfigToJSON();
        }
        private void UpdateIconPath(Theme theme) { IconPath = (theme == Theme.Light || theme == Theme.HighContrastWhite) ? "img/ql.light.png" : "img/ql.dark.png"; }
        private void OnThemeChanged(Theme currentTheme, Theme newTheme) { UpdateIconPath(newTheme); }
    }
}