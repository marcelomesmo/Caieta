using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMOD {
    public static class FMODInit {
        static bool isInit = false;
        public static void ForceLoadLibs() {
            if (isInit) {
                return;
            }

            //force load fmod.dll
            Memory.GetStats(out int currentAllocated, out int maxAllocated);
            isInit = true;
        }
    }
}
