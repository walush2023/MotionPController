# Motion PController

![image](https://github.com/walush2023/MotionPController/blob/master/icon/logo.jpg)

Turn your phone into a wireless motion-sensing controller for your PC. Pair the `Motion PController` mobile app with the desktop companion to drive games, navigate Windows, and act as a touchpad, air mouse, media remote, numeric keypad, or full Xbox 360 gamepad — all over your local Wi-Fi network.

## Features

- **Virtual Xbox 360 gamepad** — emulated through [ViGEmBus](https://github.com/nefarius/ViGEmBus) using the [ViGEmClient](https://github.com/ViGEm/ViGEmClient) SDK, including rumble (large/small motor) feedback back to the phone.
- **Mouse control** — absolute and relative cursor movement, left/right click, scroll wheel (vertical and horizontal).
- **Keyboard control** — any virtual key, plus built-in shortcuts for zoom, Action Center, Task View, virtual desktops, show desktop, and back navigation.
- **Media / launcher shortcuts** — one-tap launch for YouTube, Netflix, and Disney+.
- **Text input** — send UTF-8 text from your phone directly to the focused window.
- **Auto-discovery via QR code** — the desktop app continuously regenerates a QR code containing all of your PC's local IP addresses; the phone scans it to pair instantly.
- **Multi-connection support** — up to 4 simultaneous controllers, each backed by its own virtual gamepad.
- **System tray integration** — minimize to tray, auto-launch at Windows startup, single-instance enforcement.

## Get started

### Installation

1. Install the **ViGEmBus driver** — required for the virtual Xbox 360 controller. Download the latest installer from [ViGEmBus releases](https://github.com/nefarius/ViGEmBus/releases/latest). The desktop app will prompt you to install it on first launch if it is missing.
2. Download the pre-built, production-signed Windows 10/11 binaries from the [latest release](https://github.com/walush2023/MotionPController/releases/latest).
3. Install the **Motion PController** mobile app on your phone by scanning the QR code below:

[<img src="https://github.com/walush2023/MotionPController/blob/master/icon/qr.png" width="180">](https://github.com/walush2023/MotionPController/releases/latest)

### Mobile usage

1. Make sure your phone and PC are connected to the same Wi-Fi network.
2. Launch `Motion PController` on your PC — a QR code containing your PC's local IPs will appear.
3. In the mobile app, tap **scan** and point your camera at the QR code.
4. Once connected, tap **connect to "<PC name>"** to reconnect quickly next time.

> ⚠️ Please ensure that you use a safety strap or rope when performing any motion or movement that involves a risk of dropping your phone.

## Use cases

- Touchpad gestures
- Air mouse
- Media controller
- Numeric keypad
- Xbox 360 gamepad
- Motion-sensing games
    - Motorcycle / racing
    - Flight Simulator (yoke & stick)

## Network

| Protocol | Port  | Purpose                                                          |
| -------- | ----- | ---------------------------------------------------------------- |
| TCP      | 7063  | Pairing handshake, machine-name exchange, rumble feedback to phone |
| UDP      | 7064  | Real-time input events (mouse, keyboard, gamepad, system)         |

Make sure these ports are allowed through your Windows Firewall for private networks.

## Build from source

Requirements:

- Visual Studio 2019 or later with the **.NET desktop development** workload
- .NET Framework 4.7.2

```
git clone https://github.com/walush2023/MotionPController.git
cd MotionPController
# open MotionPController.sln in Visual Studio and build, or:
msbuild MotionPController.sln /p:Configuration=Release
```

Key dependencies (restored via NuGet):

- [Nefarius.ViGEm.Client](https://www.nuget.org/packages/Nefarius.ViGEm.Client/) — Xbox 360 controller emulation
- [QRCoder](https://www.nuget.org/packages/QRCoder/) — QR code generation
- [Costura.Fody](https://www.nuget.org/packages/Costura.Fody/) — single-file assembly packaging

## Support development

If this project is useful to you, consider upgrading to **Motion PController Pro** in the mobile app to enjoy an ad-free experience and help fund continued maintenance and updates.

## License

Copyright (c) Walush. All rights reserved.

Licensed under the [GPL](LICENSE.txt) license.
