# Quick Lookup Plugin for PowerToys Run

![build-status](https://github.com/GTGalaxi/quick-lookup-ptrun/actions/workflows/dotnet.yml/badge.svg) <a href="https://github.com/GTGalaxi/quick-lookup-ptrun/releases">![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/GTGalaxi/quick-lookup-ptrun?include_prereleases)</a>

This plugin for [PowerToys Run](https://learn.microsoft.com/en-us/windows/powertoys/run) allows you to quickly search for an IP address, domain name, hash or any other data points in a list of Cyber Security tools. It's perfect for security analysts, penetration testers, or anyone else who needs to quickly lookup information when investigating artifacts or alerts.

## Installation

> **NOTE: As of version 0.72.0 or 0.73.0 (Not 100% sure as the release notes don't mention this), the directory for Powertoys Run plugins was changed!**
>
> If you already installed this or another plugin and it recently stopped working this may be why. You will need to move the plugin(s) from the old directory to the new one.
> 
> The old plugin directory was:
> - **For machine wide install of PowerToys:** `C:\Program Files\PowerToys\modules\launcher\Plugins`
> - **For per user install of PowerToys:** `C:\Users\<yourusername>\AppData\Local\PowerToys\modules\launcher\Plugins`

To install the plugin:

- Navigate to your Powertoys Run Plugin folder
  - **For machine wide install of PowerToys:** `C:\Program Files\PowerToys\RunPlugins`
  - **For per user install of PowerToys:** `C:\Users\<yourusername>\AppData\Local\PowerToys\RunPlugins`
- Create a new folder called QuickLookup
- Extract the contents of the zip file into the folder you just created
- Restart PowerToys and the plugin should be loaded under the Run tool settings and work when promted with `ql`

![ptr-ql](https://user-images.githubusercontent.com/10473238/232273294-1e9d4fec-fb8a-45e2-8780-0214aa6ef528.png)

## Usage

To use the plugin, simply open PowerToys Run by pressing Alt+Space and type the activation command `ql` followed by the tool category and the data you want to lookup.

![ptr-v1 2 0](https://user-images.githubusercontent.com/10473238/231605857-2427613f-7206-4899-8d82-3c660cecfd98.gif)

The plugin will open the data searched in a new tab in your default browser for each tool registered with that category.

![QuickLookup](https://user-images.githubusercontent.com/10473238/227844315-0a865672-9eb3-4f35-afc5-d6c196fd009d.gif)

## Default Tools

This plugin currently comes default with the following tools:

- [Shodan](https://www.shodan.io) - IP Lookup
- [GreyNoise](https://viz.greynoise.io) - IP Lookup
- [Spur](https://spur.us) - IP Lookup
- [VirusTotal](https://www.virustotal.com) - IP, Domain & Hash Lookup
- [Censys](https://search.censys.io) - IP & Domain Lookup
- [CriminalIP](https://www.criminalip.io) - IP & Domain Lookup
- [Whois](https://www.whois.com/whois) - Whois Lookup
- [EasyCounter](https://whois.easycounter.com) - Whois Lookup
- [Whoisology](https://whoisology.com) - Whois Lookup

## Configuration

> **NOTE: Prior to version 1.3.0 `tools.conf` was the default configuration file used.**
>
> The plugin will now automatically convert the `tools.conf` list to `tools.json` if it does not already exist in JSON form and will then default to using that instead.\
> The legacy config file will remain however will not be used and will not be included in future builds starting from **v1.3.0**

By default, the plugin will use the precofigured tools listed above. You can modify these settings by editing the `tools.json` file in the plugin folder.\
The format for the configuration file follows the below standard:

```json
{
    "Name": "VirusTotal",
    "URL": "https://www.virustotal.com/gui/search/{0}",
    "Categories": [ "ip", "domain", "hash"],
    "Enabled": true
}
```

In the URL, `{0}` will be replace with the search input. As such, only sites that work based on URL data *(GET Requests)* are supported for now.\
For example, `https://www.virustotal.com/gui/search/{0}` would become `https://www.virustotal.com/gui/search/1.1.1.1`

<!-- ```ini
[TOOL]                                  ; Section identifier for a new tool
NAME=Shodan                             ; Name of the tool
URL="https://www.shodan.io/host/{0}"    ; URL of the tool. {0} will be replaced with the user input from PowerToys Run
CATEGORIES=ip                           ; Comma-seperated list of Categories the tool can work with
ENABLED=1                               ; Boolean value of 0 or 1 to toggle the active state of the tool
``` -->

## License

This plugin is licensed under the MIT License. Feel free to use, modify, and distribute the code as you see fit. If you make any improvements or bugfixes, please consider contributing back to the project by opening a pull request.
