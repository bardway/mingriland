// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;

using TLAuto.BaseEx.Extensions;
using TLAuto.Machine.Controls.Models.Enums;
#endregion

namespace TLAuto.Machine.App.Common
{
    public class ApplicationExForMachine : ApplicationEx
    {
        protected override bool IsCheckTwice { get; } = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 1)
            {
                var diffType = (DifficulySystemType)Enum.Parse(typeof(DifficulySystemType), e.Args[0]);
                ConfigHelper.DiffType = diffType;
            }
            else
            {
                if (e.Args.Length > 1)
                {
                    var newArgsLength = e.Args.Length - 1;
                    var newArgs = new string[newArgsLength];
                    Array.Copy(e.Args, newArgs, newArgsLength);
                    ConfigHelper.Args = newArgs;
                    var diffType = (DifficulySystemType)Enum.Parse(typeof(DifficulySystemType), e.Args[newArgsLength]);
                    ConfigHelper.DiffType = diffType;
                }
            }
            base.OnStartup(e);
        }
    }
}