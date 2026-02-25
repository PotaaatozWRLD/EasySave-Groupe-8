using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using EasySave.Shared;

namespace EasyLog;

/// <summary>
/// Logger implementation that sends log entries to a remote server via TCP.
/// </summary>
public class NetworkLogger : ILogger, IDisposable
{
    private readonly string _serverIp;
    private readonly int _serverPort;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly object _lock = new();

    public NetworkLogger(string serverIp, int serverPort)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;
    }

    private void EnsureConnection()
    {
        if (_client == null || !_client.Connected)
        {
            try
            {
                _client = new TcpClient();
                // Connect with a short timeout to ensure we don't block heavily
                var result = _client.BeginConnect(_serverIp, _serverPort, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1)); // 1s timeout
                
                if (success)
                {
                    _client.EndConnect(result);
                    _stream = _client.GetStream();
                }
                else
                {
                    _client.Close();
                    _client = null;
                }
            }
            catch
            {
                // Connection failed - ignore to avoid breaking the application
                _client = null;
                _stream = null;
            }
        }
    }

    public void WriteLog(LogEntry logEntry)
    {
        lock (_lock)
        {
            try
            {
                EnsureConnection();

                if (_stream != null && _stream.CanWrite)
                {
                    var options = new JsonSerializerOptions { WriteIndented = false };
                    string json = JsonSerializer.Serialize(logEntry, options);
                    byte[] data = Encoding.UTF8.GetBytes(json + Environment.NewLine);
                    _stream.Write(data, 0, data.Length);
                }
            }
            catch
            {
                // Failed to send log - ignore network errors
                _client?.Close();
                _client = null;
                _stream = null;
            }
        }
    }

    public void UpdateState(StateEntry stateEntry)
    {
        // Network logger currently only handles execution logs, not real-time state
        // But we could send state updates too if needed.
        // For now, no-op.
    }

    public void Dispose()
    {
        _stream?.Dispose();
        _client?.Dispose();
    }
}
