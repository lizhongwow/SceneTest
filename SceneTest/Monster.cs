using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SceneTest
{
    public class MonsterData : IMapUnit
    {
        #region simple properties
        public int hp { get; set; }
        //public int atkrange { get; set; }
        //public int speed { get; set; }
        //public int atkcdtm { get; set; }
        //public int size { get; set; }

        public int mid { get; set; }
        public int iid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int face { get; set; }
        public int level { get; set; }    // 怪物等级            

        public int atk { get; set; } // 外功攻击
        public int def { get; set; }   // 外功防御
        public int matk { get; set; }  // 内功攻击
        public int mdef { get; set; }   // 内功防御

        //public int addResist = [0,0,0,0,0,0,0], // 属性攻击

        //public int atk_min = 100,  // 最小攻击力
        //public int atk_max = 100,  // 最大攻击力
        //public int matk_min = 100, // 最小魔法攻击力
        //public int matk_max = 100, // 最大魔法攻击力

        public int cridmg { get; set; }     // 会心一击额外伤害
        public int exatk { get; set; }      // 卓越一击几率
        public int exatk_debuffs { get; set; } // 卓越一击率抵抗, 相对 exatk 的抵抗 ，千分比， 算法：受到卓越一击的概率降低xx
        public int exper_add { get; set; } //卓越一击几率伤害百分比 增加
        public int exdmg { get; set; }      // 卓越一击额外伤害
        public int dobrate { get; set; }    // 双倍伤害几率
        public int igdef_rate { get; set; } // 无视防御几率
        public int igdef_rate_debuffs { get; set; } // 抵抗无视防御几率

        public int def_red { get; set; }    // 防御减益，千分比
        public int dmg_red { get; set; }    // 伤害减免
        public int atk_dmg_mul { get; set; }// 伤害增益，千分比
        public int igdmg_red { get; set; }  // 无视伤害减免几率，千分比
        public int igatk_dmg_mul { get; set; }// 无视伤害增益几率，千分比
        public int mpsteal { get; set; }    // 吸蓝

        public int hpsuck_dmgm { get; set; }//每次攻击吸收生命值增加伤害比例



        public int criatk { get; set; }     // 暴击几率
        public int criatk_debuffs { get; set; } //暴击抵抗，相对 criatk 的抵抗 ，千分比， 算法：受到暴击击的概率降低xx 
        public int cridef { get; set; }     // 暴击防御
        public int crimul { get; set; }  // 暴击倍数（千分比）
        public int atk_rate { get; set; } // 命中率
        public int miss_rate { get; set; }// 闪避

        //public int             main_ele {get;set;},   // 五行主属性
        //            public int sub_ele = -1,    // 五行辅属性

        public int hpr { get; set; }        // 生命恢复 每0.1秒
        public int mpr { get; set; }        // 法力恢复 每0.1秒

        //public int atktp = atk_type.ATKTP_ATK, // 攻击类型
        public int atkrange { get; set; }  // 攻击距离
        public int atkcdtm { get; set; } // 攻击间隔
        public int castspd { get; set; }  // 施法速度
        public int speed { get; set; }    // 当前移动速度
        public int size { get; set; }       // 体积（圆形半径）

        public int invisible { get; set; }  // 是否隐身
        public int observer { get; set; }   // 是否真视
        public int hpsteal { get; set; }    // 吸血，按普通攻击造成内功、外功伤害千分比
        public int rev_atk { get; set; }    // 伤害反弹，按普通攻击造成内功、外功伤害千分比
        public int rev_dmg { get; set; }    // 伤害反弹，反弹固定伤害
        public int rev_dmg_mul { get; set; }// 伤害反弹，按受到所有伤害千分比反弹
        public int skcost_red { get; set; } // 技能消耗减少千分比

        public int atkstep { get; set; }    // 当前攻击节奏
        public int atksteptm { get; set; }  // 上一次攻击节奏时间

        //public int skcast_red = {},    // 技能释放时间调整值映射表
        //public int skcd_red = {},      // 技能cd时间调整值映射表
        //public int skper = {},         // 技能提升百分比映射表
        //public int stat_red = {},      // 状态时间调整映射表
        //public int lvldrop = {},       // 等级对应掉落

        //public int exp     = 1,
        public int drop { get; set; }
        public int r_x { get; set; }
        public int r_y { get; set; }

        public int skill_gen_cd { get; set; } // 技能公共cd时间
        public int skill_gen_cdst { get; set; }// 技能公共cd起始时间
        //public int skill_fly= {},
        public int last_atk_tm { get; set; }
        public int lvlsideid { get; set; }          // 副本中阵营id

        public bool isdie { get; set; }

        //public int last_mvpts = null,      // 最后一次移动的路径,辅助判断非法移动消息用

        // 怪物特有属性
        public int org_init_x { get; set; }
        public int org_init_y { get; set; }
        public int defrang { get; set; }  // 防守区域半径
        public int tracerang { get; set; }// 追击距离
        public int tracerang2 { get; set; }  // 追击距离平方
        public int running_tm { get; set; } // 返回时间
        public int init_x { get; set; }    // 初始位置
        public int init_y { get; set; }    // 初始位置
        public int revange { get; set; }  // 是否反击
        public double respawn_tm { get; set; }  // 复活时间
        public bool dieshow { get; set; }// 是否死亡后显示尸体
        public bool wrdboss { get; set; }// 是否是世界boss（世界boss只在1线出现）
        public bool atk_oth_mon { get; set; } // 是否主动攻击其他怪物
        public bool keepdist { get; set; }  // 是否保持与攻击目标间的距离
        public bool followply { get; set; } // 是否跟随玩家
        #endregion
        //resumer = {tm=3000, hpper=333, mpper=1000},  // 回血回魔时间间隔，回血千分比，回魔千分比

        //hate_add_mon = {},  // 对其他怪物的额外仇恨
        //hate_add_carr ={},  // 对职业的额外仇恨

        //patrol  = null, // 巡逻数据

        //arrive  = null, // 到达某个地点触发的ai动作

        //exdrop  = [],        // 额外掉落id数组

        public patroling patrol { get; set; }

        //public int lvlsideid { get; set; }

        public atking atking { get; set; }

        public moving moving { get; set; }
        public jumping jumping { get; set; }
        public follow follow { get; set; }

        public int cid { get; set; }




        public Dictionary<int, SkillData> skill = new Dictionary<int, SkillData>();

        public monstarconf_cond_act next_castsk = new monstarconf_cond_act();


        Dictionary<int, SkillData> IMapUnit.skill { get; set; }

        public IMapUnit DefaultMapUnit
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pk_v
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int map_id
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        double IMapUnit.x
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        double IMapUnit.y
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public double lx
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public double ly
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int lmpid
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int sid
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool in_pczone
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public UnitState states
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public List<BState> bstates
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public holding holding
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public casting casting
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public teleping teleping
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        long IMapUnit.skill_gen_cd
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        long IMapUnit.skill_gen_cdst
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public List<Point2D> last_mvpts
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        long IMapUnit.last_atk_tm
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkatkrate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkmisrate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int exp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkdef
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<int, int> addResist
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<int, int> skill_cd
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public atk_type atktp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int matk_min
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int matk_max
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkmatk
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int atk_min
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int atk_max
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkatk
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkdmg_red
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int hpsuck
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int max_hp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int dp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int max_dp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int mp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int max_mp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int hpsuck_dmgmul
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkatk_dmg_mul
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkigdp_rate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int pkigdp_rate_debuffs
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool cfmv
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int full_hp_rate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int full_mp_rate
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public pk_state_type pk_state
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ghost
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int kp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int cur_kp
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int teamid
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int clanid
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        double IPoint2D.x
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        double IPoint2D.y
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
    public class Monster : IBaseUnit
    {
        public Dictionary<int, int> ai_act_cnts = new Dictionary<int, int>();
        public IMapUnit map_unit { get; set; }
        public IMapUnit get_pack_data()
        {
            return map_unit;

        }
        #region properties

        public bool dying = false;

        public int iid
        {
            get;
            set;
        }

        public int last_trace_target_tm
        {
            get;
            set;
        }


        public BattleAttrs battleAttrs = new BattleAttrs();

        public MonsterData mondata = null;

        public monsterconf monconf = null;

        public monsterconf monster_config = null;

        public long lastdmgtm
        {
            get;
            set;
        }

        public Dictionary<int, int> hatelist = new Dictionary<int, int>();

        public long next_mov_tm
        {
            get;
            set;
        }

        public long next_resume_tm
        {
            get;
            set;
        }

        public long next_pick_atk_target_tm
        {
            get;
            set;
        }

        public long next_pick_follow_tm
        {
            get;
            set;
        }

        public int running_tm
        {
            get;
            set;
        } // 持续逃跑时间

        public long last_do_pl_atk_tm
        {
            get;
            set;
        }

        public int check_pos_tm
        {
            get;
            set;
        } // 检查位置是否合法间隔时间

        public double respawn_tm
        {
            get;
            set;
        }

        public double die_tm
        {
            get;
            set;
        }

        public IBaseUnit owner_ply = null;

        public bool first_respawn
        {
            get;
            set;
        }

        public int origin_x
        {
            get;
            set;
        }

        public int origin_y
        {
            get;
            set;
        }

        public bool shkilawd { get; set; }
        public Dictionary<int, int> my_skill_cds = new Dictionary<int, int>();
        public Dictionary<int, int> dmglist = new Dictionary<int, int>();

        #endregion

        public Monster()
        {
            this.mondata = new MonsterData();
            this.mondata.mid = this.monster_config.mid;
        }

        public void initbattleAttrs()
        {
            if (null == this.monconf)
            {
                Console.WriteLine("monster_config is null");
                monster_config = monsterconf.default_value;
            }

            //::g_dump("monster_Conf:",monster_config);
            this.battleAttrs = new BattleAttrs();
            this.battleAttrs.max_hp = monster_config.hp;
            #region initbattleAtrrs
            this.battleAttrs.constitution = monster_config.constitution;
            this.battleAttrs.strength = monster_config.strength;
            this.battleAttrs.intelligence = monster_config.intelligence;
            this.battleAttrs.physics = monster_config.physics;
            this.battleAttrs.magic = monster_config.magic;
            this.battleAttrs.physics_def = monster_config.physics_def;
            this.battleAttrs.magic_def = monster_config.magic_def;
            this.battleAttrs.critical_damage = monster_config.critical_damage;
            this.battleAttrs.critical_def = monster_config.critical_def;
            this.battleAttrs.ice_att = monster_config.ice_att;
            this.battleAttrs.fire_att = monster_config.fire_att;
            this.battleAttrs.thunder_att = monster_config.thunder_att;
            this.battleAttrs.ice_def = monster_config.ice_def;
            this.battleAttrs.fire_def = monster_config.fire_def;
            this.battleAttrs.thunder_def = monster_config.thunder_def;

            this.mondata.hp = monster_config.hp;
            #endregion
            // if(this.mondata.hp < this.battleAttrs.max_hp)
            //     this.mondata.hp = this.battleAttrs.max_hp;
            //dumpsys2("monster,initbattleAttrs mondata.hp:"+this.mondata.hp);
            //this.mondata.max_hp=this.mondata.hp;

            this.mondata.atkrange = monster_config.att.atkrange;
            this.mondata.speed = (int)(monster_config.att.speed * 0.7);
            this.mondata.atkcdtm = monster_config.att.atkcdtm;
            this.mondata.size = monster_config.size;
        }

        public void set_conf(monsterconf m_c)
        {
            this.monster_config = m_c;
            this.monconf = m_c;
        }

        public void set_origin_location(int x, int y)
        {
            this.origin_x = x;
            this.origin_y = y;
        }

        public void set_last_skill_tm(int skillid, int last_tm)
        {
            my_skill_cds[skillid] = last_tm;
        }

        public int get_last_skill_tm(int skillid)
        {
            int tm = 0;
            my_skill_cds.TryGetValue(skillid, out tm);

            return tm;
        }

        public void die(IBaseUnit killer, bool clear_owner = true)
        {
            if (dying)
                return;

            dying = true;

            this.mondata.isdie = true;

            long cur_clock_tm = DateTime.Now.ToBinary();

            if (this.mondata.respawn_tm < 3100)
                this.respawn_tm = 3100 + cur_clock_tm;
            else
                this.respawn_tm = this.mondata.respawn_tm + cur_clock_tm;

            this.die_tm = cur_clock_tm;

            this.mondata.atking = null;
            this.mondata.moving = null;

            if (null != this.mondata.jumping)
            {
                this.mondata.x = this.mondata.jumping.dest_x;
                this.mondata.y = this.mondata.jumping.dest_y;

                this.mondata.jumping = null;
            }

            if (null != this.mondata.patrol)
            {
                this.mondata.patrol.idx = 0;
                this.mondata.patrol.cnt = this.mondata.patrol.init_cnt;
            }

            foreach (IBaseUnit bu in this.gmap.map_sprites.Values)
            {
                IMapUnit ux = bu.get_pack_data();
                if (null != ux.atking)
                {
                    if (ux.atking.tar_iid == this.iid)
                        ux.atking = null;
                }

                bu.on_spr_die(this.iid);
            }

            if (null != killer)
            {
                if (killer.get_sprite_type() == map_sprite_type.MstMonster)
                    if (null != killer.owner_ply)
                        killer = killer.owner_ply;
                //ISprite owner = this;
                //owner.remove_from_hurt_from_iids(this.iid);
            }

            this.hatelist.Clear();

            this._on_leave_battle(cur_clock_tm);



            //this.mondata.atking = null;
            //this.mondata.moving = null;
            //if (null != this.mondata.jumping)
            //{
            //    this.mondata.x = this.mondata.jumping.dest_x;
            //    this.mondata.y = this.mondata.jumping.dest_y;
            //    this.mondata.jumping = null;
            //}

            foreach (KeyValuePair<int, IBaseUnit> pair in this.gmap.map_sprites)
            {
                IMapUnit unit = pair.Value.get_pack_data();
                if (null != unit.atking)
                {
                    if (unit.atking.tar_iid == this.mondata.iid)
                        unit.atking = null;
                }
            }
        }

        public bool onhate(IBaseUnit atker, int hate_value, bool callhatp = true)
        {
            if (this.collect_tar > 0)
                return false;

            if (this.mondata.revange == 0)
                return false;

            if (this.running_tm > 0)
                return false;

            if (atker.isdie())
                return false;

            if (atker.get_sprite_type() == map_sprite_type.MstMonster)
                if (atker.collect_tar > 0)
                    return false;

            if (!can_atk(atker))
                return false;

            int atker_iid = atker.get_pack_data().iid;
            if (this.iid == atker_iid)
                return false;


            if (this.monconf.ai.hate_add > 0)
            {
                hate_value = this.monconf.ai.hate_add;
            }

            if (this.hatelist.ContainsKey(atker_iid))
                this.hatelist[atker_iid] += hate_value;
            else
            {
                this.hatelist[atker_iid] = hate_value;

                if (this.monconf.ai.hatp > 0)
                {
                    long dist2 = this.mondata.defrang * this.mondata.defrang;
                    foreach (KeyValuePair<int, IBaseUnit> pair in this.gmap.map_mons)
                    {
                        var m = pair.Value as Monster;
                        if (m.iid == this.iid)
                            continue;

                        if (m.monconf.ai.hatp <= 0)
                            continue;

                        if (!same_hatp(m))
                            continue;

                        if (m.hatelist.ContainsKey(atker_iid))
                            continue;

                        if (Utility.distance2(this, m) < dist2)
                        {
                            m.onhate(atker, hate_value, false);
                        }
                    }
                }
            }

            if (this.mondata.atking == null)
            {
                if (this.mondata.patrol != null && this.mondata.patrol.tracing == null)
                {
                    this.mondata.patrol.movto.trinitx = this.mondata.x;
                    this.mondata.patrol.movto.trinity = this.mondata.y;
                    this.mondata.patrol.movto.tracing = true;
                }

                long now = DateTime.Now.ToBinary();
                //Dictionary<string,long>
                this.mondata.atking = new atking() { tar_iid = atker.iid, start_tm = now - this.mondata.atkcdtm, trace_tm_left = 0 };

                //TODO broadcast
                this.broad_cast_zone_msg(12, new NetMsgData());
            }

            return true;
        }

        public void ondmg(oldPlayer atker, int dmg)
        {
            if (!this.shkilawd)
                return;

            if (this.get_sprite_type() == map_sprite_type.MstMonster)
                return;

            int cid = atker.pinfo.cid;
            if (this.dmglist.ContainsKey(cid))
                this.dmglist[cid] += dmg;
            else
            {
                this.dmglist[cid] = dmg;
            }

            this.lastdmgtm = DateTime.Now.ToBinary();
        }

        public bool isaily(IBaseUnit spr)
        {
            if (null != this.owner_ply)
                return this.owner_ply.isaily(spr);

            if (spr.get_sprite_type() == map_sprite_type.MstMonster && null != spr.owner_ply)
            {
                spr = spr.owner_ply;
            }

            IMapUnit pl = spr.get_pack_data();
            if (pl.lvlsideid > 0)
            {
                return pl.lvlsideid == this.mondata.lvlsideid;
            }

            if (spr.get_sprite_type() == map_sprite_type.MstMonster)
                return true;

            return false;
        }

        public bool can_atk(IBaseUnit spr)
        {
            if (null != this.owner_ply)
            {
                return this.owner_ply.can_atk(spr);
            }

            if (this.get_sprite_type() == map_sprite_type.MstMonster && null != spr.owner_ply)
                spr = spr.owner_ply;

            IMapUnit pl = spr.get_pack_data();
            if (pl.lvlsideid > 0)
                return this.mondata.lvlsideid != pl.lvlsideid;

            if (this.mondata.lvlsideid == 0 && spr.get_sprite_type() == map_sprite_type.MstMonster)
                return false;

            return true;
        }

        public void on_spr_die(int iid)
        {
            bool needRepickTarget = false;
            MonsterData pl = this.get_pack_data() as MonsterData;

            if (null != pl.atking)
            {
                if (pl.atking.tar_iid == iid)
                {
                    pl.atking = null;
                    needRepickTarget = true;
                }
            }

            if (this.hatelist.ContainsKey(iid))
                this.hatelist.Remove(iid);

            if (needRepickTarget)
                this._pick_atk_target(DateTime.Now.ToBinary());

            if (this.hatelist.Count <= 0)
                _on_leave_battle(DateTime.Now.ToBinary());
        }

        public bool isdie()
        {
            return this.mondata.isdie;
        }

        public bool isghost()
        {
            return false;
        }

        public bool ignore_dmg()
        {
            return this.collect_tar > 0;
        }

        public SkillData get_skil_data(int skid)
        {
            if (this.mondata.skill.ContainsKey(skid))
                return this.mondata.skill[skid];

            return new SkillData() { skid = skid, sklvl = 1 };
        }

        public void re_calc_cha_data()
        {
            int old_speed = this.mondata.speed;
            int old_max_hp = this.battleAttrs.max_hp;

            float hp_percent = ((float)this.mondata.hp) / this.battleAttrs.max_hp;

            Utility.calc_mon_data(this);

            if (this.battleAttrs.max_hp < 0)
                this.battleAttrs.max_hp = 1;

            this.mondata.hp = (int)(this.battleAttrs.max_hp * hp_percent);


            if (old_speed != this.mondata.speed || old_max_hp != this.battleAttrs.max_hp)
                this.broad_cast_zone_msg(26, new NetMsgData());
        }



        public void _pick_atk_target(long current_time)
        {
            if (null != this.owner_ply)
            {
                IMapUnit ownerUnit = this.owner_ply.get_pack_data();
                if (ownerUnit.atking != null)
                {
                    if (null == this.mondata.atking)
                    {
                        this.mondata.atking = new atking() { start_tm = current_time, tar_iid = ownerUnit.atking.tar_iid, trace_tm_left = 0 };
                    }
                    else
                    {
                        this.mondata.atking.tar_iid = ownerUnit.atking.tar_iid;
                    }
                }

                return;
            }

            if (this.hatelist.Count <= 0)
                return;

            int hate_target_iid = 0;
            IBaseUnit hate_target = null;

            foreach (SimpleState sx in this.get_pack_data().states.state_par)
            {
                if (null == sx.desc)
                    continue;

                hate_target = gmap.getSprite(sx.frm_iid);

                if (null == hate_target)
                    continue;

                hate_target_iid = sx.frm_iid;

                break;
            }

            if (null != hate_target)
            {

            }
        }

        public void broad_cast_zone_msg(int cmd, NetMsgData data)
        {

        }


        public void remove_from_hurt_from_iids(int iid)
        {

        }

        //public ISprite Owner_ply { get; set; }

        public bool same_hatp(Monster another)
        {
            return this.monconf.ai.hatp == another.monconf.ai.hatp;
        }

        public void set_die(bool die)
        {
            this.mondata.isdie = die;
        }

        public void respawn(bool broadcast)
        {
            this.mondata.init_x = this.mondata.org_init_x;
            this.mondata.init_y = this.mondata.org_init_y;

            this.mondata.x = this.mondata.init_x * StaticStuff.GRID_WIDTH + 16;
            this.mondata.y = this.mondata.init_y * StaticStuff.GRID_WIDTH + 16;

            this.mondata.hp = this.battleAttrs.max_hp;
            this.respawn_tm = 0;
            this.die_tm = 0;

            this.set_die(false);

            this.dying = false;

            if (this.mondata.wrdboss)
            {

            }
            else
            {
                if (broadcast)
                    this.broad_cast_zone_msg(20, new NetMsgData());

                if (null != this.gmap && this.gmap.blvlmap)
                {
                    if (!first_respawn)
                    {
                        //this.gmap.worldsvr.on_mon_respawn( this );
                    }
                }
            }
        }

        public void setlevel(int lvl)
        {

        }

        public void setcalnatts()
        {

        }

        public void adjust_hp_per(int new_hp_per)
        {
            if (new_hp_per > 100)
            {
                this.battleAttrs.max_hp = (this.battleAttrs.max_hp * new_hp_per) / 100;
                this.mondata.hp = this.battleAttrs.max_hp;
            }
            else
            {
                this.mondata.hp = this.battleAttrs.max_hp * new_hp_per / 100;
                if (this.mondata.hp <= 0)
                    this.mondata.hp = 1;
            }
        }

        public bool _atk_target_in_range()
        {
            IBaseUnit target = this.gmap.getSprite(this.mondata.atking.tar_iid);

            if (null == target)
                return false;

            double dis2 = Utility.distance2(this, target);
            double range2 = Math.Pow(this.mondata.atkrange, 2);

            return dis2 < range2;
        }

        public void _clear_hate_list()
        {
            this.hatelist.Clear();
        }

        public void atk_by(IBaseUnit atker)
        {
            this.onhate(atker, 1);
        }

        public void _trig_die_act(long cur_clock_tm)
        {
            if (null != this.monconf.ai.cond)
            {
                foreach (monsterconf_cond cx in this.monconf.ai.cond)
                {
                    if (1 == cx.die)
                    {
                        if (cx.minlvl > 0 && (this.mondata.level < cx.minlvl))
                            continue;

                        _do_ai_act(cx.act, cur_clock_tm);
                    }
                }
            }
        }

        public void _do_ai_act(List<monstarconf_cond_act> acts, long c_tm)
        {
            foreach (monstarconf_cond_act ax in acts)
            {
                if (ax.cnt > 0)
                {
                    if (this.ai_act_cnts.ContainsKey(ax.actkey))
                        if (this.ai_act_cnts[ax.actkey] <= 0)
                            continue;
                }

                if (ax.r < 100)
                {
                    if (new Random().Next(0, 100) > ax.r)
                        continue;
                }

                if (ax.castsk > 0)
                {
                    if (null == this.mondata.next_castsk)
                    {
                        this.mondata.next_castsk = ax;
                        this._do_next_castsk(c_tm);
                    }
                }

                if (ax.trig.Count > 0)
                {
                    foreach (int tx in ax.trig)
                    {
                        this.gmap.trig_other_triger(this, tx);
                    }
                }

                if (ax.x > 0 && ax.y > 0)
                {
                    this.mondata.atking = null;
                    this.running_tm = this.mondata.running_tm;

                    _move_to(ax.x, ax.y);
                }

                if (ax.cnt > 0)
                {
                    if (this.ai_act_cnts.ContainsKey(ax.actkey))
                        this.ai_act_cnts[ax.actkey]--;
                    else
                        this.ai_act_cnts[ax.actkey] = ax.cnt - 1;
                }

                if (1 == ax.suicide)
                {
                    this.die(this);

                    //broadcast
                    break;
                }
            }
        }

        public void _on_leave_battle(long c_tm)
        {
            this.ai_act_cnts.Clear();
        }

        public void update_resume(long c_tm)
        {
            if (this.isdie())
                return;

            if (this.mondata.atking != null)
                return;

            if (this.hatelist.Count > 0)
                return;

            if (c_tm < this.next_resume_tm)
                return;

            if (this.mondata.hp < this.battleAttrs.max_hp)
            {
                double hp_per = 0.34;//
                int hp_recover = (int)(this.battleAttrs.max_hp * hp_per);
                this.mondata.hp += hp_recover;
                if (this.mondata.hp > this.battleAttrs.max_hp)
                    this.mondata.hp = this.battleAttrs.max_hp;
            }

            this.next_resume_tm = c_tm + 3000 * 1000;
        }

        public void _do_next_castsk(long c_tm)
        {

        }

        public bool _move_to(int x, int y)
        {
            if (this.mondata.speed <= 0)
                return true;

            Point2D cur_grid = new Point2D() { x = this.mondata.x / StaticStuff.GRID_WIDTH, y = this.mondata.y / StaticStuff.GRID_WIDTH };

            List<Point2D> path = new List<Point2D>();//TODO  产生路径

            if (null == path)
                return false;

            if (path.Count <= 0)
                return false;

            return true;
        }

        public void update_ai_mover(long c_tm)
        {
            if (c_tm <= next_mov_tm)
                return;

            long now = DateTime.Now.ToBinary();

            if (this.mondata.patrol == null && this.mondata.moving == null && this.mondata.speed > 0)
            {
                bool ifmove = false;
                Point2D grid = new Point2D(this.mondata.init_x, this.mondata.init_y);
                int start_x = (int)grid.x - this.mondata.r_x;
                int start_y = (int)grid.y - this.mondata.r_y;

                if (start_x < 0)
                    start_x = 0;
                if (start_y < 0)
                    start_y = 0;

                int end_x = start_x + 2 * this.mondata.r_x;
                int end_y = start_y + 2 * this.mondata.r_y;

                Point2D cur_grid = new Point2D(this.mondata.x / StaticStuff.GRID_WIDTH, this.mondata.y / StaticStuff.GRID_WIDTH);

                if (start_x > cur_grid.x || start_y > cur_grid.y || end_x < cur_grid.x || end_y < cur_grid.y)
                    ifmove = true;
                else
                {
                    if (this.monconf.ai.mover > 0 && new Random().Next(0, 100) < monconf.ai.mover)
                    {
                        ifmove = true;
                    }
                }

                if (ifmove && this.mondata.speed > 0)
                {
                    int to_x = Utility.random(start_x, end_x + 1);
                    int to_y = Utility.random(start_y, end_y + 1);

                    List<Point2D> path = Utility.findPath(this.gmap.mapid, cur_grid, new Point2D(to_x, to_y));

                    if (null != path && path.Count > 0)
                    {
                        moving move = new moving();
                        this.mondata.moving = move;

                        move.start_tm = now;
                        move.pts = path;
                        move.to_x = to_x;
                        move.to_y = to_y;
                        move.float_x = this.mondata.x;
                        move.float_y = this.mondata.y;

                        //TODO broadcast
                    }
                }
            }

            this.next_mov_tm = now + this.monconf.ai.thinktm;
        }

        public void update(gameWorld world, long now, long time_elapsed)
        {
            if (gmap.map_fined)
                return;

            if (first_respawn)
            {
                respawn(false);
                first_respawn = false;
                return;
            }

            if (respawn_tm > 0)
            {
                if (this.die_tm > 0 && respawn_tm - this.die_tm > 3000)
                {
                    this.set_die(true);
                    this.die_tm = 0;
                }

                if (this.mondata.respawn_tm > 0 && respawn_tm < now)
                {
                    respawn(true);

                    this._trig_respawn_act(now);
                }

                return;
            }

            if (this.collect_tar > 0)
                return;

            oldSkill.update_pl_state(now, this);

            long tm_left = this.gmap.update_pl_move(this, now);

            if (this.mondata.moving == null)
            {
                if (this.mondata.atking != null)
                {
                    this.mondata.atking.trace_tm_left = tm_left;
                    grid_map.update_pl_atk_tracing(this, now);
                    this.gmap.update_pl_move(this, now);
                }
            }

            if (this.running_tm <= 0 && this.monconf.ai.aggres > 0 && this.owner_ply == null)
            {
                List<IBaseUnit> inz_players = this.get_inz_plys();
                foreach (IBaseUnit sprite in inz_players)
                {
                    if (sprite.isdie() || sprite.isghost())
                        continue;

                    if (!this.can_atk(sprite))
                        continue;

                    IMapUnit pl = sprite.get_pack_data();

                    if (pl.invisible > this.mondata.observer)
                        continue;

                    long dist2 = Utility.distance2(this, sprite);
                    long atk_dist2 = (long)Math.Pow(this.mondata.defrang, 2);

                    if (dist2 < atk_dist2)
                    {
                        if (!this.hatelist.ContainsKey(sprite.iid))
                        {
                            this.onhate(sprite, 1);
                            break;
                        }
                    }
                    else
                    {
                        if (this.monconf.ai.clhor > 0)
                        {
                            if (this.hatelist.Count > 0)
                                this.hatelist.Clear();
                        }

                        if (this.mondata.atking != null && sprite.iid == this.mondata.atking.tar_iid)
                            this.cancel_atk();
                    }
                }

                if (this.mondata.atk_oth_mon)
                {
                    foreach (var sp_x in this.gmap.map_mons.Values)
                    {
                        if (sp_x.isdie() || sp_x.isghost())
                            continue;

                        if (!this.can_atk(sp_x))
                            continue;

                        if (sp_x.get_pack_data().invisible > this.mondata.observer)
                            continue;

                        if (sp_x.collect_tar > 0)
                            continue;

                        long dist2 = Utility.distance2(sp_x, this);
                        long atk_dist2 = (long)Math.Pow(this.mondata.defrang, 2);

                        if (dist2 < atk_dist2)
                        {
                            if (!this.hatelist.ContainsKey(sp_x.iid))
                                if (this.onhate(sp_x, 1))
                                    break;
                        }
                        else if (this.monconf.ai.clhor < 0)
                        {
                            if (this.hatelist.ContainsKey(sp_x.iid))
                                this.hatelist.Remove(sp_x.iid);

                            if (this.mondata.atking != null && this.mondata.atking.tar_iid == sp_x.iid)
                                this.cancel_atk();
                        }
                    }
                }
            }

            if (now > this.next_pick_atk_target_tm)
                this._pick_atk_target(now);

            if (this.mondata.followply && now > this.next_pick_follow_tm && this.mondata.follow == null)
            {
                foreach (IBaseUnit sp_x in this.gmap.map_players.Values)
                {
                    if (!this.isaily(sp_x))
                        continue;

                    if (sp_x.get_pack_data().invisible > this.mondata.observer)
                        continue;

                    var pl = sp_x.get_pack_data();
                    this.mondata.follow = new follow() { do_ai = false, start_tm = now, tar_iid = sp_x.iid, trace_tm_left = 0, frang = 40000, trang = 8100 };
                    break;
                }

                this.next_pick_follow_tm = now + 1000;
            }

            _do_next_castsk(now);
            grid_map.update_pl_atk(this, now);

            grid_map.update_skill_casting(this, this.mondata, now);
            grid_map.update_skill_holding(this, this.mondata, now);
            grid_map.update_jumping(this, this.mondata, now);
            grid_map.update_teleping(this, this.mondata, now);

            if (this.mondata.atking != null)
            {
                this.mondata.atking.trace_tm_left = 0;
                this.gmap.update_pl_atk_tracing(this, now);

                if (now > this.next_mov_tm)
                {
                    if (this.mondata.keepdist)
                        this.gmap.update_keep_atk_range(this, now);

                    if (this.monconf.ai.cond != null)
                    {
                        foreach (var cond in this.monconf.ai.cond)
                        {
                            _do_ai_act(cond.act, now);
                        }
                    }

                    int init_x = this.mondata.init_x * StaticStuff.GRID_WIDTH + 16;
                    int init_y = this.mondata.init_y * StaticStuff.GRID_WIDTH + 16;
                    bool need_fall_back = true;

                    if (this.mondata.patrol != null && this.mondata.patrol.movto != null)
                    {
                        init_x = this.mondata.patrol.movto.trinitx;
                        init_y = this.mondata.patrol.movto.trinity;
                        need_fall_back = false;
                    }

                    if (this.mondata.follow != null)
                    {
                        if (this.gmap.map_sprites.ContainsKey(this.mondata.follow.tar_iid))
                        {
                            IBaseUnit follow_pl = this.gmap.map_sprites[this.mondata.follow.tar_iid];
                            init_x = follow_pl.x;
                            init_y = follow_pl.y;
                        }
                    }

                    long dist2 = (long)(Math.Pow(this.mondata.x - init_x, 2) + Math.Pow(this.mondata.y - init_y, 2));

                    if (dist2 > this.mondata.tracerang2)
                    {
                        this.mondata.atking = null;
                        this.running_tm = this.mondata.running_tm;
                        this._clear_hate_list();

                        if (need_fall_back && this.mondata.speed > 0)
                        {
                            Point2D cur_g = new Point2D(this.mondata.x / StaticStuff.GRID_WIDTH,
                                this.mondata.y / StaticStuff.GRID_WIDTH);
                            List<Point2D> path =
                                Utility.findPath(this.gmap.mapid, cur_g.x, cur_g.y, this.mondata.init_x,
                                    this.mondata.init_y);

                            if (path != null && path.Count > 0)
                            {
                                this.mondata.moving = new moving()
                                {
                                    start_tm = now,
                                    pts = path,
                                    to_x = this.mondata.init_x,
                                    to_y = this.mondata.init_y,
                                    float_x = this.mondata.x,
                                    float_y = this.mondata.y
                                };

                                //TODO broad cast
                            }
                        }
                    }

                    this.next_mov_tm = now + this.monconf.ai.thinktm;
                }
            }
            else if (this.mondata.follow != null)
            {
                this.mondata.follow.trace_tm_left = 0;
                this.gmap.update_pl_follow_tracing(this, now);

                if (this.mondata.moving == null && this.mondata.follow != null && this.mondata.follow.do_ai)
                {
                    IBaseUnit f_pl = this.gmap.map_sprites[this.mondata.follow.tar_iid];
                    this.mondata.init_x = f_pl.x / StaticStuff.GRID_WIDTH;
                    this.mondata.init_y = f_pl.y / StaticStuff.GRID_WIDTH;

                    this.update_ai_mover(now);
                }
            }
            else
            {
                this._try_patrol(now);
                this.update_ai_mover(now);
            }

            this.update_resume(now);

            if (this.shkilawd)
            {
                if (now - this.lastdmgtm > 10)
                {

                }
            }
        }

        public void _trig_respawn_act(long now)
        {

        }

        public List<IBaseUnit> get_inz_plys()
        {

        }

        public void cancel_atk()
        {

        }

        public void _try_patrol(long now)
        {

        }

    }
}
