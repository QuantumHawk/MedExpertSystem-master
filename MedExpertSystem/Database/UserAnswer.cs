using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedExpertSystem.Database
{
    public static class UserAnswer
    {
        public static string Name { get; set; }
        public static string Surname { get; set; }
        public static decimal [] List1 { get; set; } = new decimal[9];
        public static decimal [] List2 { get; set; } = new decimal[9];
        public static decimal [] List3 { get; set; } = new decimal[9];
        public static decimal [] List4 { get; set; } = new decimal[9];
        public static decimal [] List5 { get; set; } = new decimal[9];
        public static decimal [] ListEntropy1 { get; set; } = new decimal[9];
        public static decimal [] ListEntropy2 { get; set; } = new decimal[9];
        public static decimal [] ListEntropy3 { get; set; } = new decimal[9];
        public static decimal [] ListEntropy4 { get; set; } = new decimal[9];
        public static decimal [] ListEntropy5 { get; set; } = new decimal[9];
        public static int Index { get; set; }
        public static int  FAnsw { get; set; }
        public static int  SAnsw { get; set; }
        public static int  TAnsw { get; set; }
    }
}