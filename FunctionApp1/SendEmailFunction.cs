using FunctionApp1.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

public class SendEmailFunction
{
    private readonly IEmailService _emailService;

    public SendEmailFunction(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [Function("SendEmail")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var queryParams = req.Url.Query;
        var email = System.Web.HttpUtility.ParseQueryString(queryParams).Get("mail");

        if (string.IsNullOrEmpty(email))
        {
            var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync("Please pass a valid email in the query string.");
            return badRequestResponse;
        }

        try
        {
            var subject = "Email Notification";
            var body = $"Hello {email}, This message is from Azure Function.";

            await _emailService.SendEmailAsync(email, subject, body);

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync($"Email sent to {email}.");
            return response;
        }
        catch (Exception ex)
        {
            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Error sending email.");
            return errorResponse;
        }
    }
}
