using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ICSController
{
    class Evaluation
    {
        private static List<Measurement> MeasurementEvaluationList;
        private static Measurement processed;

        /// <summary>
        /// Main function for evaluation thread
        /// </summary>
        public static void EvaluationThread()
        {
            while (true)
            {
                WaitForEvalWindow();

                MeasurementEvaluationList = new List<Measurement>();
                DateTime evaluationBegin = DateTime.Now;

                while (true)
                {
                    Console.WriteLine("processing");
                    // this is here in case, there won´t come any messages during evaluation
                    if (Program.ReceivedMeasurements.Count == 0)
                    {
                        break;
                    }

                    // take first measurement and process it on free lock
                    lock (Program.RMlock)
                    {
                        processed = Program.ReceivedMeasurements[0];
                    }

                    // if processed measurement came after evaluation begun, then ignore, break and print evaluation results 
                    if (DateTime.Compare(evaluationBegin, processed.Time) < 0)
                    {
                        break;
                    }

                    // process measurement if same MAC already occurred
                    if ( !PlaceIfSameMAC() )
                    {
                        //add new item in list on new MAC
                        MeasurementEvaluationList.Add(processed);
                    }

                    //remove first measurement from list after processing on free lock
                    lock (Program.RMlock)
                    {
                        Program.ReceivedMeasurements.Remove(processed);
                    }
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
            for (byte i = 0; i < MeasurementEvaluationList.Count; i++)
            {
                if (MeasurementEvaluationList[i].BLE_MAC == processed.BLE_MAC)
                {
                    if (MeasurementEvaluationList[i].BLE_RSSI < processed.BLE_RSSI)
                    {
                        //replace measurement if higher RSSI
                        MeasurementEvaluationList[i] = processed;
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
                Console.WriteLine("evaluation began with " + Program.ReceivedMeasurements.Count + " received messages");

                if (Program.ReceivedMeasurements.Count == 0)
                {
                    Console.WriteLine("evaluation aborted");
                    continue;
                }
                else
                {
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
            //print evaluation results
            Console.WriteLine("Evaluation results at " + evaluationBegin + ":");
            Console.WriteLine("--------------------");

            for (byte i = 0; i < MeasurementEvaluationList.Count; i++)
            {
                if (MeasurementEvaluationList[i].BLE_RSSI < Program.RssiCutoff || Program.RssiCutoff == 0)
                {
                    MeasurementEvaluationList[i].ConsolePrint();
                }

            }
            Console.WriteLine("--------------------");
            Console.WriteLine("evaluation ended");

        }
    }
}
