// 
// 
// 

#include "CommandHelper.h"
#include "arduino.h"

void Command::AnalysisCommand(char receiveChar)
{
	data += receiveChar;
}

String Command::GetKeyCommand()
{
	int keyLength = data.indexOf('-');
	if (keyLength != -1)
	{
		key = data.substring(0, keyLength);
	}
	return key;
}

String Command::GetValueCommand()
{
	int keyLength = data.indexOf('-');
	int valueLength = data.indexOf('|');
	if (valueLength != -1)
	{
		value = data.substring(keyLength + 1, valueLength);
	}
	return value;
}

void Command::Clear()
{
	key = "";
	value = "";
	data = "";
}