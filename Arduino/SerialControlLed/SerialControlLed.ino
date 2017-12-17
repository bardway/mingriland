#include "CommandHelper.h"
#include "LedHelper.h"

Led led1;
Led led2;
Led led3;
Led led4;

Command command;
boolean readCompleted = false;//指示是否完成读取串口数据

String text;

int device = 2;

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
	ShowLed2(0, 9, 0, device);
	ShowLed2(1, 0, 1, device);
	ShowLed2(2, 1, 1, device);
	ShowLed2(3, 1, 2, device);
}

void loop()
{
	//if (readCompleted)//判断串口是否接收到数据并完成读取
	//{
	//	
	//}
	//delay(2);
}

void serialEvent()
{
	while (Serial.available())
	{
		char inChar = (char)Serial.read();
		if (inChar != '\n')
		{
			//Serial.print(inChar);
			command.AnalysisCommand(inChar);
			if (inChar == '|')
			{
				String key = command.GetKeyCommand();
				String value = command.GetValueCommand();
				if (key == "LedControl")
				{
					//Serial.println("key:" + key);
					int mode = ToInt32(value[0]);
					if (mode == 0)
					{
						ShowLed2(ToInt32(value[1]), ToInt32(value[2]), ToInt32(value[3]), ToInt32(value[4]));
					}
					else if (mode == 1)
					{
						ShowLed3(ToInt32(value[1]), ToInt32(value[2]), ToInt32(value[3]), ToInt32(value[4]));
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

void ShowLed2(int index, char number1, char number2, int deviceNumber)
{
	if (device != deviceNumber)
		return;
	switch (index)
	{
	case 0:
		led1.ShowLed2(number1);
		led1.ShowLed2(number2);
		break;
	case 1:
		led2.ShowLed2(number1);
		led2.ShowLed2(number2);
		break;
	case 2:
		led3.ShowLed2(number1);
		led3.ShowLed2(number2);
		break;
	case 3:
		led4.ShowLed2(number1);
		led4.ShowLed2(number2);
	default:
		break;
	}
}

void ShowLed3(int index, char number1, char number2, char number3)
{
	switch (index)
	{
	case 0:
		led1.ShowLed3(number1);
		led1.ShowLed3(number2);
		led1.ShowLed3(number3);
		break;
	case 1:
		led2.ShowLed3(number1);
		led2.ShowLed3(number2);
		led2.ShowLed3(number3);
		break;
	default:
		break;
	}
}
