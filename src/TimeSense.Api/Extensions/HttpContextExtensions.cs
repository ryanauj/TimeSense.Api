using System;
using System.Security.Authentication;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TimeSense.Api.Extensions
{
    public static class HttpContextExtensions
    {
        private const string MockCognitoIdentityId = "MOCK_COGNITO_IDENTITY_ID";

        public static string GetUserId(this HttpContext httpContext, ILogger logger = null)
        {
            var lambdaRequest = httpContext.Items[AbstractAspNetCoreFunction.LAMBDA_REQUEST_OBJECT] as APIGatewayProxyRequest;
            
            var lambdaRequestCognitoId = lambdaRequest?.RequestContext?.Identity?.CognitoIdentityId;
            if (!string.IsNullOrWhiteSpace(lambdaRequestCognitoId))
            {
                Log(logger, $"Using lambda request cognito id: '{lambdaRequestCognitoId}'");
                return lambdaRequestCognitoId;
            }
            
            if (lambdaRequest?.Headers?.ContainsKey(MockCognitoIdentityId) ?? false)
            {
                var lambdaRequestMockCognitoId = lambdaRequest.Headers[MockCognitoIdentityId];
                Log(logger, $"Using lambda request mock cognito id: '{lambdaRequestMockCognitoId}'");
                return lambdaRequestMockCognitoId;
            }
            
            if (httpContext?.Request?.Headers?.ContainsKey(MockCognitoIdentityId) ?? false)
            {
                var mockCognitoId = httpContext.Request.Headers[MockCognitoIdentityId];
                Log(logger, $"Using headers mock cognito id: '{mockCognitoId}'");
                return mockCognitoId;
            }

            throw new AuthenticationException("UserId not found in request!");
        }

        private static void Log(ILogger logger, string message)
        {
            if (logger != null) logger.LogInformation(message);
            else Console.WriteLine(message);
        }
    }
}