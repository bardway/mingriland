// 
// 
// 

#include "CommandHelper.h"
#include "arduino.h"

void Command::AnalysisCommand(char receiveChar)
{
	data += receiveChar;
	//Serial.println(data);
	if (receiveChar == '|')
	{
		int keyLength = data.indexOf('-');
		if (keyLength != -1)
		{
			key = data.substring(0, keyLength);
		}
		int valueLength = data.indexOf('|');
		if (valueLength != -1)
		{
			value = data.substring(keyLength + 1, valueLength);
		}
		data = "";
	}
}

String Command::GetKeyCommand()
{
	return key;
}

String Command::GetValueCommand()
{
	return value;
}

void Command::Clear()
{
	key = "";
	value = "";
}

bool Command::IsClear()
{
	return data == "";
}