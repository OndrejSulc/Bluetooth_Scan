using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ICSController
{
    class SavedMeasurements
    {
        public static Channel<Measurement> receivedMeasurementsChannel = Channel.CreateUnbounded<Measurement>();

        public static bool AddMeasurement(Measurement newMeasurement)
        {
            return receivedMeasurementsChannel.Writer.TryWrite(newMeasurement);
            
        }


        public static async Task<Measurement> PopMeasurementAsync() 
        {
            while (await receivedMeasurementsChannel.Reader.WaitToReadAsync())
            {
                if (receivedMeasurementsChannel.Reader.TryRead(out var msg))
                    return msg;
            }
            
            
            throw new System.MissingFieldException();
        }
    }
}
