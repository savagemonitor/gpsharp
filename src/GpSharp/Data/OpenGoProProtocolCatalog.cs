namespace GpSharp.Data;

using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Canonical setting and status names published by Open GoPro.
/// </summary>
/// <remarks>
/// Catalog entries are metadata only. CameraState continues to preserve raw protocol IDs and JSON
/// values, and an absent entry means the current public catalog has no authoritative mapping.
/// </remarks>
public static class OpenGoProProtocolCatalog
{
    private const string SettingsDocumentation = "https://gopro.github.io/OpenGoPro/docs/ble/settings/#setting-";
    private const string StatusesDocumentation = "https://gopro.github.io/OpenGoPro/docs/ble/statuses/#status-";
    private const string SettingsSdkSource =
        "https://github.com/gopro/OpenGoPro/blob/0f963572611c4410a15678531e9681a6ff874edb/demos/kotlin/kmp_sdk/wsdk/src/commonMain/kotlin/com/gopro/open_gopro/entity/queries/Settings.kt";
    private const string StatusesSdkSource =
        "https://github.com/gopro/OpenGoPro/blob/0f963572611c4410a15678531e9681a6ff874edb/demos/kotlin/kmp_sdk/wsdk/src/commonMain/kotlin/com/gopro/open_gopro/entity/queries/Statuses.kt";
    private const string StatusesTypeSource =
        "https://github.com/gopro/OpenGoPro/blob/0f963572611c4410a15678531e9681a6ff874edb/demos/python/sdk_wireless_camera_control/open_gopro/api/ble_statuses.py";

    private static readonly IReadOnlyDictionary<int, CameraProtocolValueMetadata> Settings =
        new ReadOnlyDictionary<int, CameraProtocolValueMetadata>(CreateSettings());

    private static readonly IReadOnlyDictionary<int, CameraProtocolValueMetadata> Statuses =
        new ReadOnlyDictionary<int, CameraProtocolValueMetadata>(CreateStatuses());

    /// <summary>
    /// Gets canonical metadata for a documented setting ID, or null when the ID is not in the
    /// published setting catalog.
    /// </summary>
    public static CameraProtocolValueMetadata? GetSetting(int settingId) =>
        Settings.TryGetValue(settingId, out var metadata) ? metadata : null;

    /// <summary>
    /// Gets canonical metadata for a documented status ID, or null when the ID is not in the
    /// published status catalog.
    /// </summary>
    public static CameraProtocolValueMetadata? GetStatus(int statusId) =>
        Statuses.TryGetValue(statusId, out var metadata) ? metadata : null;

    /// <summary>
    /// Gets the canonical display name for a documented setting, or null when it has no published mapping.
    /// </summary>
    public static string? GetSettingDisplayName(int settingId) =>
        GetSetting(settingId)?.Name;

    /// <summary>
    /// Gets the canonical display name for a documented status, or null when it has no published mapping.
    /// </summary>
    public static string? GetStatusDisplayName(int statusId) =>
        GetStatus(statusId)?.Name;

