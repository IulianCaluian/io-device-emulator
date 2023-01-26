
using ioDeviceEmulator.Server.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ioDeviceEmulator.Server.Controllers
{
    [Route("api/sim")]
    [ApiController]
    public class MoxaInternalsSimulator : ControllerBase
    {
        DeviceState _deviceState;

        public MoxaInternalsSimulator(DeviceState deviceState)
        {
            _deviceState = deviceState;
        }

        [HttpPut]
        [Route("di/close/{index}")]
        public IActionResult CloseInput(int index)
        {
            // Perform some action to activate the input with the specified index
            bool isActivated = _deviceState.CloseInput(index);

            if (isActivated)
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
            bool isActivated = _deviceState.OpenInput(index);

            if (isActivated)
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
