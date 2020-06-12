using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    class Evaluator
    {
        private readonly EvaluationData data = new EvaluationData(); //shared access between tasks
        private readonly MeasurementsChannel incomingMeasurementsChannel;

        private CancellationTokenSource tokenSource;
        private Task measurementProcessingTask;
        private Task evaluationResultPrinterTask;

        public Evaluator(MeasurementsChannel channelFromWhichMeasurementsAreRead)
        {
            incomingMeasurementsChannel = channelFromWhichMeasurementsAreRead;
        }

        public void StartEvaluation()
        {
            tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            EvaluationResultPrinter resultPrinterObj = new EvaluationResultPrinter(data, ct);
            evaluationResultPrinterTask = new Task(async () =>
            {
                try
                {
                    await resultPrinterObj.StartEvaluationThreadAsync();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Evaluation printing task canceled..");
                }
            }, ct);

            MeasurementProcessing measurementProcessingObj = new MeasurementProcessing(incomingMeasurementsChannel, data, ct);
            measurementProcessingTask = new Task(async () =>
            {
                try
                {
                    await measurementProcessingObj.ProcessMeasurementAsync();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Measurement processing task canceled..");
                }
            }, ct);

            measurementProcessingTask.Start();
            Console.WriteLine("Measurement processing task started..");

            evaluationResultPrinterTask.Start();
            Console.WriteLine("Evaluation printing task started..");
        }

        public async Task EndEvaluation()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();

                await measurementProcessingTask;
                await evaluationResultPrinterTask;
            }
        }

        public void DumpData()
        {
            lock (data.measurementListLock)
            {
                data.measurementEvaluationList = new List<Measurement>();
            }
        }

        public List<Measurement> GetMeasurements()
        {
            List<Measurement> returnedDeepCopy = new List<Measurement>();
            lock (data.measurementListLock)
            {
                for (int i = 0; i < data.measurementEvaluationList.Count; i++)
                {
                    returnedDeepCopy.Add(data.measurementEvaluationList[i].CreateDeepCopy());
                }
            }

            return returnedDeepCopy;
        }
    }
}
