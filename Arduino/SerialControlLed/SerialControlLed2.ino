#include "CommandHelper.h"
#include "LedHelper.h"
#include "SoftwareSerial.h"

Led led1;
Led led2;
Led led3;
Led led4;

Command command;

SoftwareSerial mySerial(1, 0);

void setup()
{
	led1.Init(2, 3, 4);
	led2.Init(5, 6, 7);
	led3.Init(8, 9, 10);
	led4.Init(11, 12, 13);

	

	Serial.begin(9600, SERIAL_8N1);
	while (!Serial) {
		; // wait for serial port to connect. Needed for native USB port only
	}

	mySerial.begin(9600);

	ShowLed3(0, 4, 0, 0);
	ShowLed3(1, 5, 0, 0);
	ShowLed3(2, 6, 0, 0);
	ShowLed3(3, 1, 2, 3);
}

void loop()
{
	if (mySerial.available())
	{
		char inChar = (char)mySerial.read();
		if (inChar != '\n')
		{
			command.AnalysisCommand(inChar);
			if (inChar == '|')
			{
				String key = command.GetKeyCommand();
				String value = command.GetValueCommand();
				if (key == "LedControl")
				{
					//Serial.println("key:" + key);
					int mode = ToInt32(value[0]);
					if (mode == 1)
					{
						int index = ToInt32(value[1]);
						if (index == 3)
							ShowLed3(index, ToInt32(value[2]), ToInt32(value[3]), ToInt32(value[4]));
						else
							ShowLed3(index, ToInt32(value[2]), 0, 0);
					}
				}
				command.Clear();
			}
		}
	}

}

void serialEvent()
{
	while (Serial.available())
	{
		char inChar = (char)Serial.read();
		if (inChar != '\n')
		{
			command.AnalysisCommand(inChar);
			if (inChar == '|')
			{
				String key = command.GetKeyCommand();
				String value = command.GetValueCommand();
				if (key == "LedControl")
				{
					//Serial.println("key:" + key);
					int mode = ToInt32(value[0]);
					if (mode == 1)
					{
						int index = ToInt32(value[1]);
						if (index == 3)
							ShowLed3(index, ToInt32(value[2]), ToInt32(value[3]), ToInt32(value[4]));
						else
							ShowLed3(index, ToInt32(value[2]), 0, 0);
					}
				}
				command.Clear();
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

void ShowLed3(int index, char number1, char number2, char number3)
{
	switch (index)
	{
	case 0:
		led1.ShowLed3(number1);
		break;
	case 1:
		led2.ShowLed3(number1);
		break;
	case 2:
		led3.ShowLed3(number1);
		break;
	case 3:
		led4.ShowLed3(number1);
		led4.ShowLed3(number2);
		led4.ShowLed3(number3);
		break;
	default:
		break;
	}
}
