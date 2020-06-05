using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICSController.EvaluationNamespace
{
    class MeasurementProcessing 
    {
        public static async Task ProcessMeasurement()
        {
            Console.WriteLine("Measurement processing thread started..");
            Measurement processedMeasurement;

            while (true)
            {
                processedMeasurement = await SavedMeasurements.PopMeasurementAsync();
                lock (EvaluationData.measurementListLock)
                {
                    if (!PlaceIfSameMAC(processedMeasurement))
                    {
                        EvaluationData.measurementEvaluationList.Add(processedMeasurement);
                    }
                }

                Console.WriteLine("measurement processed");
            }
        }


        private static bool PlaceIfSameMAC(Measurement processedMeasurement)
        {
            bool registeredMAC = false;

            for (byte i = 0; i < EvaluationData.measurementEvaluationList.Count; i++)
            {
                if (EvaluationData.measurementEvaluationList[i].BLE_MAC == processedMeasurement.BLE_MAC)
                {
                    if (EvaluationData.measurementEvaluationList[i].BLE_RSSI < processedMeasurement.BLE_RSSI)
                    {
                        EvaluationData.measurementEvaluationList[i] = processedMeasurement;
                    }
                    registeredMAC = true;
                    break;
                }
            }

            return registeredMAC;
        }
    }
}
