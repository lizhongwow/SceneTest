using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SceneTestLib.Confs
{
    public class Atomic_Int
    {
        private int _atomic_int;

        public Atomic_Int(int cur_value)
        {
            _atomic_int = cur_value;
        }

        public int Next_Value()
        {
            return Interlocked.Increment(ref this._atomic_int);
        }

    }
}
