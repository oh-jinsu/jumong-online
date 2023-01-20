using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class HttpClient : Node
{
    public class Response
    {
        public readonly int statusCode;

        public readonly string text;

        public Response(int statusCode, string text)
        {
            this.statusCode = statusCode;

            this.text = text;
        }
    }

    private const int POLL_INTERVAL = 1;

    public async Task<Result<Response>> GetAsync(Uri uri)
    {
        var client = ConnectClient(uri.host);

        string[] headers = { "User-Agent: Pirulo/1.0 (Godot)", "Accept: */*" };

        return await RequestAsyc(client, HTTPClient.Method.Get, uri.endpoint, headers);
    }

    public async Task<Result<Response>> PostAsync(Uri uri, string body)
    {
        var client = ConnectClient(uri.host);

        string[] headers = { "User-Agent: Pirulo/1.0 (Godot)", "Accept: */*", "Content-Type: application/json;charset=utf-8" };

        return await RequestAsyc(client, HTTPClient.Method.Post, uri.endpoint, headers, body);
    }

    private static HTTPClient ConnectClient(string host)
    {
        var client = new HTTPClient();

        var error = client.ConnectToHost(host);

        if (error != Error.Ok)
        {
            throw new Exception("Failed to connect to host: " + error);
        }

        while (client.GetStatus() == HTTPClient.Status.Connecting || client.GetStatus() == HTTPClient.Status.Resolving)
        {
            OS.DelayMsec(POLL_INTERVAL);

            client.Poll();
        }

        if (client.GetStatus() != HTTPClient.Status.Connected)
        {
            throw new Exception("Failed to connect to host: " + client.GetStatus());
        }

        return client;
    }

    private async Task<Result<Response>> RequestAsyc(HTTPClient client, HTTPClient.Method method, string endpoint, string[] headers, string body = "")
    {
        var error = client.Request(method, endpoint, headers, body);

        if (error != Error.Ok)
        {
            throw new Exception("Failed to request");
        }

        while (client.GetStatus() == HTTPClient.Status.Requesting)
        {
            if (OS.HasFeature("web"))
            {
                await ToSignal(Engine.GetMainLoop(), "idle_frame");
            }
            else
            {
                OS.DelayMsec(POLL_INTERVAL);
            }

            client.Poll();
        }

        if (client.GetStatus() != HTTPClient.Status.Body && client.GetStatus() != HTTPClient.Status.Connected)
        {
            return new Err<Response>();
        }

        if (!client.HasResponse())
        {
            return new Err<Response>();
        }

        var buffer = new List<byte>();

        while (client.GetStatus() == HTTPClient.Status.Body)
        {
            var chunk = client.ReadResponseBodyChunk();

            if (chunk.Length == 0)
            {
                OS.DelayMsec(POLL_INTERVAL);
            }
            else
            {
                buffer.AddRange(chunk);
            }

            client.Poll();
        }

        var text = Encoding.UTF8.GetString(buffer.ToArray());

        var response = new Response(client.GetResponseCode(), text);

        return new Ok<Response>(response);
    }
}
