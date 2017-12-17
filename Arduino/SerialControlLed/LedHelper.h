// LedHelper.h

#ifndef _LEDHELPER_h
#define _LEDHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

class Led
{
public:
	void Init(int sdk, int clk, int load);
	void ShowLed2(char number);
	void ShowLed3(char number);
	void SPI_5953(unsigned char out_data);
private:
	int LOAD;
	int  CLK;
	int  SDK;
};

#endif

