namespace GpSharp.GoPro;

using GpSharp.Core;
using GpSharp.GoPro.Commands;
using GpSharp.GoPro.Features;
using GpSharp.GoPro.Settings;
using GpSharp.GoPro.Status;

/// <summary>
/// The main device interface for a GoPro camera.
/// </summary>
public class GoPro
{
    /// <summary>
    /// Gets a value indicating whether the GoPro is ready to accept commands.
    /// </summary>
    public bool IsReady { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the GoPro is actively encoding.
    /// </summary>
    public bool IsEncoding { get; private set; }

    /// <summary>
    /// Gets the commands container for executing camera operations.
    /// </summary>
    public ICommandsContainer Commands { get; }

    /// <summary>
    /// Gets the settings container for camera configuration.
    /// </summary>
    public ISettingsContainer Settings { get; }

    /// <summary>
    /// Gets the statuses container for camera state information.
    /// </summary>
    public IStatusesContainer Statuses { get; }

    /// <summary>
    /// Gets the features container for advanced camera features.
    /// </summary>
    public IFeaturesContainer Features { get; }

    /// <summary>
    /// Initializes a new instance of the GoPro class.
    /// </summary>
    public GoPro()
    {
        Commands = new CommandsContainer();
        Settings = new SettingsContainer();
        Statuses = new StatusesContainer();
        Features = new FeaturesContainer();
    }
}
