using ioDeviceEmulator.Server.Repo;
using ioDeviceEmulator.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ioDeviceEmulator.Server.Controllers
{
    [Route("api/slot/0")]
    [ApiController]
    public class MoxaApiController : ControllerBase
    {
        DeviceState _deviceState;
        public MoxaApiController(DeviceState deviceState)
        {
            _deviceState = deviceState;
        }

        [HttpGet]
        [Route("io/di")]
        public string Get()
        {
            var dis = _deviceState.GetDigitalInputs();

            List<dynamic> dynamicDiList = new List<dynamic>();
            foreach (var di in dis)
            {
                dynamicDiList.Add(di.ToJsonObject());
            }

            // Create the parent object
            var jsonObject = new { slot = 0, io = new { di = dynamicDiList } };

            var json = JsonConvert.SerializeObject(jsonObject);

            return json;
        }

    }
}
