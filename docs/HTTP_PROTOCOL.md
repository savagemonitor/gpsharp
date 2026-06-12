# HTTP Protocol Specification

This document describes the HTTP-based communication protocol used by GpSharp to interact with GoPro cameras.

## Overview

The SDK communicates with GoPros via standard HTTP requests (GET, POST, PUT). The camera acts as an HTTP server on a specific port (usually 443 or 80).

## Base URL Structure

All requests are made to the base URL of the discovered device:
`http://<device_ip>:<port>/`

## Request Methods

### GET
Used to retrieve information, such as:
- **Status**: `GET /status/<key>` (e.                
- **Settings**: `GET /settings/<key>` 
- **Media List**: `GET /camera/media_list`

### POST
Used to execute commands or send payloads that don't require a return body, such as:
- **Shutter Control**: `POST /camera/shutter` with JSON payload `{ "on": true }`
- **Capture Photo**: `POST /camera/capture_photo`

### PUT
Used to update settings on the camera:
- **Update Setting**: `PUT /settings/<key>` with JSON payload `{ "value": <new_value> }`

## Payload Format

All request and response payloads are encoded in **JSON**. 

Example: To change the video resolution.
**Request:**
`PUT /settings/video_resolution`
`Content-Type: application 
{ "value": "4K" }`

## Error Handling

The SDK handles standard HTTP error codes:
- `200 OK`: Success.
- `400 Bad Request`: Malformed request or invalid parameter.
- `404 Not Found`: The requested endpoint or setting does not exist.
- `500 Internal Server Error`: An error occurred on the camera side.

All errors are wrapped in the SDK's internal result pattern to ensure robust error handling and prevent unnecessary exceptions during standard operation.
