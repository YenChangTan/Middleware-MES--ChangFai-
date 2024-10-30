using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Middleware.model
{
    public class Pcb
    {
        public string UUID {  get; set; }
        public int number {  get; set; }
        public bool isDefected { get; set; }

        public bool requested = false; 
    }
}
