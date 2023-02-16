using Microsoft.AspNetCore.Mvc;
using RetSettlementDates.Api.Responses;
using RetSettlementDates.Domain.Agregators;
using RetSettlementDates.Domain.Indexes;
using System;
using System.Globalization;
using System.Linq;

namespace RetSettlementDates.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettlementsController : ControllerBase
    {
        [HttpGet("from/{start}/to/{end}")]
        public IActionResult GetByDates(
            [FromRoute]string start, [FromRoute] string end, 
            [FromQuery]string? location, 
            [FromServices]ILocationDateIndex index)
        {
            var isValid = DateTime.TryParseExact(start, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime cleanStart)
                & DateTime.TryParseExact(end, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime cleanEnd);

            if (!isValid)
                return BadRequest("The dates should be yyyy-MM-dd");

            var response = index.Query(location, cleanStart, cleanEnd);

            return Ok(response.Select(s => new SettlementResponse(s)));
        }

        [HttpGet("group/year")]
        public IActionResult GetGroupedByYear(
            [FromQuery] string? location,
            [FromServices] IYearMonthAggregator index)
        {
            var response = index.QueryByYear(location);
            return Ok(response.Select(s => new GroupSettlementResponse(s.location, s.year)));
        }

        [HttpGet("group/month")]
        public IActionResult GetGroupedByMonth(
            [FromQuery] string? location,
            [FromServices] IYearMonthAggregator index)
        {
            var response = index.QueryByMonth(location);
            return Ok(response.Select(s => new GroupSettlementResponse(s.location, s.month)));
        }
    }
}
