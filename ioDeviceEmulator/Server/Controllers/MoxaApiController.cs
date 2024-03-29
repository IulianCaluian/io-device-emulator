﻿using Grpc.Core;
using ioDeviceEmulator.Server.GrpcServices;
using ioDeviceEmulator.Server.Repo;
using ioDeviceEmulator.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace ioDeviceEmulator.Server.Controllers
{
    [Route("api/slot/0/io")]
    [ApiController]
    public class MoxaApiController : ControllerBase
    {
        DeviceState _deviceState;

        public MoxaApiController(DeviceState deviceState, IOEventsStreamService ioEventsStreamService)
        {
            _deviceState = deviceState;
        }

        [HttpGet]
        [Route("di")]
        public string GetDigitalInputs()
        {
            var dis = _deviceState.GetDigitalInputs();

            List<dynamic> dynamicDiList = new List<dynamic>();
            foreach (DigitalInput di in dis)
            {
                dynamicDiList.Add(di.ToJsonObject());
            }

            // Create the parent object
            var jsonObject = new { slot = 0, io = new { di = dynamicDiList } };

            var json = JsonConvert.SerializeObject(jsonObject);

            return json;
        }

        [HttpGet("relay")]
        public string GetRelays()
        {
            var relays = _deviceState.GetRelays();
            List<dynamic> dynamicRelsList = new List<dynamic>();
            foreach (Relay di in relays)
            {
                dynamicRelsList.Add(di.ToJsonObject());
            }

            // Create the parent object
            var jsonObject = new { slot = 0, io = new { relay = dynamicRelsList } };

            var json = JsonConvert.SerializeObject(jsonObject);

            return json;
        }

        [HttpPut]
        [Route("relay/{index}/relayStatus")]
        public IActionResult ChangeStatusRelay(int index, [FromBody] JObject json)
        {
            /// Reference: Accepting Raw Request Body Content in ASP.NET Core API Controllers

            // Perform some action to activate the input with the specified index
            dynamic data;
            try
            {
               data = json;
               // data = JsonConvert.DeserializeObject(json);
            } catch(Exception ex) {
                return BadRequest(GetJsonError("The json format in the request is invalid.", 201));
            }

            if (data?.io == null)
            {
                return BadRequest(GetJsonError("One of the node value is invalid.", 202));
            }

            if (data.io.relay == null)
            {
                return BadRequest(GetJsonError("A required relay node was not specified in the request body.", 203));
            }

            if (data.io.relay[$"{index}"] == null)
            {
                return BadRequest(GetJsonError("A required channel index was not specified in the request body.", 204));
            }

            if (data.io.relay[$"{index}"].relayStatus == null)
            {
                return BadRequest(GetJsonError("A required node was not specified in the request body.", 206));
            }

            int relayStatus;

            try
            {
                relayStatus = data.io.relay[$"{index}"].relayStatus;
                if (relayStatus < 0 || relayStatus > 1)
                    throw new Exception();
            }catch(Exception ex)
            {
                return BadRequest(GetJsonError("One of the channel content could not be set.", 300));
            }

            bool op = UpdateRelayStatus(index, relayStatus);

            if (op) {

                return Ok();
            }
            else
            {
                return BadRequest(GetJsonError("Updating relay status failed.", 301));
            }
         

        }

        [HttpPut("relay")]
        public IActionResult ChangeRelays([FromBody] JObject json)
        {
            /// Reference: Accepting Raw Request Body Content in ASP.NET Core API Controllers

            // Perform some action to activate the input with the specified index
            dynamic data;
            try
            {
                data = json;
                // data = JsonConvert.DeserializeObject(json);
            }
            catch (Exception ex)
            {
                return BadRequest(GetJsonError("The json format in the request is invalid.", 201));
            }

            if (data?.io == null)
            {
                return BadRequest(GetJsonError("One of the node value is invalid.", 202));
            }

            if (data.io.relay == null)
            {
                return BadRequest(GetJsonError("A required relay node was not specified in the request body.", 203));
            }

            // Other validations...


            IEnumerable<restRelayChannel> listRelay = new List<restRelayChannel>();

            JToken? diTok = data.io.relay;

            var parsedList = diTok?.ToObject<IEnumerable<restRelayChannel>>();

            if (parsedList != null)
                listRelay = parsedList;


            bool op = UpdateRelays(listRelay);

            if (op)
            {
                return Ok();
            }
            else
            {
                return BadRequest(GetJsonError("Updating relay status failed.", 301));
            }

        }

        private bool UpdateRelayStatus(int index, int status)
        {
            return _deviceState.SetRelayStatus(index, status, "");
        }

        private bool UpdateRelays(IEnumerable<restRelayChannel> restRelayList)
        {
            List<Relay> listRels = new List<Relay>();

            foreach (var restRelay in restRelayList)
            {
                if (restRelay.relayMode == 0)
                {
                    listRels.Add( restRelay.GetRelayRelay());

                } 
                else
                {
                    listRels.Add(restRelay.GetRelayPulse());
                }
            }


            //TODO extract groups

            return _deviceState.UpdateRelays(listRels);
        }


        private string GetJsonError(string messageStr, int codeInt)
        {
            var jsonObject = new { error = new { message = messageStr, code = codeInt } };
            var jsonString = JsonConvert.SerializeObject(jsonObject);
            return jsonString;
        }
    }

    public class restRelayChannel
    {
        public uint relayIndex { get; set; }
        public uint relayMode { get; set; }
        public uint relayStatus { get; set; }
        public uint relayTotalCount { get; set; }
        public uint relayCurrentCount { get; set; }
        public uint relayCurrentCountReset { get; set; }
        public uint relayPulseStatus { get; set; }
        public uint relayPulseCount { get; set; }
        public uint relayPulseOnWidth { get; set; }
        public uint relayPulseOffWidth { get; set; }

        internal RelayRelay GetRelayRelay()
        {
            return new RelayRelay()
            {
                Index = (int)relayIndex,

                Status = (int)relayStatus,
                TotalCount = (int)relayTotalCount,
                CurrentCount = (int)relayCurrentCount,
                CurrentCountReset = (int)relayCurrentCountReset,
            };
        }

        internal RelayPulse GetRelayPulse()
        {
            return new RelayPulse()
            {
                Index = (int)relayIndex,

                TotalCount = (int)relayTotalCount,
                CurrentCount = (int)relayCurrentCount,
                CurrentCountReset = (int)relayCurrentCountReset,

                PulseStatus = (int)relayPulseStatus,
                PulseCount = (int)relayPulseCount,
                PulseOnWidth = (int)relayPulseOnWidth,
                PulseOffWidth = (int)relayPulseOffWidth
            };
        }

        //public RelayChannel GetDomainModel()
        //{
        //    RelayChannel dic = new RelayChannel();
        //    dic.Index = relayIndex;
        //    dic.Mode = relayMode;
        //    dic.Status = relayStatus;
        //    dic.TotalCount = relayTotalCount;
        //    dic.CurrentCount = relayCurrentCount;
        //    dic.CurrentCountReset = relayCurrentCountReset;
        //    dic.PulseStatus = relayPulseStatus;
        //    dic.PulseCount = relayPulseCount;
        //    dic.PulseOnWidth = relayPulseOnWidth;
        //    dic.PulseOffWidth = relayPulseOffWidth;
        //    return dic;

        //}
    }
}
