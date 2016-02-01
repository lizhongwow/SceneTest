using SceneTestLib;
using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            skill_conf sk = new skill_conf();

            Type t = sk.GetType();

            ConfReader.init();

            var confs = ConfReader.BState_Confs;

            int x = 0;
        }
    }
}
