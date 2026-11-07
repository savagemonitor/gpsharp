# Open GoPro status types

This table records the protocol types used by GPSharp when decoding status values. Open GoPro's
current [status documentation](https://gopro.github.io/OpenGoPro/docs/ble/statuses) is authoritative
for canonical status names and descriptions. The type mapping is cross-checked against the generated
Open GoPro Python [`BleStatuses`](https://github.com/gopro/OpenGoPro/blob/0f963572611c4410a15678531e9681a6ff874edb/demos/python/sdk_wireless_camera_control/open_gopro/api/ble_statuses.py)
implementation at commit
[`0f963572611c4410a15678531e9681a6ff874edb`](https://github.com/gopro/OpenGoPro/commit/0f963572611c4410a15678531e9681a6ff874edb),
which specifies both the Python value type and the Construct wire parser. The matching generated
[Kotlin status ID enum](https://github.com/gopro/OpenGoPro/blob/0f963572611c4410a15678531e9681a6ff874edb/demos/kotlin/kmp_sdk/wsdk/src/commonMain/kotlin/com/gopro/open_gopro/entity/queries/Statuses.kt)
supplies the SDK identifier cross-reference.

The CLR column names the metadata decoder type, not a change to the camera's JSON representation.
`IStatus.GetValueAsync<T>()` continues to deserialize the JSON value requested by the caller, and
`CameraState.Statuses` always preserves the original `JsonElement`. Enums resolve known integer
options to `CameraProtocolOption` (numeric code plus canonical name); an unknown enum option is
not guessed and remains available only as its raw JSON value. Array and object JSON values can be
requested as `T[]`, dictionaries, models, or `JsonElement`, including for statuses whose protocol
type is unknown.

| ID | Open GoPro status | Documented type / wire parser | GPSharp CLR type |
|---:|---|---|---|
| 1 | Battery Present | `bool` / `Flag` | `bool` |
| 2 | Internal Battery Bars | enum | `CameraProtocolOption` |
| 6 | Overheating | `bool` / `Flag` | `bool` |
| 8 | Busy | `bool` / `Flag` | `bool` |
| 9 | Quick Capture | `bool` / `Flag` | `bool` |
| 10 | Encoding | `bool` / `Flag` | `bool` |
| 11 | LCD Lock | `bool` / `Flag` | `bool` |
| 13 | Video Encoding Duration | `int` / `Int32ub` | `uint` |
| 17 | Wireless Connections Enabled | `bool` / `Flag` | `bool` |
| 19 | Pairing State | enum | `CameraProtocolOption` |
| 20 | Last Pairing Type | enum | `CameraProtocolOption` |
| 21 | Last Pairing Success | `int` / `Int32ub` | `uint` |
| 22 | Wifi Scan State | enum | `CameraProtocolOption` |
| 23 | Last Wifi Scan Success | `int` / `Int8ub` | `byte` |
| 24 | Wifi Provisioning State | enum | `CameraProtocolOption` |
| 26 | Remote Version | `int` / `Int8ub` | `byte` |
| 27 | Remote Connected | `bool` / `Flag` | `bool` |
| 28 | Pairing State (Legacy) | undocumented type | Unknown; use raw JSON |
| 29 | Connected WiFi SSID | `str` / UTF-8 | `string` |
| 30 | Access Point SSID | `str` / UTF-8 | `string` |
| 31 | Connected Devices | `int` / `Int8ub` | `byte` |
| 32 | Preview Stream | `bool` / `Flag` | `bool` |
| 33 | Primary Storage | enum | `CameraProtocolOption` |
| 34 | Remaining Photos | `int` / `Int32ub` | `uint` |
| 35 | Remaining Video Time | `int` / `Int32ub` | `uint` |
| 38 | Photos | `int` / `Int32ub` | `uint` |
| 39 | Videos | `int` / `Int32ub` | `uint` |
| 41 | OTA | enum | `CameraProtocolOption` |
| 42 | Pending FW Update Cancel | `bool` / `Flag` | `bool` |
| 45 | Locate | `bool` / `Flag` | `bool` |
| 49 | Timelapse Interval Countdown | `int` / `Int32ub` | `uint` |
| 54 | SD Card Remaining | `int` / `Int64ub` | `ulong` |
| 55 | Preview Stream Available | `bool` / `Flag` | `bool` |
| 56 | Wifi Bars | `int` / `Int8ub` | `byte` |
| 58 | Active Hilights | `int` / `Int8ub` | `byte` |
| 59 | Time Since Last Hilight | `int` / `Int32ub` | `uint` |
| 60 | Minimum Status Poll Period | `int` / `Int32ub` | `uint` |
| 65 | Liveview Exposure Select Mode | enum | `CameraProtocolOption` |
| 66 | Liveview Y | `int` / `Int8ub` | `byte` |
| 67 | Liveview X | `int` / `Int8ub` | `byte` |
| 68 | GPS Lock | `bool` / `Flag` | `bool` |
| 69 | AP Mode | `bool` / `Flag` | `bool` |
| 70 | Internal Battery Percentage | `int` / `Int8ub` | `byte` |
| 74 | Microphone Accessory | enum | `CameraProtocolOption` |
| 75 | Zoom Level | `int` / `Int8ub` | `byte` |
| 76 | Wireless Band | enum | `CameraProtocolOption` |
| 77 | Zoom Available | `bool` / `Flag` | `bool` |
| 78 | Mobile Friendly | `bool` / `Flag` | `bool` |
| 79 | FTU | `bool` / `Flag` | `bool` |
| 81 | 5GHZ Available | `bool` / `Flag` | `bool` |
| 82 | Ready | `bool` / `Flag` | `bool` |
| 83 | OTA Charged | `bool` / `Flag` | `bool` |
| 85 | Cold | `bool` / `Flag` | `bool` |
| 86 | Rotation | enum | `CameraProtocolOption` |
| 88 | Zoom while Encoding | `bool` / `Flag` | `bool` |
| 89 | Flatmode | `int` / `Int8ub` | `byte` |
| 93 | Video Preset | `int` / `Int32ub` | `uint` |
| 94 | Photo Preset | `int` / `Int32ub` | `uint` |
| 95 | Timelapse Preset | `int` / `Int32ub` | `uint` |
| 96 | Preset Group | `int` / `Int32ub` | `uint` |
| 97 | Preset | `int` / `Int32ub` | `uint` |
| 98 | Preset Modified | `int` / `Int32ub` | `uint` |
| 99 | Remaining Live Bursts | `int` / `Int32ub` | `uint` |
| 100 | Live Bursts | `int` / `Int32ub` | `uint` |
| 101 | Capture Delay Active | `bool` / `Flag` | `bool` |
| 102 | Media Mod State | enum | `CameraProtocolOption` |
| 103 | Time Warp Speed | enum | `CameraProtocolOption` |
| 104 | Linux Core | `bool` / `Flag` | `bool` |
| 105 | Lens Type | enum | `CameraProtocolOption` |
| 106 | Hindsight | `bool` / `Flag` | `bool` |
| 107 | Scheduled Capture Preset ID | `int` / `Int32ub` | `uint` |
| 108 | Scheduled Capture | `bool` / `Flag` | `bool` |
| 110 | Display Mod Status | bitfield enum | `CameraProtocolOption` |
| 111 | SD Card Write Speed Error | `bool` / `Flag` | `bool` |
| 112 | SD Card Errors | `int` / `Int8ub` | `byte` |
| 113 | Turbo Transfer | `bool` / `Flag` | `bool` |
| 114 | Camera Control ID | enum | `CameraProtocolOption` |
| 115 | USB Connected | `bool` / `Flag` | `bool` |
| 116 | USB Controlled | enum | `CameraProtocolOption` |
| 117 | SD Card Capacity | `int` / `Int32ub` | `uint` |
| 118 | Photo Interval Capture Count | `int` / `Int32ub` | `uint` |
| 122 | Camera Name | `str` / UTF-8 | `string` |
| 123 | Active Preset Is Custom | type not published in the cited SDK parser | Unknown; use raw JSON |

Identifiers absent from the current public status ID tables and SDK sources remain uncatalogued:
`3, 4, 14, 61, 90, 91, 109, 119` from the sanitized camera snapshot. No name or type is inferred
for these IDs. This means no authoritative mapping was found in the cited sources; it does not
assert that a specific camera firmware does not implement them.
