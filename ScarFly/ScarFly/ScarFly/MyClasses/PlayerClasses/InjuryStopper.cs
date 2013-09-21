using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScarFly.MyClasses
{
    public class InjuryStopper
    {
        public InjuryStopper(string name, int count)
        {
            this.Name = name;
            this.Count = count;
        }

        public string Name { get; set; }
        public int Count { get; set; }
    }
}
