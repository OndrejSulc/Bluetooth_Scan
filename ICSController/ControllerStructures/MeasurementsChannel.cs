using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ICSController
{
    class MeasurementsChannel
    {
        public Channel<Measurement> receivedMeasurementsChannel= Channel.CreateUnbounded<Measurement>();
              
        public bool AddMeasurement(Measurement newMeasurement)
        {
            return receivedMeasurementsChannel.Writer.TryWrite(newMeasurement);
        }


        public async Task<Measurement> PopMeasurementAsync() 
        {
            while (await receivedMeasurementsChannel.Reader.WaitToReadAsync())
            {
                if (receivedMeasurementsChannel.Reader.TryRead(out var msg))
                {
                    return msg;
                }
            }
            
            throw new System.MissingFieldException();
        }


        public async Task<Measurement> PopMeasurementAsync(CancellationToken ct)
        {
            while (await receivedMeasurementsChannel.Reader.WaitToReadAsync(ct))
            {
                if (receivedMeasurementsChannel.Reader.TryRead(out var msg))
                {
                    return msg;
                }
            }

            throw new System.MissingFieldException();
        }
    }
}
