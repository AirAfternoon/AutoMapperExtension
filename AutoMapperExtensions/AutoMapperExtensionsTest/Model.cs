using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapperExtensionsTest
{
    public class Model
    {
    }

    public class ModelA
    {
        public string Field1 { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }

    public class ModelB
    {
        public string Field1 { get; set; }
        public string Time { get; set; }
    }

    public class ModelC
    {
        public string Field1 { get; set; }
        public string Time { get; set; }

    }
}
