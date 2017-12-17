// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace TLAuto.Machine.Plugins.Boxing.Models
{
    /// <summary>
    /// 每一轮的打击数据
    /// </summary>
    public class BoxingData
    {
        public int HitCount { set; get; }

        public int Delay { set; get; }

        public List<int> Number { set; get; }

        public int GetImageIndex(int buttonIndex)
        {
            switch (buttonIndex)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 4;
                case 4:
                    return 5;
                case 5:
                    return 6;
                case 6:
                    return 7;
                case 7:
                    return 8;
                default:
                    throw new ArgumentException();
            }
        }

        public async Task PlayMusic(int imageIndex, BoxingMachinePluginsProvider provider, int gameCount)
        {
            var tick = DateTime.Now.Ticks;
            var random = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            var index = random.Next(0, 1);
            if (gameCount > 1)
            {
                switch (imageIndex)
                {
                    case 1:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "yumi2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                    case 2:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "mogu2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                    case 3:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "lamian2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                    case 4:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "baozi2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                    case 5:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "taozi2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                    case 6:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "jiaozi2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                    case 7:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "jidan2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                    case 8:
                        await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "luobo2.wav", "shengxiao");
                        await Task.Delay(500);
                        break;
                }
            }
            else
            {
                switch (imageIndex)
                {
                    case 1:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "yumi1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "yumi2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                    case 2:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "mogu1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "mogu2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                    case 3:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "lamian1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "lamian2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                    case 4:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "baozi1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "baozi2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                    case 5:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "taozi1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "taozi2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                    case 6:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "jiaozi1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "jiaozi2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                    case 7:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "jidan1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "jidan2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                    case 8:
                        switch (index)
                        {
                            case 0:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "luobo1.wav", "shengxiao");
                                await Task.Delay(1000);
                                break;
                            case 1:
                                await provider.PlayMusic0(BoxingMachinePluginsProvider.SignKey, "luobo2.wav", "shengxiao");
                                await Task.Delay(500);
                                break;
                        }
                        break;
                }
            }
        }
    }
}