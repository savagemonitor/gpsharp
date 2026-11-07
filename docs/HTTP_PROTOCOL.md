# HTTP Protocol

GpSharp Phase 1 uses the Open GoPro camera HTTP API. The camera base URL is constructed from the
discovered address and port, for example `http://10.5.5.9:8080/` for the camera's Wi-Fi access
point.

## Supported Phase 1 operations

| SDK operation | HTTP request |
|---|---|
| `SetShutterAsync(true)` | `GET /gopro/camera/shutter/start` |
| `SetShutterAsync(false)` | `GET /gopro/camera/shutter/stop` |
| `GetMediaListAsync()` | `GET /gopro/media/list` |
| `ISetting.GetValueAsync<T>()` | `GET /gopro/camera/setting?setting=<id>` |
| `ISetting.SetValueAsync(value)` | `GET /gopro/camera/setting?setting=<id>&option=<value>` |
| `IStatusesContainer.GetCameraStateAsync()` / `IStatus.GetValueAsync<T>()` | `GET /gopro/camera/state` |

These endpoints follow the corresponding HTTP operations in the OpenGoPro Kotlin SDK.
`HttpConnection` retries requests that fail with `HttpRequestException` up to five times and then
returns the final failure in the operation's `Result<T>`. It never converts failures to successful
empty values.

## Media list response

The media-list JSON contains a `media` array. Each entry uses `d` for the directory and `fs` for
its files; file entries use `n` for name, `cre`/`mod` for timestamps, and `s` for size. The
size may be a JSON number or a numeric string. GpSharp
deserializes these fields into `MediaList`, `MediaDirectory`, and `MediaFile`. Unrecognized
properties are retained in each model's `AdditionalProperties`.

## Settings and status values

Setting and status identifiers are protocol IDs in the range `0..255`. The named settings map to
video resolution ID `2` and video frame rate ID `3`; named statuses map to encoding ID `10` and
internal battery percentage ID `70`. `GetSetting(int)` and `GetStatus(int)` expose other IDs.
Setting reads deserialize the response's integer `option` into the caller's requested type;
writes accept non-negative integer option values, including enums. The camera is authoritative and
may reject an option.

The state endpoint returns `settings` and `status` objects keyed by numeric protocol IDs. Values
are retained as `JsonElement` in `CameraState`, allowing callers to interpret camera/model-specific
fields without lossy conversion. `GetCameraStateAsync` reads this snapshot once; `GetStatus` reads
a fresh snapshot for each `GetValueAsync` call. Missing IDs, malformed JSON, unsupported requested
types, and HTTP failures are `Result<T>` failures. Cancellation propagates as
`OperationCanceledException`.

When a setting write is rejected with HTTP 403, GpSharp preserves the response as a
`SettingRejectedException` if it contains the Open GoPro error, setting ID, option ID, and
`supported_options` metadata. This is reactive rejection metadata, not proactive capability
discovery.

The Kotlin SDK's generic setting/status API, capability discovery, and update streams are
BLE-backed. These HTTP operations implement the corresponding Open GoPro HTTP subset; they do not
provide Kotlin BLE parity, automatic updates, or BLE connectivity state. Physical writes alter
camera configuration, so the automated suite uses mocked HTTP responses only. Media download,
credential-backed HTTPS, and other HTTP operations remain outside Phase 1.
