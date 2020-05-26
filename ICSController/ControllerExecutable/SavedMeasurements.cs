using System;
using System.Collections.Generic;
using System.Text;

namespace ICSController
{
    class SavedMeasurements
    {
        private static List<Measurement> receivedMeasurements = new List<Measurement>();
        private static object rmLock = new object();


        /// <summary>
        /// Add new Measurement object into private list
        /// </summary>
        /// <param name="newMeasurement">Measurement object to be added to private list</param>
        /// <returns>true</returns>
        public static bool AddMeasurement(Measurement newMeasurement)
        {
            lock (rmLock)
            {
                receivedMeasurements.Add(newMeasurement);
            }

            return true;
        }


        /// <summary>
        /// Similar to SavedMeasurements.AddMeasurement() except return value is count of measurements in list (including newly added one) 
        /// </summary>
        /// <param name="newMeasurement"></param>
        /// <returns>Count of measurements</returns>
        public static int AddMeasurementReturnCount(Measurement newMeasurement)
        {
            bool returnVal = AddMeasurement(newMeasurement);

            if (returnVal)
                return receivedMeasurements.Count;
            

            return -1;
        }


        /// <summary>
        /// Returns oldest measurement object and deletes it from private list
        /// in case list is empty, method return null
        /// </summary>
        /// <returns>Oldest measurement or null</returns>
        public static Measurement? PopMeasurement() 
        {
            if (receivedMeasurements.Count > 0)
            {
                Measurement returnedMeasurement;

                lock (rmLock)
                {
                    returnedMeasurement = receivedMeasurements[0];
                    receivedMeasurements.RemoveAt(0);
                }

                return returnedMeasurement;
            }
            
            return null;
        }


        /// <summary>
        /// Returns count of measurements in private list
        /// </summary>
        /// <returns></returns>
        public static int GetCountOfMeasurements() 
        {
            return receivedMeasurements.Count;
        }


        /// <summary>
        /// Return true if private list is empty
        /// </summary>
        /// <returns></returns>
        public static bool IsEmpty()
        {
            return receivedMeasurements.Count == 0 ? true : false ;
        }

    }
}
