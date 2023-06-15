namespace FlowinglyTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{

    private readonly ILogger<EmailController> _logger;

    public EmailController(ILogger<EmailController> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// Extract Xml data from email content
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("extract")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(long), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReadMailContent(IFormFile formFile) 
    {
        var emailContent = await formFile.ReadAsStringAsync();
        var result =await Utils.BuildXml(emailContent);

        if (result.GetElementsByTagName("total").Count < 1)
        {
            return BadRequest(new ApiResponse()
            {
                statuscode = StatusCodes.Status400BadRequest,
                data = "Invalid Xml"
            });
        }
        else
        {
            var xDocument = XDocument.Parse(result.OuterXml);
            var total = xDocument.Elements("Expenses").Elements("expense")
                .Elements("total")
                .Select(exp => exp.Value)
                .FirstOrDefault();

            var costCentre = xDocument.Elements("Expenses").Elements("expense")
                .Elements("cost_centre")
                .Select(exp => exp.Value)
                .FirstOrDefault();

            var exclTaxTotal=Utils.CalculateTotalExclTax(total);

            var taxResponse = new TaxResponse()
            {
                costcentre = string.IsNullOrEmpty(costCentre) ? "UNKNOWN" : costCentre,
                total_excludingtax = exclTaxTotal,
                salestax = Convert.ToString(Utils.salesTax),
                total = total
            };

            return Ok(new ApiResponse()
            {
                statuscode = StatusCodes.Status200OK,
                data = taxResponse
            });
        }   
    }
}