    private static Dictionary<int, CameraProtocolValueMetadata> CreateSettings()
    {
        var entries = new[]
        {
            Setting(2, "Video Resolution", "Integer enum option", null,
                (1, "NUM_4K"), (4, "NUM_2_7K"), (6, "NUM_2_7K_4_3"), (7, "NUM_1440"),
                (9, "NUM_1080"), (12, "NUM_720"), (18, "NUM_4K_4_3"), (21, "NUM_5_6K"),
                (24, "NUM_5K"), (25, "NUM_5K_4_3"), (26, "NUM_5_3K_8_7"), (27, "NUM_5_3K_4_3"),
                (28, "NUM_4K_8_7"), (31, "NUM_8K"), (35, "NUM_5_3K_21_9"), (36, "NUM_4K_21_9"),
                (37, "NUM_4K_1_1"), (38, "NUM_900"), (39, "NUM_4K_SPH"), (100, "NUM_5_3K"),
                (107, "NUM_5_3K_8_7_V2"), (108, "NUM_4K_8_7_V2"), (109, "NUM_4K_9_16_V2"),
                (110, "NUM_1080_9_16_V2"), (111, "NUM_2_7K_4_3_V2"), (112, "NUM_4K_4_3_V2"),
                (113, "NUM_5_3K_4_3_V2")),
            Setting(3, "Frames Per Second", "Integer enum option", null,
                (0, "NUM_240_0"), (1, "NUM_120_0"), (2, "NUM_100_0"), (3, "NUM_90_0"),
                (5, "NUM_60_0"), (6, "NUM_50_0"), (8, "NUM_30_0"), (9, "NUM_25_0"),
                (10, "NUM_24_0"), (13, "NUM_200_0"), (15, "NUM_400_0"), (16, "NUM_360_0"),
                (17, "NUM_300_0")),
            Setting(5, "Video Timelapse Rate", "Integer enum option",
                "How frequently a frame is captured during Video Timelapse.",
                (0, "NUM_0_5_SECONDS"), (1, "NUM_1_SECOND"), (2, "NUM_2_SECONDS"),
                (3, "NUM_5_SECONDS"), (4, "NUM_10_SECONDS"), (5, "NUM_30_SECONDS"),
                (6, "NUM_60_SECONDS"), (7, "NUM_2_MINUTES"), (8, "NUM_5_MINUTES"),
                (9, "NUM_30_MINUTES"), (10, "NUM_60_MINUTES"), (11, "NUM_3_SECONDS")),
            Setting(30, "Photo Timelapse Rate", "Integer enum option", null,
                (11, "NUM_3_SECONDS"), (100, "NUM_60_MINUTES"), (101, "NUM_30_MINUTES"),
                (102, "NUM_5_MINUTES"), (103, "NUM_2_MINUTES"), (104, "NUM_60_SECONDS"),
                (105, "NUM_30_SECONDS"), (106, "NUM_10_SECONDS"), (107, "NUM_5_SECONDS"),
                (108, "NUM_2_SECONDS"), (109, "NUM_1_SECOND"), (110, "NUM_0_5_SECONDS")),
            Setting(32, "Nightlapse Rate", "Integer enum option", null,
                (4, "NUM_4_SECONDS"), (5, "NUM_5_SECONDS"), (10, "NUM_10_SECONDS"),
                (15, "NUM_15_SECONDS"), (20, "NUM_20_SECONDS"), (30, "NUM_30_SECONDS"),
                (100, "NUM_60_SECONDS"), (120, "NUM_2_MINUTES"), (300, "NUM_5_MINUTES"),
                (1800, "NUM_30_MINUTES"), (3600, "NUM_60_MINUTES"), (3601, "AUTO")),
            Setting(43, "Webcam Digital Lenses", "Integer enum option", null,
                (0, "WIDE"), (2, "NARROW"), (3, "SUPERVIEW"), (4, "LINEAR")),
            Setting(59, "Auto Power Down", "Integer enum option", null,
                (0, "NEVER"), (1, "NUM_1_MIN"), (4, "NUM_5_MIN"), (6, "NUM_15_MIN"),
                (7, "NUM_30_MIN"), (11, "NUM_8_SECONDS"), (12, "NUM_30_SECONDS")),
            Setting(83, "GPS", "Integer enum option", null, (0, "OFF"), (1, "ON")),
            Setting(88, "LCD Brightness", "Integer percentage",
                "LCD brightness is a percentage from 10 through 100."),
            Setting(91, "LED", "Integer enum option", null,
                (0, "OFF"), (2, "ON"), (3, "ALL_ON"), (4, "ALL_OFF"), (5, "FRONT_OFF_ONLY"), (100, "BACK_ONLY")),
            Setting(108, "Video Aspect Ratio", "Integer enum option", null,
                (0, "NUM_4_3"), (1, "NUM_16_9"), (3, "NUM_8_7"), (4, "NUM_9_16"),
                (5, "NUM_21_9"), (6, "NUM_1_1")),
            Setting(121, "Video Lens", "Integer enum option", null,
                (0, "WIDE"), (2, "NARROW"), (3, "SUPERVIEW"), (4, "LINEAR"), (7, "MAX_SUPERVIEW"),
                (8, "LINEAR_HORIZON_LEVELING"), (9, "HYPERVIEW"), (10, "LINEAR_HORIZON_LOCK"),
                (11, "MAX_HYPERVIEW"), (12, "ULTRA_SUPERVIEW"), (13, "ULTRA_WIDE"),
                (14, "ULTRA_LINEAR"), (104, "ULTRA_HYPERVIEW")),
            Setting(122, "Photo Lens", "Integer enum option", null,
                (0, "WIDE_12_MP"), (10, "LINEAR_12_MP"), (15, "NUM_9MP_WIDE"), (19, "NARROW"),
                (27, "WIDE_23_MP"), (28, "LINEAR_23_MP"), (31, "WIDE_27_MP"), (32, "LINEAR_27_MP"),
                (37, "NUM_9MP_LINEAR"), (38, "NUM_13MP_LINEAR"), (39, "NUM_13MP_WIDE"),
                (40, "NUM_13MP_ULTRA_WIDE"), (41, "ULTRA_WIDE_12_MP"), (44, "NUM_13MP_ULTRA_LINEAR"),
                (100, "MAX_SUPERVIEW"), (101, "WIDE"), (102, "LINEAR")),
            Setting(123, "Time Lapse Digital Lenses", "Integer enum option", null,
                (19, "NARROW"), (31, "WIDE_27_MP"), (32, "LINEAR_27_MP"),
                (100, "MAX_SUPERVIEW"), (101, "WIDE"), (102, "LINEAR")),
            Setting(125, "Photo Output", "Integer enum option", null,
                (0, "STANDARD"), (1, "RAW"), (2, "HDR"), (3, "SUPERPHOTO")),
            Setting(128, "Media Format", "Integer enum option", null,
                (13, "TIME_LAPSE_VIDEO"), (20, "TIME_LAPSE_PHOTO"),
                (21, "NIGHT_LAPSE_PHOTO"), (26, "NIGHT_LAPSE_VIDEO")),
            Setting(134, "Anti-Flicker", "Integer enum option", null,
                (0, "NTSC"), (1, "PAL"), (2, "NUM_60HZ"), (3, "NUM_50HZ")),
            Setting(135, "Hypersmooth", "Integer enum option", null,
                (0, "OFF"), (1, "LOW"), (2, "HIGH"), (3, "BOOST"), (4, "AUTO_BOOST"), (100, "STANDARD")),
            Setting(150, "Video Horizon Leveling", "Integer enum option", null, (0, "OFF"), (2, "LOCKED")),
            Setting(151, "Photo Horizon Leveling", "Integer enum option", null, (0, "OFF"), (2, "LOCKED")),
            Setting(156, "Video Duration", "Integer enum option", null,
                (1, "NUM_15_SECONDS"), (2, "NUM_30_SECONDS"), (3, "NUM_1_MINUTE"),
                (4, "NUM_5_MINUTES"), (5, "NUM_15_MINUTES"), (6, "NUM_30_MINUTES"),
                (7, "NUM_1_HOUR"), (8, "NUM_2_HOURS"), (9, "NUM_3_HOURS"),
                (10, "NUM_5_SECONDS"), (100, "NO_LIMIT")),
            Setting(157, "Multi Shot Duration", "Integer enum option", null,
                (0, "OFF"), (1, "NUM_15_SECONDS"), (2, "NUM_30_SECONDS"), (3, "NUM_1_MINUTE"),
                (4, "NUM_5_MINUTES"), (5, "NUM_15_MINUTES"), (6, "NUM_30_MINUTES"),
                (7, "NUM_1_HOUR"), (8, "NUM_2_HOURS"), (9, "NUM_3_HOURS"), (100, "NO_LIMIT")),
            Setting(162, "Max Lens", "Integer enum option", null, (0, "OFF"), (1, "ON")),
            Setting(167, "HindSight", "Integer enum option", null,
                (2, "NUM_15_SECONDS"), (3, "NUM_30_SECONDS"), (4, "OFF")),
            Setting(168, "Scheduled Capture", "Integer bitfield",
                "A bitfield: bit 0 enables scheduled capture, bit 1 selects 24-hour format, bits 2-8 encode minutes, and bits 9-13 encode hours."),
            Setting(171, "Photo Single Interval", "Integer enum option", null,
                (0, "OFF"), (2, "NUM_0_5S"), (3, "NUM_1S"), (4, "NUM_2S"), (5, "NUM_5S"),
                (6, "NUM_10S"), (7, "NUM_30S"), (8, "NUM_60S"), (9, "NUM_120S"), (10, "NUM_3S")),
            Setting(172, "Photo Interval Duration", "Integer enum option", null,
                (0, "OFF"), (1, "NUM_15_SECONDS"), (2, "NUM_30_SECONDS"), (3, "NUM_1_MINUTE"),
                (4, "NUM_5_MINUTES"), (5, "NUM_15_MINUTES"), (6, "NUM_30_MINUTES"),
                (7, "NUM_1_HOUR"), (8, "NUM_2_HOURS"), (9, "NUM_3_HOURS")),
            Setting(173, "Video Performance Mode", "Integer enum option", null,
                (0, "MAXIMUM_VIDEO_PERFORMANCE"), (1, "EXTENDED_BATTERY"), (2, "TRIPOD_STATIONARY_VIDEO")),
            Setting(175, "Control Mode", "Integer enum option", null, (0, "EASY"), (1, "PRO")),
            Setting(176, "Easy Mode Speed", "Integer enum option", options: EasyModeSpeedOptions()),
            Setting(177, "Enable Night Photo", "Integer enum option", null, (0, "OFF"), (1, "ON")),
            Setting(178, "Wireless Band", "Integer enum option", null, (0, "NUM_2_4GHZ"), (1, "NUM_5GHZ")),
            Setting(179, "Star Trails Length", "Integer enum option", null,
                (1, "SHORT"), (2, "LONG"), (3, "MAX")),
            Setting(180, "System Video Mode", "Integer enum option", null,
                (0, "HIGHEST_QUALITY"), (101, "EXTENDED_BATTERY"), (102, "LONGEST_BATTERY"),
                (111, "STANDARD_QUALITY"), (112, "BASIC_QUALITY")),
            Setting(182, "Video Bit Rate", "Integer enum option", null,
                (0, "STANDARD"), (1, "HIGH"), (2, "MAX")),
            Setting(183, "Bit Depth", "Integer enum option", null, (0, "NUM_8_BIT"), (2, "NUM_10_BIT")),
            Setting(184, "Profiles", "Integer enum option", null,
                (0, "STANDARD"), (1, "HDR"), (2, "LOG"), (101, "HLG_HDR")),
            Setting(186, "Video Easy Mode", "Integer enum option", null,
                (0, "HIGHEST_QUALITY"), (1, "STANDARD_QUALITY"), (2, "BASIC_QUALITY"),
                (3, "STANDARD_VIDEO"), (4, "HDR_VIDEO")),
            Setting(187, "Lapse Mode", "Integer enum option", null,
                (0, "TIMEWARP"), (1, "STAR_TRAILS"), (2, "LIGHT_PAINTING"), (3, "VEHICLE_LIGHTS"),
                (4, "MAX_TIMEWARP"), (5, "MAX_STAR_TRAILS"), (6, "MAX_LIGHT_PAINTING"),
                (7, "MAX_VEHICLE_LIGHTS"), (8, "TIME_LAPSE_VIDEO"), (9, "NIGHT_LAPSE_VIDEO")),
            Setting(189, "Max Lens Mod", "Integer enum option", null,
                (0, "NONE"), (1, "MAX_LENS_1_0"), (2, "MAX_LENS_2_0"), (3, "MAX_LENS_2_5"),
                (4, "MACRO"), (5, "ANAMORPHIC"), (6, "ND_4"), (7, "ND_8"), (8, "ND_16"),
                (9, "ND_32"), (10, "STANDARD_LENS"), (100, "AUTO_DETECT")),
            Setting(190, "Max Lens Mod Enable", "Integer enum option", null, (0, "OFF"), (1, "ON")),
            Setting(191, "Easy Night Photo", "Integer enum option", null,
                (0, "SUPER_PHOTO"), (1, "NIGHT_PHOTO"), (2, "BURST")),
            Setting(192, "Multi Shot Aspect Ratio", "Integer enum option", null,
                (0, "NUM_4_3"), (1, "NUM_16_9"), (3, "NUM_8_7"), (4, "NUM_9_16")),
            Setting(193, "Framing", "Integer enum option", null,
                (0, "WIDESCREEN"), (1, "VERTICAL"), (2, "FULL_FRAME"),
                (100, "TRADITIONAL_4_3_V2"), (101, "WIDESCREEN_16_9_V2"),
                (103, "FULL_FRAME_8_7_V2"), (104, "VERTICAL_9_16_V2"),
                (105, "ULTRA_WIDESCREEN_21_9_V2"), (106, "FULL_FRAME_1_1_V2")),
            Setting(194, "Camera Mode", "Integer enum option", null, (0, "SINGLE_LENS"), (1, "NUM_360_")),
            Setting(196, "360 Photo Files Extension", "Integer enum option", null, (0, "NUM_JPG"), (1, "NUM_36P")),
            Setting(216, "Beep Volume", "Integer percentage", null,
                (70, "LOW"), (85, "MEDIUM"), (100, "HIGH")),
            Setting(219, "Setup Screen Saver", "Integer enum option", null,
                (0, "NEVER"), (1, "NUM_1_MIN"), (2, "NUM_2_MIN"), (3, "NUM_3_MIN"), (4, "NUM_5_MIN")),
            Setting(223, "Setup Language", "Integer enum option", null,
                (0, "ENGLISH_US"), (1, "ENGLISH_UK"), (2, "ENGLISH_AUS"), (3, "GERMAN"),
                (4, "FRENCH"), (5, "ITALIAN"), (6, "SPANISH"), (7, "SPANISH_NA"),
                (8, "CHINESE"), (9, "JAPANESE"), (10, "KOREAN"), (11, "PORTUGUESE"),
                (12, "RUSSIAN"), (13, "ENGLISH_IND"), (14, "SWEDISH")),
            Setting(227, "Photo Mode", "Integer enum option", null,
                (0, "SUPERPHOTO"), (1, "NIGHT_PHOTO"), (2, "BURST")),
            Setting(232, "Video Framing", "Integer enum option", null,
                (0, "NUM_4_3"), (1, "NUM_16_9"), (3, "NUM_8_7"), (4, "NUM_9_16"),
                (5, "NUM_21_9"), (6, "NUM_1_1")),
            Setting(233, "Multi Shot Framing", "Integer enum option", null,
                (0, "NUM_4_3"), (1, "NUM_16_9"), (3, "NUM_8_7"), (4, "NUM_9_16")),
            Setting(234, "Frame Rate", "Integer enum option", null,
                (0, "NUM_240_0"), (1, "NUM_120_0"), (2, "NUM_100_0"), (3, "NUM_90_0"),
                (5, "NUM_60_0"), (6, "NUM_50_0"), (8, "NUM_30_0"), (9, "NUM_25_0"),
                (10, "NUM_24_0"), (13, "NUM_200_0"), (15, "NUM_400_0"), (16, "NUM_360_0"),
                (17, "NUM_300_0")),
            Setting(236, "Automatic Wi-Fi Access Point", "Integer enum option", null, (0, "OFF"), (1, "ON")),
            Setting(237, "Auto Power On USB", "Integer enum option", null, (0, "OFF"), (1, "ON")),
            Setting(239, "General Sub Mode"),
            Setting(256, "Image Tuning"),
            Setting(257, "Subject Tracking"),
            Setting(273, "Output"),
        };

        return ToDictionary(entries);
    }

