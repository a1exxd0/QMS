using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QSim_Console_Test.Scripts
{
    [JsonObject] public class TestClass
    {
        [JsonProperty] int? item1;
        [JsonProperty] TestClass2[]? items;

        internal TestClass(int? item1, TestClass2[]? items)
        {
            this.item1 = item1;
            this.items = items;
        }

        internal string Ser()
        {
            TestClass2[] x = new TestClass2[2] { new TestClass2(1, 2), new TestClass2(3, 4) };

            var tC = new TestClass(1, x);
            return JsonConvert.SerializeObject(tC);
        }

    }
    [JsonObject]
    internal class TestClass2
    {
        [JsonProperty] int a;
        [JsonProperty] int b;

        internal TestClass2(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }
    internal class runtime
    {
        static void min(string[] args)
        {
            TestClass stuff1 = new TestClass(1, null);
            string stuff = stuff1.Ser();
            Console.WriteLine(stuff);
        }
    }
}
