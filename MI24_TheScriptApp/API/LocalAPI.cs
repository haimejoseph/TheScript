using MI24_TheScriptApp.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MI24_TheScriptApp.API
{
    public class LocalAPI
    {
        private readonly string baseAddress = "http://localhost:5000";
        private readonly HttpListener httpListener;

        public LocalAPI()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(baseAddress + "/");
        }

        public async Task StartAsync()
        {
            httpListener.Start();
            await AcceptRequestsAsync();
        }

        public void Stop()
        {
            httpListener.Stop();
            httpListener.Close();
        }

        private async Task AcceptRequestsAsync()
        {
            while (true)
            {
                try
                {
                    HttpListenerContext context = await httpListener.GetContextAsync();
                    _ = HandleRequestAsync(context); // Avoid using 'await' on HandleRequestAsync directly to avoid blocking the loop.
                }
                catch (HttpListenerException)
                {
                    // HttpListenerException is thrown when httpListener is stopped or closed.
                    break;
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions that might occur during accepting requests.
                    Console.WriteLine($"Error accepting request: {ex.Message}");
                }
            }
        }

        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod == "POST" && context.Request.Url.AbsolutePath == "/api/data")
                {
                    string jsonData;
                    using (var reader = new StreamReader(context.Request.InputStream))
                    {
                        jsonData = await reader.ReadToEndAsync();
                    }

                    await HandlePostRequestAsync(jsonData);

                    // Send a success response back to the caller.
                    byte[] responseBytes = Encoding.UTF8.GetBytes("Data received successfully!");
                    context.Response.StatusCode = 200; // Status code for success.
                    context.Response.ContentType = "text/plain";
                    context.Response.ContentLength64 = responseBytes.Length;
                    await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
                else
                {
                    // If the request is not a valid POST to the specified endpoint, return a 404 response.
                    context.Response.StatusCode = 404; // Status code for not found.
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs during data handling, send an error response.
                byte[] errorBytes = Encoding.UTF8.GetBytes("Error occurred while processing the request: " + ex.Message);
                context.Response.StatusCode = 500; // Status code for internal server error.
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength64 = errorBytes.Length;
                await context.Response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
            }
            finally
            {
                // Close the response stream to indicate the response is complete.
                context.Response.Close();
            }
        }

        private async Task HandlePostRequestAsync(string jsonData)
        {
            try
            {
                // Process the data received from the Windows service here.
                var data = JsonConvert.DeserializeObject<SqlTransaction>(jsonData);
                MessageBox.Show($"POST Request recieved!");
                // Do something with the data (e.g., display on the form, save to a file, etc.).
                // TODO
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during data processing.
                MessageBox.Show($"An error occurred while handling the POST request: {ex.Message}");
            }
        }
    }
}
