using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
  
    public class oldPlayer:IBaseUnit
    {
        public IMapUnit pinfo { get; set; }

        public bool isaily(IBaseUnit spr)
        {
            return false;
        }
    }
}
