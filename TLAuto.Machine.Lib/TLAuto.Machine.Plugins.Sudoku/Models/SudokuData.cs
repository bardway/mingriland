// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System.Collections.Generic;
#endregion

namespace TLAuto.Machine.Plugins.Sudoku.Models
{
    public class SudokuData
    {
        public int ButtonIndex { set; get; }

        public List<int> RelayIndexs { get; } = new List<int>();
    }
}