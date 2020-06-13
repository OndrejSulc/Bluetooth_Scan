using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    internal class MeasurementProcessing 
    {
        private readonly MeasurementsChannel incomingMeasurementsChannel;
        private readonly EvaluationData sharedEvalDataStorage;
        private readonly CancellationToken ct;

        public MeasurementProcessing(MeasurementsChannel channelWhereMeasurementsAreReadFrom, EvaluationData sharedEvalDataStorageObj, CancellationToken cancellationToken)
        {
            incomingMeasurementsChannel = channelWhereMeasurementsAreReadFrom;
            sharedEvalDataStorage = sharedEvalDataStorageObj;
            ct = cancellationToken;
        }

        public async Task ProcessMeasurementAsync()
        {
            while (true)
            {
                var processedMeasurement = await incomingMeasurementsChannel.PopMeasurementAsync(ct);
                ct.ThrowIfCancellationRequested();

                lock (sharedEvalDataStorage.MeasurementListLock)
                {
                    if (!PlaceIfSameMac(processedMeasurement))
                    {
                        sharedEvalDataStorage.MeasurementEvaluationList.Add(processedMeasurement);
                    }
                }

                Console.WriteLine("measurement processed");
            }
        }

        private bool PlaceIfSameMac(Measurement processedMeasurement)
        {
            var registeredMac = false;

            for (byte i = 0; i < sharedEvalDataStorage.MeasurementEvaluationList.Count; i++)
            {
                if (sharedEvalDataStorage.MeasurementEvaluationList[i].BleMac == processedMeasurement.BleMac)
                {
                    if (sharedEvalDataStorage.MeasurementEvaluationList[i].BleRssi < processedMeasurement.BleRssi)
                    {
                        sharedEvalDataStorage.MeasurementEvaluationList[i] = processedMeasurement;
                    }
                    registeredMac = true;
                    break;
                }
            }

            return registeredMac;
        }
    }
}
