using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableTopInc.API.Public.Functions.Utils
{
    public class Swagger
    {
        private static ExpandoObject _cache;
        
        private const string SwaggerFunctionName = "Swagger";

        [FunctionName(SwaggerFunctionName)]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequestMessage req,
            ILogger log)
        {
            var assembly = Assembly.GetExecutingAssembly();

            dynamic doc = new ExpandoObject();
            if (_cache == null)
            {
                doc.swagger = "2.0";
                doc.info = new ExpandoObject();
                doc.info.title = assembly.GetName().Name;
                doc.info.version = "1.0.0";
                doc.host = req.RequestUri.Authority;
                doc.basePath = "/";
                doc.schemes = new[] {"https"};
                if (doc.host.Contains("127.0.0.1") || doc.host.Contains("localhost"))
                {
                    doc.schemes = new[] {"http"};
                }

                doc.definitions = new ExpandoObject();
                doc.paths = GeneratePaths(assembly, doc);
                doc.securityDefinitions = GenerateSecurityDefinitions();

                _cache = doc;
            }
            else
            {
                doc = _cache;
            }

            return await Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(doc, new JsonMediaTypeFormatter())
            });
        }

        private static dynamic GenerateSecurityDefinitions()
        {
            dynamic securityDefinitions = new ExpandoObject();
            securityDefinitions.apikeyQuery = new ExpandoObject();
            securityDefinitions.apikeyQuery.type = "apiKey";
            securityDefinitions.apikeyQuery.name = "code";
            securityDefinitions.apikeyQuery.@in = "query";

            // Microsoft Flow import doesn't like two apiKey options, so we leave one out.

            //securityDefinitions.apikeyHeader = new ExpandoObject();
            //securityDefinitions.apikeyHeader.type = "apiKey";
            //securityDefinitions.apikeyHeader.name = "x-functions-key";
            //securityDefinitions.apikeyHeader.@in = "header";
            return securityDefinitions;
        }

        private static dynamic GeneratePaths(Assembly assembly, dynamic doc)
        {
            dynamic paths = new ExpandoObject();
            var methods = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(FunctionNameAttribute), false).Length > 0)
                .ToArray();
            foreach (var methodInfo in methods)
            {
                var route = "/api/";

                var functionAttr = (FunctionNameAttribute)methodInfo.GetCustomAttributes(typeof(FunctionNameAttribute), false)
                    .Single();

                if (functionAttr.Name == SwaggerFunctionName) continue;

                HttpTriggerAttribute triggerAttribute = null;
                foreach (var parameter in methodInfo.GetParameters())
                {
                    triggerAttribute = parameter.GetCustomAttributes(typeof(HttpTriggerAttribute), false)
                        .FirstOrDefault() as HttpTriggerAttribute;
                    if (triggerAttribute != null) break;
                }
                
                if (triggerAttribute == null)
                {
                    continue; // Trigger attribute is required in an Azure function
                }

                if (!string.IsNullOrWhiteSpace(triggerAttribute.Route))
                {
                    route += triggerAttribute.Route;
                }
                else
                {
                    route += functionAttr.Name;
                }

                dynamic path = new ExpandoObject();

                var verbs = triggerAttribute.Methods ?? new[] { "get", "post", "delete", "head", "patch", "put", "options" };
                foreach (var verb in verbs)
                {
                    dynamic operation = new ExpandoObject();
                    operation.operationId = ToTitleCase(functionAttr.Name) + ToTitleCase(verb);
                    operation.produces = new[] { "application/json" };
                    operation.consumes = new[] { "application/json" };
                    operation.parameters = GenerateFunctionParametersSignature(methodInfo, route, doc);

                    // Summary is title
                    operation.summary = GetFunctionName(methodInfo, functionAttr.Name);
                    // Verbose description
                    operation.description = GetFunctionDescription(methodInfo, functionAttr.Name);

                    operation.responses = GenerateResponseParameterSignature(methodInfo, doc);
                    dynamic keyQuery = new ExpandoObject();
                    keyQuery.apikeyQuery = new string[0];
                    operation.security = new ExpandoObject[] { keyQuery };

                    // Microsoft Flow import doesn't like two apiKey options, so we leave one out.
                    //dynamic apikeyHeader = new ExpandoObject();
                    //apikeyHeader.apikeyHeader = new string[0];
                    //operation.security = new ExpandoObject[] { keyQuery, apikeyHeader };

                    AddToExpando(path, verb, operation);
                }
                AddToExpando(paths, route, path);
            }
            return paths;
        }

        private static string GetFunctionDescription(MethodInfo methodInfo, string funcName)
        {
            var displayAttr = (DisplayAttribute)methodInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault();
            return !string.IsNullOrWhiteSpace(displayAttr?.Description) ? displayAttr.Description : $"This function will run {funcName}";
        }

        /// <summary>
        /// Max 80 characters in summary/title
        /// </summary>
        private static string GetFunctionName(MethodInfo methodInfo, string funcName)
        {
            var displayAttr = (DisplayAttribute)methodInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault();
            if (!string.IsNullOrWhiteSpace(displayAttr?.Name))
            {
                return displayAttr.Name.Length > 80 ? displayAttr.Name.Substring(0, 80) : displayAttr.Name;
            }
            return $"Run {funcName}";
        }

        private static string GetPropertyDescription(PropertyInfo propertyInfo)
        {
            var displayAttr = (DisplayAttribute)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault();
            return !string.IsNullOrWhiteSpace(displayAttr?.Description) ? displayAttr.Description : $"This returns {propertyInfo.PropertyType.Name}";
        }

        private static dynamic GenerateResponseParameterSignature(MethodInfo methodInfo, dynamic doc)
        {
            dynamic responses = new ExpandoObject();

            var responseTypeAttrs = (IEnumerable<ProducesResponseTypeAttribute>)methodInfo.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false);
            if (responseTypeAttrs.Any())
                responseTypeAttrs = responseTypeAttrs.Concat(new[] { (ProducesResponseTypeAttribute)null });

            foreach (var responseTypeAttr in responseTypeAttrs)
            {
                dynamic responseDef = new ExpandoObject();
                responseDef.description = "OK";
                var responseCode = 200;
                var returnType = methodInfo.ReturnType;

                if (responseTypeAttr != null)
                {
                    responseCode = responseTypeAttr.StatusCode;
                }
                if (returnType.IsGenericType)
                {
                    var genericReturnType = returnType.GetGenericArguments().FirstOrDefault();
                    if (genericReturnType != null)
                    {
                        returnType = genericReturnType;
                    }
                }
                if (returnType == typeof(HttpResponseMessage))
                {
                    returnType = responseTypeAttr == null ? typeof(void) : responseTypeAttr.Type;
                }
                if (returnType != typeof(void))
                {
                    responseDef.schema = new ExpandoObject();

                    if (returnType.Namespace == "System")
                    {
                        // Warning:
                        // Although valid, it's always better to wrap single values in an object
                        // Returning { Value = "foo" } is better than just "foo"
                        SetParameterType(returnType, responseDef.schema, null);
                    }
                    else
                    {
                        var name = returnType.Name;
                        if (returnType.IsGenericType)
                        {
                            var realType = returnType.GetGenericArguments()[0];
                            if (realType.Namespace == "System")
                            {
                                var inlineSchema = GetObjectSchemaDefinition(null, returnType);
                                responseDef.schema = inlineSchema;
                            }
                            else
                            {
                                AddDefinition(doc, responseDef.schema, returnType, name);
                            }
                        }
                        else if (returnType.IsArray)
                        {
                            AddToExpando(responseDef.schema, "type", "array");
                            responseDef.schema.items = new ExpandoObject();
                            AddDefinition(doc, responseDef.schema.items, returnType, returnType.GetElementType().Name);
                        }
                        else
                        {
                            AddDefinition(doc, responseDef.schema, returnType, name);
                        }
                    }
                }
                AddToExpando(responses, $"{responseCode}", responseDef);
            }

            return responses;
        }

        private static void AddDefinition(dynamic doc, dynamic responseDefNode, Type returnType, string name)
        {
            AddToExpando(responseDefNode, "$ref", "#/definitions/" + name);
            AddParameterDefinition((IDictionary<string, object>)doc.definitions, returnType);
        }

        private static List<object> GenerateFunctionParametersSignature(string route, dynamic doc)
        {
            return GenerateFunctionParametersSignature(null, route, doc);
        }

        private static List<object> GenerateFunctionParametersSignature(MethodInfo methodInfo, string route, dynamic doc)
        {
            var parameterSignatures = new List<object>();
            foreach (var parameter in methodInfo.GetParameters())
            {
                if (parameter.ParameterType == typeof(HttpRequestMessage)) continue;
                if (parameter.ParameterType == typeof(HttpRequest)) continue;
                if (parameter.ParameterType == typeof(ExecutionContext)) continue;
                if (parameter.ParameterType == typeof(TraceWriter)) continue;
                if (parameter.ParameterType == typeof(ILogger)) continue;
                if (parameter.ParameterType == typeof(CloudTable)) continue;

                var hasUriAttribute = parameter.GetCustomAttributes().Any(attr => attr is FromUriAttribute);


                if (route.Contains('{' + parameter.Name))
                {
                    dynamic opParam = new ExpandoObject();
                    opParam.name = parameter.Name;
                    opParam.@in = "path";
                    opParam.required = true;
                    SetParameterType(parameter.ParameterType, opParam, null);
                    parameterSignatures.Add(opParam);
                }
                else if (hasUriAttribute && parameter.ParameterType.Namespace == "System")
                {
                    dynamic opParam = new ExpandoObject();
                    opParam.name = parameter.Name;
                    opParam.@in = "query";
                    opParam.required = parameter.GetCustomAttributes().Any(attr => attr is RequiredAttribute);
                    SetParameterType(parameter.ParameterType, opParam, doc.definitions);
                    parameterSignatures.Add(opParam);
                }
                else if (hasUriAttribute && parameter.ParameterType.Namespace != "System")
                {
                    AddObjectProperties(parameter.ParameterType, "", parameterSignatures, doc);
                }
                else
                {
                    dynamic opParam = new ExpandoObject();
                    opParam.name = parameter.Name;
                    opParam.@in = "body";
                    opParam.required = true;
                    opParam.schema = new ExpandoObject();
                    if (parameter.ParameterType.Namespace == "System")
                    {
                        SetParameterType(parameter.ParameterType, opParam.schema, null);
                    }
                    else
                    {
                        AddToExpando(opParam.schema, "$ref", "#/definitions/" + parameter.ParameterType.Name);
                        AddParameterDefinition((IDictionary<string, object>)doc.definitions, parameter.ParameterType);
                    }
                    parameterSignatures.Add(opParam);
                }
            }
            return parameterSignatures;
        }

        private static void AddObjectProperties(Type t, string parentName, List<object> parameterSignatures, dynamic doc)
        {
            var publicProperties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in publicProperties)
            {
                if (!string.IsNullOrWhiteSpace(parentName))
                {
                    parentName += ".";
                }
                if (property.PropertyType.Namespace != "System")
                {
                    AddObjectProperties(property.PropertyType, parentName + property.Name, parameterSignatures, doc);
                }
                else
                {
                    dynamic opParam = new ExpandoObject();

                    opParam.name = parentName + property.Name;
                    opParam.@in = "query";
                    opParam.required = property.GetCustomAttributes().Any(attr => attr is RequiredAttribute);
                    opParam.description = GetPropertyDescription(property);
                    SetParameterType(property.PropertyType, opParam, doc.definitions);
                    parameterSignatures.Add(opParam);
                }
            }
        }

        private static void AddParameterDefinition(IDictionary<string, object> definitions, Type parameterType)
        {
            if (!definitions.TryGetValue(parameterType.Name, out var objDef))
            {
                objDef = GetObjectSchemaDefinition(definitions, parameterType);
                definitions.Add(parameterType.Name, objDef);
            }
        }

        private static dynamic GetObjectSchemaDefinition(IDictionary<string, object> definitions, Type parameterType)
        {
            dynamic objDef = new ExpandoObject();
            objDef.type = "object";
            objDef.properties = new ExpandoObject();
            var publicProperties = parameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var requiredProperties = new List<string>();
            foreach (var property in publicProperties)
            {
                if (property.GetCustomAttributes().Any(attr => attr is RequiredAttribute))
                {
                    requiredProperties.Add(property.Name);
                }
                dynamic propDef = new ExpandoObject();
                propDef.description = GetPropertyDescription(property);
                SetParameterType(property.PropertyType, propDef, definitions);
                AddToExpando(objDef.properties, property.Name, propDef);
            }
            if (requiredProperties.Count > 0)
            {
                objDef.required = requiredProperties;
            }
            return objDef;
        }

        private static void SetParameterType(Type parameterType, dynamic opParam, dynamic definitions)
        {
            var inputType = parameterType;

            var setObject = opParam;
            if (inputType.IsArray)
            {
                opParam.type = "array";
                opParam.items = new ExpandoObject();
                setObject = opParam.items;
                parameterType = parameterType.GetElementType();
            }
            else if (inputType.IsGenericType && !inputType.FullName.Contains("Nullable"))
            {
                opParam.type = "array";
                opParam.items = new ExpandoObject();
                setObject = opParam.items;
                parameterType = parameterType.GetGenericArguments()[0];
            }

            if (inputType.Namespace == "System" || (inputType.IsGenericType && inputType.GetGenericArguments()[0].Namespace == "System"))
            {
                setObject.type = null;
                var type = Type.GetTypeCode(inputType);
                
                SetType(type, setObject);

                if (setObject.type == null && inputType.IsGenericType)
                {
                    type = Type.GetTypeCode(inputType.GetGenericArguments()[0]);
                    SetType(type, setObject);
                }

                if (setObject.type == null)
                {
                    setObject.type = "string";
                }
            }
            else if (inputType.IsEnum)
            {
                opParam.type = "string";
                opParam.@enum = Enum.GetNames(inputType);
            }
            else if (definitions != null)
            {
                AddToExpando(setObject, "$ref", "#/definitions/" + parameterType.Name);
                AddParameterDefinition((IDictionary<string, object>)definitions, parameterType);
            }
        }

        private static void SetType(TypeCode type, dynamic setObject)
        {
            switch (type)
            {
                case TypeCode.Int32:
                    setObject.format = "int32";
                    setObject.type = "integer";
                    break;
                case TypeCode.Int64:
                    setObject.format = "int64";
                    setObject.type = "integer";
                    break;
                case TypeCode.Single:
                    setObject.format = "float";
                    setObject.type = "number";
                    break;
                case TypeCode.Double:
                    setObject.format = "double";
                    setObject.type = "number";
                    break;
                case TypeCode.String:
                    setObject.type = "string";
                    break;
                case TypeCode.Byte:
                    setObject.format = "byte";
                    setObject.type = "string";
                    break;
                case TypeCode.Boolean:
                    setObject.type = "boolean";
                    break;
                case TypeCode.DateTime:
                    setObject.format = "date";
                    setObject.type = "string";
                    break;
            }
        }

        private static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
        }

        private static void AddToExpando(ExpandoObject obj, string name, object value)
        {
            if (((IDictionary<string, object>)obj).ContainsKey(name))
            {
                // Fix for functions with same routes but different verbs
                var existing = (IDictionary<string, object>)((IDictionary<string, object>)obj)[name];
                var append = (IDictionary<string, object>)value;
                foreach (var keyValuePair in append)
                {
                    existing.Add(keyValuePair);
                }
            }
            else
            {
                ((IDictionary<string, object>)obj).Add(name, value);
            }
        }
    }
}