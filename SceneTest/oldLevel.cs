
















    	
    			pl.dota=null;
    			ply.flush_db_data(false,false);
    			ply.re_calc_cha_data();
    			return;
    			return;
    			this.dota.try_remove(pl.cid);
    			this.finish();
    			this.finish();
    			Utility.trace_err("Level.init(),level conf not found for ltpid:"+ltpid);
    			Utility.trace_err("Level.init(),level_info not exist,llid:"+llid);
    		if((!))
    		if(flush_data)
    		if(null == linfo){
    		if(null == this.lvl_conf){
    		if(this.dota){
    		IMapUnit pl=ply.get_pack_data();
    		Level linfo=Level.get_level_info(llid);
    		pl.llid=0;
    		pl.map_id = pl.lmpid;
    		pl.x = pl.lx;
    		pl.y = pl.ly;
    		this.creator = linfo.creator;
    		this.diff_lvl = linfo.diff_lvl;
    		this.is_score_km = (_get_score_conf("score_awd_km") != null);
    		this.ltpid = linfo.ltpid;
    		this.lvl_conf = get_level_conf(ltpid);
    		}
    		}
    		}
    	private void _on_player_leave(IBaseUnit ply,bool flush_data=false){
    	public Dictionary<int,IBaseUnit> dota=new Dictionary<int,IBaseUnit>();
    	public int creator=0;
    	public int diff_lvl=0;
    	public int llid=0;
    	public int ltpid=0;
    	public static Level get_level_info(int llid){
    	public static void create_level()
    	public void init(llid,service){
    	{
    	}
    	}
    	}
    	}
    public class Level
    {
    }
namespace SceneTest
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
{
}