using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    internal class EvaluationResultPrinter
    {
        private readonly EvaluationData evaluationData;
        private readonly CancellationToken ct;

        public EvaluationResultPrinter(EvaluationData sharedEvalDataStorageObj, CancellationToken cancellationToken)
        {
            evaluationData = sharedEvalDataStorageObj;
            ct = cancellationToken;
        }

        public async Task StartEvaluationThreadAsync()
        {
            while (true)
            {
                await Task.Delay(Options.EvaluationIntervalMilliseconds,ct);

                var evaluationStart = DateTime.Now;
                Console.WriteLine("Evaluation started at " + evaluationStart);

                lock (evaluationData.MeasurementListLock)
                {
                    if (evaluationData.MeasurementEvaluationList.Count == 0)
                    {
                        Console.WriteLine("Evaluation aborted: 0 measurements...");
                    }
                    else
                    {
                        PrintResults();
                        evaluationData.MeasurementEvaluationList = new List<Measurement>();
                    }
                }
            }
        }

        private void PrintResults() 
        {
            Console.WriteLine("Evaluation results at " + DateTime.Now + ":");
            Console.WriteLine("--------------------");
           
            foreach (var measurementInList in evaluationData.MeasurementEvaluationList)
            {
                if ((measurementInList.BleRssi > Options.RssiCutoff) || Options.RssiCutoff == 0)
                {
                    Console.WriteLine(measurementInList);
                }
            }

            Console.WriteLine("--------------------");
            Console.WriteLine("evaluation ended");
        }
    }
}
