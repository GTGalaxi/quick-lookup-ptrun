# Quick Lookup Plugin for PowerToys Run 

![build-status](https://github.com/GTGalaxi/quick-lookup-ptrun/actions/workflows/dotnet.yml/badge.svg) <a href="https://github.com/GTGalaxi/quick-lookup-ptrun/releases">![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/GTGalaxi/quick-lookup-ptrun?include_prereleases)</a>

This plugin for [PowerToys Run](https://learn.microsoft.com/en-us/windows/powertoys/run) allows you to quickly search for an IP address, domain name, hash or any other data points in a list of Cyber Security tools. It's perfect for security analysts, penetration testers, or anyone else who needs to quickly lookup information when investigating artifacts or alerts.

## Installation

To install the plugin, download the latest release from the [releases page](https://github.com/GTGalaxi/quick-lookup-ptrun/releases) and extract the contents of the ZIP file to a folder called QuickLookup in the PowerToys Run Plugins foler (e.g. `C:\Program Files\PowerToys\modules\launcher\Plugins`). Then, open/restart PowerToys and you should see the QuickLookup plugin loaded under the Run tool.

![image](https://user-images.githubusercontent.com/10473238/220018777-8bed80bd-dcfa-4ddf-adeb-17d6b9dc93f4.png)

## Usage

To use the plugin, simply open PowerToys Run by pressing Alt+Space and type the activation command `ql` followed by the tool category and the data you want to lookup.

![ptr-v1 2 0](https://user-images.githubusercontent.com/10473238/231605857-2427613f-7206-4899-8d82-3c660cecfd98.gif)

The plugin will open the data searched in a new tab in your default browser for each tool registered with that category.

![QuickLookup](https://user-images.githubusercontent.com/10473238/227844315-0a865672-9eb3-4f35-afc5-d6c196fd009d.gif)

## Default Tools

This plugin currently comes default with the following tools:

* Shodan - IP Lookup
* GreyNoise - IP Lookup
* VirusTotal - IP, Domain & Hash Lookup
* Censys - IP & Domain Lookup
* CriminalIP - IP & Domain Lookup
* Whois - Whois Lookup
* EasyCounter - Whois Lookup
* Whoisology - Whois Lookup

## Configuration

By default, the plugin will use the precofigured tools listed above. You can modify these settings by editing the `tools.conf` file in the plugin folder.
The format for the configuration file follows the below standard:

**NOTE: The order of properties per tool is important as the parser is just simple regex! This will be implemented properly in a later version.**

```ini
[TOOL]                                  ; Section identifier for a new tool
NAME=Shodan                             ; Name of the tool
URL="https://www.shodan.io/host/{0}"    ; URL of the tool. {0} will be replaced with the user input from PowerToys Run
CATEGORIES=ip                           ; Comma-seperated list of Categories the tool can work with
ENABLED=1                               ; Boolean value of 0 or 1 to toggle the active state of the tool
```

## License

This plugin is licensed under the MIT License. Feel free to use, modify, and distribute the code as you see fit. If you make any improvements or bugfixes, please consider contributing back to the project by opening a pull request.
