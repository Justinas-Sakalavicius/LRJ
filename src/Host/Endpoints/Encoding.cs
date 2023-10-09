using Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace Host.Endpoints;

public static class Encoding
{
    public static void MapEncodingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/encoding/start", StartJob);
        app.MapPost("/api/encoding/cancel", StopJob);
    }

    public static IResult StartJob(IEncodingService _encodingService, [FromQuery] string input, CancellationToken ct)
    {
        if (ct.IsCancellationRequested)
        {
            return Results.BadRequest("Processing canceled");
        }

        // max input based on cache storign time 360 chars
        if (string.IsNullOrWhiteSpace(input))
        {
            return Results.BadRequest("Input text is required.");
        }

        return Results.Ok(_encodingService.StartEncodingAsync(input));
    }

    public static IResult StopJob(IEncodingService _encodingService, CancellationToken ct)
    {
        return Results.Ok(_encodingService.StopEncodingAsync());
    }
}
