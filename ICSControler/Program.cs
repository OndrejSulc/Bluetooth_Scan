using System;
using System.Text;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections.Generic;
using System.Threading;

namespace ICSControler
{
    class Program
    {
        public static List<Measurement> RecievedMeasurements = new List<Measurement>();
        private static readonly object RMlock = new object();

        static void Main(string[] args)
        {
            //setup connection to mqtt broker
            var client = new MqttClient("192.168.1.15");

            client.MqttMsgPublishReceived += MeasurementRecieved;

            var clientId = Guid.NewGuid().ToString();
            client.Connect(clientId,"luni","1641999");

            client.Subscribe(
                new string[] { "testTopic" },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
            //



            //continue to evaluation loop
            EvaluationThread();
        }


        static void MeasurementRecieved(object sender, MqttMsgPublishEventArgs e)
        {
            byte operationCounter = 0;

            string msgSensor = "";
            string msgName = "";
            string msgMAC = "";
            string msgRSSI = "";

            bool correct = true;

            // process measurement data
            for (int i = 0; i < e.Message.Length; i++)
            {
                char character = ((char)e.Message[i]);

                if (character == ';')
                {
                    operationCounter += 1;
                }
                else 
                {
                    if (operationCounter == 0)
                    {
                        msgSensor = msgSensor + character;
                    }
                    else if (operationCounter == 1)
                    {
                        msgName = msgName + character;
                    }
                    else if (operationCounter == 2)
                    {
                        msgMAC = msgMAC + character;
                    }
                    else if (operationCounter == 3)
                    {
                        msgRSSI = msgRSSI + character;
                    }
                    else 
                    {
                        Console.WriteLine("measurement recieve failed.. ending process");
                        correct = false;
                    }
                }
            }

            //add to list if processing was successful
            if (correct)
            {
                //create new measurement object
                Measurement newMeasurement = new Measurement { Time = DateTime.Now, Sensor = msgSensor, Name = msgName, MAC = msgMAC, RSSI = (sbyte)int.Parse(msgRSSI) };

                //check print
                Console.WriteLine("\nrecieved measurement");
                newMeasurement.ConsolePrint();

                //assign to list on free lock
                lock (RMlock)
                {
                    RecievedMeasurements.Add(newMeasurement);
                    Console.WriteLine("Count of recieved: " + RecievedMeasurements.Count);
                }
                
            }
        }

        static void EvaluationThread()
        {
            List<Measurement> MeasurementEvaluationList;

            while (true) 
            {
                //wait for next evaluation window
                Thread.Sleep(7_000);
                Console.WriteLine("evaluation begins in 3 seconds");
                Thread.Sleep(3_000);
                Console.WriteLine("evaluation began with "+ RecievedMeasurements.Count + " recieved messages");

                if (RecievedMeasurements.Count == 0) 
                {
                    Console.WriteLine("evaluation aborted");
                    continue;
                }

                MeasurementEvaluationList = new List<Measurement>();
                DateTime evaluationBegin = DateTime.Now;
                Measurement processed;

                while ( true )
                {
                    if (RecievedMeasurements.Count == 0)
                    {
                        break;
                    }
                    //take first measurement and process it on free lock
                    lock (RMlock)
                    {
                        processed = RecievedMeasurements[0];
                    }

                    // if processed measurement came after evalueation begun then ignore, break and print evaluation results 
                    if (DateTime.Compare(evaluationBegin, processed.Time) < 0) 
                    {
                        break;
                    }

                    bool notRegisteredMAC = true;

                    //search for same MAC in processed measurements
                    for (byte i = 0; i < MeasurementEvaluationList.Count; i++)
                    {
                        if (MeasurementEvaluationList[i].MAC == processed.MAC) 
                        {
                            if (MeasurementEvaluationList[i].RSSI < processed.RSSI)
                            {
                                //replace measurement if better RSSI
                                MeasurementEvaluationList[i] = processed;
                            }
                            notRegisteredMAC = false;
                            break;
                        }
                    }

                    if (notRegisteredMAC) 
                    {
                        //add new item in list on new MAC
                        MeasurementEvaluationList.Add(processed);
                    }

                    //remove first measurement from list after processing on free lock
                    lock (RMlock)
                    {
                        RecievedMeasurements.Remove(processed);
                    }
                }


                //print evaluation results
                Console.WriteLine("Evaluation results at " + evaluationBegin + ":");
                Console.WriteLine("--------------------");

                for (byte i = 0; i < MeasurementEvaluationList.Count; i++)
                {
                    MeasurementEvaluationList[i].ConsolePrint();
                }
                Console.WriteLine("--------------------");
                Console.WriteLine("evaluation ended");

            }
        }
    }
}

