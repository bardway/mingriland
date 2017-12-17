// CommandHelper.h

#ifndef _COMMANDHELPER_h
#define _COMMANDHELPER_h

#if defined(ARDUINO) && ARDUINO >= 100
#include "arduino.h"
#else
#include "WProgram.h"
#endif

class Command
{
public:
	void AnalysisCommand(char receiveChar);
	String GetKeyCommand();
	String GetValueCommand();
	void Clear();
private:
	String data;
	String key;
	String value;
};

#endif

