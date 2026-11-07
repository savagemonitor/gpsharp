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

## Deferred operations

The Kotlin SDK implements settings and statuses over BLE, not this HTTP surface. Those GpSharp
placeholders throw `NotSupportedException` until BLE support is implemented. Media download,
credential-backed HTTPS, and other HTTP operations are also outside the Phase 1 subset.
