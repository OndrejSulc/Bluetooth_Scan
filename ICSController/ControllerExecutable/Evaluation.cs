using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ICSController
{
    class Evaluation
    {
        private static List<Measurement> measurementEvaluationList;
        private static Measurement processedMeasurement;

        /// <summary>
        /// Main function for evaluation thread
        /// </summary>
        public static void EvaluationThread()
        {
            while (true)
            {
                WaitForEvalWindow();

                measurementEvaluationList = new List<Measurement>();
                DateTime evaluationBegin = DateTime.Now;

                while (true)
                {
                    processedMeasurement = SavedMeasurements.PopMeasurement();

                    // this is here in case, there won´t come any messages during evaluation
                    if ( processedMeasurement == null )
                        break;
                    
                    // if processed measurement came after evaluation begun, then ignore, break and print evaluation results 
                    if ( DateTime.Compare( evaluationBegin, processedMeasurement.Time ) < 0)
                        break;
                    
                    // process measurement if same MAC already occurred
                    if ( !PlaceIfSameMAC() )
                        measurementEvaluationList.Add( processedMeasurement );
                }

                PrintResults(evaluationBegin);
            }
        }


        /// <summary>
        /// Searches for measurement with same MAC. If it finds same MAC then it compares RSSI and keeps higher one.
        /// </summary>
        /// <returns>True if it found same MAC</returns>
        private static bool PlaceIfSameMAC()
        {
            bool registeredMAC = false;

            //search for same MAC in processed measurements
            for (byte i = 0; i < measurementEvaluationList.Count; i++)
            {
                if (measurementEvaluationList[i].BLE_MAC == processedMeasurement.BLE_MAC)
                {
                    if (measurementEvaluationList[i].BLE_RSSI < processedMeasurement.BLE_RSSI)
                    {
                        //replace measurement if higher RSSI
                        measurementEvaluationList[i] = processedMeasurement;
                    }
                    registeredMAC = true;
                    break;
                }
            }

            return registeredMAC;
        }


        /// <summary>
        /// Thread waits given time. Then checks count of received measurements and eventually releases loop to continue in main
        /// </summary>
        private static void WaitForEvalWindow()
        {
            while (true)
            {
                //wait for next evaluation window
                Thread.Sleep(Program.EvaluationIntervalMiliseconds);

                Console.WriteLine("evaluation began with " + SavedMeasurements.GetCountOfMeasurements() + " received messages");

                if ( SavedMeasurements.IsEmpty() )
                {
                    Console.WriteLine("evaluation aborted");
                    continue;
                }
                else
                {
                    //break loop and get back to main function
                    break;
                }
            }
            
        }


        /// <summary>
        /// Prints time from evaluationBegin and data stored in MeasurementEvaluationList.
        /// </summary>
        /// <param name="evaluationBegin"></param>
        private static void PrintResults(DateTime evaluationBegin) 
        {
            Console.WriteLine("Evaluation results at " + evaluationBegin + ":");
            Console.WriteLine("--------------------");
        
            foreach (var measurementInList in measurementEvaluationList )
            {
                if ( (measurementInList.BLE_RSSI > Program.RssiCutoff) || Program.RssiCutoff == 0)
                    measurementInList.ConsolePrint();
                   
            }

            Console.WriteLine("--------------------");
            Console.WriteLine("evaluation ended");

        }
    }
}
