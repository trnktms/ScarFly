using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses.LevelElementClasses
{
    public class BarrierIndex
    {
        public BarrierIndex(int column, int row, int id)
        {
            this.Column = column;
            this.Row = row;
            this.ID = id;
        }

        public int Column { get; set; }
        public int Row { get; set; }
        public int ID { get; set; }
    }
}
