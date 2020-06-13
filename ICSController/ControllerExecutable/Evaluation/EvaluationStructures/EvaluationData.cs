using System.Collections.Generic;

namespace ICSController.Evaluation
{
    internal class EvaluationData
    {
        public List<Measurement> MeasurementEvaluationList = new List<Measurement>();
        public readonly object MeasurementListLock = new object();
    }
}
