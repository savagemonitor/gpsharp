namespace GpSharp.Network;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using GpSharp.Data;
using GpSharp.Core;

/// <summary>
/// Implementation of Network Discovery using UDP multicast/broadcast.
/// </summary>
public class NetworkDiscovery : IDiscoveryService
{
    private const int DiscoveryPort = 5353; // Standard mDNS port

    /// <summary>
    /// Scans the network for GoPros by listening for UDP discovery packets.
    /// </summary>
    public async Task<IEnumerable<DiscoveredDevice>> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var discoveredDevices = new List<DiscoveredDevice>();

        using var udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, DiscoveryPort));

        // Set a timeout for the deployment process
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(5)); 

        try
        {
            while (!cts.IsCancellationRequested)
            {
                var receiveTask = udpClient.ReceiveAsync();
                var completedTask = await Task.WhenAny(receiveTask, Task.Delay(1000, cts.Token));

                if (completedTask == receiveTask)
                {
                    var result = await receiveTask;
                    var message = Encoding.UTF8.GetString(result.Buffer);
                    
                    // Expecting a JSON format like: {"id":"1234", "name":"GoPro", "ip":"192.168.1.50", "port":443, "mac":"00:11:22:33:44:55", "model":"Hero12"}
                    var device = ParseDiscoveryMessage(message, result.RemoteEndPoint.Address.ToString());
                    if (device != null)
                    {
                        discoveredDevices.Add(device);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Discovery timed out or was cancelled
        }
        catch (Exception ex)
        {
            // Log error 
            Console.WriteLine($"Discovery error: {ex.Message}");
        }

        return discoveredDevices;
    }

    private DiscoveredDevice? ParseDiscoveryMessage(string message, string senderIp)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message)) return null;

            // Simple check: does it contain our expected marker?
            if (!message.Contains("gopro-discovery")) return null;

            // Mocking the extraction of data from the message...
            var dto = JsonSerializer.Deserialize<DiscoveryMessageDto>(message);
            if (dto == null) return null;

            return new DiscoveredDevice(
                new GoProId(dto.Id),
                dto.Name, 
                dto.IpAddress, 
                dto.Port, 
                dto.MacAddress, 
                dto.Model
            );
        }
        catch
        {
            return null;
        }
    }

    private class DiscoveryMessageDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("ip")]
        public string IpAddress { get; set; } = string.Empty;

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("mac")]
        public string MacAddress { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;
    }
}
