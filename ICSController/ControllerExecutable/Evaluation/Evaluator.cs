using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    class Evaluator
    {
        private EvaluationData evaluationData = new EvaluationData(); //measurements are filled asynchronously by measurementProcessingTask
        private Task measurementProcessingTask;

        public Evaluator(MeasurementsChannel channelFromWhichMeasurementsAreRead)
        {
            MeasurementProcessing measurementProcessingObj = new MeasurementProcessing(channelFromWhichMeasurementsAreRead, evaluationData);
            measurementProcessingTask=  new Task( async () => await measurementProcessingObj.ProcessMeasurementAsync() );
        }


        public async Task StartEvaluationThreadAsync()
        {
            measurementProcessingTask.Start();
            
            Console.WriteLine("Evaluation printing thread started..");
            DateTime EvaluationStart;

            while (true)
            {
                await Task.Delay(Options.EvaluationIntervalMiliseconds);

                EvaluationStart = DateTime.Now;
                Console.WriteLine("Evaluation started at " + EvaluationStart);

                lock (evaluationData.measurementListLock)
                {
                    if (evaluationData.measurementEvaluationList.Count == 0)
                    {
                        Console.WriteLine("Evaluation aborted: 0 measurements...");
                        continue;
                    }
                    else
                    {
                        PrintResults();
                        evaluationData.measurementEvaluationList = new List<Measurement>();
                    }
                }
            }
        }


        private void PrintResults() 
        {
            Console.WriteLine("Evaluation results at " + DateTime.Now + ":");
            Console.WriteLine("--------------------");
           
            foreach (var measurementInList in evaluationData.measurementEvaluationList)
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
