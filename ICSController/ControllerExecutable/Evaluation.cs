using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController
{
    class Evaluation
    {
        private static List<Measurement> measurementEvaluationList = new List<Measurement>();
        private static readonly object measurementListLock = new object();
        private static Measurement processedMeasurement;



        /// <summary>
        /// Main function for evaluation thread
        /// </summary>
        public static async Task StartEvaluationThread()
        {
            Task printTask = new Task(StartEvaluationIntervalThread);
            printTask.Start();

            await ProcessMeasurement();
        }


        private static async Task ProcessMeasurement()
        {
            Console.WriteLine("Measurement processing thread started..");

            while (true)
            {
                processedMeasurement = await SavedMeasurements.PopMeasurementAsync();
                lock (measurementListLock)
                {
                    // process measurement if same MAC already occurred
                    if (!PlaceIfSameMAC())
                    {
                        measurementEvaluationList.Add(processedMeasurement);
                    }
                }

                Console.WriteLine("measurement processed");
            }
        }


        /// <summary>
        /// Searches for measurement with same MAC. If it finds same MAC then it compares RSSI and keeps higher one.
        /// </summary>
        /// <returns>True if it found same MAC</returns>
        private static bool PlaceIfSameMAC()
        {
            bool registeredMAC = false;

            for (byte i = 0; i < measurementEvaluationList.Count; i++)
            {
                if (measurementEvaluationList[i].BLE_MAC == processedMeasurement.BLE_MAC)
                {
                    if (measurementEvaluationList[i].BLE_RSSI < processedMeasurement.BLE_RSSI)
                    {
                        measurementEvaluationList[i] = processedMeasurement;
                    }
                    registeredMAC = true;
                    break;
                }
            }

            return registeredMAC;
        }

        
        private static void StartEvaluationIntervalThread()
        {
            Console.WriteLine("Evaluation printing thread started..");
            DateTime EvaluationStart;
            while (true)
            {
                Thread.Sleep(Options.EvaluationIntervalMiliseconds);

                EvaluationStart = DateTime.Now;
                Console.WriteLine("Evaluation started at " + EvaluationStart);

                lock (measurementListLock)
                {
                    if (measurementEvaluationList.Count == 0)
                    {
                        Console.WriteLine("Evaluation aborted: 0 measurements...");
                        continue;
                    }
                    else
                    {
                        PrintResults();
                        measurementEvaluationList = new List<Measurement>();
                    }
                }
            }
        }


        /// <summary>
        /// Prints time from evaluationBegin and data stored in MeasurementEvaluationList.
        /// </summary>
        /// <param name="evaluationBegin"></param>
        private static void PrintResults() 
        {
            Console.WriteLine("Evaluation results at " + DateTime.Now + ":");
            Console.WriteLine("--------------------");
           
            foreach (var measurementInList in measurementEvaluationList )
            {
                if ( (measurementInList.BLE_RSSI > Options.RssiCutoff) || Options.RssiCutoff == 0)
                    measurementInList.PrintToConsole();
            }

            Console.WriteLine("--------------------");
            Console.WriteLine("evaluation ended");
        }
    }
}
