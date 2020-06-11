using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    class MeasurementProcessing 
    {
        private readonly MeasurementsChannel incomingMeasurementsChannel;
        private readonly EvaluationData sharedEvalDataStorage;
        private CancellationToken ct;


        public MeasurementProcessing(MeasurementsChannel channelWhereMeasurementsAreReadFrom, EvaluationData sharedEvalDataStorageObj, CancellationToken cancelationToken)
        {
            incomingMeasurementsChannel = channelWhereMeasurementsAreReadFrom;
            sharedEvalDataStorage = sharedEvalDataStorageObj;
            ct = cancelationToken;
        }


        public async Task ProcessMeasurementAsync()
        {
            
            Measurement processedMeasurement;

            while (true)
            {
                processedMeasurement = await incomingMeasurementsChannel.PopMeasurementAsync(ct);
                ct.ThrowIfCancellationRequested();

                lock (sharedEvalDataStorage.measurementListLock)
                {
                    if (!PlaceIfSameMAC(processedMeasurement))
                    {
                        sharedEvalDataStorage.measurementEvaluationList.Add(processedMeasurement);
                    }
                }

                Console.WriteLine("measurement processed");
            }
        }


        private bool PlaceIfSameMAC(Measurement processedMeasurement)
        {
            bool registeredMAC = false;

            for (byte i = 0; i < sharedEvalDataStorage.measurementEvaluationList.Count; i++)
            {
                if (sharedEvalDataStorage.measurementEvaluationList[i].BLE_MAC == processedMeasurement.BLE_MAC)
                {
                    if (sharedEvalDataStorage.measurementEvaluationList[i].BLE_RSSI < processedMeasurement.BLE_RSSI)
                    {
                        sharedEvalDataStorage.measurementEvaluationList[i] = processedMeasurement;
                    }
                    registeredMAC = true;
                    break;
                }
            }

            return registeredMAC;
        }
    }
}
