using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;

namespace ClientCertificateAuth
{
    internal sealed class ClientCertificateMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            HttpRequestData? requestData = await context.GetHttpRequestDataAsync();
            if (requestData == null || IsTargetFunctionAnonymous(context))
            {
                await next(context);
                return;
            }

            requestData.Headers.TryGetValues("X-ARR-ClientCert", out IEnumerable<string>? certificates);
            var certificateValidator = context.InstanceServices.GetService<CertificateValidator>();
            if (!(await certificateValidator!.IsAnyCertificateValid(certificates)))
            {
                var newHttpResponse = requestData.CreateResponse(HttpStatusCode.Unauthorized);
                await newHttpResponse.WriteAsJsonAsync(new { Error = "You are not auhorized." }, newHttpResponse.StatusCode);
                var invocationResult = context.GetInvocationResult();
                invocationResult.Value = newHttpResponse;

                return;
            }

            await next(context);
        }

        private bool IsTargetFunctionAnonymous(FunctionContext context)
        {
            var methodNameStart = context.FunctionDefinition.EntryPoint.LastIndexOf(".");
            var className = context.FunctionDefinition.EntryPoint[..(methodNameStart)];
            var methodName = context.FunctionDefinition.EntryPoint[(methodNameStart + 1)..];
            var classType = Type.GetType(className);
            var isClassAnonymous = classType?.GetCustomAttributes(false).Any(a => a.GetType() == typeof(AnonymousAccessAttribute)) ?? false;

            if (isClassAnonymous)
                return true;

            var methodsWithName = classType?.GetMember(methodName) ?? new MemberInfo[0];
            foreach (var mi in methodsWithName)
            {
                var customAttributes = mi.GetCustomAttributes(false);
                var isMatchingFunctionName = customAttributes.Where(ca => (ca as FunctionAttribute)?.Name == context.FunctionDefinition.Name).Any();
                if (isMatchingFunctionName)
                {
                    var isMethodAnonymous = customAttributes.Any(ca => ca is AnonymousAccessAttribute);
                    if (isMethodAnonymous)
                        return true;
                    else
                        return false;
                }
            }

            return false;
        }
    }
}