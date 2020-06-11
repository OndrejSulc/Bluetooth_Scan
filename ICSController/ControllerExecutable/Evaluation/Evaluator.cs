using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICSController.Evaluation
{
    class Evaluator
    {
        private readonly EvaluationData data = new EvaluationData();
        private readonly Task measurementProcessingTask;
        private readonly Task evaluationResultPrinterTask;


        public Evaluator(MeasurementsChannel channelFromWhichMeasurementsAreRead)
        {
            EvaluationResultPrinter resultPrinterObj = new EvaluationResultPrinter(data);
            evaluationResultPrinterTask = new Task( async() => await resultPrinterObj.StartEvaluationThreadAsync() );

            MeasurementProcessing measurementProcessingObj = new MeasurementProcessing(channelFromWhichMeasurementsAreRead, data);
            measurementProcessingTask = new Task(async () => await measurementProcessingObj.ProcessMeasurementAsync());
        }


        public void StartEvaluation()
        {
            measurementProcessingTask.Start();
            evaluationResultPrinterTask.Start();
        }

               
        public List<Measurement> GetMeasurements()
        {
            lock (data.measurementListLock)
            {
                return data.measurementEvaluationList;
            }
        }
    }
}
