using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class ModifierIndex
    {
        public ModifierIndex(int column, int row, string id)
        {
            this.Column = column;
            this.Row = row;
            this.ID = id;
        }

        public int Column { get; set; }
        public int Row { get; set; }
        public string ID { get; set; }
    }
}
