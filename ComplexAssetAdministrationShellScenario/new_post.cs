using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ComplexAssetAdministrationShellScenario
{
    public class MaintenanceData
    {
        public string ConversationId { get; set; } = string.Empty;
        public string MessageId { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public int MaintenanceThreshold { get; set; }
        public DateTime ActualMaintenanceStart { get; set; }
        public DateTime ActualMaintenanceEnd { get; set; }
        public int MaintenanceDuration { get; set; }
    }

    public class MesAasPostHandler
    {
        public async Task HandlePostRequest(HttpContext context, DataStorage dataStorage)
        {
            try
            {
                // Read the request body
                string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

                // Check if the request body is not null or empty
                if (string.IsNullOrEmpty(requestBody))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Request body is null or empty.");
                    return;
                }

                // Deserialize the JSON data to a C# object
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                var requestData = JsonSerializer.Deserialize<MaintenanceData>(requestBody, options);

                // Check for null or empty strings in the request data
                if (string.IsNullOrEmpty(requestData.ConversationId) ||
                    string.IsNullOrEmpty(requestData.MessageId) ||
                    string.IsNullOrEmpty(requestData.MachineName))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("ConversationId, MessageId, and MachineName cannot be null or empty.");
                    return;
                }

                // Process the request data (add your custom logic here)
                // For example, you can access individual properties like:
                string conversationId = requestData.ConversationId;
                string messageId = requestData.MessageId;
                // 

                // Serialize the processed data back to JSON
                string responseData = JsonSerializer.Serialize(requestData);

                // Set the response content type to application/json
                context.Response.ContentType = "application/json";

                // Set the response status code to 200 OK
                context.Response.StatusCode = StatusCodes.Status200OK;

                // Write the JSON data as the response body
                await context.Response.WriteAsync("Your POST request has been processed successfully and we received this data from your side: " + responseData);
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
