using Google.Protobuf.WellKnownTypes;
using System.Data;
using System.Reactive;

namespace ioDeviceEmulator.Server.Repo
{
    public class DeviceModel
    {
        public List<DigitalInput> DigitalInputs { get; private set; }
        public List<Relay> Relays { get; private set; }
        private DeviceModel() {


        }

        public static DeviceModel E1214()
        {
            DeviceModel deviceState = new DeviceModel();

            deviceState.DigitalInputs = new List<DigitalInput>();
            for (int i = 0; i < 6; i++)
                deviceState.DigitalInputs.Add(new DigitalInputDI() { Index = i });

            deviceState.Relays = new List<Relay>();
            for (int i = 0; i < 6; i++)
                deviceState.Relays.Add(new RelayRelay() { Index = i });

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
    }


    public abstract class Relay
    {
        public int Index { get; set; }
        public int Mode { get; protected set; }
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

    }

    public class RelayCounter : Relay
    {
        public RelayCounter()
        {
            Mode = 1;
        }

         public int PulseStatus       { get; set; }
         public int PulseCount        { get; set; }
         public int PulseOnWidth      { get; set; }
         public int PulseOffWidth { get; set; }

    }

}
