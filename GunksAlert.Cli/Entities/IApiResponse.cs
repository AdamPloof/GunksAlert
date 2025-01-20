using System;

namespace GunksAlert.Cli.Entities;

/// <summary>
/// A container for info about responses to API requests
/// </summary>
public interface IApiResponse {
    /// <summary>
    /// Human readable summary of the response
    /// </summary>
    /// <returns></returns>
    public string GetMessage();

    /// <summary>
    /// The URI that the request was originally sent to
    /// </summary>
    /// <returns></returns>
    public Uri GetRequestUri();

    /// <summary>
    /// The deserialized content from the response body
    /// </summary>
    /// <returns></returns>
    public ApiResponseContent? GetContent();
}
