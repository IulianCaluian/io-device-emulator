
using ioDeviceEmulator.Server.GrpcServices;
using ioDeviceEmulator.Server.Repo;
using ioDeviceEmulator.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ioDeviceEmulator.Server.Controllers
{
    [Route("api/sim")]
    [ApiController]
    public class MoxaInternalsSimulator : ControllerBase
    {
        DeviceState _deviceState;
        private readonly IOEventsStreamService _ioEventsStreamService;

        public MoxaInternalsSimulator(DeviceState deviceState, IOEventsStreamService ioEventsStreamService)
        {
            _deviceState = deviceState;
            _ioEventsStreamService = ioEventsStreamService;
        }

        [HttpPut]
        [Route("di/close/{index}")]
        public IActionResult CloseInput(int index)
        {
            // Perform some action to activate the input with the specified index
            bool opResult = _deviceState.CloseInput(index);

            if (opResult)
            {
                _ioEventsStreamService.EventSubject.OnNext(new Models.IOEvent()
                {
                    EventDate = DateTime.Now,
                    IOType = ioElementType.DigitalInput,
                    Index = index,
                    Status = 1,
                    Summary = "Simulator API close digital input"
                });

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPut]
        [Route("di/open/{index}")]
        public IActionResult OpenInput(int index)
        {
            // Perform some action to activate the input with the specified index
            bool opResuult = _deviceState.OpenInput(index);

            if (opResuult)
            {
                _ioEventsStreamService.EventSubject.OnNext(new Models.IOEvent()
                {
                     EventDate = DateTime.Now,
                     IOType = ioElementType.DigitalInput,
                     Index = index,
                     Status = 0,
                     Summary = "Simulator API open digital input"
                });

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }


    }
}
