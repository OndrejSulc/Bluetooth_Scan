using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.EvaluationNamespace
{
    class Evaluation
    {
        public static async Task StartEvaluationThread()
        {
            Task processTask =  new Task( async () => await MeasurementProcessing.ProcessMeasurement() );
            processTask.Start();
            
       
            Console.WriteLine("Evaluation printing thread started..");
            DateTime EvaluationStart;

            while (true)
            {
                await Task.Delay(Options.EvaluationIntervalMiliseconds);

                EvaluationStart = DateTime.Now;
                Console.WriteLine("Evaluation started at " + EvaluationStart);

                lock (EvaluationData.measurementListLock)
                {
                    if (EvaluationData.measurementEvaluationList.Count == 0)
                    {
                        Console.WriteLine("Evaluation aborted: 0 measurements...");
                        continue;
                    }
                    else
                    {
                        PrintResults();
                        EvaluationData.measurementEvaluationList = new List<Measurement>();
                    }
                }
            }
        }


        private static void PrintResults() 
        {
            Console.WriteLine("Evaluation results at " + DateTime.Now + ":");
            Console.WriteLine("--------------------");
           
            foreach (var measurementInList in EvaluationData.measurementEvaluationList )
            {
                if ((measurementInList.BLE_RSSI > Options.RssiCutoff) || Options.RssiCutoff == 0)
                {
                    Console.WriteLine(measurementInList);
                }
            }

            Console.WriteLine("--------------------");
            Console.WriteLine("evaluation ended");
        }
    }
}
