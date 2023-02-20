# PowerToys Run Plugin for Quick IP Lookups
This plugin for [PowerToys Run](https://learn.microsoft.com/en-us/windows/powertoys/run) allows you to quickly search for an IP address in a list of Cyber Security tools. It's perfect for security analysts, penetration testers, or anyone else who needs to quickly lookup information on an IP address.

## Installation
To install the plugin, download the latest release from the [releases page](https://github.com/GTGalaxi/quick-lookup-ptrun/releases) and extract the contents of the ZIP file to a folder called QuickLookup in the PowerToys Run Plugins foler (e.g. `C:\Program Files\PowerToys\modules\launcher\Plugins`). Then, open/restart PowerToys and you should see the QuickLookup plugin loaded under the Run tool.

![image](https://user-images.githubusercontent.com/10473238/220018777-8bed80bd-dcfa-4ddf-adeb-17d6b9dc93f4.png)


## Usage
To use the plugin, simply open PowerToys Run by pressing Alt+Space and type the activation command `ql` followed by the IP address you want to lookup. The plugin will open the IP address in a new tab for each tool registered.

![image](https://user-images.githubusercontent.com/10473238/220019548-0c0cb81e-c401-4af7-95f6-7bec9220889f.png)


## Default Tools
This plugin currently comes default with the following tools:

* Shodan
* GreyNoise
* VirusTotal
* Censys
* CriminalIP

## Configuration
By default, the plugin will use the precofigured tools listed above. You can modify these settings by editing the `tools.conf` file in the plugin folder.
The format for the configuration file follows the below standard

```ini
[TOOL]                                  ; Section identifier for a new tool
NAME=Shodan                             ; Name of the tool
URL=https://www.shodan.io/host/{0}      ; URL of the tool. {0} will be replaced with the user input from PowerToys Run
ENABLED=1                               ; Boolean value of 0 or 1 to toggle the active state of the tool
```

## License
This plugin is licensed under the MIT License. Feel free to use, modify, and distribute the code as you see fit. If you make any improvements or bugfixes, please consider contributing back to the project by opening a pull request.
