using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Data;
using System.Reactive;

namespace ioDeviceEmulator.Server.Repo
{
    public class DeviceModel
    {
        public List<DigitalInput> DigitalInputs { get; private set; }
        public List<Relay> Relays { get; private set; }
        private DeviceModel() {

            DigitalInputs = new List<DigitalInput>();
            Relays = new List<Relay>(); 
        }

        public static DeviceModel E1214()
        {
            DeviceModel deviceState = new DeviceModel();

            deviceState.DigitalInputs = new List<DigitalInput>
            {
                new DigitalInputDI() { Index = 0 },
                new DigitalInputDI() { Index = 1 },
                new DigitalInputDI() { Index = 2 },
                new DigitalInputCounter() { Index = 3 },
                new DigitalInputCounter() { Index = 4 },
                new DigitalInputCounter() { Index = 5 }
            };

            deviceState.Relays = new List<Relay>
            {
                new RelayPulse() { Index = 0 },
                new RelayPulse() { Index = 1 },
                new RelayRelay() { Index = 2 },
                new RelayRelay() { Index = 3 },
                new RelayRelay() { Index = 4 },
                new RelayPulse() { Index = 5 }
            };

            return deviceState;
        }
    }


    public abstract class DigitalInput
    {
        public int Index { get; set; }
        public int Mode { get; protected set; }

        public virtual object ToJsonObject()
        {
            object obj = new object();
            return obj;
        }
    }

    public class DigitalInputDI : DigitalInput
    {
        public DigitalInputDI()
        {
            Mode = 0;
        }

        public int Status { get; set; }

        public override object ToJsonObject()
        {
            jsonDigitalInputDI jObj = new jsonDigitalInputDI();
            jObj.diIndex = Index;
            jObj.diMode = Mode;
            jObj.diStatus = Status;
            return jObj;
        }
    }

    public class jsonDigitalInputDI {
        public int diIndex { get; set; }
        public int diMode { get;  set; }
        public int diStatus { get; set; }
    }

    public class DigitalInputCounter : DigitalInput
    {
        public int CounterValue { get; set; }
        public int CounterReset { get; protected set; }
        public int CounterOverflowFlag { get; set; }
        public int CounterOverflowClear { get; set; }
        public int CounterStatus { get; set; }

        public DigitalInputCounter()
        {
            Mode = 1;
        }

        public override object ToJsonObject()
        {
            jsonDigitalInputCounter jObj = new jsonDigitalInputCounter();
            jObj.diIndex = Index;
            jObj.diMode = Mode;
            jObj.diCounterValue = CounterValue;
            jObj.diCounterStatus = CounterStatus;
            jObj.diCounterReset = CounterReset;
            jObj.diCounterOverflowFlag = CounterOverflowFlag;
            jObj.diCounterOverflowClear = CounterOverflowClear;
            return jObj;
        }
    }

    public class jsonDigitalInputCounter
    {
        public int diIndex { get; set; }
        public int diMode { get; set; }
        public int diCounterValue { get; set; }
        public int diCounterStatus { get; set; }
        public int diCounterReset { get; set; }
        public int diCounterOverflowFlag { get; set; }
        public int diCounterOverflowClear { get; set; }
    }


    public abstract class Relay
    {
        public int Index { get; set; }
        public int Mode { get; protected set; }

        public virtual object ToJsonObject()
        {
            object obj = new object();
            return obj;
        }
    }
    public class RelayRelay : Relay
    {
        public RelayRelay()
        {
            Mode = 0;
        }
         
        public int Status { get; set; }

        public int TotalCount { get; set; }
        public int CurrentCount { get; set; }
        public int CurrentCountReset { get; set; }

        public override object ToJsonObject()
        {
            jsonRelayRelay jObj = new jsonRelayRelay();
            
            jObj.relayIndex = Index;
            jObj.relayMode = Mode;

            jObj.relayStatus = Status;
            
            jObj.relayTotalCount = TotalCount;
            jObj.relayCurrentCount = CurrentCount;
            jObj.relayCurrentCountReset = CurrentCountReset;
            
            return jObj;
        }

    }

    public class RelayPulse : Relay
    {
        public RelayPulse()
        {
            Mode = 1;
        }

         public int PulseStatus       { get; set; }
         public int PulseCount        { get; set; }
         public int PulseOnWidth      { get; set; }
         public int PulseOffWidth { get; set; }

        public int TotalCount { get; set; }
        public int CurrentCount { get; set; }
        public int CurrentCountReset { get; set; }

        public override object ToJsonObject()
        {
            jsonRelayPulse jObj = new jsonRelayPulse();
            
            jObj.relayIndex = Index;
            jObj.relayMode = Mode;
            
            jObj.relayPulseStatus = PulseStatus;
            jObj.relayPulseCount = PulseCount;
            jObj.relayPulseOnWidth = PulseOnWidth;
            jObj.relayPulseOffWidth = PulseOffWidth;

            jObj.relayTotalCount = TotalCount;
            jObj.relayCurrentCount = CurrentCount;
            jObj.relayCurrentCountReset = CurrentCountReset;
            
            return jObj;
        }

    }

    public class jsonRelayRelay
    {
        public int relayIndex { get; set; }
        public int relayMode { get; set; }
        public int relayStatus { get; set; }
        public int relayTotalCount { get; set; }
        public int relayCurrentCount { get; set; }
        public int relayCurrentCountReset { get; set; }
    }

    public class jsonRelayPulse
    {
        public int relayIndex { get; set; }
        public int relayMode { get; set; }
        public int relayPulseStatus { get; set; }
        public int relayPulseCount { get; set; }
        public int relayPulseOnWidth { get; set; }
        public int relayPulseOffWidth { get; set; }
        public int relayTotalCount { get; set; }
        public int relayCurrentCount { get; set; }
        public int relayCurrentCountReset { get; set; }
    }

}
