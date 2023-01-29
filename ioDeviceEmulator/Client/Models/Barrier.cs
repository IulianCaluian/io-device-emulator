using System.ComponentModel;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ioDeviceEmulator.Client.Models
{
    public class Barrier
    {

         private readonly int _autoClosingTime;
 

        // Control:
        public BarrierTerminal Start { get; set; }
        public BarrierTerminal Stop { get; set; }

        /// <summary>
        /// Push button
        /// </summary>
        public BarrierTerminal Start2 { get; set; } 

        // Safety:
        public BarrierTerminal PhotoCell { get; set; }
        public BarrierTerminal LoopDetector { get; set; }


        public Barrier(int autoClosingTimeInMs)
        {
            _autoClosingTime = autoClosingTimeInMs;

            Start = new BarrierTerminal();
            Start.PropertyChanged += Start_PropertyChanged;

            Stop = new BarrierTerminal();
            Start2 = new BarrierTerminal();
            
            PhotoCell = new BarrierTerminal();
            LoopDetector = new BarrierTerminal();
        }

        private void Start_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Activated")
            //{
            //    var myObject = sender as BarrierTerminal;
            //    if (myObject != null)
            //    {
            //        if (myObject.Activated)
            //        {
            //            Task.Run(() => Open());
            //        } 
            //        else
            //        {
            //            Task.Run(() => AutomaticClose());
            //        }
                    
            //    }
            //}
        }



        // Logic:


  


        

    }
}
