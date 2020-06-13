using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    internal class Evaluator
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
            var ct = tokenSource.Token;

            var resultPrinterObj = new EvaluationResultPrinter(data, ct);
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

            var measurementProcessingObj = new MeasurementProcessing(incomingMeasurementsChannel, data, ct);
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
            lock (data.MeasurementListLock)
            {
                data.MeasurementEvaluationList = new List<Measurement>();
            }
        }

        public List<Measurement> GetMeasurements()
        {
            var returnedDeepCopy = new List<Measurement>();
            lock (data.MeasurementListLock)
            {
                foreach (var measurement in data.MeasurementEvaluationList)
                {
                    returnedDeepCopy.Add(measurement.CreateDeepCopy());
                }
            }

            return returnedDeepCopy;
        }
    }
}
