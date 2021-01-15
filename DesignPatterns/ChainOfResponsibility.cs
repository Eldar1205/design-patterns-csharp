using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace DesignPatterns
{
    interface IRequestHandler
    {
        HttpResponseMessage Handle(HttpRequestMessage request);
    }

    #region Bad god class implementation that handles all the requests 
    class GodRequestHandler : IRequestHandler
    {
        public HttpResponseMessage Handle(HttpRequestMessage request)
        {
            var requestMethod = request.Method;
            var requestPath = request.RequestUri.GetLeftPart(UriPartial.Path);

            // GET a single image
            if (requestMethod == HttpMethod.Get && requestPath.StartsWith("api/images/"))
            {
                var imageId = requestPath.Remove(0, "api/images/".Length);
                byte[] image = null; // Replace with code to retrieve image by id

                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(image) };
            }

            // PUT image
            if (requestMethod == HttpMethod.Put && requestPath.StartsWith("api/images/"))
            {
                var imageId = requestPath.Remove(0, "api/images/".Length);
                var newImage = request.Content.ReadAsByteArrayAsync().Result;

                bool didImageExist = true; // Add code to persist image with given id to DB and return indication whether it existed

                var statusCode = didImageExist ? HttpStatusCode.NoContent : HttpStatusCode.NotFound;
                return new HttpResponseMessage(statusCode);
            }

            // POST image
            if (requestMethod == HttpMethod.Post && requestPath == "api/images")
            {
                var image = request.Content.ReadAsByteArrayAsync().Result;
                int imageId = 0; // Replace with code to persist image to DB and get back id

                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Headers.Location = new Uri("/api/images/" + imageId);

                return response;
            }

            // Fallback if we reached here: Bad request (not the right response for real code, it's just for the example)
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    class GotRequestHandlerApp
    {
        static void Main()
        {
            var requestHandler = new GodRequestHandler();
            var response = requestHandler.Handle(new HttpRequestMessage(HttpMethod.Get, "api/images/5"));
        }
    }
    #endregion

    #region Good implementation using Chain of Responsibility Pattern

    abstract class RequestHandlerChainItem : IRequestHandler
    {
        private readonly IRequestHandler _nextChainItem;

        public RequestHandlerChainItem(IRequestHandler nextChainItem = null)
        {
            _nextChainItem = nextChainItem;
        }

        public HttpResponseMessage Handle(HttpRequestMessage request)
        {
            RequestPath = request.RequestUri.GetLeftPart(UriPartial.Path);
            return InnerHandle(request) ?? _nextChainItem?.Handle(request);
        }

        protected abstract HttpResponseMessage InnerHandle(HttpRequestMessage request);
        protected string RequestPath { get; private set; } // For reuse and optimization purposes
    }

    class GetImageRequestHandler : RequestHandlerChainItem
    {
        public GetImageRequestHandler(IRequestHandler nextChainItem) : base(nextChainItem)
        {
        }

        protected override HttpResponseMessage InnerHandle(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get && RequestPath.StartsWith("api/images/"))
            {
                var imageId = RequestPath.Remove(0, "api/images/".Length);
                byte[] image = null; // Replace with code to retrieve image by id

                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(image) };
            }

            return null;
        }
    }

    class PutImageRequestHandler : RequestHandlerChainItem
    {
        public PutImageRequestHandler(IRequestHandler nextChainItem) : base(nextChainItem)
        {
        }

        protected override HttpResponseMessage InnerHandle(HttpRequestMessage request)
        {
            // PUT image
            if (request.Method == HttpMethod.Put && RequestPath.StartsWith("api/images/"))
            {
                var imageId = RequestPath.Remove(0, "api/images/".Length);
                var newImage = request.Content.ReadAsByteArrayAsync().Result;

                bool didImageExist = true; // Add code to persist image with given id to DB and return indication whether it existed

                var statusCode = didImageExist ? HttpStatusCode.NoContent : HttpStatusCode.NotFound;
                return new HttpResponseMessage(statusCode);
            }

            return null;
        }
    }

    class PostImageRequestHandler : RequestHandlerChainItem
    {
        public PostImageRequestHandler(IRequestHandler nextChainItem) : base(nextChainItem)
        {
        }

        protected override HttpResponseMessage InnerHandle(HttpRequestMessage request)
        {
            // POST image
            if (request.Method == HttpMethod.Post && RequestPath == "api/images")
            {
                var image = request.Content.ReadAsByteArrayAsync().Result;
                int imageId = 0; // Replace with code to persist image to DB and get back id

                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Headers.Location = new Uri("/api/images/" + imageId);

                return response;
            }

            return null;
        }
    }

    class BadRequestFallbackHandler : RequestHandlerChainItem
    {
        protected override HttpResponseMessage InnerHandle(HttpRequestMessage request)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
    }

    class ChainOfResponsibilityRequestHandlerApp
    {
        static void Main()
        {
            var requestHandler = new GetImageRequestHandler(new PutImageRequestHandler(new PostImageRequestHandler(new BadRequestFallbackHandler())));
            var response = requestHandler.Handle(new HttpRequestMessage(HttpMethod.Get, "api/images/5"));
        }
    }
    #endregion
}
