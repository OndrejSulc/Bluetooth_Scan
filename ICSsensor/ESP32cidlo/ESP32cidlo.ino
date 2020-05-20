#include <BLEDevice.h>

#include <SPI.h>
#include <WiFi.h>
#include <PubSubClient.h>

 
//// ENTER NETWORK INFO HERE ////////////

const char* networkSSID = "";
const char* networkPW = "";

const char* mqttServer = "";
const int mqttPort = 1883;

const char* mqttID = "testTopic/ESP01"; // topic/sensorID
const char* mqttUser = "";
const char* mqttPW = "";

//////////////////////////////////////////

WiFiClient espClient;
PubSubClient mqttClient(espClient);

void blinkLED(int count, int delaytime)
{
  for (int i=0;i<count;i++)
  {
    digitalWrite(2, HIGH); 
    delay(delaytime);           
    digitalWrite(2, LOW);  
    delay(delaytime); 
  }
}

void setupBluetooth()
{
  BLEDevice::init("");
  Serial.println("Bluetooth device initialized");
}


void setupWifi()
{
  WiFi.begin(networkSSID, networkPW);
 
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.println("Connecting to WiFi..");
    blinkLED(3,1000);
  }
  
  Serial.println("Connected to the WiFi network");
}


void setupMQTTclient()
{
  mqttClient.setServer(mqttServer, mqttPort);   

  Serial.println("Connecting to MQTT server");
  bool mqttConnection = 0;
  while(!mqttConnection)
  {
    mqttConnection = mqttClient.connect(mqttID, mqttUser, mqttPW);
    if (mqttConnection) 
    {
      Serial.println("Connected to the MQTT server");
    } 
    else 
    {
      Serial.println("Connection to the MQTT server failed.. new attempt");
      blinkLED(3,1000);
    }
  } 
}


void setup() 
{
  Serial.begin(115200);

  pinMode(2, OUTPUT);    // sets the digital pin 2 as output
  
  blinkLED(2,500);
  
  
  setupBluetooth();

  blinkLED(2,500);

  setupWifi();

  blinkLED(2,500);
  
  setupMQTTclient();

  blinkLED(2,500);

  Serial.println("Sensor is ready");
 
}

void loop()
{
  digitalWrite(2, LOW);
  // This is needed at the top of the loop!
  mqttClient.loop();

  // Ensure that we are subscribed to the topic "testTopic"
  mqttClient.subscribe(mqttID); 

  
  Serial.println("\n------------- BEGIN -------------------\n");
 
  BLEScan *scan = BLEDevice::getScan();
  scan->setActiveScan(true);
  BLEScanResults results = scan->start(1);

  for (int i = 0; i < results.getCount(); i++)
  {
    
    BLEAdvertisedDevice device = results.getDevice(i);

    int rssi = device.getRSSI();

    BLEAddress address = device.getAddress();
    std::string addSTR = address.toString();

    char mqttMessage[40];

  
    strcpy(mqttMessage,device.getName().c_str());
    strcat(mqttMessage,";");
    
    strcat(mqttMessage,addSTR.c_str()); 
    strcat(mqttMessage,";");

    char rssiStr[3];
    itoa(rssi,rssiStr,10);
    
    strcat(mqttMessage,rssiStr);   
    strcat(mqttMessage,";");

    // Attempt to publish a value to the topic
    if(mqttClient.publish( mqttID, mqttMessage))
    {
      Serial.print("Published: ");
      Serial.println(mqttMessage);
    }
    else
    {
      Serial.println("Could not send message!");
    }
  
    // Dont overload the server!
    delay(1000);
  }
  
  delay(3000);
  Serial.println("\n------------- END -------------------\n");
}
