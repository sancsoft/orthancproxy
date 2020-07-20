
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OrthancProxy.Controllers
{
    [ApiController]
    // {*path} allows us to capture all routes.
    [Route("orthanc/{*path}")]
    public class OrthancController : ControllerBase
    {
        private readonly ILogger<OrthancController> _logger;
        
        public OrthancController(ILogger<OrthancController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAsync(string path)
        {
            try 
            {
                // Get the base url to which we are acting as a proxy for.
                var baseUrl = Request.Headers["BaseUrl"].FirstOrDefault();
                if(baseUrl == null) 
                {
                    return BadRequest("BaseUrl header is required");
                }

                // Create the HttpClient to make our request.
                HttpClient httpClient = new HttpClient();

                // Copy the Basic Auth and add it to our request.
                var auth = Request.Headers["Authorization"].FirstOrDefault();
                if(auth != null) 
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", auth);
                }
                else 
                {
                    // Maybe the target endpoint is unsecured?
                }

                // Set the BaseAddress for our request.
                httpClient.BaseAddress = new Uri(baseUrl);

                // Add any query params to the path.
                string pathAndQuery = path + Request.QueryString;

                // Make the GET request.
                var httpResponse = await httpClient.GetAsync(pathAndQuery);

                // Forward the response.
                switch(httpResponse.StatusCode) 
                {
                    case HttpStatusCode.OK:
                        // TODO: Find a better way to do this.
                        // Need to know when we are asking for a file, and in turn, pass along the appropriate content type.
                        if(path.Contains("file")) {
                            return File((await httpResponse.Content.ReadAsStreamAsync()), "application/dicom");
                        }
                        return Ok(await httpResponse.Content.ReadAsStringAsync());
                    case HttpStatusCode.Unauthorized:
                        return Unauthorized();
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    default:
                        return BadRequest("Unhandled status code: " + (int)httpResponse.StatusCode);
                }
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
