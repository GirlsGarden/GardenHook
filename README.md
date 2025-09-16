# GardenHook

## [中文](README_TC.md)

MUV-LUV Girls Garden mod for DMM Game Player version

## Introduce

Welcome to MUV-LUV Jail's Garden, prisoners.

Did you go to jail today?

Due to KM$ does not allow us to use the skip feature in the maze, so I found a way to unlock it.

KM\$, fuck you.gif

## Feature

1. Make skip button always available
2. Remove the dynamic mosaic
3. In-game Screenshot
4. Auto skip battle

## Requirement

1. Windows 10 or newer
2. MUV-LUV Girls Garden DMM Game Player version

## Installation

Download and extract [Release](https://github.com/GirlsGarden/GardenHook/releases) zip to your MUV-LUV Girls Garden
install
location `C:\Users\<username>\muv_luv_girlsgardenx_cl`

## Config

You can edit config.json(`./BepInEx/plugins/config.json`) if you don't like default settings.

| Name      | Default Value | Description                                        |
|-----------|---------------|----------------------------------------------------|
| speed     | 0.5           | Increase/Decrease game speed each step (per click) | 
| auto_skip | false         | Auto skip battle                                   |

## Key binding

| Key | Type       | Description                                                                   |
|-----|------------|-------------------------------------------------------------------------------|
| F1  | Reload     | Reload GardenHook config                                                      |
| F4  | Skip       | Auto skip battle                                                              |
| F5  | Freeze     | Freeze game, mean set game speed to 0x                                        |
| F6  | Reset      | Reset game speed to 1x/normal                                                 | 
| F7  | Decrease   | Decrease game speed (2-0.5 etc), depends on your `speed` config               | 
| F8  | Increase   | Increase game speed (1+0.5 etc), depends on your `speed` config               |
| F12 | Screenshot | Screenshot current frame and save to Pictures(`C:\Users\<username>\Pictures`) |

## Contributing

You're free to contribute to GardenHook as long as the features are useful, such as battle stats log, or something else,
except modifying battle data.

## Disclaimer

Using GardenHook violates MUV-LUV Girls Garden and DMM's terms of service.

I will NOT be held responsible for any bans!