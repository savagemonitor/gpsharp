namespace GpSharp.GoPro.Statuses;

using System;
using System.Threading;
using System.Threading.Tasks;
using GpSharp.Core;
using GpSharp.Data;
using GpSharp.GoPro.Connectors;
using GpSharp.GoPro.Settings;

/// <summary>
/// Provides access to camera status values over the Open GoPro HTTP API.
/// </summary>
public class StatusesContainer : IStatusesContainer
{
    public IStatus InternalBatteryPercentage { get; }
    public IStatus IsEncoding { get; }

    private readonly CameraStateClient _stateClient;

    public StatusesContainer(IHttpConnection connection)
    {
        _stateClient = new CameraStateClient(connection);
        InternalBatteryPercentage = new GenericStatus(70, _stateClient);
        IsEncoding = new GenericStatus(10, _stateClient);
    }

    public IStatus GetStatus(int statusId) =>
        new GenericStatus(SettingsContainer.ValidateId(statusId, nameof(statusId)), _stateClient);

    public Task<Result<CameraState>> GetCameraStateAsync(CancellationToken cancellationToken = default) =>
        _stateClient.GetAsync(cancellationToken);
}
