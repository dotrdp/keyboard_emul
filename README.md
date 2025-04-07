# VirtualKeyboard

Stardew Valley mod that add simple virtual keyboard

## Install

### PC User

1. Install the latest version of [SMAPI](https://smapi.io/). 
2. Download this mod and unzip it into Stardew Valley/Mods.
3. Run the game using SMAPI.


### Android User

1. Install the latest version of [SMAPILoader](https://github.com/NRTnarathip/SMAPILoader).
2. Download this mod.
3. Unzip it into Android\data\abc.smapi.gameloader\files\Mods or Use SMAPILauncher -> Mod Manager -> install Mod and select mod zip
4. SMAPILauncher -> Start Game

## How to use

**[warn] If you're upgrading from an older version, please delete the old config.json file first.**

a simple virtual keyboard that support multi keys will show on screen.

press toggle button on screen will show keyboard, and press again will disappear

config.json file is automatically generated after first run with mod.

default config.json may like this:
```json
{
  "vToggle": {
    "key": "None",
    "rectangle": {
      "X": 96,
      "Y": 12,
      "Width": 64,
      "Height": 64
    }
  },
  "ButtonScale": 1.0,
  "Buttons": [
    [
      {
        "key": "P",
        "alias": ""
      },
      {
        "key": "I",
        "alias": ""
      },
      {
        "key": "O",
        "alias": ""
      },
      {
        "key": "Q",
        "alias": ""
      }
    ]
  ]
}
```

- if you want change button, you can change "Buttons"-"key", all support key you can find here [Enum Keys](https://docs.monogame.net/api/Microsoft.Xna.Framework.Input.Keys.html)
```json
﻿  "Buttons": [
    [
      {
        "key": "M",
        "alias": ""
      },
      {
        "key": "LeftShift",
        "alias": ""
      },
      {
        "key": "NumPad2",
        "alias": ""
      },
      {
        "key": "F10",
        "alias": ""
      }
    ]
  ]
```

- if you want reduce button, just delete struct
```json
  "Buttons": [
    [
      {
        "key": "P",
        "alias": ""
      }
    ]
  ]
```

- if you want add button, just add struct in array
```json
﻿  "Buttons": [
    [
      {
        "key": "P",
        "alias": ""
      },
      {
        "key": "I",
        "alias": ""
      },
      {
        "key": "O",
        "alias": ""
      },
      {
        "key": "Q",
        "alias": ""
      },
      {
        "key": "W",
        "alias": ""
      },
      {
        "key": "E",
        "alias": ""
      }
    ]
  ]
```

- if you want add new line button, just add new array in "Buttons"
```json
﻿  "Buttons": [
    [
      {
        "key": "P",
        "alias": ""
      },
      {
        "key": "I",
        "alias": ""
      },
      {
        "key": "O",
        "alias": ""
      },
      {
        "key": "Q",
        "alias": ""
      }
    ]
  ]
```

- if ﻿you want change toggle button positoin, edit "vToggle"-"rectangle". default positoin X(96) is suit for my android, you should change this for your device. X(36) is good for windows.
```json
   "vToggle": {
    "key": "None",
    "rectangle": {
      "X": 96,
      "Y": 12,
      "Width": 64,
      "Height": 64
    }
  }
```

- if you want scale button size, edit "ButtonScale". default value is 1.0
```json
  "ButtonScale": 1.0
```

## Compatibility
- Work with [SMAPI](https://smapi.io/) 4.1.10 with Stardew Valley 1.6.15 on Windows, other version not test.
- Work with [SMAPILoader](https://github.com/NRTnarathip/SMAPILoader) ﻿1.1.4 with Stardew Valley 1.6.15 on Android
