using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICSController.EvaluationNamespace
{
    class MeasurementProcessing
    {
        private static Measurement processedMeasurement;

        public static async Task ProcessMeasurement()
        {
            Console.WriteLine("Measurement processing thread started..");

            while (true)
            {
                processedMeasurement = await SavedMeasurements.PopMeasurementAsync();
                lock (EvaluationData.measurementListLock)
                {
                    if (!PlaceIfSameMAC())
                    {
                        EvaluationData.measurementEvaluationList.Add(processedMeasurement);
                    }
                }

                Console.WriteLine("measurement processed");
            }
        }


        private static bool PlaceIfSameMAC()
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
