using System;

namespace GunksAlert.Cli.Entities;

public class ApiErrorResponse : IApiResponse {
    private Uri _requestUri;
    private string _msg;
    private ApiResponseContent? _content;

    public ApiErrorResponse(Uri requestUri, string msg, ApiResponseContent? content = null) {
        _requestUri = requestUri;
        _msg = msg;
        _content = content;
    }

    public string GetMessage() {
        return _msg;
    }

    public Uri GetRequestUri() {
        return _requestUri;
    }

    public ApiResponseContent? GetContent() {
        return _content;
    }
}
