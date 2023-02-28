
using Grpc.Core;
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
        IOEventsStreamService _ioEventsStreamService;



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
            bool opResult = _deviceState.SetInputStatus(index, 1, "Simulator API closed digital input");

            if (opResult)
            {
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
            bool opResuult = _deviceState.SetInputStatus(index, 0, "Simulator API open digital input");

            if (opResuult)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut]
        [Route("relay/close/{index}")]
        public IActionResult CloseRelay(int index)
        {
            // Perform some action to activate the input with the specified index
            bool opResult = _deviceState.SetRelayStatus(index, 1, "Simulator API close relay");

            if (opResult)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPut]
        [Route("relay/open/{index}")]
        public IActionResult OpenRelay(int index)
        {
            // Perform some action to activate the input with the specified index
            bool opResuult = _deviceState.SetRelayStatus(index, 0, "Simulator API open relay");

            if (opResuult)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }


    }
}
