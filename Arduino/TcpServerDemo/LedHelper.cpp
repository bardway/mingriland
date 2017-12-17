// 
// 
// 

#include "LedHelper.h"
#include "arduino.h"

unsigned char tableLed[] = { 0xc0,0xcf,0xa4,0xb0,0x99,0x92,0x82, 0xf8,0x80,0x90, 0x88,0x83,0xc6,0xa1,0x86,0x8e,0xff,0x7f };
unsigned char tableLed3[] = { 0x3f,0x30,0x5b,0x4f,0x66,0x6d,0x7d, 0x07,0x7f,0x6f, 0x77,0x7c,0x39,0x5e,0x79,0x71,0x00,0x80 };

void Led::Init(int load, int clk, int sdk)
{
	LOAD = load;
	CLK = clk;
	SDK = sdk;
	pinMode(LOAD, OUTPUT);
	pinMode(CLK, OUTPUT);
	pinMode(SDK, OUTPUT);
}

void Led::ShowLed2(char number)
{
	digitalWrite(LOAD, LOW);
	delay(50);
	SPI_5953(tableLed[number]);
	digitalWrite(LOAD, 1);
	delay(50);
}

void Led::ShowLed3(char number)
{
	digitalWrite(LOAD, LOW);
	delay(50);
	SPI_5953(tableLed3[number]);
	digitalWrite(LOAD, 1);
	delay(50);
}

void Led::SPI_5953(unsigned char out_data)
{
	unsigned char i, temp;
	for (i = 0; i < 8; i++)
	{
		digitalWrite(CLK, LOW);
		temp = out_data & 0x80;
		if (temp == 0x80)
			digitalWrite(SDK, 1);
		else digitalWrite(SDK, 0);
		out_data = out_data << 1;
		digitalWrite(CLK, 1);
	}
}
