using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneTest
{
    public interface IService
    {
        /// <summary>
        /// key 为sid
        /// </summary>
        Dictionary<int,IBaseUnit> sgplayers { get; set; }

        void ply_change_map(IBaseUnit sprite, grid_map map);
    }
}
