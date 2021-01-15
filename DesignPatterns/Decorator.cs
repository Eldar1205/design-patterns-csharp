using System;
using System.IO;

namespace DesignPatterns
{
    class HttpRequest
    {
    }

    class HttpResponse
    {
        public int StatusCode;

        public HttpResponse(int statusCode) => StatusCode = statusCode;
    }

    interface IHttpRequestHandler
    {
        HttpResponse Handle(HttpRequest request);
    }

    class EchoHttpRequestHandler : IHttpRequestHandler
    {
        public virtual HttpResponse Handle(HttpRequest request) => new HttpResponse(200);
    }

    #region Bad extensions of basic implementation using inheritence
    class LoggingEchoHttpRequestHandler : EchoHttpRequestHandler
    {
        public override HttpResponse Handle(HttpRequest request)
        {
            File.AppendAllLines("log.txt", new[] { "Http request handler begin" });
            var response = base.Handle(request);
            File.AppendAllLines("log.txt", new[] { "Http request handler end" });

            return response;
        }
    }

    class ExceptionHandlingLoggingEchoHttpRequestHandler : LoggingEchoHttpRequestHandler
    {
        public override HttpResponse Handle(HttpRequest request)
        {
            try
            {
                return base.Handle(request);
            }
            catch
            {
                return new HttpResponse(500);
            }
        }
    } 
    #endregion

    #region Good implementation using Decorator pattern
    class LoggerHttpRequestHandlerDecorator : IHttpRequestHandler
    {
        private readonly IHttpRequestHandler _httpRequestHandlerImplementation;

        public LoggerHttpRequestHandlerDecorator(IHttpRequestHandler httpRequestHandlerImplementation) => 
            _httpRequestHandlerImplementation = httpRequestHandlerImplementation;

        public HttpResponse Handle(HttpRequest request)
        {
            File.AppendAllLines("log.txt", new[] { "Http request handler begin" });
            var response = _httpRequestHandlerImplementation.Handle(request);
            File.AppendAllLines("log.txt", new[] { "Http request handler end" });

            return response;
        }
    }

    class ExceptionHandlingHttpRequestHandlerDecorator : IHttpRequestHandler
    {
        private readonly IHttpRequestHandler _httpRequestHandlerImplementation;

        public ExceptionHandlingHttpRequestHandlerDecorator(IHttpRequestHandler httpRequestHandlerImplementation)
            => _httpRequestHandlerImplementation = httpRequestHandlerImplementation;

        public HttpResponse Handle(HttpRequest request)
        {
            try
            {
                return _httpRequestHandlerImplementation.Handle(request);
            }
            catch
            {
                return new HttpResponse(500);
            }
        }
    } 
    #endregion
}
