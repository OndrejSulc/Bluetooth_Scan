using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController.EvaluationNamespace
{
    class EvaluationData
    {
        public static List<Measurement> measurementEvaluationList = new List<Measurement>();
        public static readonly object measurementListLock = new object();
    }
}