    private static Dictionary<int, CameraProtocolValueMetadata> CreateStatuses()
    {
        var entries = new[]
        {
            Status(1, "Battery Present", "Boolean", "Is the system's internal battery present?"),
            Status(2, "Internal Battery Bars", "Enum", null,
                (0, "ZERO"), (1, "ONE"), (2, "TWO"), (3, "THREE"), (4, "CHARGING")),
            Status(6, "Overheating", "Boolean", "Is the system currently overheating?"),
            Status(8, "Busy", "Boolean", "Is the camera busy?"),
            Status(9, "Quick Capture", "Boolean", "Is Quick Capture enabled?"),
            Status(10, "Encoding", "Boolean", "Is the system currently encoding?"),
            Status(11, "LCD Lock", "Boolean", "Is LCD lock active?"),
            Status(13, "Video Encoding Duration", "Unsigned 32-bit integer",
                "Duration of the current video in seconds; zero when not encoding."),
            Status(17, "Wireless Connections Enabled", "Boolean", "Are wireless connections enabled?"),
            Status(19, "Pairing State", "Enum", null,
                (0, "NEVER_STARTED"), (1, "STARTED"), (2, "ABORTED"), (3, "CANCELLED"), (4, "COMPLETED")),
            Status(20, "Last Pairing Type", "Enum", null,
                (0, "NOT_PAIRING"), (1, "PAIRING_APP"), (2, "PAIRING_REMOTE_CONTROL"), (3, "PAIRING_BLUETOOTH_DEVICE")),
            Status(21, "Last Pairing Success", "Unsigned 32-bit integer",
                "Time since boot in milliseconds of the last successful pairing completion."),
            Status(22, "Wi-Fi Scan State", "Enum", null,
                (0, "NEVER_STARTED"), (1, "STARTED"), (2, "ABORTED"), (3, "CANCELED"), (4, "COMPLETED")),
            Status(23, "Last Wi-Fi Scan Success", "Unsigned 8-bit integer",
                "Time since boot in milliseconds that the Wi-Fi access point scan completed."),
            Status(24, "Wi-Fi Provisioning State", "Enum", null,
                (0, "NEVER_STARTED"), (1, "STARTED"), (2, "ABORTED"), (3, "CANCELED"), (4, "COMPLETED")),
            Status(26, "Remote Version", "Unsigned 8-bit integer", "Wireless remote-control version."),
            Status(27, "Remote Connected", "Boolean", "Is a wireless remote control connected?"),
            Status(28, "Pairing State (Legacy)", "Unknown"),
            Status(29, "Connected Wi-Fi SSID", "String", "Network name when the camera is operating as a Wi-Fi station."),
            Status(30, "Access Point SSID", "String", "Network name the camera advertises in access-point mode."),
            Status(31, "Connected Devices", "Unsigned 8-bit integer", "Number of wireless devices connected to the camera."),
            Status(32, "Preview Stream", "Boolean", "Is Preview Stream enabled?"),
            Status(33, "Primary Storage", "Enum", null,
                (-1, "UNKNOWN"), (0, "OK"), (1, "SD_CARD_FULL"), (2, "SD_CARD_REMOVED"),
                (3, "SD_CARD_FORMAT_ERROR"), (4, "SD_CARD_BUSY"), (8, "SD_CARD_SWAPPED")),
            Status(34, "Remaining Photos", "Unsigned 32-bit integer", "Number of photos available with current settings."),
            Status(35, "Remaining Video Time", "Unsigned 32-bit integer", "Remaining capture time in seconds with current settings."),
            Status(38, "Photos", "Unsigned 32-bit integer", "Total number of photos on the SD card."),
            Status(39, "Videos", "Unsigned 32-bit integer", "Total number of videos on the SD card."),
            Status(41, "OTA", "Enum", null,
                (0, "IDLE"), (1, "DOWNLOADING"), (2, "VERIFYING"), (3, "DOWNLOAD_FAILED"),
                (4, "VERIFY_FAILED"), (5, "READY"), (6, "GOPRO_APP_DOWNLOADING"),
                (7, "GOPRO_APP_VERIFYING"), (8, "GOPRO_APP_DOWNLOAD_FAILED"),
                (9, "GOPRO_APP_VERIFY_FAILED"), (10, "GOPRO_APP_READY")),
            Status(42, "Pending Firmware Update Cancel", "Boolean", "Is a firmware update cancellation pending?"),
            Status(45, "Locate", "Boolean", "Is the locate-camera feature active?"),
            Status(49, "Timelapse Interval Countdown", "Unsigned 32-bit integer", "Current timelapse interval countdown."),
            Status(54, "SD Card Remaining", "Unsigned 64-bit integer", "Remaining SD card space in kilobytes."),
            Status(55, "Preview Stream Available", "Boolean", "Is Preview Stream supported in the current mode?"),
            Status(56, "Wi-Fi Bars", "Unsigned 8-bit integer", "Wi-Fi signal strength in bars."),
            Status(58, "Active Hilights", "Unsigned 8-bit integer", "Number of hilights in the current video."),
            Status(59, "Time Since Last Hilight", "Unsigned 32-bit integer", "Time since boot in milliseconds of the latest hilight."),
            Status(60, "Minimum Status Poll Period", "Unsigned 32-bit integer", "Minimum recommended status polling interval in milliseconds."),
            Status(65, "Liveview Exposure Select Mode", "Enum", null,
                (0, "DISABLED"), (1, "AUTO"), (2, "ISO_LOCK"), (3, "HEMISPHERE")),
            Status(66, "Liveview Y", "Unsigned 8-bit integer", "Liveview exposure-select y coordinate in percent."),
            Status(67, "Liveview X", "Unsigned 8-bit integer", "Liveview exposure-select x coordinate in percent."),
            Status(68, "GPS Lock", "Boolean", "Does the camera currently have a GPS lock?"),
            Status(69, "AP Mode", "Boolean", "Is access-point mode enabled?"),
            Status(70, "Internal Battery Percentage", "Unsigned 8-bit integer", "Internal battery level as a percentage."),
            Status(74, "Microphone Accessory", "Enum", null,
                (0, "ACCESSORY_NOT_CONNECTED"), (1, "ACCESSORY_CONNECTED"),
                (2, "ACCESSORY_CONNECTED_AND_A_MICROPHONE_IS_PLUGGED_INTO_THE_ACCESSORY")),
            Status(75, "Zoom Level", "Unsigned 8-bit integer", "Digital zoom level as a percentage."),
            Status(76, "Wireless Band", "Enum", null, (0, "NUM_2_4_GHZ"), (1, "NUM_5_GHZ")),
            Status(77, "Zoom Available", "Boolean", "Is digital zoom available?"),
            Status(78, "Mobile Friendly", "Boolean", "Are current video settings mobile friendly?"),
            Status(79, "FTU", "Boolean", "Is the camera in its First Time Use flow?"),
            Status(81, "5GHz Available", "Boolean", "Is the 5GHz wireless band available?"),
            Status(82, "Ready", "Boolean", "Is the system fully booted and ready to accept commands?"),
            Status(83, "OTA Charged", "Boolean", "Is the internal battery sufficiently charged to start OTA?"),
            Status(85, "Cold", "Boolean", "Is the camera too cold to continue recording?"),
            Status(86, "Rotation", "Enum", null,
                (0, "NUM_0_DEGREES_UPRIGHT"), (1, "NUM_180_DEGREES_UPSIDE_DOWN"),
                (2, "NUM_90_DEGREES_LAYING_ON_RIGHT_SIDE"), (3, "NUM_270_DEGREES_LAYING_ON_LEFT_SIDE")),
            Status(88, "Zoom While Encoding", "Boolean", "Can this camera model zoom while encoding?"),
            Status(89, "Flatmode", "Unsigned 8-bit integer", "Current Flatmode ID."),
            Status(93, "Video Preset", "Unsigned 32-bit integer", "Current video preset ID."),
            Status(94, "Photo Preset", "Unsigned 32-bit integer", "Current photo preset ID."),
            Status(95, "Timelapse Preset", "Unsigned 32-bit integer", "Current timelapse preset ID."),
            Status(96, "Preset Group", "Unsigned 32-bit integer", "Current preset group ID."),
            Status(97, "Preset", "Unsigned 32-bit integer", "Current preset ID."),
            Status(98, "Preset Modified", "Unsigned 32-bit integer", "Preset change indicator."),
            Status(99, "Remaining Live Bursts", "Unsigned 32-bit integer", "Number of live bursts available with current settings."),
            Status(100, "Live Bursts", "Unsigned 32-bit integer", "Total number of live bursts on the SD card."),
            Status(101, "Capture Delay Active", "Boolean", "Is capture delay currently counting down?"),
            Status(102, "Media Mod State", "Enum", null,
                (0, "MICROPHONE_REMOVED"), (2, "MICROPHONE_ONLY"), (3, "MICROPHONE_WITH_EXTERNAL_MICROPHONE")),
            Status(103, "Time Warp Speed", "Enum", null,
                (0, "NUM_15X"), (1, "NUM_30X"), (2, "NUM_60X"), (3, "NUM_150X"),
                (4, "NUM_300X"), (5, "NUM_900X"), (6, "NUM_1800X"), (7, "NUM_2X"),
                (8, "NUM_5X"), (9, "NUM_10X"), (10, "AUTO"), (11, "NUM_1X_REALTIME"),
                (12, "NUM_1_2X_SLOW_MOTION")),
            Status(104, "Linux Core", "Boolean", "Is the system's Linux core active?"),
            Status(105, "Lens Type", "Enum", null,
                (0, "DEFAULT"), (1, "MAX_LENS"), (2, "MAX_LENS_2_0"), (3, "MAX_LENS_2_5"),
                (4, "MACRO_LENS"), (5, "ANAMORPHIC_LENS"), (6, "NEUTRAL_DENSITY_4"),
                (7, "NEUTRAL_DENSITY_8"), (8, "NEUTRAL_DENSITY_16"), (9, "NEUTRAL_DENSITY_32")),
            Status(106, "Hindsight", "Boolean", "Is Video Hindsight Capture active?"),
            Status(107, "Scheduled Capture Preset ID", "Unsigned 32-bit integer"),
            Status(108, "Scheduled Capture", "Boolean", "Is Scheduled Capture set?"),
            Status(110, "Display Mod Status", "Bitfield enum", null,
                (0, "NUM_000_DISPLAY_MOD_0_HDMI_0_DISPLAY_MOD_CONNECTED_FALSE"),
                (1, "NUM_001_DISPLAY_MOD_0_HDMI_0_DISPLAY_MOD_CONNECTED_TRUE"),
                (2, "NUM_010_DISPLAY_MOD_0_HDMI_1_DISPLAY_MOD_CONNECTED_FALSE"),
                (3, "NUM_011_DISPLAY_MOD_0_HDMI_1_DISPLAY_MOD_CONNECTED_TRUE"),
                (4, "NUM_100_DISPLAY_MOD_1_HDMI_0_DISPLAY_MOD_CONNECTED_FALSE"),
                (5, "NUM_101_DISPLAY_MOD_1_HDMI_0_DISPLAY_MOD_CONNECTED_TRUE"),
                (6, "NUM_110_DISPLAY_MOD_1_HDMI_1_DISPLAY_MOD_CONNECTED_FALSE"),
                (7, "NUM_111_DISPLAY_MOD_1_HDMI_1_DISPLAY_MOD_CONNECTED_TRUE")),
            Status(111, "SD Card Write Speed Error", "Boolean", "Is there an SD card minimum write speed error?"),
            Status(112, "SD Card Errors", "Unsigned 8-bit integer", "Number of SD card write-speed errors since boot."),
            Status(113, "Turbo Transfer", "Boolean", "Is Turbo Transfer active?"),
            Status(114, "Camera Control ID", "Enum", null,
                (0, "CAMERA_IDLE_NO_ONE_IS_ATTEMPTING_TO_CHANGE_CAMERA_SETTINGS"),
                (1, "CAMERA_CONTROL_CAMERA_IS_IN_A_MENU_OR_CHANGING_SETTINGS_TO_INTERVENE_APP_MUST_REQUEST_CONTROL"),
                (2, "CAMERA_EXTERNAL_CONTROL_AN_OUTSIDE_ENTITY_APP_HAS_CONTROL_AND_IS_IN_A_MENU_OR_MODIFYING_SETTINGS")),
            Status(115, "USB Connected", "Boolean", "Is the camera connected to a PC via USB?"),
            Status(116, "USB Controlled", "Enum", null, (0, "DISABLED"), (1, "ENABLED")),
            Status(117, "SD Card Capacity", "Unsigned 32-bit integer", "Total SD card capacity in kilobytes."),
            Status(118, "Photo Interval Capture Count", "Unsigned 32-bit integer"),
            Status(122, "Camera Name", "UTF-8 string", "Custom camera name."),
            Status(123, "Active Preset Is Custom", "Unknown"),
        };

        return ToDictionary(entries);
    }

