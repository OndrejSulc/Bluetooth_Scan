using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController.Evaluation
{
    class EvaluationData
    {
        public List<Measurement> measurementEvaluationList = new List<Measurement>();
        public object measurementListLock = new object();
    }
}
