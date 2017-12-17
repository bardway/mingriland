#include <SPI.h>         
#include <Ethernet.h>
#include <EthernetUdp.h> 

byte mac[] = {
	0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED
};
IPAddress ip(192, 168, 3, 177);
unsigned int localPort = 8888;

EthernetUDP Udp;



void setup()
{
	pinMode(6, INPUT);

	Ethernet.begin(mac, ip);
	Udp.begin(localPort);

	Serial.begin(9600);
}

void loop()
{
	int buttonState1 = digitalRead(6);
	if (buttonState1 == HIGH)
	{
		Serial.println("zheng");
		Udp.beginPacket("192.168.3.2", localPort);
		Udp.write("citie-value|");
		Udp.endPacket();
	}
	delay(50);
}