    private static CameraProtocolValueMetadata Setting(
        int id,
        string name,
        string? valueKind = null,
        string? description = null,
        params (long Value, string Name)[] options) =>
        Create(id, name, description, valueKind, SettingsDocumentation + id,
            id <= 237 ? SettingsSdkSource : null, id <= 237 ? SettingsSdkSource : null, options);

    private static (long Value, string Name)[] EasyModeSpeedOptions() =>
    [
        (0, "NUM_8X_ULTRA_SLO_MO"),
        (1, "NUM_4X_SUPER_SLO_MO"),
        (2, "NUM_2X_SLO_MO"),
        (3, "NUM_1X_SPEED_LOW_LIGHT_"),
        (4, "NUM_4X_SUPER_SLO_MO_EXT_BATT_"),
        (5, "NUM_2X_SLO_MO_EXT_BATT_"),
        (6, "NUM_1X_SPEED_EXT_BATT_LOW_LIGHT_"),
        (7, "NUM_8X_ULTRA_SLO_MO_50HZ_"),
        (8, "NUM_4X_SUPER_SLO_MO_50HZ_"),
        (9, "NUM_2X_SLO_MO_50HZ_"),
        (10, "NUM_1X_SPEED_50HZ_LOW_LIGHT_"),
        (11, "NUM_4X_SUPER_SLO_MO_50HZ_EXT_BATT_"),
        (12, "NUM_2X_SLO_MO_50HZ_EXT_BATT_"),
        (13, "NUM_1X_SPEED_50HZ_EXT_BATT_LOW_LIGHT_"),
        (14, "NUM_8X_ULTRA_SLO_MO_EXT_BATT_"),
        (15, "NUM_8X_ULTRA_SLO_MO_50HZ_EXT_BATT_"),
        (16, "NUM_8X_ULTRA_SLO_MO_LONG_BATT_"),
        (17, "NUM_4X_SUPER_SLO_MO_LONG_BATT_"),
        (18, "NUM_2X_SLO_MO_LONG_BATT_"),
        (19, "NUM_1X_SPEED_LONG_BATT_LOW_LIGHT_"),
        (20, "NUM_8X_ULTRA_SLO_MO_50HZ_LONG_BATT_"),
        (21, "NUM_4X_SUPER_SLO_MO_50HZ_LONG_BATT_"),
        (22, "NUM_2X_SLO_MO_50HZ_LONG_BATT_"),
        (23, "NUM_1X_SPEED_50HZ_LONG_BATT_LOW_LIGHT_"),
        (24, "NUM_2X_SLO_MO_4K_"),
        (25, "NUM_4X_SUPER_SLO_MO_2_7K_"),
        (26, "NUM_2X_SLO_MO_4K_50HZ_"),
        (27, "NUM_4X_SUPER_SLO_MO_2_7K_50HZ_"),
        (100, "NUM_8X_ULTRA_SLO_MO_V2_"),
        (101, "NUM_4X_SUPER_SLO_MO_V2_"),
        (102, "NUM_2X_SLO_MO_V2_"),
        (103, "NUM_1X_SPEED_LOW_LIGHT_V2_"),
        (104, "NUM_8X_ULTRA_SLO_MO_50HZ_V2_"),
        (105, "NUM_4X_SUPER_SLO_MO_50HZ_V2_"),
        (106, "NUM_2X_SLO_MO_50HZ_V2_"),
        (107, "NUM_1X_SPEED_50HZ_LOW_LIGHT_V2_"),
        (108, "NUM_8X_ULTRA_SLO_MO_LONG_BATT_V2_"),
        (109, "NUM_4X_SUPER_SLO_MO_LONG_BATT_V2_"),
        (110, "NUM_2X_SLO_MO_LONG_BATT_V2_"),
        (111, "NUM_1X_SPEED_LONG_BATT_LOW_LIGHT_V2_"),
        (112, "NUM_8X_ULTRA_SLO_MO_50HZ_LONG_BATT_V2_"),
        (113, "NUM_4X_SUPER_SLO_MO_50HZ_LONG_BATT_V2_"),
        (114, "NUM_2X_SLO_MO_50HZ_LONG_BATT_V2_"),
        (115, "NUM_1X_SPEED_50HZ_LONG_BATT_LOW_LIGHT_V2_"),
        (116, "NUM_2X_SLO_MO_4K_V2_"),
        (117, "NUM_2X_SLO_MO_4K_50HZ_V2_"),
        (118, "NUM_1X_SPEED_LOW_LIGHT_V2_VERTICAL_"),
        (119, "NUM_1X_SPEED_50HZ_LOW_LIGHT_V2_VERTICAL_"),
        (120, "NUM_2X_SLO_MO_V2_VERTICAL_"),
        (121, "NUM_2X_SLO_MO_50HZ_V2_VERTICAL_"),
        (122, "NUM_1X_SPEED_FULL_FRAME_LOW_LIGHT_V2_"),
        (123, "NUM_1X_SPEED_50HZ_FULL_FRAME_LOW_LIGHT_V2_"),
        (124, "NUM_2X_SLO_MO_FULL_FRAME_V2_"),
        (125, "NUM_2X_SLO_MO_50HZ_FULL_FRAME_V2_"),
        (126, "NUM_1X_SPEED_4K_LOW_LIGHT_V2_"),
        (127, "NUM_1X_SPEED_4K_50HZ_LOW_LIGHT_V2_"),
        (128, "NUM_1X_SPEED_2_7K_LOW_LIGHT_V2_"),
        (129, "NUM_1X_SPEED_2_7K_50HZ_LOW_LIGHT_V2_"),
        (130, "NUM_2X_SLO_MO_2_7K_V2_"),
        (131, "NUM_2X_SLO_MO_2_7K_50HZ_V2_"),
        (132, "NUM_2X_SLO_MO_LONG_BATT_V2_VERTICAL_"),
        (133, "NUM_2X_SLO_MO_50HZ_LONG_BATT_V2_VERTICAL_"),
        (134, "NUM_1X_SPEED_LONG_BATT_LOW_LIGHT_V2_VERTICAL_"),
        (135, "NUM_1X_SPEED_50HZ_LONG_BATT_LOW_LIGHT_V2_VERTICAL_"),
        (136, "NUM_1X_SPEED_4K_FULL_FRAME_LOW_LIGHT_V2_"),
        (137, "NUM_1X_SPEED_4K_50HZ_FULL_FRAME_LOW_LIGHT_V2_"),
        (138, "NUM_1X_NORMAL_SPEED_1_1_30_FPS_4K_V2_"),
        (139, "NUM_1X_NORMAL_SPEED_1_1_25_FPS_4K_V2_"),
        (140, "NUM_2X_SLO_MO_SPEED_1_1_4K_60_FPS_V2_"),
        (141, "NUM_2X_SLO_MO_SPEED_1_1_4K_50_FPS_V2_"),
        (142, "NUM_1X_NORMAL_SPEED_21_9_30_FPS_5_3K_V2_"),
        (143, "NUM_1X_NORMAL_SPEED_21_9_25_FPS_5_3K_V2_"),
        (144, "NUM_2X_SLO_MO_SPEED_21_9_5_3K_60_FPS_V2_"),
        (145, "NUM_2X_SLO_MO_SPEED_21_9_5_3K_50_FPS_V2_"),
        (146, "NUM_1X_NORMAL_SPEED_21_9_30_FPS_4K_V2_"),
        (147, "NUM_1X_NORMAL_SPEED_21_9_25_FPS_4K_V2_"),
        (148, "NUM_2X_SLO_MO_SPEED_21_9_4K_60_FPS_V2_"),
        (149, "NUM_2X_SLO_MO_SPEED_21_9_4K_50_FPS_V2_"),
        (150, "NUM_120_4X_SUPER_SLO_MO_SPEED_21_9_4K_V2_"),
        (151, "NUM_100_4X_SUPER_SLO_MO_SPEED_21_9_4K_V2_"),
        (152, "NUM_1X_NORMAL_SPEED_30_FPS_4_3_5_3K_V2_"),
        (153, "NUM_1X_NORMAL_SPEED_25_FPS_4_3_5_3K_V2_"),
        (154, "NUM_1X_NORMAL_SPEED_30_FPS_4_3_4K_V2_"),
        (155, "NUM_1X_NORMAL_SPEED_25_FPS_4_3_4K_V2_"),
        (156, "NUM_2X_SLO_MO_SPEED_4_3_4K_60_FPS_V2_"),
        (157, "NUM_2X_SLO_MO_SPEED_4_3_4K_50_FPS_V2_"),
        (158, "NUM_120_4X_SUPER_SLO_MO_SPEED_2_7K_4_3_V2_"),
        (159, "NUM_100_4X_SUPER_SLO_MO_SPEED_2_7K_4_3_V2_"),
    ];

