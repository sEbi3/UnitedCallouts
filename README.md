<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/sEbi3/UnitedCallouts">
    <img src="Logos/UnitedCallouts.png" alt="Logo" width="128" height="128">
  </a>
<h3 align="center">UnitedCallouts (Build 1.5.8.1)</h3>
  <p align="center">UnitedCallouts is a plugin for <a href="https://www.lcpdfr.com/lspdfr/index/"><strong>LSPDFR</strong></a> which has over <strong>1.4 million registered downloads</strong> and adds over 20 new varied and realistic callouts with different locations, endings and possibilities.
    <br/>
    <h3 align="center">
  <b><a href="https://www.lcpdfr.com/downloads/gta5mods/scripts/20730-unitedcallouts-robbery-drugs-burglary-more/">Download</a></b> ・
  <a href="https://github.com/sEbi3/UnitedCallouts/wiki">Wiki</a> ・
  <a href="https://github.com/sEbi3/UnitedCallouts/releases">Changelogs</a>
</h3>
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
  * [Want to stay up to date?](#want-to-stay-up-to-date)
* [Getting Started](#getting-started)
  * [Requirements](#requirements)
  * [Installation](#installation)
* [Usage](#usage)
* [Contributing](#contributing)

## This plugin was built with
* <a href="https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48">.NET Framework 4.8</a>
* <a href="https://www.nuget.org/packages/RagePluginHook#readme-body-tab">RAGE Plugin Hook 1.95</a>
* <a href="https://www.lcpdfr.com/downloads/gta5mods/g17media/7792-lspd-first-response/">LSPD: First Response 0.4.9 (Build 8242)</a>
* Other API References used:
  * <a href="https://learn.microsoft.com/en-us/dotnet/api/system.drawing?view=netframework-4.8">System.Drawing</a><br>
  * <a href="https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms?view=netframework-4.8">System.Windows.Forms</a><br><br>

<!-- FEATURES OF UNITEDCALLOUTS -->
## Features of UnitedCallouts

### Variation 
* This plugin adds several and different dialogues for each callout to have more variation.<br>
* Every callout has different endings and possibilities for more variation and realism.<br>
* In this pack are currently <code>25 varied and realistic callouts</code> for LSPDFR. (Have a look below for a full list of all available callouts.)<br>
* A lot of callouts have different vehicles and weapons to have more variation.<br>
* All callouts have several locations in order to have more variation and realism.<br>
  * Some callouts use specific locations in order to have more realistic locations.<br>
    An apartment burglary won't be on a street and a fight between two people won't be at a police station.<br>
  * All callouts have a location choosing system in order to get the locations near the Player.<br><br>
  
### Customization
* This plugin adds a hotkey for dialogs and for forcing your current callout to end.<br>
  * "Y" for speaking to a suspect. (You'll get a notification when you must talk to a suspect)<br>
  * "END" to force your current callout to end. (In some cases, you may want to end the callout by yourself)<br>
  * You can change both hotkeys in the <code>UnitedCallouts.ini.</code><br>

* Option to enable/disable the callouts you want in the <code>UnitedCallouts.ini.</code><br>
  * You may want to use this option to avoid some callouts you don't want to play.<br>

* Option to enable/disable AI backup responding to a callout with the Player in the <code>UnitedCallouts.ini.</code>.<br>
  * This option allows realistic AI units to respond to certain callouts with the Player. This means a local unit won't respond to a gang fight.
    If the Player is in a pursuit, you'll get help by local units and an air unit instead of a SWAT team.<br>
  * If you don't have a powerful PC, set this option in the <code>UnitedCallouts.ini</code> to <code>false</code>.<br><br>
  
### Want to stay up to date?
* UnitedCallouts has a version control system to let you know when UnitedCallouts or LSPDFR has been updated.<br>
  * You can still play with UnitedCallouts on your own risk. It's always recommended to update to the latest build that is available.<br>
  * UnitedCallouts may have issues when LSPDFR gets an update that change parts of it's API.<br><br>


<!-- GETTING STARTED -->
## Getting Started

### Requirements
* LSPD First Response 0.4.9 <i>(or higher)</i> <a href="https://www.lcpdfr.com/files/file/7792-lspd-first-response">Download here</a>
* RAGE Plugin Hook 1.95 <i>(or higher)</i> <a href="https://ragepluginhook.net/Downloads.aspx">Download here</a>
* Enable All Interiors <i>(required for certain callouts)</i> <a href="https://www.gta5-mods.com/scripts/enable-all-interiors-wip">Download here</a>
* A legal copy of GTA5 <i>(Retail, EpicGames or Steam)</i><br>

### Installation

* Copy the files of <code>"plugins\LSPDFR"</code> in: <code>"GTA V directory\plugins\LSPDFR"</code>.<br>
* Copy the folder of <code>"lspdfr\audio\scanner"</code> in: <code>"GTA V directory\lspdfr\audio\scanner"</code>.<br>
* Alternative: Drag and drop the folder <code>"lspdfr"</code> and <code>"plugins"</code> into your main directory of GTA5.<br><br>

## Usage

Once you're in game open the RPH console with F4 <i>(by default)</i> and load LSPDFR with <code>"LoadPlugin LSPD First Response.dll"</code>. 
You can also load LSPDFR on start up <i>(Take a look at the RPH settings for that)</i>.

Now go on duty and UnitedCallouts will automatically gets loaded from LSPDFR. 
You'll now get callouts with the time. You can also force callouts with the RPH console by using the command <code>"StartCallout [CALLOUT-NAME]</code>.<br><br>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**. 

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request<br><br>
