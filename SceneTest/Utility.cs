
using SceneTestLib;
using SceneTestLib.Confs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SceneTest
{
    public class Utility
    {
        public static long time()
        {
            return DateTime.Now.ToBinary();
        }

        public static map_conf get_map_conf(int map_id)
        {
            map_conf conf = null;
            ConfReader.Map_Confs.TryGetValue(map_id, out conf);

            return conf;
        }

        public static level_conf get_level_conf(int ltpid)
        {

        }

        public static monsterconf get_monster_conf(int mid)
        {

        }

        public static game_err_code check_target_type(IBaseUnit caster, IBaseUnit tar_sprite, int aff, map_pk_setting_type map_pk_seting)
        {
            IMapUnit pl = caster.get_pack_data();
            IMapUnit tar_pl = tar_sprite.get_pack_data();
            if (pl.iid != tar_pl.iid)
            {
                // 作用于其他角色          
                if ((aff & (int)skill_aff_type.SAT_ENERMY) != (int)skill_aff_type.SAT_ENERMY)
                {
                    if (caster.can_atk(tar_sprite))
                        return game_err_code.SKIL_INVALIDE_TAREGET;//敌人

                    // 根据aff判断盟友关系从而确定是否受此状态影响
                    var isaily = caster.isaily(tar_sprite);
                    if (isaily)
                    {
                        if ((aff & (int)skill_aff_type.SAT_ALLY) != (int)skill_aff_type.SAT_ALLY)
                            return game_err_code.SKIL_INVALIDE_TAREGET;
                        // 不对盟友产生影响                 
                    }
                    else
                    {//中立方
                        if ((aff & (int)skill_aff_type.SAT_MID) != (int)skill_aff_type.SAT_MID)
                            return game_err_code.SKIL_INVALIDE_TAREGET;
                    }
                }
                else if (caster.get_sprite_type() == map_sprite_type.MstPlayer)
                {// 根据地图、个人pk状态决定是否可以释放技能    
                    if (tar_sprite.get_sprite_type() == map_sprite_type.MstPlayer)
                    {
                        if (map_pk_seting == map_pk_setting_type.MPST_PEACE)
                            return game_err_code.CANT_PK_IN_PEACE_MAP;

                        if (pl.in_pczone || tar_pl.in_pczone)
                            return game_err_code.CANT_PK_IN_PEACE_ZONE;

                        if (map_pk_seting == map_pk_setting_type.MPST_NORMAL)
                        {
                            if (!tar_sprite.can_atk_direct())
                            {
                                //不可攻击的玩家
                                if (!caster.can_atk(tar_sprite))
                                    return game_err_code.CANT_ATK;
                            }
                        }
                        else
                        {
                            //pk地图
                            if (!caster.can_atk(tar_sprite))
                                return game_err_code.CANT_ATK;
                        }
                    }
                    else if (tar_sprite.get_sprite_type() == map_sprite_type.MstMonster)
                    {
                        if (tar_sprite.owner_ply != null)
                        {
                            //战宠     
                            var owner_sprite = tar_sprite.owner_ply;
                            var ower_pl = owner_sprite.get_pack_data();
                            if (ower_pl.iid == pl.iid)
                                return game_err_code.CANT_ATTACK_SELF_PET;

                            if (map_pk_seting == map_pk_setting_type.MPST_PEACE)
                                return game_err_code.CANT_PK_IN_PEACE_MAP;

                            if (pl.in_pczone || ower_pl.in_pczone)
                                return game_err_code.CANT_PK_IN_PEACE_ZONE;

                            if (map_pk_seting == map_pk_setting_type.MPST_NORMAL)
                            {
                                if (!owner_sprite.can_atk_direct())
                                {
                                    //不可攻击的玩家
                                    if (!caster.can_atk(owner_sprite))
                                        return game_err_code.CANT_ATK;
                                }
                            }
                            else
                            {
                                //pk地图
                                if (!caster.can_atk(owner_sprite))
                                    return game_err_code.CANT_ATK;
                            }
                        }
                        else
                        {
                            if (!caster.can_atk(tar_sprite))
                                return game_err_code.CANT_ATK;
                        }
                    }
                }
                else
                {
                    if (!caster.can_atk(tar_sprite))
                        return game_err_code.CANT_ATK;
                }
            }
            else
            {
                // 作用于自己
                if ((aff & (int)skill_aff_type.SAT_SELF) != (int)skill_aff_type.SAT_SELF)
                    return game_err_code.SKIL_INVALIDE_TAREGET; // 不对自己产生影响
            }

            return game_err_code.RES_OK;
        }

        public static skill_conf get_skil_skill_desc(int skill_id)
        {

        }

        public static skill_lv_conf get_skil_skill_lvl_desc(int skill_id, int level)
        {

        }
        public static void calc_cha_data(IBaseUnit sprite, IMapUnit sprite_pl, bool full_hp = false)
        {

        }

        public static Point2D normalize_vec2(double x, double y)
        {
            if (x.CompareTo(0) == 0)
            {
                if (y.CompareTo(0) == 0)
                    return new Point2D(0, 0);

                return new Point2D(0, 1);
            }

            if (y.CompareTo(0) == 0)
            {
                if (x.CompareTo(0) == 0)
                    return new Point2D(0, 0);

                return new Point2D(1, 0);
            }

            double r = Math.Sqrt(x * x + y * y);

            return new Point2D(x / r, y / r);
        }

        public static Point2D normalize_vec2(Point2D point)
        {
            return normalize_vec2(point.x, point.y);
        }

        public static void trace_info(string info)
        {

        }
        public static void trace_err(string err)
        {

        }
        public static void calc_mon_data(Monster m)
        {

        }

        public static int distance(IBaseUnit a, IBaseUnit b)
        {
            IMapUnit ma = a.get_pack_data();
            IMapUnit mb = a.get_pack_data();

            if (ma == null || mb == null)
                return 0;

            return (int)Math.Sqrt(Math.Pow(ma.x - mb.x, 2) + Math.Pow(ma.y - mb.y, 2));
        }

        public static int distance2(IBaseUnit a, IBaseUnit b)
        {
            IMapUnit ma = a.get_pack_data();
            IMapUnit mb = a.get_pack_data();

            if (ma == null || mb == null)
                return 0;

            return (int)(Math.Pow(ma.x - mb.x, 2) + Math.Pow(ma.y - mb.y, 2));
        }

        public static int random(int start, int end)
        {
            Random r = new Random((int)DateTime.Now.ToBinary());

            return r.Next(start, end);
        }

        public static double random(double start, double end)
        {
            Random r = new Random((int)DateTime.Now.ToBinary());

            return (start + r.NextDouble() * (end - start));
        }

        public static int random_section(int start, int section)
        {
            return random(start, start + section);
        }

        public static List<Point2D> findPath(int mapid, int grid_start_x, int grid_start_y, int grid_end_x, int grid_end_y)
        {
            return new List<Point2D>();
        }
        public static List<Point2D> findPath(int mapid, Point2D start_grid, Point2D end_grid)
        {
            return findPath(mapid, (int)start_grid.x, (int)start_grid.y, (int)end_grid.x, (int)end_grid.y);
        }

        public static skill_conf GetSkillConf(int skillid)
        {
            return new skill_conf();
        }

        public static Point2D valpoint_on_line(IPoint2D frm_pos, IPoint2D to_pos, int max_rang, int sublen = 0)
        {
            double dist_x = to_pos.x - frm_pos.x;
            double dist_y = to_pos.y - frm_pos.y;

            double dist2 = dist_x * dist_x + dist_y * dist_y;

            Point2D vec = normalize_vec2(new Point2D(dist_x, dist_y));

            double dest_x = to_pos.x;
            double dest_y = to_pos.y;

            if (max_rang * max_rang < dist2)
            {
                dest_x = frm_pos.x + vec.x * max_rang;
                dest_y = frm_pos.y + vec.y * max_rang;
            }

            return new Point2D(dest_x, dest_y);
        }

        public static Point2D valpoint_on_line(IMapUnit frm_mapUnit, IPoint2D to_pos, int max_rang, int sublen = 0)
        {
            Point2D frm_pos = new Point2D(frm_mapUnit.x, frm_mapUnit.y);

            return valpoint_on_line(frm_pos, to_pos, max_rang, sublen);
        }

        public static int GetDicInt(Dictionary<string, string> dic, string keyname)
        {
            string v = null;
            dic.TryGetValue(keyname, out v);
            if (v != null)
                return Convert.ToInt32(v);

            return 0;
        }

        public static void debug(string msg)
        {

        }

        public static bool skill_aff_type_is_self(int aff)
        {
            return (aff & (int)skill_aff_type.SAT_SELF) == (int)skill_aff_type.SAT_SELF;
        }

        public static bool skill_aff_type_is_ally(int aff)
        {
            return (aff & (int)skill_aff_type.SAT_ALLY) == (int)skill_aff_type.SAT_ALLY;
        }

        public static bool skill_aff_type_is_mid(int aff)
        {
            return (aff & (int)skill_aff_type.SAT_MID) == (int)skill_aff_type.SAT_MID;
        }

        public static bool skill_aff_type_is_enermy(int aff)
        {
            return (aff & (int)skill_aff_type.SAT_ENERMY) == (int)skill_aff_type.SAT_ENERMY;
        }

        public static State_Conf get_skil_state_desc(int state_id)
        {

        }

        public static bstate_conf get_bstate_desc(int bstate_id)
        {

        }

        public static Variant ConvertXMLtoVariant(string xml_file_name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xml_file_name);

            Variant root = new Variant();

            pushVariant(doc.ChildNodes[0] as XmlElement, root);

            return root;

        }

        public static void pushVariant(XmlElement element, Variant v)
        {
            Variant vx = new Variant();
            foreach (XmlAttribute attr in element.Attributes)
                vx[attr.Name] = attr.Value;

            if (v.ContainsKey(element.Name))
            {
                var ob = v[element.Name];
                if (!ob.isArr)
                {
                    var new_ob = new Variant();
                    new_ob.pushBack(ob);
                    v[element.Name] = new_ob;
                    ob = new_ob;
                }

                ob.pushBack(vx);
            }
            else
            {
                v[element.Name] = vx;
            }

            if (element.ChildNodes.Count > 0)
            {
                foreach (var elx in element.ChildNodes)
                {
                    if (elx is XmlElement)
                        pushVariant(elx as XmlElement, vx);
                }
            }
        }
    }
}