    private static CameraProtocolValueMetadata Status(
        int id,
        string name,
        string? valueKind = null,
        string? description = null,
        params (long Value, string Name)[] options) =>
        Create(id, name, description, valueKind, StatusesDocumentation + id,
            id <= 122 ? StatusesSdkSource : null,
            id is >= 1 and <= 122 and not 28 ? StatusesTypeSource : null, options);

    private static CameraProtocolValueMetadata Create(
        int id,
        string name,
        string? description,
        string? valueKind,
        string documentationUrl,
        string? sdkSourceUrl,
        string? typeSourceUrl,
        (long Value, string Name)[] options)
    {
        var mappedOptions = new CameraProtocolOption[options.Length];
        for (var index = 0; index < options.Length; index++)
        {
            mappedOptions[index] = new CameraProtocolOption(options[index].Value, options[index].Name);
        }

        return new CameraProtocolValueMetadata(
            id, name, description, valueKind, documentationUrl, sdkSourceUrl, typeSourceUrl,
            System.Array.AsReadOnly(mappedOptions));
    }

    private static Dictionary<int, CameraProtocolValueMetadata> ToDictionary(
        IEnumerable<CameraProtocolValueMetadata> entries)
    {
        var result = new Dictionary<int, CameraProtocolValueMetadata>();
        foreach (var entry in entries)
        {
            result.Add(entry.Id, entry);
        }

        return result;
    }
}
