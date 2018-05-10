using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitroComposer;

namespace Test {
    class Program {
        static void Main(string[] args) {
            var sdat = SDat.Open(args[0]);
            var strm=sdat.OpenStream("STRM_BGM03DS_REQ");
        }
    }
}
