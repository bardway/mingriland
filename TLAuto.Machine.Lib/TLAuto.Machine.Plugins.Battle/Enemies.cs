// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using TLAuto.Machine.Plugins.Core.Models;
#endregion

namespace TLAuto.Machine.Plugins.Battle
{
    public static class Enemies
    {
        private static readonly List<HitButon> Hitters = new List<HitButon>();
        private static readonly List<Tuple<int, int, int, HitIdList>> Games = new List<Tuple<int, int, int, HitIdList>>();

        private static readonly HitIdList Boss = new HitIdList {14, 15, 16, 17, 18, 19, 20};

        private static readonly HitIdList S1 = new HitIdList {5, 8};
        private static readonly HitIdList S2 = new HitIdList {6, 9};
        private static readonly HitIdList S3 = new HitIdList {7, 10};

        private static readonly HitIdList H1 = new HitIdList {22, 24};
        private static readonly HitIdList H2 = new HitIdList {27, 28, 29};

        private static readonly HitIdList D1 = new HitIdList {1};
        private static readonly HitIdList D2 = new HitIdList {2};
        private static readonly HitIdList D3 = new HitIdList {3};
        private static readonly HitIdList D4 = new HitIdList {4};
        private static readonly HitIdList D5 = new HitIdList {21, 23};
        private static readonly HitIdList D6 = new HitIdList {26};

        private static readonly HitIdList A1 = new HitIdList {11};
        private static readonly HitIdList A2 = new HitIdList {12};
        private static readonly HitIdList A3 = new HitIdList {13};
        private static readonly HitIdList A4 = new HitIdList {25};
        private static readonly HitIdList A5 = new HitIdList {30};

        public static void LoadEnemy(int buttonid, MachineButtonItem button, MachineRelayItem light)
        {
            var hitButon = new HitButon(buttonid, button, light);
            switch (buttonid)
            {
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    hitButon.Sound = string.Empty;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 21:
                case 23:
                case 26:
                    hitButon.Sound = "d.wav";
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    hitButon.Sound = "soldier.mp3";
                    break;
                case 22:
                case 24:
                case 27:
                case 28:
                case 29:
                    hitButon.Sound = "heavy.mp3";
                    break;
                case 11:
                case 12:
                case 13:
                case 25:
                case 30:
                    hitButon.Sound = "assassin.mp3";
                    break;
            }
            Hitters.Add(hitButon);
        }

        public static bool CheckIfOff(int device, int number)
        {
            var r = Hitters.FirstOrDefault(h => (h.Hitter.DeviceNumber == device) && (h.Hitter.Number == number));
            return (r != null) && !r.Light.IsNo;
        }

        public static void ArrangeEnemey()
        {
            Games.Add(Tuple.Create(0, 0, 4, S1));
            Games.Add(Tuple.Create(0, 0, 4, S3));
            Games.Add(Tuple.Create(0, 0, 4, S2));

            Games.Add(Tuple.Create(0, 1, 4, S1 + S3 + H1));
            Games.Add(Tuple.Create(0, 1, 4, S1 + H2));
            Games.Add(Tuple.Create(0, 1, 4, S3 + H2));

            Games.Add(Tuple.Create(0, 2, 4, S2 + H2 + D1));
            Games.Add(Tuple.Create(0, 2, 4, S1 + S3 + H1 + D3));
            Games.Add(Tuple.Create(0, 2, 4, S1 + H2 + D5));
            Games.Add(Tuple.Create(0, 2, 4, S3 + H1 + D5));
            Games.Add(Tuple.Create(0, 2, 4, S2 + H2 + D4));
            Games.Add(Tuple.Create(0, 2, 4, S1 + S3 + H1 + D6));
            Games.Add(Tuple.Create(0, 3, 10, S1 + S2 + S3 + H2 + H2 + D1 + D2 + D3 + D4 + D5 + D6));

            Games.Add(Tuple.Create(1, 0, 5, S1 + S3 + D6));
            Games.Add(Tuple.Create(1, 0, 5, H1 + S2));
            Games.Add(Tuple.Create(1, 0, 5, H2 + D1 + D4));

            Games.Add(Tuple.Create(1, 1, 10, S1 + S2 + S3 + D5 + Boss));
            Games.Add(Tuple.Create(1, 2, 5, S1 + S3 + D6));
            Games.Add(Tuple.Create(1, 2, 5, H1 + S2));
            Games.Add(Tuple.Create(1, 2, 5, H2 + D1 + D4));

            Games.Add(Tuple.Create(1, 3, 10, S1 + S2 + S3 + D5 + Boss + A2));
            Games.Add(Tuple.Create(1, 4, 5, S1 + S3 + D6 + A4));
            Games.Add(Tuple.Create(1, 4, 5, H1 + S2 + A1));
            Games.Add(Tuple.Create(1, 4, 5, H2 + D1 + D4 + A3));
            Games.Add(Tuple.Create(1, 4, 5, S1 + S2 + S3 + D5 + A5));
            Games.Add(Tuple.Create(1, 4, 10, A1 + A2 + A3 + A4 + A5 + Boss));

            Games.Add(Tuple.Create(2, 0, 7, Boss + S2 + H2 + D1 + D5 + A4));
            Games.Add(Tuple.Create(2, 0, 7, Boss + S1 + S3 + H1 + D6));
            Games.Add(Tuple.Create(2, 0, 7, Boss + S1 + S2 + S3 + H1 + H2));
            Games.Add(Tuple.Create(2, 0, 7, Boss + D1 + D2 + D3 + D4 + D5 + D6 + A1 + A2 + A3 + A4 + A5));
            Games.Add(Tuple.Create(2, 1, 15, Boss + D1 + D2 + D3 + D4 + D5 + D6 + A1 + A2 + A3 + A4 + A5 + S1 + S2 + S3 + H1 + H2));
        }

        public static Tuple<int, List<List<HitButon>>> DeployTroop(int currentgame, int currentround)
        {
            Debug.WriteLine($"{DateTime.Now} : currentgame:{currentgame} + currentround:{currentround} ");
            var games = Games.Where(g => (g.Item1 == currentgame) && (g.Item2 == currentround)).ToList();
            var bs2 = new List<List<HitButon>>();

            var currentgames = games.Where(g => (g.Item1 == currentgame) && (g.Item2 == currentround));
            foreach (var game in currentgames)
            {
                var bs = game.Item4.Select(buttonid => Hitters.FirstOrDefault(h => h.ButtonId == buttonid)).ToList();
                foreach (var hitButon in bs)
                {
                    hitButon.ResetHitting();
                }

                bs2.Add(bs);
            }
            return Tuple.Create(games.FirstOrDefault().Item3, bs2);
        }

        public static async void RestBattle()
        {
            foreach (var hitButon in Hitters)
            {
                await hitButon.Light.Control(false);
            }
        }

        public static int GetGamecount()
        {
            return Games.Select(g => g.Item2).Distinct().Count();
        }

        public static int GetRoundcount(int game)
        {
            return Games.Where(g => g.Item1 == game).Select(g => g.Item2).Distinct().Count();
        }
    }
}