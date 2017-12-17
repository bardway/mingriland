#include "LedHelper.h"
#include "CommandHelper.h"
#include <SPI.h>
#include <Ethernet.h>

byte _mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress _ip(192, 168, 3, 20);
IPAddress _dns(192, 168, 3, 1);
IPAddress _gateway(192, 168, 3, 1);
IPAddress _subnet(255, 255, 255, 0);
EthernetServer server(8888);
Command command;

Led led2;
Led led3;


int reset = 0;
int ledCount = 0;

unsigned char ledValues[] = { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };

unsigned char ledValues2[] = { 0,0 };

void setup()
{
	led2.Init(7,6,5);
	led3.Init(10,9,8);

	Ethernet.begin(_mac, _ip, _dns, _gateway, _subnet);
	server.begin();

	Serial.begin(9600);
	while (!Serial) {
		; // wait for serial port to connect. Needed for native USB port only
	}
	/*Serial.print("Chat server address:");
	Serial.println(Ethernet.localIP());*/

	ShowLed();
}

void loop()
{
	/*int result = Ethernet.maintain();
	Serial.println(result);*/
	EthernetClient client = server.available();
	if (client) {
		if (client.available() > 0) {
			char thisChar = client.read();
			command.AnalysisCommand(thisChar);
			String key = command.GetKeyCommand();
			String value = command.GetValueCommand();
			if (key == "Test")
			{
				/*Serial.println(key);
				Serial.println(value);*/
				client.write("true");
				command.Clear();
			}
			else if (key == "Led")
			{
				/*  Serial.println(key);
				Serial.println(value);*/
				client.write("true");
				command.Clear();
				for (int i = 0; i < 2; i++)
				{
					ledValues2[i] = ToInt32(value[i]);
				}
				ShowLed();
			}
		}
	}
}

int ToInt32(char c)
{
	String convert;
	convert.concat(c);
	int one = convert.toInt();
	return one;
}

void ShowLed()
{
	for (int i = 0; i < 2; i++)
	{
		led2.ShowLed2(ledValues2[i]);
	}
	/*for (int i = 0; i < 6; i++)
	{
	led3.ShowLed3(ledValues[i]);
	}*/
}

