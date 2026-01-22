<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/sEbi3/UnitedCallouts">
    <img src="Logos/UnitedCalloutsLogo.png" alt="Logo" width="128" height="128">
  </a>
<h3 align="center">UnitedCallouts (Build 1.5.8.1)</h3>
  <p align="center">UnitedCallouts is a plugin for <a href="https://www.lcpdfr.com/lspdfr/index/"><strong>LSPDFR</strong></a>, a popular Grand Theft Auto V modification, with more than <strong>1.5 million downloads</strong> and adds over 20 new varied and realistic callouts with different scenarios, locations, and possible outcomes to expand the base LSPDFR experience.
    <br/>
    <h4 align="center">
  <b><a href="https://www.lcpdfr.com/downloads/gta5mods/scripts/20730-unitedcallouts-robbery-drugs-burglary-more/">Download</a></b> ・
  <a href="https://github.com/sEbi3/UnitedCallouts/wiki">Wiki</a> ・
  <a href="https://github.com/sEbi3/UnitedCallouts/releases">Changelogs</a>
</h4>
  <div align="center">
  <img src="https://img.shields.io/github/downloads/sebi3/UnitedCallouts/total"/>
  <img src="https://img.shields.io/github/release/sebi3/UnitedCallouts"/>
  <img src="https://img.shields.io/github/repo-size/sEbi3/UnitedCallouts"/>
  <img src="https://img.shields.io/github/stars/sEbi3/UnitedCallouts"/>
</div>
        <br/>
  </p>
</p>

<!-- TABLE OF CONTENTS -->
## Table of Contents
* [UnitedCallouts Wiki for Help](https://github.com/sEbi3/UnitedCallouts/wiki)
* [Features of UnitedCallouts](#features-of-unitedcallouts)
  * [Variation](#variation)
  * [Customization](#customization)
  * [Stay up to Date](#stay-up-to-Date)
* [Getting Started](#getting-started)
  * [Requirements](#requirements)
  * [Installation](#installation)
* [Usage](#usage)
* [Contributing](#contributing)

## Dependencies
* <a href="https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48">.NET Framework 4.8</a>
* <a href="https://www.nuget.org/packages/RagePluginHook#readme-body-tab">RAGE Plugin Hook 1.95</a>
* <a href="https://www.lcpdfr.com/downloads/gta5mods/g17media/7792-lspd-first-response/">LSPD: First Response 0.4.9 (Build 8242)</a>
* Other API References used:
  * <a href="https://learn.microsoft.com/en-us/dotnet/api/system.drawing?view=netframework-4.8">System.Drawing</a><br>
  * <a href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms?view=netframework-4.8">System.Windows.Forms</a><br><br>

<!-- FEATURES OF UNITEDCALLOUTS -->
## Features of UnitedCallouts

### Variation 
* This plugin introduces a wide range of unique and dynamic callouts with multiple dialogue options to enable meaningful interaction with suspects and involved parties.<br>
* Every callout is designed with multiple possible outcomes and endings, significantly increasing replayability and overall realism.<br>
* The current release includes 25 varied and realistic callouts for the LSPDFR modification, with a strong focus on authenticity and scenario diversity.<br>
* Many callouts feature scenario-specific vehicles and weapons, ensuring that each situation feels distinct and context-appropriate within gameplay.<br>
* All callouts support multiple potential locations to prevent repetition and enhance immersion.<br>
  * Certain scenarios are restricted to logical, real-world locations to maintain realism.<br>
    For example, an apartment burglary will only occur at residential buildings, while altercations between individuals will not take place at police stations.<br>
  * A built-in location selection system dynamically chooses callout locations near the player, ensuring seamless integration into patrol gameplay.<br><br>
  
### Customization
* This plugin includes configurable hotkeys for dialogue interaction and for manually ending the active callout:<br>
  * "<b>Y</b>" Initiates dialogue with a suspect or involved person. <i>(A notification will appear when dialogue interaction is available.)</i><br>
  * "<b>END</b>" Forces the current callout to conclude. <i>(Useful in situations where manual intervention is required.)</i><br>
  * All hotkeys can be fully customized through the configuration file to match individual preferences.<br>
* Players can enable or disable specific callouts via configuration settings.<br>
  * This allows for a tailored gameplay experience and the exclusion of scenarios the player prefers not to encounter.<br>
* Option to enable/disable AI backup responding to a callout with the player.<br>
  * This system ensures realistic unit deployment based on the situation. 
    For example, local patrol units will not respond to large-scale gang conflicts, while pursuits may trigger local support units and air assistance instead of specialized teams.<br>
  * For performance optimization, this feature can be disabled on lower-end systems by setting the option to false in the configuration file.<br><br>

### Stay up to Date
* UnitedCallouts includes an integrated version control system that notifies players when a new update is available.<br>
  * While older versions may remain playable, using outdated builds is done at the player’s own risk.
    Updating to the latest available version is always strongly recommended for stability and compatibility.<br>
  * UnitedCallouts may have issues when LSPDFR gets an update that changes parts of its API.
    Compatibility updates will be addressed as required to ensure continued functionality.<br><br>

<!-- GETTING STARTED -->
## Getting Started

### Requirements
* LSPD First Response 0.4.9 <i>(or higher)</i> <a href="https://www.lcpdfr.com/files/file/7792-lspd-first-response">Download here</a>
* RAGE Plugin Hook 1.95 <i>(or higher)</i> <a href="https://ragepluginhook.net/Downloads.aspx">Download here</a>
* Enable All Interiors <i>(optional)</i> <a href="https://www.gta5-mods.com/scripts/enable-all-interiors-wip">Download here</a><br>
> [!WARNING]
> Callouts that require interior access are disabled by default.<br>
> To enable these callouts, <i>Enable All Interiors</i> must be installed and the corresponding options must be enabled in the configuration file.
> 
> Note: Alternatively, any other third-party plugin that allows the player to enter interiors can also be used.

* A legal copy of Grand Theft Auto V<br> 
  <i>(Supported platforms include Retail, Epic Games, and Steam versions.)</i><br>

#

### Installation
Follow one of the methods below to install UnitedCallouts:

<b> Manual Installation</b>

* Copy the contents of the <code>"plugins\LSPDFR"</code> folder into: <code>"GTA V directory\plugins\LSPDFR"</code>.<br>
* Copy the contents of the <code>"lspdfr\audio\scanner"</code> folder into: <code>"GTA V directory\lspdfr\audio\scanner"</code>.<br>

<b> Alternative Installation</b>
* Drag and drop the <code>"lspdfr"</code> and <code>"plugins"</code> folders into your main directory of GTAV.<br>

# 

### Usage

* Launch the game and open the <b>RAGE Plugin Hook console</b> by pressing <b>F4</b> (default key).
* Load LSPDFR manually using: <code>LoadPlugin LSPD First Response.dll</code><br>
<i>(LSPDFR can also be configured to load automatically on startup via RAGE Plugin Hook settings.)</i>
* Once on duty, <b>UnitedCallouts will be loaded automatically</b> by LSPDFR.
* Callouts from this plugin will now appear during gameplay.
* You may also manually start a specific callout via the console using: <code>StartCallout [CALLOUT-NAME]</code><br>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open-source community an inspiring place to learn, collaborate, and grow.
Any form of contribution — whether code improvements, bug reports, or feature suggestions — is highly appreciated.
