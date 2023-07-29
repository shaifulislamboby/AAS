using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ComplexAssetAdministrationShellScenario
{
    public class MesAasPostHandler
    {
        public async Task HandlePostRequest(HttpContext context)
        {
            try
            {
                // Read the request body
                string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

                // Deserialize the JSON data to a C# object
                var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);

                // Process the request data (add your custom logic here)
                // ... Add your custom logic here ...

                // Serialize the processed data back to JSON
                string responseData = JsonSerializer.Serialize(requestData);
                
                // Set the response content type to application/json
                context.Response.ContentType = "application/json";

                // Set the response status code to 200 OK
                context.Response.StatusCode = StatusCodes.Status200OK;

                // Write the JSON data as the response body
                await context.Response.WriteAsync("POST request has been processed successfully.");
            }
            catch (JsonException)
            {
                // Handling JSON parse errors
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid JSON data in the request body.");
            }
            catch (Exception ex)
            {
                // Handling other exceptions
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"An error occurred: {ex.Message}");
            }
        }
    }
}