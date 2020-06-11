using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    class EvaluationResultPrinter
    {
        private EvaluationData evaluationData;
        private CancellationToken ct;

        public EvaluationResultPrinter(EvaluationData sharedEvalDataStorageObj, CancellationToken cancelationToken)
        {
            evaluationData = sharedEvalDataStorageObj;
            ct = cancelationToken;
        }

        public async Task StartEvaluationThreadAsync()
        {
            DateTime EvaluationStart;

            while (true)
            {
                await Task.Delay(Options.EvaluationIntervalMiliseconds,ct);

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
