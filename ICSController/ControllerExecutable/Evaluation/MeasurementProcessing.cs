using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    class MeasurementProcessing 
    {
        private readonly MeasurementsChannel incomingMeasurementsChannel;
        private readonly EvaluationData sharedEvalDataStorage;

        public MeasurementProcessing(MeasurementsChannel channelWhereMeasurementsAreReadFrom, EvaluationData sharedEvalDataStorageObj)
        {
            incomingMeasurementsChannel = channelWhereMeasurementsAreReadFrom;
            sharedEvalDataStorage = sharedEvalDataStorageObj;
        }


        public async Task ProcessMeasurementAsync()
        {
            Console.WriteLine("Measurement processing thread started..");
            Measurement processedMeasurement;

            while (true)
            {
                processedMeasurement = await incomingMeasurementsChannel.PopMeasurementAsync();
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
