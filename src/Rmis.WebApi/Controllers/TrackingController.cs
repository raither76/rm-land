using System;
using Microsoft.AspNetCore.Mvc;
using Rmis.Application;
using Rmis.Application.Abstract;

namespace Rmis.WebApi.Controllers
{
    [Route("[controller]")]
    public class TrackingController : ControllerBase
    {
        private readonly ITrackingService _trackingService;

        public TrackingController(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }
        
        [HttpGet("/TrackInfo")]
        public IActionResult GetTrackInfo(string trainNumber)
        {
            try
            {
                TrackInfoDto trackInfo = _trackingService.GetLastTrackInfo(trainNumber);
                if (trackInfo == null)
                    return this.NotFound("Данные о текущем положении поезда отстутствуют");
                
                return this.Ok(trackInfo);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
        
        [HttpPost("/TrackInfo")]
        public IActionResult SaveTrackInfo([FromBody] TrackInfoDto trackInfo)
        {
            try
            {
                _trackingService.SaveTrackInfo(trackInfo);
                
                return this.Ok("Данные о местоположении поезда успешно обработаны");
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}