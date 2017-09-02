using UnityEngine;
using SysUtils;
using System;
using System.Collections.Generic;
using System.Collections;

public class UtilTools
{
    public static void GameInitFailed()
    {
        Config.bGameInitFailed = true;
//        GuideUI.StaticShowMBox(1, Config.GetUdpateLangage("GameDataError"), UtilTools.QuitGame, null);
    }
    /// <summary>
    /// 用户自己退出确定
    /// </summary>
    public static void QuitGame()
    {
//        DataCollectionManager.Instance.SubmitCollect(DataCollectType.GAME_END);
//        SnailPluginsUtils.pcPlugins.U3D_Exit();
        Application.Quit();
    }

    public static float GetGroundHeightAt(Vector3 target)
    {
        RaycastHit hitdist;
        int mask = 1 << LayerMask.NameToLayer("Ground");
        Ray r = new Ray();
        //   r.origin = new Vector3(target.x, 50, target.z);
        Vector3 tempV = Vector3.zero;
        tempV.x = target.x;
        tempV.y = 50;
        tempV.y = target.z;
        r.origin = tempV;

        r.direction = Vector3.down;

        if (Physics.Raycast(r, out hitdist, 60, mask))
        {
            return hitdist.point.y + 0.2f;
        }
        return 0f;
    }

    /// <summary>
    ///  根据设置人数  获得当前高低模检测距离
    /// </summary>
    /// <param name="maxPlayerNum"></param>
    /// <returns></returns>
    public static float GetLowModelCheckDis(int maxPlayerNum)
    {
        float tempfloat = 0.0f;
//        if (maxPlayerNum < GameConfigManager.CheckLowModelPlayerValue)
//            tempfloat = GameConfigManager.CheckLowModelDis1Value;
//        else if (maxPlayerNum >= 20)
//            tempfloat = GameConfigManager.CheckLowModelDis2Value;
//
//        if (tempfloat == 0.0f)
//            tempfloat = 15.0f;
        return tempfloat;
    }
	/*
    /// <summary>
    /// 获取obj Radius大小
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
    public static float GetObjRadius(CGameObject cobj)
    {
        if (cobj == null)
            return 1f;

        if (UtilTools.cobjIsPlayer(cobj))
        {
            if (cobj.mModelInfo != null)
            {
                return cobj.mModelInfo.fScaleByModelValue;
            }
            else
                return 1f;
        }
        else
        {
            CNpcObject cnpcObj = cobj as CNpcObject;
            if (cnpcObj == null || cnpcObj.mNpcInfo == null)
                return 1f;

            return cnpcObj.mNpcInfo.fRadius;
        }
    }*/

   /* /// <summary>
    /// 是否还需要移动
    /// </summary>
    /// <param name="cgobj"></param>
    /// <returns></returns>
    public static bool BeNeedMove(CGameObject cgobj)WEIBOBUG
    {
        bool bMove = false;

        if (cgobj.mMoveState || !MathUtils.Distance2DLessEqual(cgobj.DynamicUnitPosition, cgobj.vDestPos, GameObjectDefine.MOVE_SKILL_CHECK_MIN_DISTANCE))
            bMove = true;

        return bMove;
    }*/

    /// <summary>
    /// 技能效果持续时间
    /// </summary>
    /// <returns></returns>
    public static float GetSkillEffectTime(string strSkillID)
    {
        if (!string.IsNullOrEmpty(strSkillID))
        {
//            SkillControl.SkillInfo skillInfo = SkillControl.GetSkillInfo(strSkillID);
//            if (skillInfo != null)
//            {
//                string effectID = SkillControl.GetPlayEffectID(skillInfo);
//                if (!string.IsNullOrEmpty(effectID))
//                {
//                    EffectGroupInfo mEffectInfo = Instance.Get<PlayEffectGroupContorl>().GetEffectInfo(effectID);
//                    if (mEffectInfo != null)
//                        return mEffectInfo.fTotalEffectTime;
//                }
//            }
        }
        return 0.0f;
    }

//    /// <summary>
//    /// 切换对应的坐骑id
//    /// </summary>
//    /// <param name="iRideID">玩家对象</param>
//    /// <param name="beFly">是否飞行中</param>
//    /// <returns></returns>
//    public static void SelectRideModleID (CGameObject cobj , bool beFly)//WEIBOBUG
//    {
//        if (cobj == null)
//            return;
//
//        int rideTempID = CHelper.QueryCustomInt(cobj, CustomPropDefine.ActivatedRideCustom);
//        cobj.SetInt(CustomPropDefine.ActivatedRideCustom, SwithRideID(rideTempID, beFly));
//    }

    /// <summary>
    /// 切换对应的坐骑id
    /// </summary>
    /// <param name="iRideID">当前坐骑id</param>
    /// <param name="beFly">是否飞行中</param>
    /// <returns></returns>
    public static int SwithRideID(int modelID, bool beFly)
    {
        if (modelID != 0)
        {
//            RideModel _rideModel = MountManager.GetRideModelByID(modelID); // 获取ride_model配置
//            if (_rideModel != null)
//            {
//                RideStarUP _rideStar = MountManager.GetRideStarByModelID(_rideModel.RideID, modelID);
//                if (_rideStar != null)
//                {
//                    if (beFly)
//                        return _rideStar.flyModelID;
//                    else
//                        return _rideStar.modelID;
//                }
//            }
        }
        return modelID;
    }

//    /// <summary>
//    /// 得到boss挂点位置,获取最近的脚
//    /// </summary>
//    /// <param name="cTempTarget"></param>
//    /// <returns></returns>
	//    public static Vector3 GetBossMuiltPoint(CGameObject cTempTarget)//, float distance//WEIBOBUG
//    {
//        Vector3 rolePos = ObjectManager.mRole.Position;
//        Vector3 vResult = cTempTarget.Position;
//        if (!(cTempTarget is CNpcObject))
//        {
//            return vResult;
//        }
//
//        CNpcObject npc = cTempTarget as CNpcObject;
//        if (npc.mNpcInfo == null || !npc.mNpcInfo.bMultiPoint)
//            return vResult;
//
//        //需要得到最近点
//        Transform[] trans = FindTag(npc.mVisualTrans, GameObjectDefine.MuiltPoint_Boss);
//        float sqrMinDistace = float.MaxValue;
//        for (int j = 0; j < trans.Length; j++)
//        {
//            float x = trans[j].position.x - rolePos.x;
//            float z = trans[j].position.z - rolePos.z;
//            if (x * x + z * z < sqrMinDistace)
//            {
//                sqrMinDistace = x * x + z * z;
//                vResult = trans[j].position;
//            }
//        }
//        return vResult;
//    }
//
    // 根据对象飞行高度  计算出当前的剔除距离的比例
	//    public static float getFlyCullingValue(CGameObject cobj)//WEIBOBUG
//    {
//        if (cobj == null || cobj.mDynamicUnit == null)
//            return 1.0f;
//
//        float sampleHeight = GameScene.mainScene.SampleHeight(cobj.DynamicUnitPosition);
//        if (MountManager.getSceneMaxFlyHeight(GameSceneManager.miSceneID) <= sampleHeight)
//            return 1.0f;
//
//        string sSceneID = GameSceneManager.mstrSceneResource;
//        SceneConfigInfo CameraInfoData = SceneConfigManager.GetCameraInfo(sSceneID);
//        return ((cobj.DynamicUnitPosition.y - sampleHeight) / (MountManager.getSceneMaxFlyHeight(GameSceneManager.miSceneID) - sampleHeight) * CameraInfoData.fCullingRate);
//    }

//    // 根据角色设置 剔除距离比例
	//    public static void SetTrrainCullingRateByCobj(CGameObject cobj)//WEIBOBUG
//    {
//        float cullValue = getFlyCullingValue(cobj);
//        if (cullValue < 1.0f)
//            cullValue = 1.0f;
//        //LogSystem.LogError("===============================  " + cullValue);
//        GameScene.mainScene.SetTrrainCullingRate(cullValue);
//    }

    /// <summary>
    /// 是否是双手武器
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
	//    public static bool CobjIsLeftWeapon(CGameObject cobj)//WEIBOBUG
//    {
//        string strWeaponModel = CHelper.QueryPropertyString(cobj, ObjPropDefine.Weapon);
//        if (string.IsNullOrEmpty(strWeaponModel))
//            return false;
//
//        return strWeaponModel.Contains("#");
//    }

    /// <summary>
    /// 影藏 部件 特效类
    /// </summary>
    /// <param name="cObj"></param>
    public static void HideGameObjectLoadEffect(GameObject  cObj)
    {
        if (cObj == null)
            return;

        ModelLoadEffect[] effects = cObj.transform.GetComponentsInChildren<ModelLoadEffect>();
        if (effects != null && effects.Length >0)
        {
            for (int i = 0; i < effects.Length; ++i)
            {
                ModelLoadEffect _tempEffect = effects[i];
                if (_tempEffect != null && (_tempEffect.mEffectType == ModelLoadEffect.EffectType.FBX || _tempEffect.mEffectType == ModelLoadEffect.EffectType.WEAPON))
                    _tempEffect.enabled = false;
            }
        }
    }

    /// <summary>
    /// 是否需要组装
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
	//    public static bool cobjIsPlayer(CGameObject cobj)//WEIBOBUG
//    {
//        if (cobj == null)
//            return false;
//
//        bool bBuildPart = false;
//        if (cobj is CPlayerObject || cobj is CRoleObject)
//        {
//            bBuildPart = true;
//        }
//        else
//        {
//			CNpcObject npc = cobj as CNpcObject;
//			if (npc.mNpcInfo.iNpcType == NpcType.SceneNPC)
//				bBuildPart = true;
//			else 
//			{
//				bBuildPart = CNpcObject.bBattleNpc (cobj) || CNpcObject.bPVPAINpc(cobj) || CNpcObject.bRoomRobotNpc(cobj);
//			}
//				
//        }
//        return bBuildPart;
//    }

    /// <summary>
    /// DynamicCollider场景 位置同步 坐标检测
    /// </summary>
    /// <returns></returns>
    public static Vector3 DCScenePos(Vector3 Pos)
    {
		//        SceneConfigInfo mSceneConfigInfo = SceneConfigManager.GetCameraInfo(GameSceneManager.mstrSceneResource);//WEIBOBUG
//        if (GameScene.mainScene != null && mSceneConfigInfo != null && mSceneConfigInfo.IsUseDynamicCollider == 1)
//        {
//            Pos.y = -99f;
//            Pos.y = GameScene.mainScene.SampleHeight(Pos);
//        }
        return Pos;
    }

    /// <summary>
    /// 是否有服务器数据
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
	//    public static bool bObjHasServerData(CGameObject cobj)//WEIBOBUG
//    {
//        if (cobj is CPlayerObject || cobj is CRoleObject)
//        {
//            return true;
//        }
//        else
//        {
//            return CNpcObject.bBattleNpc(cobj);
//        }
//    }

	//    public static void PrintVarlist(string strTitle, VarList args, bool flag = false)//WEIBOBUG
//    {
//        if (!flag)
//            return;
//        string str = UtilTools.StringBuilder("[", strTitle, " ", DateTime.Now.ToLongTimeString(), "]");
//
//        for (int i = 0; i < args.GetCount(); ++i)
//        {
//            switch (args.GetType(i))
//            {
//                case VarType.Bool:
//                    str += (args.GetBool(i) ? "true" : "false");
//                    break;
//
//                case VarType.Int:
//                    str += args.GetInt(i);
//                    break;
//
//                case VarType.String:
//                    str += args.GetString(i);
//                    break;
//
//                case VarType.WideStr:
//                    str += args.GetWideStr(i);
//                    break;
//                case VarType.Object:
//                    str += args.GetObject(i);
//                    break;
//                case VarType.Int64:
//                    str += args.GetInt64(i);
//                    break;
//                case VarType.Float:
//                    str += args.GetFloat(i);
//                    break;
//                default:
//                    str += "unknown";
//                    break;
//            }
//
//            str += " | ";
//        }
//        LogSystem.LogWarning(str);
//    }

    public static string GetSplitFirst(string orgin, char splitTag)
    {
        string result = string.Empty;

        if (orgin == null)
            return result;

        string[] strs = orgin.Split(splitTag);

        if (strs != null && strs.Length > 0)
            result = strs[0];

        return result;
    }

    /// <summary>
    /// 赋值VarList
    /// </summary>
    /// <param name="args"></param>
    /// <param name="index"></param>
    /// <param name="newList"></param>
	//    public static void CopyVarList(ref VarList args, ref VarList newList, int start, int count)//WEIBOBUG
//    {
//        int index = start;
//
//        for (; index < args.GetCount() && count > 0; index++, count--)
//        {
//            int type = args.GetType(index);
//
//            switch (type)
//            {
//                case VarType.Bool:
//                    newList.AddBool(args.GetBool(index));
//                    break;
//                case VarType.Int:
//                    newList.AddInt(args.GetInt(index));
//                    break;
//                case VarType.String:
//                    newList.AddString(args.GetString(index));
//                    break;
//                case VarType.WideStr:
//                    newList.AddWideStr(args.GetWideStr(index));
//                    break;
//                case VarType.Object:
//                    newList.AddObject(args.GetObject(index));
//                    break;
//                case VarType.Float:
//                    newList.AddFloat(args.GetFloat(index));
//                    break;
//                case VarType.Double:
//                    newList.AddDouble(args.GetDouble(index));
//                    break;
//                case VarType.Int64:
//                    newList.AddInt64(args.GetInt64(index));
//                    break;
//            }
//        }
//    }

    /// <summary>
    /// 自动根据类型读取index位置的数据 返回值为string
    /// </summary>
    /// <param name="args"></param>
    /// <param name="index"></param>
    /// <returns></returns>
	//    public static string GetVarList(VarList args, int index)//WEIBOBUG
//    {
//        int type = args.GetType(index);
//
//        switch (type)
//        {
//            case VarType.Bool:
//                return args.GetBool(index) ? "true" : "false";
//            case VarType.Int:
//                return args.GetInt(index).ToString();
//            case VarType.String:
//                return args.GetString(index);
//            case VarType.WideStr:
//                return args.GetWideStr(index);
//            case VarType.Object:
//                return args.GetObject(index).ToString();
//            case VarType.Float:
//                return args.GetFloat(index).ToString();
//            case VarType.Double:
//                return args.GetDouble(index).ToString();
//            case VarType.Int64:
//                return args.GetInt64(index).ToString();
//            default:
//                return "null";
//        }
//    }
    public static string GetIconByJob(int job)
    {
        switch (job)
        {
            case 1:
                return "zhanshi";
                break;
            case 2:
                return "mushi";
                break;
            case 3:
                return "fashi";
                break;
            case 4:
                return "youxia";
                break;
            default:
                return "zhanshi";
                break;
        }
    }
    /// <summary>
    /// 格式化字符串 id,id,id,id,id
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string[] FormatStringDot(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return value.Split(SplitChars.chDot, System.StringSplitOptions.RemoveEmptyEntries);
        }
        return new string[1] { string.Empty };
    }

    /// <summary>
    /// 格式化字符串 id,id,id 转化为int数组
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int[] FormatIntDot(string value)
    {
        int[] result = null;

        if (!string.IsNullOrEmpty(value))
        {
            string[] split = value.Split(SplitChars.chDot, System.StringSplitOptions.RemoveEmptyEntries);
            if (split != null && split.Length > 0)
            {
                result = new int[split.Length];
                for (int i = 0; i < split.Length; i++)
                {
                    result[i] = IntParse(split[i]);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 当前坐骑是否能够起飞
    /// </summary>
    public static bool CanRideFlyUp (bool beWaring = false)
    {
		//        RideModel mRideModel = MountManager.getRideModelByCobj(ObjectManager.mRole);//WEIBOBUG
//        if (mRideModel == null)// 坐骑模型不存在
//            return false;
//
//        if (mRideModel.type != RideType.RIDE) // 不是坐骑
//            return false;
//
//        if (mRideModel.IsFly != 1)// 不是飞行坐骑
//            return false;
//
//        // 当前是否可以起飞
//        // 1.当前场景是否可以飞行
//        // 2.是否有阻挡物不可以飞行
//        if (!MoveUtils.BeFlyingOff(GameSceneManager.miSceneID, mRideModel, beWaring))
//        {
//            LogSystem.LogWarning("当前位置不能起飞   或者 不在飞行场景");
//            return false;
//        }
//
//        if (SmlsMapMenager.Instance.GetNoFlyRange(ObjectManager.mRole.DynamicUnitPosition))
//        {
//            if (beWaring)
//                SystemText.ShowNoneSystemText(TextManager.GetString("UI30120"));
//            LogSystem.LogWarning("无缝地图  禁飞区内");
//            return false;
//        }
//
//        // 当前不是在地面类型（排除起飞 降落 飞行中的类型）
//        if (ObjectManager.mRole.currFlyState != (int)RideFlyState.RIDE_FLY_STATE_WALK)
//            return false;

        return true;
    }

    /// <summary>
    /// 格式化集合坐标
    /// 格式 : x,y,z ; x,y,z ; x,y,z
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static List<Vector3> FormatStringVector3(string value)
    {
        List<Vector3> list = new List<Vector3>();
        string[] ss = value.Split(SplitChars.chSemon, System.StringSplitOptions.RemoveEmptyEntries);
        //         string[] no = new string[] { };
        //         string[] num = new string[] { };

        string item = string.Empty;
        string[] temp;
        Vector3 vec;
        for (int i = 0; i < ss.Length; i++)
        {
            item = ss[i];
            temp = item.Split(SplitChars.chDot, System.StringSplitOptions.RemoveEmptyEntries);
            vec = Vector3.zero;
            if (temp.Length >= 3)
            {
                vec.x = UtilTools.FloatParse(temp[0]);
                vec.y = UtilTools.FloatParse(temp[1]);
                vec.z = UtilTools.FloatParse(temp[2]);
                list.Add(vec);
            }
        }

        return list;
    }


    /// <summary>
    /// 判断当前对象是否可以移动
    /// True 可以移动  false  不可以移动
    /// </summary>
    /// <returns></returns>
	//    public static bool getCObjCanMove(CGameObject gobj)//WEIBO
//    {
//        if (CHelper.QueryCantMove(gobj)  ||  CHelper.QueryCustomBool(gobj, CustomPropDefine.STATE_LOCK_MOVE) || CHelper.QueryCantGainMove(gobj) == 1)
//            return false ;
//        return true;
//    }


    // 获得角色跑步速度
	//    public static float getCRoleObjectRunSpeed(CGameObject _cobj)//WEIBOBUG
//    {
//        if (_cobj == null)
//            return 1.0f;
//
//        string _str = "run";
//        if (UtilTools.cobjIsPlayer(_cobj))
//        {
//            int sexVal = 0;
//            _cobj.mDataObject.QueryPropInt(ObjPropDefine.Race, ref sexVal);
//            if (sexVal == 0)
//                _str = "run";
//            else
//                _str = UtilTools.StringBuilder("run", sexVal);//string.Format("{0}{1}", "run", sexVal);
//
//            ActionInfoConfig aInfo = Instance.Get<ActionControl>().GetActionInfo(_str);
//            return aInfo.fSpeedAnimation;
//        }
//        else
//            return 1.0f;
//    }

    /// <summary>
    /// 获取当前动作文件
    /// </summary>
//    public static ActionInfoConfig GetActionInfoConfig(string actionName)//WEIBOBUG
//    {
//        return Instance.Get<ActionControl>().GetActionInfo(actionName) ;
//    }

//    // 根据角色移动速度 计算角色跑步动画播放速度
	//    public static float getCRoleObjectRunAnimSpeed(CGameObject _cobj)//WEIBOBUG
//    {
//        if (_cobj == null)
//            return 1.0f;
//
//        if (GameObjectDefine.fCommonSpeed <= 0 || _cobj.getMoveSpeed <= 0)
//            return 1.0f;
//
//        if (UtilTools.cobjIsPlayer(_cobj))
//        {
//            return _cobj.getMoveSpeed / GameObjectDefine.fCommonSpeed;
//        }
//        else
//            return 1.0f;
//    }

    /// <summary>
    /// 格式化秘籍道具字符串
    /// 格式 ： 编号:数量；编号:数量
    /// 例如 :  item0001:5;item0002:5
    /// </summary>
    /// <param name="value"></param>
    /// <param name="itemNo"></param>
    /// <param name="itemNum"></param>
    public static Dictionary<string, int> FormatKeyNumber(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new Dictionary<string, int>();
        }
        return FormatKeyNumber(value, SplitChars.chDot, SplitChars.chColon);
    }

    /// <summary>
    /// 格式化装备数据字符串
    /// 格式 ： 编号,数量；编号,数量
    /// 例如 :  item0001,5;item0002,5
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, int> FormatKeyNumberDot(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new Dictionary<string, int>();
        }
        return FormatKeyNumber(value, SplitChars.chSemon, SplitChars.chDot);
    }

    /// <summary>
    /// 格式化装备数据字符串
    /// 格式 ： 编号,数量；编号,数量
    /// 例如 :  item0001:5,item0002:5
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
	public static DictionaryEx<string, int> FormatKeyNumberDot2(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
			return new DictionaryEx<string, int>();
        }
        return FormatKeyNumber(value, SplitChars.chDot, SplitChars.chColon);
    }

    /// <summary>
    /// 格式化装备数据字符串
    /// 格式 ： 编号,数量；编号,数量
    /// 例如 :  item0001:5;item0002:5;
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Dictionary<string, int> FormatKeyNumberDot3(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return new Dictionary<string, int>();
        }
        return FormatKeyNumber(value, SplitChars.chSemon, SplitChars.chColon);
    }

    /// <summary>
    /// 格式化字符串数据
    /// </summary>
    /// <param name="value"></param>
    /// <param name="splt1"></param>
    /// <param name="splt2"></param>
    /// <returns> Dictionary<string, string> </returns>
    public static Dictionary<string, string> FormatKeyString(string value, char[] splt1, char[] splt2)
    {
        Dictionary<string, string> taskitems = new Dictionary<string, string>();
        //string[] ss = value.Split(SplitChars.chDot, System.StringSplitOptions.RemoveEmptyEntries);
        string[] ss = value.Split(splt1, System.StringSplitOptions.RemoveEmptyEntries);
        // string[] no = new string[] { };
        //string[] num = new string[] { };

        string item = string.Empty;
        string[] temp;
        for (int i = 0; i < ss.Length; i++)
        {
            item = ss[i];
            temp = item.Split(splt2, System.StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2)
            {
                if (taskitems.ContainsKey(temp[0]))
                {
                    string key = temp[0];
                    int old = UtilTools.IntParse(taskitems[key]);
                    int newi = UtilTools.IntParse(temp[1]);
                    taskitems[key] = UtilTools.StringBuilder(old, newi);
                }
                else
                {
                    taskitems.Add(temp[0], temp[1]);
                }
            }
        }

        return taskitems;
    }
    /// <summary>
    /// 根据货币类型获得货币总量 (需求绑金时返回金币加绑金的总量)
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
//    public static long GetCapitalNumByType(CapitalType type)//WEIBOBUG
//    {
//        switch (type)
//        {
//            case CapitalType.CapitalSilver:
//                return Instance.Get<MoneyData>().MSilver;
//            case CapitalType.CapitalGold:
//                return Instance.Get<MoneyData>().MGold ;
//            case CapitalType.CapitalBindGold:
//                return Instance.Get<MoneyData>().MGold + Instance.Get<MoneyData>().MBGold;
//            case CapitalType.CapitalDiamond:
//                return Instance.Get<MoneyData>().MDiamond;
//            case CapitalType.CapitalBattleSoul:
//                return Instance.Get<MoneyData>().battleSoul;
//            case CapitalType.CapitalGuild:
//                return Instance.Get<MoneyData>().guildContri;
//            case CapitalType.CapitalWarrior:
//                return Instance.Get<MoneyData>().CWarrior;
//            case CapitalType.CapitalCRFeats:
//                return Instance.Get<MoneyData>().CExploit;
//            default:
//                return -1;
//        }
//    }
    /// <summary>
    /// 根据货币类型获得货币名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
//    public static string GetCapitalNameByType(CapitalType type)//WEIBOBUG
//    {
//        switch (type)
//        {
//            case CapitalType.CapitalSilver:
//                return "CapitalSilver";
//            case CapitalType.CapitalGold:
//                return "CapitalGold";
//            case CapitalType.CapitalBindGold:
//                return "CapitalBindGold";
//            case CapitalType.CapitalDiamond:
//                return "CapitalDiamond";
//            case CapitalType.CapitalBattleSoul:
//                return "CapitalBattleSoul";
//            case CapitalType.CapitalGuild:
//                return "CapitalGuild";
//            default:
//                return string.Empty;
//        }
//    }
    /// <summary>
    /// 格式化字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
	public static DictionaryEx<string, int> FormatKeyNumber(string value, char[] first, char[] second)
    {
		DictionaryEx<string, int> taskitems = new DictionaryEx<string, int>();
        //string[] ss = value.Split(SplitChars.chDot, System.StringSplitOptions.RemoveEmptyEntries);
        string[] ss = value.Split(first, System.StringSplitOptions.RemoveEmptyEntries);
        //         string[] no = new string[] { };
        //         string[] num = new string[] { };

        string item = string.Empty;
        string[] temp;
        for (int i = 0; i < ss.Length; i++)
        {
            item = ss[i];
            temp = item.Split(second, System.StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2)
            {
                if (taskitems.ContainsKey(temp[0]))
                {
                    string key = temp[0];
                    int old = taskitems[key];
                    int newi = UtilTools.IntParse(temp[1]);
                    taskitems[key] = old + newi;
                }
                else
                {
                    taskitems.Add(temp[0], UtilTools.IntParse(temp[1]));
                }
            }
        }

        return taskitems;
    }

    /// <summary>
    /// String 强转 Int 时调用 默认返回 0
    /// 避免转换过程中包含空字符、以及非数字字符
    /// 导致程序报错
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IntParse(string value, int defaultValue = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        int result;
        if (int.TryParse(value, out result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// String强转Long
    /// </summary>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long LongParse(string value, long defaultValue = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        long result;
        if (long.TryParse(value, out result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// String 强转 Float 时调用 默认返回 0
    /// 避免转换过程中包含空字符、以及非数字字符
    /// 导致程序报错
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float FloatParse(string value, float defaultValue = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        float result;
        if (float.TryParse(value, out result))
        {
            return result;
        }
        return defaultValue;
    }

    /// <summary>
    /// 保留小数
    /// </summary>
    /// <param name="f">值</param>
    /// <param name="count">保留位数</param>
    /// <returns></returns>
    public static float KeepFloat(float f, int count = 1)
    {
        double b = System.Math.Round(f, count);
        return (float)b;
    }

    /// <summary>
    /// String 强转 bool 时调用 默认返回 false
    /// 避免转换过程中包含空字符、以及非数字字符
    /// 导致程序报错
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool BoolParse(string value, bool defaultValue = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        Boolean result;
        int iResult;
        if (Boolean.TryParse(value, out result))
        {
            return result;
        }
        else if (int.TryParse(value, out iResult))
        {
            return iResult == 1;
        }
        return defaultValue;
    }

    /// <summary>
    /// 检测文件加载完成后回调
    /// </summary>
    /// <param name="UIAsset"></param>
    /// <param name="NameAsset"></param>
    /// <param name="NumAsset"></param>
    public static void OnStringFileLoaded(System.Action completeCallback, int iTotal, bool[] args)
    {
        int iLoad = 0;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == true)
            {
                iLoad++;
            }
        }

        if (iLoad == args.Length)
        {
            ///完成回调
            if (completeCallback != null)
            {
                completeCallback();
            }
            return;
        }
    }

    public static string[] Split(string src, char p)
    {
        return src.Split(p);
    }

    public static bool isEmpty(string src)
    {
        return src == null || src == string.Empty || src.Equals(string.Empty);
    }

    public static Vector4 ParseVector4(string str)
    {
        if (isEmpty(str)) return Vector4.zero;
        string[] strp = Split(str, ',');
        if (strp == null || strp.Length != 4) return Vector4.zero;
        return new Vector4(float.Parse(strp[0]), float.Parse(strp[1]), float.Parse(strp[2]), float.Parse(strp[3]));
    }

    public static Vector3 ParseVector3(string str)
    {
        if (isEmpty(str)) return Vector3.zero;
        string[] strp = Split(str, ',');
        if (strp == null || strp.Length != 3) return Vector3.zero;
        Vector3 TempVector3 = Vector3.zero; ;
        TempVector3.x = float.Parse(strp[0]);
        TempVector3.y = float.Parse(strp[1]);
        TempVector3.z = float.Parse(strp[2]);
        return TempVector3;

        //return new Vector3(float.Parse(strp[0]), float.Parse(strp[1]), float.Parse(strp[2]));
    }

    public static bool TryParseVector3(string str, out Vector3 vector)
    {
        vector = Vector3.zero;
        if (isEmpty(str)) return false;
        string[] strp = Split(str, ',');
        if (strp == null || strp.Length != 3) return false;
        //  vector = new Vector3(float.Parse(strp[0]), float.Parse(strp[1]), float.Parse(strp[2]));
        vector.x = float.Parse(strp[0]);
        vector.y = float.Parse(strp[1]);
        vector.z = float.Parse(strp[2]);

        return true;
    }

    public static Vector2 ParseVector2(string str)
    {
        if (string.IsNullOrEmpty(str))
            return Vector2.zero;

        string[] strp = Split(str, ',');
        if (strp.Length != 2)
            return Vector2.zero;

        Vector2 vTemp2 = Vector2.zero;
        vTemp2.x = float.Parse(strp[0]);
        vTemp2.y = float.Parse(strp[1]);
        return vTemp2;
    }

//    public static Rectangle2D ParseRect(string pos, float rot, string size)//WEIBO
//    {
//        if (string.IsNullOrEmpty(size)) return new Rectangle2D();
//        string[] strp = Split(size, ',');
//        if (strp == null || strp.Length != 2) return new Rectangle2D();
//        return new Rectangle2D(ParseVector2(pos), Mathf.Deg2Rad * rot, float.Parse(strp[0]), float.Parse(strp[1]));
//    }

    public static string Vector3String(Vector3 v)
    {
        return UtilTools.StringBuilder(v.x, ",", v.y, ",", v.z);
    }

    /// <summary>
    /// 转2维数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int[,] ParseInts(string str)
    {
        int[,] temp = null;
        if (!string.IsNullOrEmpty(str))
        {
            string[] strp = str.Split(',');
            if (strp.Length % 2 == 0)
            {
                int len = strp.Length / 2;
                temp = new int[len, 2];
                for (int i = 0; i < len; i++)
                {
                    temp[i, 0] = int.Parse(strp[2 * i]);
                    temp[i, 1] = int.Parse(strp[2 * i + 1]);
                }
            }
        }
        return temp;
    }

    /// <summary>
    /// 转1维float数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float[] ParseFloat(string str, char strSymbol=',')
    {
        if (string.IsNullOrEmpty(str))
        {
            return new float[0];
        }
        string[] strTmps = str.Split(strSymbol);
        float[] tmp = new float[strTmps.Length];
        for(int i = 0; i<tmp.Length; i++)
        {
            tmp[i] = FloatParse(strTmps[i]);
        }
        return tmp;
    }

    /// <summary>
    /// 转1维int数据
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int[] ParseInt(string str, char separator=',')
    {
        if (string.IsNullOrEmpty(str))
        {
            return new int[0];
        }
        string[] strTmps = str.Split(separator);
        int[] tmp = new int[strTmps.Length];
        for(int i = 0; i<tmp.Length; i++)
        {
            tmp[i] = IntParse(strTmps[i]);
        }
        return tmp;
    }
    private static System.Text.StringBuilder mstrbuilder = new System.Text.StringBuilder();
    /// <summary>
    /// 合并字符
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string StringBuilder(params object[] args)
    {
        mstrbuilder.Remove(0, mstrbuilder.Length);
        if (args != null)
        {
            int len = args.Length;
            for (int i = 0; i < len; ++i)
            {
                mstrbuilder.Append(args[i]);
            }
        }
        return mstrbuilder.ToString();
    }
    public delegate void OnWaitFrame();
    public static IEnumerator WaitForEndOfFrame(OnWaitFrame onWaitFrame)
    {
        yield return new WaitForEndOfFrame();
        if (onWaitFrame != null)
        {
            onWaitFrame();
        }
    }
    public delegate void OnCapture(Texture2D tex2d);
    /// <summary>
    /// 截屏
    /// </summary>
    /// <param name="rt">屏幕范围</param>
    /// <returns></returns>
    public static IEnumerator CaptureScreenshot(Rect rt, OnCapture onCapture)
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenShot = new Texture2D((int)rt.width, (int)rt.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rt, 0, 0);
        screenShot.Apply();
        if (onCapture != null)
        {
            onCapture(screenShot);
        }
    }
    /// <summary>
    /// 保存分享文件到指定路径
    /// </summary>
    /// <param name="strFileName">文件名</param>
    /// <param name="tex2d">图片信息</param>
    public static void SaveScreenShot(string strFileName, Texture2D tex2d)
    {
        byte[] bytes = tex2d.EncodeToPNG();
        if (bytes != null)
        {
            System.IO.File.WriteAllBytes(strFileName, bytes);
        }
    }

    /// <summary>
    /// 获取字符串中的url参数
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string GetUrlValue(string text)
    {
        int linkStart = text.IndexOf("[url=", 0);
        if (linkStart != -1)
        {
            linkStart += 5;
            int linkEnd = text.IndexOf("]", linkStart);

            if (linkEnd != -1)
            {
                int closingStatement = text.IndexOf("[/url]", linkEnd);
                if (closingStatement != -1)
                    return text.Substring(linkStart, linkEnd - linkStart);
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 更改下滑线颜色
    /// </summary>
    /// <param name="text"></param>
    /// <param name="startTag"></param>
    /// <param name="endTag"></param>
    /// <returns></returns>
    public static string SetUnderlineColor(string text,string colorTag)
    {
        text = text.Replace("[u]", colorTag + "[u]");
        text = text.Replace("[/u]", "[/u][-]");


        return text;
    }


    /// <summary>
    /// 得到位置随机点
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dis"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPos(Vector3 pos, float minDis, float maxDis)
    {
        float radius = UnityEngine.Random.Range(minDis, maxDis);
        Quaternion rot = Quaternion.Euler(0, UnityEngine.Random.Range(0, 359), 0);
        Vector3 _Pos = Vector3.forward * radius;
        pos += rot * _Pos;
        return pos;
    }

    /// <summary>
    /// 随机到可行走区域
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="minDis"></param>
    /// <param name="maxDis"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPosAndCanStand(Vector3 pos, float minDis, float maxDis)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 _pos = GetRandomPos(pos, minDis, maxDis);
            if (GameScene.mainScene.IsValidForWalk(_pos, 1))
            {
                return _pos;
            }
            if (minDis > 0)
                minDis--;
        }
        return pos;
    }

    /// <summary>
    /// 掉落位置
    /// </summary>
    /// <param name="qua"></param>
    /// <param name="pos"></param>
    /// <param name="minZ"></param>
    /// <param name="maxZ"></param>
    /// <param name="minX"></param>
    /// <param name="maxX"></param>
    /// <returns></returns>
    public static Vector3 GetBoxObjectPos(Vector3 pos, Quaternion qua, int z, int x)
    {
        // Vector3 dir = new Vector3(x, 0, z);
        Vector3 dir = Vector3.zero;
        dir.x = x;
        dir.y = 0;
        dir.z = z;
        Vector3 p = qua * dir + pos;
        return p;
    }

    /// <summary>
    /// 随机位置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="qua"></param>
    /// <param name="z"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static Vector3 GetBoxObjectRandomPos(Vector3 pos, Quaternion qua, float fminz, float fmaxz, float fminx, float fmaxx)
    {
        float x = UnityEngine.Random.Range(fminx, fmaxx);
        float z = UnityEngine.Random.Range(fminz, fmaxz);
        //Vector3 dir = new Vector3(x, 0, z);
        Vector3 dir = Vector3.zero;
        dir.x = x;
        dir.y = 0;
        dir.z = z;
        Vector3 p = qua * dir + pos;
        return p;
    }

    /// <summary>
    /// 通过秒转换为时间格式
    /// </summary>
    /// <param name="sceond"></param>
    /// <returns></returns>
    public static string TimeFormat(int sceond)
    {
        DateTime me = new DateTime().AddSeconds(sceond);
        return UtilTools.StringBuilder(me.Hour.ToString("00"), ":", me.Minute.ToString("00"), ":", me.Second.ToString("00"));
    }
    /// <summary>
    /// 秒强制转化00:00:00 非24小时
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string TimeForceFormat(int second)
    {
        int Hour = (int)((float)second / 3600);
        int Minute = (int)((float)(second - Hour * 3600) / 60);
        int Second = second - Hour * 3600 - Minute * 60;
        return UtilTools.StringBuilder(Hour.ToString("00"), ":", Minute.ToString("00"), ":",Second.ToString("00"));
    }
    /// <summary>
    /// 时间转换为00:00
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string TimeMinuteFormat(int second)
    {
        int iMinute = second / 60;
        int iSecond = second % 60;
        return UtilTools.StringBuilder(iMinute.ToString("00"), ":", iSecond.ToString("00"));
    }

    /// <summary>
    /// 根据item编号 设置texture的纹理
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="itemId"></param>
    public static void SetItemTexture(UITexture texture, string itemId)
    {
		string path = string.Empty;// = UtilsProp.GetItemIconPath(itemId);
        SetMainTexture(texture, path);
    }

    /// <summary>
    /// 根据path设置texture的纹理
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="path"></param>
    public static void SetMainTexture(UITexture texture, string path)
    {
        if (texture == null) return;

        ResourceManager.LoadAsset(path, (UnityEngine.Object oAsset, string strFileName, VarStore varStore) =>
        {
            if (oAsset != null)
            {

                if (oAsset is Material)
                {
                    texture.material = oAsset as Material;
                }
                else if (oAsset is Texture)
                {
                    texture.mainTexture = oAsset as Texture;
                }
            }
            else
            {
                texture.mainTexture = null;
                texture.material = null;
            }
            
        });
    }

    /// <summary>
    /// 获取对象世界坐标
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static bool GetObjectWorldPos(GameObject obj, ref Vector3 vServerPos)
    {
        if (obj == null)
            return false;

        Transform trans = obj.transform;
        while (true)
        {
            if (trans.parent == null)
            {
                vServerPos = trans.position;
                return true;
            }
            trans = trans.parent;
        }
    }

    /// <summary>
    /// 随机数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputList"></param>
    /// <returns></returns>
    public static List<T> GetRandomList<T>(List<T> inputList)
    {
        //Copy to a array
        T[] copyArray = new T[inputList.Count];
        inputList.CopyTo(copyArray);

        //Add range
        List<T> copyList = new List<T>();
        copyList.AddRange(copyArray);

        //Set outputList and random
        List<T> outputList = new List<T>();
        System.Random rd = new System.Random(DateTime.Now.Millisecond);

        while (copyList.Count > 0)
        {
            //Select an index and item
            int rdIndex = rd.Next(0, copyList.Count - 1);
            T remove = copyList[rdIndex];

            //remove it from copyList and add it to output
            copyList.Remove(remove);
            outputList.Add(remove);
        }
        return outputList;
    }

    /// <summary>
    /// 解析道具列表信息
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
//    public static List<PackItemInfo> GetAttachmentsInfo(string items)//WEIBOBUG
//    {
//        List<PackItemInfo> mAttachments = new List<PackItemInfo>();
//        ShowTip sTip = Instance.Get<ShowTip>();
//        // 获取资产信息;
//        if (!string.IsNullOrEmpty(items))
//        {
//            // 信息格式："CapitalSilver:10,CapitalBindGold:10,CapitalSmelt:10,CapitalHonor:10";
//            string[] tmp1 = items.Split(',');
//            for (int i = 0; i < tmp1.Length; i++)
//            {
//                if (string.IsNullOrEmpty(tmp1[i]))
//                    continue;
//                string[] tmp2 = tmp1[i].Split(':');
//                string id = tmp2[0];
//                int number = UtilTools.IntParse(tmp2[1], -1);
//                if (number != -1)
//                {
//                    PackItemInfo pInfo = sTip.GetItemInfo(id, number);
//                    if (!string.IsNullOrEmpty(pInfo.iconPath))
//                        mAttachments.Add(pInfo);
//                }
//            }
//        }
//        return mAttachments;
//    }

    public static void destoryObject(string mstrEffectPath, GameObject gameObject)
    {
        CacheObjects.DestoryPoolObject(gameObject);
    }

    /// 删除指定层
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    public static void DestroyEffect(GameObject obj, int layer)
    {
        if (obj == null)
            return;
        List<Transform> mlist = new List<Transform>();
        mlist.Add(obj.transform);
        while (mlist.Count > 0)
        {
            Transform t = mlist[0];
            mlist.RemoveAt(0);
            for (int j = t.childCount - 1; j >= 0; j--)
            {
                Transform _t = t.GetChild(j);
                if (_t.gameObject.layer == layer)
                {
                    CacheObjects.DestoryPoolObject(_t.gameObject);
                }
                else
                {
                    if (_t.childCount != 0)
                    {
                        mlist.Add(_t);
                    }
                }
            }
        }
    }

    private static int g_UILayer = LayerMask.NameToLayer("UI");
    private static int g_UILayer2 = LayerMask.NameToLayer("UIEffect");

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    public static void SetLayer(GameObject go, int layer)
    {
        if (go.layer == layer)
        {
            if (go.transform.childCount > 0)
            {
                int len = go.transform.childCount;
                for (int i = 0; i < len; i++)
                {
//                    if (go.transform.GetChild(i).gameObject.layer != LayerMask.NameToLayer(GameObjectDefine.LayerDynamicCollier)) // DynamicCollier 层不做处理
//                        SetLayer(go.transform.GetChild(i).gameObject, layer);//WEIBOBUG
                }
            }
        }
        else
        {
            if (go.layer == g_UILayer2 && layer == g_UILayer)
            {
                NGUITools.AdjustParticlesToWorld(go);
            }


            go.layer = layer;
            if (go.transform.childCount > 0)
            {
                int len = go.transform.childCount;
                for (int i = 0; i < len; i++)
                {
//                    if (go.transform.GetChild(i).gameObject.layer != LayerMask.NameToLayer(GameObjectDefine.LayerDynamicCollier)) // DynamicCollier 层不做处理
//                        SetLayer(go.transform.GetChild(i).gameObject, layer);//WEIBOBUG
                }
            }
        }
    }

    /// <summary>
    ///  获取数量译文(数量超过万保留千位显示x.x万)(数量超过亿保留千万显示x.x亿)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConvertNumber(long value)
    {
//
//		if(Language.ResoucreLanguage != "NorthAmerica" )
//		{
//			long hundredmillion = 100000000;
//			long tenthousand = 10000;
//
//			if (value >= hundredmillion)
//			{
//				float tmp = (float)((int)((float)value / 10000000)) / 10;
//				return StringBuilder(tmp, TextManager.GetUIString("UI230114"));
//			}
//			else if (value >= tenthousand)
//			{
//				float tmp = (float)((int)((float)value / 1000)) / 10;
//				return StringBuilder(tmp,TextManager.GetUIString("UI230113"));
//			}
//		}
//		else
//		{
//			long billion =  1000000000;//
//			long million =  1000000;//
//			long thousand = 1000;//
//
//			if (value >= billion)
//			{
//				float tmp = (float)((int)((float)value / 100000000)) / 10;
//				return StringBuilder(tmp, TextManager.GetUIString("UI230117"));
//			}
//			else if (value >= million)
//			{
//				float tmp = (float)((int)((float)value / 100000)) / 10;
//				return StringBuilder(tmp, TextManager.GetUIString("UI230114"));
//			}
//			else if (value >= thousand)
//			{
//				float tmp = (float)((int)((float)value / 100)) / 10;
//				return StringBuilder(tmp,TextManager.GetUIString("UI230113"));
//			}
		//		}//WEIBOBUG	
       
        return value.ToString();
    }

    #region shader设置相关

    public const string SnailCommonShader = "Snail/Common-Shader";

    /// <summary>
    /// 无高光法线
    /// </summary>
    public const string SnailUnlitShadowmapName = "Snail/Unlit_Shadowmap";

    /// <summary>
    /// 角色场景中shader
    /// </summary>
    public static Shader StandardSpecularFog = Shader.Find(SnailCommonShader);

    /// <summary>
    /// 改变角色光照
    /// </summary>
    /// <param name="role"></param>
    /// <param name="changeOrRevert">0切换为界面中的光照，1为还原</param>
    /// <param name="strPrefix"></param>
    public static void ChangeLight(GameObject role, int changeOrReverr, GameObject dLight = null)
    {
        if (role == null)
        {
            return;
        }

        if (changeOrReverr == 1)
        {
            if (GameScene.mainScene != null)
            {
                if (!GameScene.mainScene.UpdateSingleLightData(role))
                {
                    //在游戏中获取不到灯光需要提示报错
//                    if (WorldStage.mbInScene)//WEIBOBUG
//                    {
//                        LogSystem.LogWarning("customLight in scene can't find");
//                    }
                }
            }
        }
        else
        {
            if (dLight != null)
            {
                DirectionalLightForCharacter dl = dLight.GetComponentInChildren<DirectionalLightForCharacter>();
                if (dl != null)
                {
                    dl.UpdateSingleLightData(role);
                }
                else
                {
                    LogSystem.LogWarning("customLight in view can't be null");
                }
            }
            else
            {
                LogSystem.LogWarning("customLight in view can't be null");
            }
        }
    }


    /// <summary>
    /// 切换自定义shader
    /// </summary>
    /// <param name="role"></param>
    /// <param name="changeOrRevert">0切换为界面中的shader，1为还原</param>
    /// <param name="strPrefix"></param>
    public static void ChangeCustomShader(GameObject role, string sourceShader ,string targetShader)
    {
        if (role == null)
        {
            return;
        }

        Shader sShader = Shader.Find(sourceShader);
        Shader tShader = Shader.Find(targetShader);
        if(sShader == null || tShader == null)
            return;


        Renderer[] mSkinnedMeshRender = role.GetComponentsInChildren<Renderer>(true);
        if (mSkinnedMeshRender == null)
            return;

        Material[] mats;
        for (int i = 0; i < mSkinnedMeshRender.Length; i++)
        {
            mats = mSkinnedMeshRender[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                if (mats[j] == null)
                    continue;

                if (mats[j].shader == sShader)
                {
                    mats[j].shader = tShader;
                }
            }
        }

    }

    /// <summary>
    /// buff替换shader 遍历替换所有shader
    /// </summary>
    /// <param name="o"></param>
    /// <param name="strShaderName"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    public static void BuffChangeShader(GameObject o, string strShaderName, int r = 0, int g = 0, int b = 0, int a = 0)
    {
        if (o == null)
            return;

        Shader shader = Shader.Find(strShaderName);
        if (shader == null)
        {
            LogSystem.LogWarning(strShaderName, " shader not found");
            return;
        }
        Renderer[] renderer = o.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderer.Length; i++)
        {
            renderer[i].material.shader = shader;
            if (r != 0 || g != 0 && b != 0 && a != 0)
            {
                Color color = new Color(Mathf.Clamp01(r / 255f), Mathf.Clamp01(g / 255f), Mathf.Clamp01(b / 255f), Mathf.Clamp01(a / 255f));
                renderer[i].material.SetColor("_RimColor", color);
            }
        }
    }

    /// <summary>
    /// 增加材质
    /// </summary>
    /// <param name="o"></param>
    /// <param name="newMat"></param>
    public static void AddMaterial(Renderer[] renderers, Material newMat)
    {
        if (renderers == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            AddMaterial(renderers[i], newMat);
        }
    }

    /// <summary>
    /// 增加材质
    /// </summary>
    /// <param name="o"></param>
    /// <param name="newMat"></param>
    public static void AddMaterial(Renderer renderer, Material newMat)
    {
        if (renderer == null)
            return;

        Material[] mats = new Material[renderer.materials.Length + 1];
        for (int j = 0; j < renderer.materials.Length; j++)
        {
            mats[j] = renderer.materials[j];
        }
        mats[renderer.materials.Length] = newMat;
        renderer.materials = mats;
    }

    /// <summary>
    /// 复原以增加的材质
    /// </summary>
    /// <param name="o"></param>
    /// <param name="addMat"></param>
    public static void SubtractMaterial(Renderer[] renderers, Material addMat)
    {
        if (renderers == null)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            SubtractMaterial(renderers[i], addMat);
        }
    }

    /// <summary>
    /// 复原以增加的材质
    /// </summary>
    /// <param name="o"></param>
    /// <param name="addMat"></param>
    public static void SubtractMaterial(Renderer renderer, Material addMat)
    {
        Material[] mats;
        int iArrayLength = renderer.materials.Length;
        if (bContainMat(renderer.materials, addMat))
        {
            mats = new Material[renderer.materials.Length - 1];
            int i = 0;
            for (int j = 0; j < iArrayLength; j++)
            {
                if (renderer.materials[j].shader.name == addMat.shader.name)
                    continue;

                mats[i] = renderer.materials[j];
                i++;
            }
            renderer.materials = mats;
        }
    }

    /// <summary>
    /// 包含材质
    /// </summary>
    /// <param name="mats"></param>
    /// <param name="mat"></param>
    /// <returns></returns>
    public static bool bContainMat(Material[] mats, Material mat)
    {
        if (mat == null)
            return false;

        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i].shader.name == mat.shader.name)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 设置对象透明
    /// </summary>
    /// <param name="cobj"></param>
    /// <param name="fTransValue"></param>
    /// <returns></returns>
//    public static bool SetObjectTransparent(CGameObject cobj, float fTransValue)
//    {
//        if (cobj == null)
//            return false;
//
////        int iStart = (int)BuildPartType.FBX;//WEIBOBUG
////        int iEnd = (int)BuildPartType.Max;
////        for (int i = iStart; i < iEnd; i++)
////        {
////            if (!SetObjectTransparent(cobj.GetRenderer(i), fTransValue))
////            {
////#if UNITY_EDITOR
////                if (i == (int)BuildPartType.FBX || i == (int)BuildPartType.Hair || i == (int)BuildPartType.Weapon)
////                {
////                    LogSystem.Log("SetObjectTransparent: renderer is null ", fTransValue, " ", (BuildPartType)i);
////                }
////#endif
////            }
////        }
//        return true;
//    }

    /// <summary>
    /// 设置对象透明
    /// </summary>
    /// <param name="renderers"></param>
    /// <returns></returns>
    public static bool SetObjectTransparent(Renderer[] renderers, float fTransValue)
    {
        if (renderers == null)
        {
            return false;
        }

        for (int i = 0; i < renderers.Length; i++ )
        {
            SetObjectTransparent(renderers[i], fTransValue);
        }
        return true;
    }

    /// <summary>
    /// 设置对象透明
    /// </summary>
    /// <param name="oRoot"></param>
    /// <returns></returns>
    public static bool SetObjectTransparent(Renderer renderer, float fTransValue)
    {
        if (renderer == null)
        {
            LogSystem.Log("SetObjectTransparent: renderer is null ", fTransValue);
            return false;
        }
        string strRenderType = "RenderType";
        string strSrcBlend = "_SrcBlend";
        string strDstBlend = "_DstBlend";
        string strZWrite = "_ZWrite";
        string strALPHABLEND_ON = "_ALPHABLEND_ON";
        string strTransparent = "Transparent";
        Material mat;
        for (int j = 0; j < renderer.materials.Length; j++)
        {
            mat = renderer.materials[j];
            if (mat == null)
                continue;

            if (mat.shader.name.Equals(SnailCommonShader) || mat.shader.name.Equals(SnailUnlitShadowmapName))
            {
                mat.SetOverrideTag(strRenderType, strTransparent);
                mat.SetInt(strSrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt(strDstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt(strZWrite, 0);
                mat.EnableKeyword(strALPHABLEND_ON);
                mat.renderQueue = 3000;
                Color cColor = mat.color;
                cColor.a = fTransValue;
                mat.color = cColor;
            }
        }
        return true;
    }

    /// <summary>
    /// 设置对象不透明
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
	//    public static bool SetObjectOpaque(CGameObject cobj)//WEIBOBUG
//    {
//        if (cobj == null)
//            return false;
//
////        int iStart = (int)BuildPartType.FBX;//WEIBOBUG
////        int iEnd = (int)BuildPartType.Max;
////        for (int i = iStart; i < iEnd; i++)
////        {
////            SetObjectOpaque(cobj.GetRenderer(i));
////        }
//        return true;
//    }

    /// <summary>
    /// 设置对象不透明
    /// </summary>
    /// <param name="renderers"></param>
    /// <returns></returns>
    public static bool SetObjectOpaque(Renderer[] renderers)
    {
        if (renderers == null)
            return false;

        for (int i = 0; i < renderers.Length; i++ )
        {
            SetObjectOpaque(renderers[i]);
        }
        return true;
    }

    /// <summary>
    /// 设置对象不透明
    /// </summary>
    /// <param name="oRoot"></param>
    /// <returns></returns>
    public static bool SetObjectOpaque(Renderer renderer)
    {
        if (renderer == null)
            return false;

        string strRenderType = "RenderType";
        string strSrcBlend = "_SrcBlend";
        string strDstBlend = "_DstBlend";
        string strZWrite = "_ZWrite";
        string strALPHABLEND_ON = "_ALPHABLEND_ON";
        Material mat;
        for (int j = 0; j < renderer.materials.Length; j++)
        {
            mat = renderer.materials[j];
            if (mat == null)
                continue;

            if (mat.shader.name.Equals(SnailCommonShader) || mat.shader.name.Equals(SnailUnlitShadowmapName))
            {
                mat.SetOverrideTag(strRenderType, "");
                mat.SetInt(strSrcBlend, (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt(strDstBlend, (int)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetInt(strZWrite, 1);
                mat.DisableKeyword(strALPHABLEND_ON);
                mat.renderQueue = -1;
                Color cColor = mat.color;
                cColor.a = 1;
                mat.color = cColor;
            }
        }
        return true;
    }

    #endregion

    /// <summary>
    /// 像机震屏
    /// </summary>
    /// <param name="str"></param>
    /// <param name="isNeedRole"></param>
    public static void ShakeCamera(string strParam)
    {
//        if (string.IsNullOrEmpty(strParam))//WEIBOBUG
//            return;
//
//        if (FirstPersonCameraControl.Instance == null)
//            return;
//
//        string[] strProp = strParam.Split('-');
//        if (strProp.Length == 3)
//        {
//            FirstPersonCameraControl.Instance.StartShakeCamera(
//                UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[2]));
//        }
//        else if (strProp.Length == 4)
//        {
//            FirstPersonCameraControl.Instance.StartShakeCamera(
//                UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[1])
//                , UtilTools.FloatParse(strProp[2]), UtilTools.FloatParse(strProp[3]), UtilTools.FloatParse(strProp[3]));
//        }
//        else if (strProp.Length == 5)
//        {
//            FirstPersonCameraControl.Instance.StartShakeCamera(
//                UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[2]), UtilTools.FloatParse(strProp[3]), UtilTools.FloatParse(strProp[4]));
//        }
//        else if (strProp.Length == 7)
//        {
//            FirstPersonCameraControl.Instance.StartShakeCamera(
//                UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[2])
//                , UtilTools.FloatParse(strProp[3]), UtilTools.FloatParse(strProp[4]), UtilTools.FloatParse(strProp[5]), UtilTools.FloatParse(strProp[6]));
//        }
    }

    //public static void ShakeCameraEnd(string strParam)
    //{
    //    if (string.IsNullOrEmpty(strParam))
    //        return;

    //    if (FirstPersonCameraControl.Instance == null)
    //        return;

    //    string[] strProp = strParam.Split('-');
    //    if (strProp.Length == 3)
    //    {
    //        FirstPersonCameraControl.Instance.EndShakeCamera(
    //            UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[2]));
    //    }
    //}

    /// <summary>
    /// 拉推像机
    /// </summary>
    /// <param name="strParam"></param>
    public static void AutoZoomInAndZoomOutCamera(string strParam)
    {
//        if (FirstPersonCameraControl.Instance == null)
//            return;
//
//        if(string.IsNullOrEmpty(strParam))
//            return;
//
//        string[] strProp = strParam.Split('|');
//        if (strProp.Length == 4)
//        {
//            FirstPersonCameraControl.Instance.AutoZoomInAndZoomOutCamera(
//                UtilTools.FloatParse(strProp[0]), UtilTools.FloatParse(strProp[1]), UtilTools.FloatParse(strProp[2]), UtilTools.FloatParse(strProp[3]));
//        }
    }

    /// <summary>
    /// 获取膜拜坐标点
    /// </summary>
    /// <param name="strScene"></param>
    /// <param name="strScenePos"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool GetWorship(string strScene, string strScenePos, out Vector3 pos)
    {
        pos = Vector3.zero;
//        if (string.IsNullOrEmpty(strScenePos) || string.IsNullOrEmpty(strScene))
//            return false;
//        string[] strTemps = strScenePos.Split(',');
//        for (int i = 0; i < strTemps.Length; i += 3)
//        {
//            if (strScene == strTemps[i])
//            {
//                pos.x = IntParse(strTemps[i + 1]);
//                pos.z = IntParse(strTemps[i + 2]);
//                return true;
//            }
//        }
        return false;
    }




    /// <summary>
    /// 设置RenderQ
    /// </summary>
    /// <param name="oModel"></param>
    /*public static void SetUIModelRenderQ(GameObject oModel, int renderQueue = 3100)
    {
        if (oModel == null)
            return;
        int effect = LayerMask.NameToLayer("Effect");
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == effect)// 
                {
                    rend.material.renderQueue = renderQueue;//将特效显示在最上层
                }
            }
        }

        ParticleSystemRenderer[] particles = oModel.GetComponentsInChildren<ParticleSystemRenderer>(true);
        if (particles != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == effect)// && 
                {
                    rend.material.renderQueue = renderQueue;//将特效显示在最上层
                }
            }
        }
    }*/

    /// <summary>
    /// 隐藏粒子特效
    /// </summary>
    /// <param name="oModel"></param>
    public static void HideParticles(GameObject oModel)
    {
//        if (oModel == null)
//            return;
//
//        ParticleSystem[] ps = oModel.GetComponentsInChildren<ParticleSystem>();
//        if (ps != null && ps.Length != 0)
//        {
//            for (int i = 0; i < ps.Length; i++)
//            {
//                ps[i].GetComponent<Renderer>().enabled = false;
//            }
//        }
    }

    /// <summary>
    /// 隐藏粒子特效
    /// </summary>
    /// <param name="oModel"></param>
    public static void OpenParticles(GameObject oModel)
    {
        if (oModel == null)
            return;

        ParticleSystem[] ps = oModel.GetComponentsInChildren<ParticleSystem>();
        if (ps != null && ps.Length != 0)
        {
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].GetComponent<Renderer>().enabled = true;
            }
        }
    }




    public static Vector3 GetScaleByParent(Transform trans)
    {
        Vector3 vScale = Vector3.one;
//        if (trans == null)
//            return vScale;
//
//        vScale.x = trans.localScale.x;
//        vScale.y = trans.localScale.x;
//        vScale.z = trans.localScale.x;
//        Transform tParent = trans.parent;
//        while (tParent != null)
//        {
//            vScale.x = vScale.x * tParent.localScale.x;
//            vScale.y = vScale.y * tParent.localScale.y;
//            vScale.z = vScale.z * tParent.localScale.z;
//            tParent = tParent.parent;
//        }

        return vScale;

    }
    public static void AdjustParticlesToUI(GameObject oModel)
    {
//        if (oModel == null)
//            return;
//
//        ParticleSystem particle = oModel.GetComponent<ParticleSystem>();
//        if (particle != null)
//        {
//            if (particle.transform.gameObject.layer != LayerMask.NameToLayer("UIEffect"))
//            {
//                //    LogSystem.LogError("AdjustParticlesToUI\t" + oModel.transform.name);
//
//                //Vector3 vScale = GetScaleByParent(particle.transform);
//                Matrix4x4 ltwMatrix = particle.transform.localToWorldMatrix;
//                float scaleRate = Mathf.Abs(ltwMatrix.m00);
//                //if (oModel.transform.name == "EMSZ122001_Bip01_HeadNub_1_1")
//                //{
//                //    LogSystem.LogError("********UUUUUUUUU");
//                //    LogSystem.LogError(scaleRate);
//                //}
//
//                particle.startSize *= scaleRate;
//                particle.startSpeed *= scaleRate;
//                float maxParticles = particle.maxParticles;
//                maxParticles *= scaleRate;
//                particle.maxParticles = (int)maxParticles;
//                ParticleSystemRenderer render = particle.GetComponent<ParticleSystemRenderer>();
//                if (render)
//                {
//                    render.maxParticleSize *= scaleRate;
//                }
//                particle.transform.gameObject.layer = LayerMask.NameToLayer("UIEffect");
//            }
//        }
    }
    /// <summary>
    /// 粒子特效适应界面模型
    /// </summary>
    /// <param name="oModel"></param>
    public static void AdjustParticlesToWorld(GameObject oModel)
    {
        if (oModel == null)
            return;

//        ParticleSystem[] particles = oModel.GetComponentsInChildren<ParticleSystem>();
//        if (particles != null)
//        {
//            for (int i = 0; i < particles.Length; i++)
//            {
//                ParticleSystem particle = particles[i];
//                if (particle.transform.gameObject.layer == LayerMask.NameToLayer("UIEffect"))
//                {
//                    //     LogSystem.LogError("AdjustParticlesToWorld\t" + oModel.transform.name);
//
//                    //Vector3 vScale = GetScaleByParent(particle.transform);
//                    Matrix4x4 ltwMatrix = particle.transform.localToWorldMatrix;
//                    float scaleRate = Mathf.Abs(ltwMatrix.m00);
//                    //if (particle.transform.name == "EMSZ122001_Bip01_HeadNub_1_1")
//                    //{
//                    //    LogSystem.LogError("********WWWWWWW");
//                    //    LogSystem.LogError(scaleRate);
//                    //}
//                    particle.startSize /= scaleRate;
//                    particle.startSpeed /= scaleRate;
//                    float maxParticles = particle.maxParticles;
//                    maxParticles /= scaleRate;
//                    particle.maxParticles = Mathf.CeilToInt(maxParticles);
//                    ParticleSystemRenderer render = particle.GetComponent<ParticleSystemRenderer>();
//                    if (render)
//                    {
//                        render.maxParticleSize /= scaleRate;
//                    }
//                    particle.transform.gameObject.layer = LayerMask.NameToLayer("UI");
//                }
//            }
//        }
    }
    /// <summary>
    /// 粒子特效适应界面模型
    /// </summary>
    /// <param name="oModel"></param>
    //public static void AdjustParticlesToUI(GameObject oModel)
    //{
    //    if (oModel == null)
    //        return;

    //    ParticleSystem[] particles = oModel.GetComponentsInChildren<ParticleSystem>(true);
    //    if (particles != null)
    //    {
    //        for (int i = 0; i < particles.Length; i++)
    //        {
    //            ParticleSystem particleSystem = particles[i];
    //            Matrix4x4 ltwMatrix = particleSystem.transform.localToWorldMatrix;
    //            float scaleRate = Mathf.Abs(ltwMatrix.m00);

    //            particleSystem.startSize *= scaleRate;
    //            particleSystem.startSpeed *= scaleRate;
    //            float maxParticles = particleSystem.maxParticles;
    //            maxParticles *= scaleRate;
    //            particleSystem.maxParticles = (int)maxParticles;
    //            ParticleSystemRenderer render = particleSystem.GetComponent<ParticleSystemRenderer>();
    //            if (render)
    //            {
    //                render.maxParticleSize *= scaleRate;
    //            }
    //        }
    //    }
    //}

    /*public static void SetObjRenderQ(GameObject oModel, int iLayer, int iRenderQueue)
    {
        if (oModel == null)
            return;
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null && rend.gameObject.layer == iLayer)
                {
                    rend.material.renderQueue = iRenderQueue;//将特效显示在最上层
                }
            }
        }
    }

    /// <summary>
    /// 渲染层级
    /// </summary>
    /// <param name="oModel"></param>
    public static void SetObjRenderQ(GameObject oModel, int iRenderQueue)
    {
        if (oModel == null)
            return;
        Renderer[] rens = oModel.GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                Renderer rend = rens[i];
                if (rend != null)
                {
                    rend.material.renderQueue = iRenderQueue;//将特效显示在最上层
                }
            }
        }
    }*/

    /// <summary>
    /// 计算go对象下所有子对象的最大层级最大等级
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static int CalculateMaxDepth(GameObject go)
    {
        int depth = -1;
//        UIWidget[] widgets = go.GetComponentsInChildren<UIWidget>();
//        for (int i = 0, imax = widgets.Length; i < imax; ++i)
//            depth = Mathf.Max(depth, widgets[i].depth);
        return depth;
    }

    public static void AdjustBgTexture(UITexture tex)
    {
        //默认分辨率1280*720

    }

    /// <summary>
    /// 检查APP补单
    /// </summary>
    /// <param name="filePath"></param>
    public static void CheckAppBill(string filePath)
    {
//        if (!FillOrderUtils.Init(filePath))
//        {
//            SystemText.ShowNoneSystemText(TextManager.GetString("sys_path_error"));
//            return;
//        }
//
//        FillOrderUtils.AppBillInfo abi = FillOrderUtils.GetAppBillInfo();
//
//        string account = abi.account;
//        string playerName = abi.playerName;
//        string serverId = abi.serverId;
//        string productId = abi.productId;
//        string orderId = abi.orderId;
//        string serial = abi.serial;
//
//        string text = string.Format("/apple_charge_resend {0} {1} {2} {3} {4} {5}",
//            playerName, account, serverId, productId, orderId, serial);
//
//        GameCommand.SendCustom(CustomHeader.CLIENT_CUSTOMMSG_GM_MULTIBYTE, text.Substring(1));
        //暂不
        //SystemMsgSender.GmCommand(text);
    }

    /// <summary>
    /// 获取目标点
    /// </summary>
    /// <param name="vDir"></param>
    /// <param name="vPos"></param>
    /// <param name="fDistance"></param>
    /// <returns></returns>
    public static Vector3 GetTarDist(Vector3 vDir, Vector3 vPos, float fDistance)
    {
        return vPos + fDistance * vDir;
    }

    /// <summary>
    /// 获取目标点
    /// </summary>
    /// <param name="rotation"></param>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector3 GetTarDist(Quaternion rotation, Vector3 pos, Vector3 dir)
    {
        return pos + rotation * dir;
    }

    /// <summary>
    /// 是否键盘移动角色
    /// </summary>
    /// <returns></returns>
    public static bool BeKeyCodeRoleMove()
    {
//        if (Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.LeftWard)) || Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.RightWard)) ||
//            Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.ForWard)) || Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.BackWard)))
//            return true;

        return false;
    }

    /// <summary>
    /// 获得Vertical 键盘参数
    /// </summary>
    /// <returns></returns>
    public static float GetVerticalValue()
    {
//        if (Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.ForWard)))
//            return 1f;
//        else if (Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.BackWard)))
//            return -1f;
        return 0.0f;
    }

    /// <summary>
    /// 获得Horizontal 键盘参数
    /// </summary>
    /// <returns></returns>
    public static float GetHorizontalValue()
    {
//        if (Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.RightWard)))
//            return 1f;
//        else if (Input.GetKey(KeyBindingManager.Instance.GetKeyCode(KeyBindingManager.PCCtrlKey.LeftWard)))
//            return -1f;
        return 0.0f;
    }

    /// <summary>
    /// 遥杆角度
    /// </summary>
    /// <returns></returns>
    public static bool GetTouchJoyAngle(out float fDegree)
    {
//        VCAnalogJoystickBase joy = VCAnalogJoystickBase.GetInstance("stick");
//#if TESTMEMOERY
//        if (BeKeyCodeRoleMove())
//        {
//            fDegree = 180.0f / Mathf.PI * Mathf.Atan2(UtilTools.GetVerticalValue(), UtilTools.GetHorizontalValue()) + 180.0f;
//            //取反
//            fDegree = -fDegree;
//            return true;
//        }
//#else
//        if (joy == null)
//        {
//            fDegree = -1;
//            return false;
//        }
//        if (joy.AxisX != 0.0f || joy.AxisY != 0.0f || BeKeyCodeRoleMove())
//        {
//            if ((joy.AxisX != 0.0f || joy.AxisY != 0.0f))
//                fDegree = joy.AngleDegrees;
//            else
//                fDegree = 180.0f / Mathf.PI * Mathf.Atan2(UtilTools.GetVerticalValue(), UtilTools.GetHorizontalValue()) + 180.0f;
//
//            //取反
//            fDegree = -fDegree;
//            return true;
//        }
//
//#endif
        fDegree = -1;
        return false;
    }

    public static void SetJoyStickEnable(bool _b)
    {
//        VCAnalogJoystickBase joy = VCAnalogJoystickBase.GetInstance("stick");
//        if (joy != null)
//        {
//            VCTouchController _vc = joy.transform.GetComponent<VCTouchController>();
//            _vc.enabled = _b;
//        }
    }


    /// <summary>
    /// 距离排序
    /// </summary>
    /// <param name="arr"></param>
    public static void ArrSoft(ref Vector3[] arr)
    {
//        if (ObjectManager.mRole == null)
//        {
//            return;
//        }
//        Vector3 sPos = ObjectManager.mRole.Position;
//        float[] fTemp = new float[8];
//        for (int f = 0; f < arr.Length; f++)
//        {
//            fTemp[f] = Vector3.Distance(sPos, arr[f]);
//        }
//        //排序
//        int start = 0;
//        int max = arr.Length - 1;
//        bool changed = true;
//
//        while (changed)
//        {
//            changed = false;
//
//            for (int i = start; i < max; ++i)
//            {
//                if (fTemp[i] > fTemp[i + 1])
//                {
//                    float temp;
//                    Vector3 vTemp;
//
//                    temp = fTemp[i];
//                    fTemp[i] = fTemp[i + 1];
//                    fTemp[i + 1] = temp;
//
//                    vTemp = arr[i];
//                    arr[i] = arr[i + 1];
//                    arr[i + 1] = vTemp;
//                    changed = true;
//                }
//                else if (!changed)
//                {
//                    start = (i == 0) ? 0 : i - 1;
//                }
//            }
//        }
    }

    /// <summary>
    /// 随机
    /// </summary>
    /// <param name="arr"></param>
    public static void ArrRandom(ref Vector3[] arr)
    {
//        System.Random ran = new System.Random();
//        int k = 0;
//        Vector3 strtmp = Vector3.zero;
//        for (int i = 0; i < arr.Length; i++)
//        {
//            k = ran.Next(0, arr.Length);
//            if (k != i)
//            {
//                strtmp = arr[i];
//                arr[i] = arr[k];
//                arr[k] = strtmp;
//            }
//        }
    }

    /// <summary>
    /// 通过npc id 找到场景对象
    /// </summary>
    /// <param name="strConfigID"></param>
    /// <returns></returns>
//    public static CNpcObject GetNpcObject(string strConfigID)
//    {
//        int iCount = ObjectManager.mObjects.mList.Count;
//        for (int i = 0; i < iCount; i++)
//        {
//            CGameObject cobj = ObjectManager.mObjects[ObjectManager.mObjects.mList[i]];
//            if (!(cobj is CNpcObject))
//            {
//                continue;
//            }
//            CNpcObject cnpcObj = cobj as CNpcObject;
//            if (cnpcObj.mNpcInfo != null && cnpcObj.mNpcInfo.strID.Equals(strConfigID))
//                return cnpcObj;
//        }
//        return null;
//    }
    /// <summary>
    /// 品质映射到颜色
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static string GetColorByQuality(int quality)
    {
        switch (quality)
        {
            case 0:
                return "FFFFFF";//白色
            case 1:
                return "55BA1D";//绿色
            case 2:
                return "1884F3";//蓝色
            case 3:
                return "AD28FE";//紫色
            case 4:
                return "FF9410";//橙色
            default:
                return "FFFFFF";//白色
        }
    }
    /// <summary>
    /// 获取数量译文(数量超过万保留千位显示x.x万)
    /// </summary>
    /// <param name="quantity"></param>
    /// <returns></returns>
    public static string GetStringByQuantity(long quantity)
    {
        string strQuantity = string.Empty;
        if (quantity >= 10000)
        {
            float tmp = (float)((int)((float)quantity / 1000)) / 10;
//            strQuantity = string.Format(TextManager.GetUIString("UI600095"), tmp);
        }
        else
        {
            strQuantity = quantity.ToString();
        }
        return strQuantity;
    }
    #region 界面UI特效渲染自适应 zhangrj

    /// <summary>
    /// 实例界面UI特效
    /// </summary>
    /// <param name="parent">特效父节点</param>
    /// <param name="name">特效名</param>
    /// <param name="scale">缩放</param>
    /// <param name="center">坐标</param>
    /// <param name="depth">深度</param>
    /// <param name="callback">实例回调（返回值）</param>
    /// <returns></returns>
    public static bool InstantiateUIEffect(Transform parent, string name, Vector3 scale, Vector3 center, int depth = 1, System.Action<GameObject> callback = null)
    {
        if (parent == null || string.IsNullOrEmpty(name))
            return false;
        ResourceManager.LoadAsset(UtilTools.StringBuilder( name), (UnityEngine.Object oAsset, string strFileName, VarStore varStore) =>
        {
            if (oAsset != null)
            {
                GameObject effect = CacheObjects.InstantiatePool(oAsset) as GameObject;
                if (effect != null)
                {
                    effect.transform.parent = parent;
                    effect.transform.localPosition = center;
                    effect.transform.localScale = scale;
                    AdaptiveRenderQueue queue = effect.GetComponent<AdaptiveRenderQueue>();
                    if (queue == null)
                        queue = effect.AddComponent<AdaptiveRenderQueue>();
                    queue.mEffectDepth = depth;
                    queue.RefreshRenderer();
                }
                if (callback != null)
                    callback(effect);
            }
        });
        return true;
    }

    /// <summary>
    /// 设置在UI界面中的模型内置特效的显示
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="depth"></param>
    public static void SetModelEffectRenderQueneByUI(GameObject effect, int depth)
    {
        if (effect == null)
            return;

		if (effect.layer == LayerMask.NameToLayer("UI") || effect.layer == LayerMask.NameToLayer("UIEffect") || effect.layer == LayerMask.NameToLayer("UIModel"))
        {
            AdaptiveRenderQueue quene = effect.GetComponent<AdaptiveRenderQueue>();
            if (quene == null)
                quene = effect.AddComponent<AdaptiveRenderQueue>();
            quene.mEffectDepth = depth;
        }
    }

    /// <summary>
    /// 创建cube
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static GameObject CreateCube(Vector3 pos, PrimitiveType type = PrimitiveType.Cube)
    {
        GameObject cube = GameObject.CreatePrimitive(type);
        cube.transform.position = pos;
        return cube;
    }
    #endregion
    

    /// <summary>
    /// 通过名称获取玩家
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
//    public static CPlayerObject GetPlayerObject(string strName)
//    {
//        DictionaryEx<ObjectID/*服务端ID*/, CGameObject> dicObjects = ObjectManager.mObjects;
//        if (dicObjects == null || dicObjects.Count == 0)
//            return null;
//
//        CGameObject cobj = null;
//        for (int i = 0; i < dicObjects.mList.Count; i++)
//        {
//            cobj = dicObjects[dicObjects.mList[i]];
//            if (cobj == null)
//                continue;
//            if (!(cobj is CPlayerObject))
//                continue;
//
//            if (strName.Equals(CHelper.QueryPropertyWideString(cobj, ObjPropDefine.Name)))
//                return cobj as CPlayerObject;
//        }
//        return null;
//    }

//    public static CNpcObject GetBossObject() 
//    {
//        DictionaryEx<ObjectID/*服务端ID*/, CGameObject> dicObjects = ObjectManager.mObjects;
//        if (dicObjects == null || dicObjects.Count == 0)
//            return null;
//        CGameObject cobj = null;
//        for (int i = 0; i < dicObjects.mList.Count; i++)
//        {
//            cobj = dicObjects[dicObjects.mList[i]];
//            if (cobj == null)
//                continue;
//            if (!(cobj is CNpcObject))
//                continue;
//
//           // if (cobj.mVisualTrans == null)
//     //           continue;
//
//            if(CNpcObject.bCheckBoss(cobj))
//                 return cobj as CNpcObject;
//        }
//        return null;
//    }

    /// <summary>
    /// 传入目标点与角度 得到目标点相对于对象偏移点
    /// </summary>
    /// <param name="cObj"></param>
    /// <param name="target"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
//    public static Vector3 GetDistByAngle(CGameObject cObj, Vector3 target, float angle = 0)
//    {
//        if (cObj == null)
//            return Vector3.zero;
//
//        Vector3 cObjPos = cObj.Position;
//        //自身与目标点夹角+偏移值
//        float auler = cObj.Rotation.eulerAngles.y + angle;
//
//        float dx = target.x - cObjPos.x;
//        //z offset
//        float dz = target.z - cObjPos.z;
//        //距离
//        float distance = Mathf.Sqrt(dx * dx + dz * dz);
//        //弧度
//        float Radian = MathUtils.Rad3Deg(auler);
//        float x = Mathf.Sin(Radian) * distance + cObjPos.x;
//        float z = Mathf.Cos(Radian) * distance + cObjPos.z;
//        Vector3 vTemp = Vector3.zero;
//        vTemp.x = x;
//        vTemp.y = cObjPos.y + GameObjectDefine.mfPlayerHeight;
//        vTemp.z = z;
//        return vTemp;
//    }

    /// <summary>
    /// 根据职业获取Spritenam
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public static string GetJobSpriteName(int job)
    {
        string result = string.Empty;

        if (job == 1)
            result = "zhanshi";
        else if (job == 2)
            result = "mushi";
        else if (job == 3)
            result = "fashi";
        else if (job == 4)
            result = "youxia";

        return result;
    }

    /// <summary>
    /// 根据职业获取Spritenam
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    public static Color GetJobLabelColor(int job)
    {
        if (job == 1)
            return new Color(206f / 255f, 64f / 255f, 49f / 255f);
        else if (job == 2)
            return new Color(222f / 255f, 206f / 255f, 71f / 255f);
        else if (job == 3)
            return new Color(69f / 255f, 184f / 255f, 226f / 255f);
        else if (job == 4)
            return new Color(92f / 255f, 206f / 255f, 69f / 255f);

        return Color.white;
    }

    /// <summary>
    /// 轻量提示
    /// </summary>
    /// <param name="text"></param>
    public static void LightTipsNone(string text)
    {
//        if (!GUIManager.HasView<SystemText>())
//            GUIManager.ShowView<SystemText>();
//
//        GUIManager.CallViewFunc<SystemText>(OnCallSystemText, text);
    }


//    private static void OnCallSystemText(IView view, params object[] args)
//    {
//        SystemText pPanel = view as SystemText;
//        string text = args[0] as string;
//        if (pPanel != null && !string.IsNullOrEmpty(text))
//        {
//            PromptType p = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);
//            p.top = true;
//            p.content = text;
//            p.style = PromptType.Style.NONE;
//
//            pPanel.AddText(p);
//        }
//    }

    /// <summary>
    /// 获取死亡声音
    /// </summary>
    /// <param name="cobj"></param>
    /// <param name="strDidSound"></param>
    /// <returns></returns>
//    public static bool TryGetDieSoundID(CGameObject cobj, out string strDidSound)
//    {
//        strDidSound = string.Empty;
//        string strResource = CHelper.QueryCustomString(cobj, CustomPropDefine.CustomResource); ;
//        ModelInfo modelInfo = RoleConfigManager.GetModelInfo(strResource);
//        if (modelInfo == null)
//            return false;
//
//        if (string.IsNullOrEmpty(modelInfo.dieSound))
//            return false;
//
//        string[] dieSoundArr = modelInfo.dieSound.Split(',');
//        int i = UnityEngine.Random.Range(0, dieSoundArr.Length);
//        strDidSound = dieSoundArr[i];
//        if (string.IsNullOrEmpty(strDidSound))
//            return false;
//        else
//            return true;
//    }

    /// <summary>
    /// 播放死亡声音
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
//    public static bool PlayDieSound(CGameObject cobj)
//    {
//        if (cobj == null || !cobj.bModelReadyAndDisplay)
//            return false;
//
//        string strSoundID;
//        if (!TryGetDieSoundID(cobj, out strSoundID))
//            return false;
//        return Instance.Get<SoundControl>().PlayObjectAudio(cobj.mVisualTrans.gameObject, strSoundID);
//    }

    /// <summary>
    /// 设置碰撞对象是否显示
    /// </summary>
    /// <param name="cobj"></param>
    /// <param name="bActive"></param>
    /// <returns></returns>
//    public static bool SetObjectCollideActive(CGameObject cobj, bool bActive)
//    {
//        if (cobj == null || cobj.moFBX == null)
//            return false;
//
//        Transform tranCollider = cobj.moFBX.transform.FindChild(GameObjectDefine.HightCollider);
//        if (tranCollider != null)
//        {
//            if (bActive)
//            {
//                SetLayer(tranCollider.gameObject, LayerMask.NameToLayer(GameObjectDefine.LayerDynamicCollier));
//            }
//            tranCollider.gameObject.SetActive(bActive);
//            return true;
//        }
//        return false;
//    }

    /// <summary>
    /// 判断boss碰撞高度
    /// </summary>
    /// <param name="cobj"></param>
    /// <param name="bOpen"></param>
//    public static void SetObjectCollideHeight(CGameObject cobj, bool bOpen)
//    {
//        CNpcObject cnpc = cobj as CNpcObject;
//        if (!cnpc.mNpcInfo.bHightCollider) return;
//
//        if (bOpen)
//        {
//            bool bSuccess = UtilTools.SetObjectCollideActive(cobj, true);
//            if (bSuccess)
//            {
//                string strSceneMoveHeight = GameConfigManager.GetConfigDataById("SceneMoveHeight");
//                float fSceneMoveHeight = UtilTools.FloatParse(strSceneMoveHeight);
//                GameScene.mainScene.SetDynamicHeight(true, fSceneMoveHeight, 1 << LayerMask.NameToLayer(GameObjectDefine.LayerDynamicCollier));
//            }
//        }
//        else
//        {
//            UtilTools.SetObjectCollideActive(cobj, false);
//            GameScene.mainScene.SetDynamicHeight(false, 0, 0);
//        }
//    }

    /// <summary>
    /// 获取对象所在层级
    /// </summary>
    /// <returns></returns>
//    public static int GetLayer(CGameObject cobj)
//    {
//        if (cobj == null)
//            return 0;
//
//        if (cobj is CNpcObject)
//        {
//            return LayerMask.NameToLayer("NPC");
//        }
//        else if (cobj is CPlayerObject)
//        {
//            return LayerMask.NameToLayer("Player");
//        }
//        else if (cobj is CRoleObject)
//        {
//            return LayerMask.NameToLayer("Role");
//        }
//
//        LogSystem.LogWarning("UtilTools::GetLayer:", cobj.ToString());
//        return 0;
//    }

    /// <summary>
    /// 查询Tag
    /// </summary>
    /// <param name="oRoot"></param>
    /// <param name="strTag"></param>
    /// <returns></returns>
    public static Transform[] FindTag(Transform oRoot, string strTag)
    {
        Queue<Transform> result = new Queue<Transform>();
        if (oRoot == null)
            return result.ToArray();

        Queue<Transform> openList = new Queue<Transform>();
        openList.Enqueue(oRoot);
        Transform trans;
        while (openList.Count > 0)
        {
            trans = openList.Dequeue();
            if (trans.CompareTag(strTag))
            {
                result.Enqueue(trans);
                continue;
            }
            for(int i = 0; i<trans.childCount; i++)
            {
                openList.Enqueue(trans.GetChild(i));
            }
        }
        //自身节点也进入比较
        openList.Enqueue(oRoot);
        return result.ToArray();
    }

    /// <summary>
    /// 对象信息
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
//    public static string ToString(CGameObject cobj)
//    {
//        if(cobj == null)
//            return string.Empty;
//
//        string strHead = UtilTools.StringBuilder(cobj.ToString(), " ",
//            cobj.mstrIdent, " " + CHelper.QueryCustomString(cobj, CustomPropDefine.CustomResource), cobj.meObjectBuildState, " ", cobj.meObjectExistState);
//
//        string body = string.Empty;
//        if (cobj is CNpcObject)
//        {
//            CNpcObject cnpcObject = cobj as CNpcObject;
//
//            if (cnpcObject.mNpcInfo != null)
//                body = cnpcObject.mNpcInfo.strID;
//            else
//            {
//                if (Config.bEditor)
//                {
//                    LogSystem.LogWarning("UtilTool toString ", strHead, " mnpcInfo 为空!!");
//                }
//            }
//        }
//        else if (cobj is CPlayerObject)
//        {
//            
//        }
//        else if (cobj is CRoleObject)
//        {
//            
//        }
//
//        return UtilTools.StringBuilder(strHead, " ", body);
//    } 

    /// <summary>
    /// 箭头处理
    /// </summary>
    public static void SwitchArrow(string strArrowList)
    {
//        string strArrowOld = CHelper.QueryCustomString(ObjectManager.mRole, CustomPropDefine.ARROW_NPC_LIST);
//        List<CGameObject> arrowOld = GetObjByStrID(strArrowOld.Split(','));
//        List<CGameObject> arrowNew = GetObjByStrID(strArrowList.Split(','));
//        //新列表中没有老单位，隐藏箭头
//        for (int j = 0; j < arrowOld.Count; ++j)
//        {
//            if (!(arrowOld[j] is CNpcObject))
//            {
//                continue;
//            } 
//            if (!arrowNew.Contains(arrowOld[j]))
//            {
//                CHelper.SetCustomBool(arrowOld[j], CustomPropDefine.ARROW_SHOW, false);
//            }
//        }
//        //老列表中没有新单位，显示
//        for (int j = 0; j < arrowNew.Count; ++j)
//        {
//            if (!(arrowNew[j] is CNpcObject))
//            {
//                continue;
//            }
//            if (!arrowOld.Contains(arrowNew[j]))
//            {
//                CHelper.SetCustomBool(arrowNew[j], CustomPropDefine.ARROW_SHOW, true);
//            }
//        }
//        CHelper.SetCustomString(ObjectManager.mRole, CustomPropDefine.ARROW_NPC_LIST, strArrowList);
    }

    /// <summary>
    /// 是否显示箭头
    /// </summary>
    /// <param name="cnpc"></param>
    /// <returns></returns>
//    public static bool ShowArrow(CNpcObject cnpc)
//    {
//        if (cnpc.mNpcInfo.iNpcType == NpcType.BOSS)
//        {
//            int result = 0;
//            if (!cnpc.mDataObject.QueryPropInt(ObjPropDefine.Step, ref result))
//            {
//                return false;
//            }
//            string strKey = UtilTools.StringBuilder(cnpc.mNpcInfo.strID, result.ToString());
//            if (!NPCManager.DicViewStep.ContainsKey(strKey))
//            {
//                return false;
//            }
//            BossStepItem temp = NPCManager.DicViewStep[strKey];
//            CameraViewBoss.Instance.VecWeakness = temp.VecWeakness;
//            if (temp.VecWeakness != Vector3.zero)
//            {
//                return true;
//            }
//        }
//        string strArrowList = CHelper.QueryCustomString(ObjectManager.mRole, CustomPropDefine.ARROW_NPC_LIST);
//        List<string> npcList = new List<string>(strArrowList.Split(','));
//        return npcList.Contains(cnpc.mNpcInfo.strID);
//    }

    /// <summary>
    /// 根据stringID查找对象
    /// </summary>
    /// <param name="strID"></param>
    /// <returns></returns>
//    public static List<CGameObject> GetObjByStrID(string[] strIDList)
//    {
//        List<CGameObject> endList = new List<CGameObject>();
//        CNpcObject tempNpc;
//        for (int i = 0; i < ObjectManager.mObjTrans.Count; ++i)
//        {
//            CGameObject cobj = null;
//            ObjectManager.mObjects2.GetTryValue(i, out cobj);
//            if (cobj == null)
//            {
//                // 杀飞行黄蜂
//                // 切换NPC箭头状态  出现的bug
//                // CNpcObject UtilTools.SwitchArrow(temp.strArrowList);
//                LogSystem.LogWarning("ObjectManager.mObjects2.mList Argument is out of range.长度越界");
//                continue;
//            }
//            if (!(cobj is CNpcObject))
//                continue;
//            tempNpc = (CNpcObject)cobj;
//
//            if (tempNpc.mNpcInfo == null)
//            {
//                continue;
//            }
//            for (int j = 0; j < strIDList.Length; ++j)
//            {
//                if (tempNpc.mNpcInfo.strID == strIDList[j])
//                {
//                    endList.Add(tempNpc);
//                }
//            }
//        }
//        return endList;
//    }

    /// <summary>
    /// npc碰撞器是否开启
    /// </summary>
    /// <param name="cNpcObject"></param>
    /// <param name="bActive"></param>
//    public static void SetNpcCollideActive(CNpcObject cNpcObject, bool bActive)
//    {
//        if (cNpcObject == null || cNpcObject.moFBX == null || !cNpcObject.mNpcInfo.bNpcCollider)
//            return;
//
//        Transform tranCollider = cNpcObject.moFBX.transform.FindChild(GameObjectDefine.NpcCollider);
//        if (tranCollider != null)
//        {
//            tranCollider.gameObject.SetActive(bActive);
//        }
//        return;
//    }

    /// <summary>
    /// 是否在队伍中(包含跨服组队)
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
//    public static bool bInTeamOrCrossTeam(CGameObject cobj)
//    {
//        if (!(cobj is CPlayerObject) && !(cobj is CRoleObject))
//            return false;
//
//        string strName = CHelper.QueryPropertyWideString(cobj, ObjPropDefine.Name);
//
//        SceneMode sceneModel = NationData.GetSceneMode();
//        if (sceneModel == SceneMode.MultiPlayerCopy     //多人组队副本
//            || sceneModel == SceneMode.FightFlag        //跨服战抢旗子
//            || sceneModel == SceneMode.CrossFourFight)  //跨服战4v4
//        {
//            //跨服组队
//            return CrossServerTeamInfoManager.isCrossTeam(strName);
//        }
//		if (sceneModel == SceneMode.TwelveTeam)  //12人组队
//		{
//			//12人副本组队
//			return TwelveTeamManager.isTwelveTeam(strName);
//		}
//        //todo: 用于副本内显示对用血条
//        else if(sceneModel == SceneMode.OneDragon)
//        {
//            return BraveBattleDataManager.isInTeam(strName);
//        }
//        else
//        {
//            //组队
//            return TeamRecordManager.IsInTeamByName(strName);
//        }
//    }

    /// <summary>
    /// 是否是治疗模式;
    /// </summary>
    /// <returns></returns>
    public static bool GetCureMode() 
    {
//        bool bPlayerCureMode = CHelper.QueryCustomBool(ObjectManager.mRole, CustomPropDefine.OBJECT_PLAYER_CURE_MODEL);
//
//        bool bnpcCureMode = CHelper.QueryCustomBool(ObjectManager.mRole, CustomPropDefine.OBJECT_NPC_CURE_MODEL);
//
//        return bPlayerCureMode || bnpcCureMode;
		return false;
    }

    /// <summary>
    /// 弹窗提示
    /// </summary>
    /// <param name="content"></param>
    public static void Alert(string content)
    {
        //弹窗提示/
		//        PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);//WEIBOBUG
//        pt.top = false;
//        pt.title = TextManager.GetUIString("UI50022");
//        pt.content = content;
//        pt.style = PromptType.Style.OK;
//
//        if (!GUIManager.HasView<SystemText>())
//            GUIManager.ShowView<SystemText>();
//        GUIManager.CallViewFunc<SystemText>(CreateAlertCallback, pt);
    }


//    private static void CreateAlertCallback(IView pView, params object[] args)//WEIBOBUG
//    {
//        SystemText stPanel = pView as SystemText;
//        if (stPanel != null && args != null)
//        {
//            PromptType pt = args[0] as PromptType;
//            if (pt != null)
//            {
//                stPanel.AddText(pt);
//            }
//        }
//    }

    /// <summary>
    /// PVP弹窗提示
    /// </summary>
    /// <param name="content"></param>
    public static void PVPAlert(string content)
    {
        //弹窗提示/
		//        PromptType pt = PoolManager.PopObject<PromptType>(PoolManager.PoolKey.Object_PromptType);WEIBOBUG
//        pt.top = false;
//        pt.title = TextManager.GetUIString("UI50022");
//        pt.content = content;
//        pt.style = PromptType.Style.OK;
//        pt.showtime = FloatParse(GameConfigManager.GetConfigDataById("BorderKillTipTime"),5f);
//
//        if (!GUIManager.HasView<SystemText>())
//            GUIManager.ShowView<SystemText>();
//        GUIManager.CallViewFunc<SystemText>(CreatePVPAlertCallback, pt);
    }

	/*
    private static void CreatePVPAlertCallback(IView pView, params object[] args)//WEIBOBUG
    {
        SystemText stPanel = pView as SystemText;
        if (stPanel != null && args != null)
        {
            PromptType pt = args[0] as PromptType;
            if (pt != null)
            {
                stPanel.AddPVPText(pt);
            }
        }
    }
*/
    /// <summary>
    /// 得到玩家真正名字（去除服务器前缀）
    /// </summary>
    /// <returns>The real name.</returns>
    /// <param name="name">Name.</param>
    public static string GetRealName(string name)
	{
		if(string.IsNullOrEmpty(name))
			return string.Empty;
		string[] data = name.Split ('_');
		if (data.Length > 0)
		{
			return  data [data.Length - 1];
		}
		return string.Empty;
	}

	/// <summary>
	/// 数字转换成图片代码
	/// </summary>
	/// <returns>The to pic.</returns>
	public static string NumToPic(int num,string style)
	{
		System.Text.StringBuilder counts = new System.Text.StringBuilder ();
		char[] cha = num.ToString ().ToCharArray ();

		for (int i = 0; i < cha.Length; i++) 
		{
			counts.Append (style);
			counts.Append (cha[i].ToString ());
		}
		return counts.ToString();
	}

    /// <summary>
    /// obj是否可显示
    /// </summary>
    /// <param name="cobj"></param>
    /// <returns></returns>
//    public static bool bCanDisplayObject(CGameObject cobj)
//    {
//        if (cobj == null)
//            return false;
//
//        if (cobj.QueryBool(CustomPropDefine.Custom_HideState))
//            return false;
//
//        if (cobj.mDataObject == null)
//            return false;
//
//        int iResult = 0;
//        cobj.mDataObject.QueryPropInt(ObjPropDefine.IsClientHide, ref iResult);
//        return iResult <= 0;
//    }

	/// <summary>
	/// 重新解析名字，获得玩家名字（含服务器名字）
	/// </summary>
	/// <returns>The server name.</returns>
	/// <param name="name">Name.</param>
	public static string GetServerName(string name)
	{
		string[] data = name.Split ('_');
		if (data.Length > 0)
		{
			string realName = data [data.Length - 1];

			string serverData = string.Empty;
			for(int i = 0; i < data.Length-1;i++)
			{
				serverData = UtilTools.StringBuilder (serverData,data[i]);
			}
			if(string.IsNullOrEmpty(serverData))
			{
				return realName;
			}
			return UtilTools.StringBuilder (realName,"(",serverData,")");
		}
		return string.Empty;
	}

    /// <summary>
    /// 设置是否产生阴影
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="bShowShadow"></param>
    /// <returns></returns>
//    public static bool SetObjectShadow(CGameObject cobj, bool bShowShadow)
//    {
//        if (cobj == null)
//            return false;
//
//        int iStart = (int)BuildPartType.FBX;
//        int iEnd = (int)BuildPartType.Max;
//        for (int i = iStart; i < iEnd; i++)
//        {
//            SetObjectShadow(cobj.GetRenderer(i), bShowShadow);
//        }
//        return true;
//    }

    /// <summary>
    /// 设置是否产生阴影
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="bShowShadow"></param>
    /// <returns></returns>
    public static bool SetObjectShadow(Renderer[] renderers, bool bShowShadow)
    {
        if (renderers == null)
            return false;

        for (int i = 0; i < renderers.Length; i++)
        {
            SetObjectShadow(renderers[i], bShowShadow);
        }
        return true;
    }

    /// <summary>
    /// 设置是否产生阴影
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="bShowShadow"></param>
    /// <returns></returns>
    public static bool SetObjectShadow(Renderer renderer, bool bShowShadow)
    {
        if (renderer == null)
            return false;

        UnityEngine.Rendering.ShadowCastingMode shadowCastMode =
            bShowShadow ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;

        renderer.shadowCastingMode = shadowCastMode;
        return true;
    }

    public static List<Transform> TransChild = null;
    public static List<Renderer> RendererChild = null;
    /// <summary>
    /// 获取renderer
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static Renderer[] GetObjectRenderer(Transform trans)
    {
        if (trans == null)
            return null;

        if (TransChild == null)
        {
            TransChild = new List<Transform>(4);
        }
        if (RendererChild == null)
        {
            RendererChild = new List<Renderer>(4);
        }
        TransChild.Add(trans);
        Renderer renderer = null;
        Transform tmpTrans = null;
        string strTagEffect = "Effect";
        string strBip = "Bip";
        while (TransChild.Count > 0)
        {
            tmpTrans = TransChild[0];
            TransChild.RemoveAt(0);
            if (tmpTrans.CompareTag(strTagEffect))
                continue;

            if (tmpTrans.name.StartsWith(strBip))
                continue;

            if (tmpTrans.gameObject.layer == SpawnPool.miEffectLayer
                || tmpTrans.gameObject.layer == SpawnPool.miUIEffectLayer)
                continue;

            renderer = tmpTrans.GetComponent<Renderer>();
            if (renderer != null)
            {
                RendererChild.Add(renderer);
            }

            if (tmpTrans.childCount > 0)
            {
                for (int i = 0; i < tmpTrans.childCount; i++)
                {
                    TransChild.Add(tmpTrans.GetChild(i));
                }
            }
        }

        if (RendererChild.Count == 0)
            return null;

        Renderer[] tmpRenderer = RendererChild.ToArray();
        RendererChild.Clear();

        return tmpRenderer;
    }
    /// <summary>
    /// 获取人物假阴影大小
    /// </summary>
    /// <param name="gameSceneObj"></param>
    /// <returns></returns>
//    public static float GetBumshadowSize(Fm_ClientNet.Interface.IGameSceneObj gameSceneObj)
//    {
//        if (gameSceneObj == null)
//            return 0f;
//
//        int raceVal = 0;
//        if (!gameSceneObj.QueryPropInt(ObjPropDefine.Race, ref raceVal))
//            return 0f;
//
//        int jobVal = 0;
//        if (!gameSceneObj.QueryPropInt(ObjPropDefine.Job, ref jobVal))
//            return 0f;
//
//        int sexVal = 0;
//        if (!gameSceneObj.QueryPropInt(ObjPropDefine.Sex, ref sexVal))
//            return 0f;
//
//        ModelInfo mInfo = RoleConfigManager.GetModelInfo(raceVal, jobVal, sexVal);
//        if (mInfo == null)
//            return 0f;
//
//        return mInfo.fShadowSize;
//    }

    /// <summary>
    /// 获取高中低配置
    /// </summary>
    /// <param name="device">机型</param>
    /// <returns></returns>
    public static bool TryGetQualityLevel(string device, ref GameQuality quality)
    {
        string strResult = string.Empty;
        if (Config.GetCustomInfoValue(Config.QUALITY_HIGH_KEY, ref strResult))
        {
            string[] tmp = strResult.Split(',');
            if (bStrInArray(device, tmp))
            {
                quality = GameQuality.HIGH;
                return true;
            }
        }

        if (Config.GetCustomInfoValue(Config.QUALITY_MIDDLE_KEY, ref strResult))
        {
            string[] tmp = strResult.Split(',');
            if (bStrInArray(device, tmp))
            {
                quality = GameQuality.MIDDLE;
                return true;
            }
        }

        if (Config.GetCustomInfoValue(Config.QUALITY_LOW_KEY, ref strResult))
        {
            string[] tmp = strResult.Split(',');
            if (bStrInArray(device, tmp))
            {
                quality = GameQuality.LOW;
                return true;
            }
        }

        if (Config.GetCustomInfoValue(Config.QUALITY_SUPERLOW_KEY, ref strResult))
        {
            string[] tmp = strResult.Split(',');
            if (bStrInArray(device, tmp))
            {
                quality = GameQuality.SUPER_LOW;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断数组中是否含用字符串
    /// 机型可能获取的值
    /// 安卓
    /// Meizu M570C, M570C, Pro 6
    /// ios
    /// iphone6(3gs), iphone6+
    /// </summary>
    /// <param name="str"></param>
    /// <param name="strArray"></param>
    /// <returns></returns>
    private static bool bStrInArray(string str, string[] strArray)
    {
        for (int i = 0; i < strArray.Length; i++)
        {
            if (str.Contains(strArray[i]))
                return true;
        }
        return false;
    }
    /// <summary>
    /// 通过铭文属性百分比取颜色
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    public  static string GetMingWenPropColorByPercent(int percent)
    {
        string color;
        if (percent <= 20)
        {
            color= GetColorByQuality(1);
        }
        else if (percent <= 47)
        {
            color = GetColorByQuality(2);
        }
        else if (percent <= 70)
        {
            color = GetColorByQuality(3);
        }
        else if(percent<100)
        {
            color = GetColorByQuality(4);
        }
        else
        {
            color = "FF0000";
        }
        return UtilTools.StringBuilder("[", color, "]");
    }
    /// <summary>
    /// 通过铭文属性百分比取铭文成长率说明
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static string GetMingWenGrowRateInfoByPercent(int percent)
    {
        string color;
        if (percent <= 20)
        {
            color = GetColorByQuality(1);
        }
        else if (percent <= 47)
        {
            color = GetColorByQuality(2);
        }
        else if (percent <= 70)
        {
            color = GetColorByQuality(3);
        }
        else if (percent < 100)
        {
            color = GetColorByQuality(4);
        }
        else
        {
            color = "FF0000";
        }
        return UtilTools.StringBuilder("[", color, "]");
    }
    /// <summary>
    /// 解析登录成功后，引擎返回的计费串，充值使用
    /// </summary>
    /// <param name="value">1:#INNER7F5555655F4241517F5555165A59504371535F7A422827447C225B0D285D23337D5754082D5B20427B565F7A5E5E5D450D222C7C57$tjxm${"openid":"47518223","expires_in":"7776000","refresh_token":"7.46d53259d841b77429ab95268e9b9e4c","access_token":"7.526271a168ad2e55177fc5b812bca2c8.c82abdd3513911e66b17c3375ce223c7.1483948474881"}</param>
    /// <returns>返回json格式</returns>
    public static string ResolveToken(string value)
    {
        const string flag = "$tjxm$";
        int index = value.IndexOf(flag);
        string result = string.Empty;
        if (index > -1)
        {
            result = value.Remove(0, index + flag.Length);
        }
        return result;
    }

	/// <summary>
	/// 登陆串截取串
	/// </summary>
	/// <param name="value">1:#INNER7F5555655F4241517F5555165A59504371535F7A422827447C225B0D285D23337D5754082D5B20427B565F7A5E5E5D450D222C7C57$tjxm${"openid":"47518223","expires_in":"7776000","refresh_token":"7.46d53259d841b77429ab95268e9b9e4c","access_token":"7.526271a168ad2e55177fc5b812bca2c8.c82abdd3513911e66b17c3375ce223c7.1483948474881"}</param>
	/// <returns>返回json格式</returns>
	public static string LoginResolveToken(string value)
	{
		const string flag = "$tjxm$";
		int index = value.IndexOf(flag);
		string result = string.Empty;
		if (index > -1) 
		{
			result = value.Substring (0, index);
		} 
		else
			return value;
		return result;
	}

	/// <summary>
	/// 检测活动是否开放
	/// </summary>
	/// <returns><c>true</c>, if activity open time was checked, <c>false</c> otherwise.</returns>
	/// <param name="OpenTime">Open time.</param>
	/// <param name="today">Today.</param>
	public static bool CheckOpenTime(DictionaryEx<int, string> OpenTime,bool showText = false)
	{
//		if(OpenTime == null)
//			return false;
//		DateTime nowTime = GameDefine.ServerCurDateTime;
//		int day = (int)nowTime.DayOfWeek;
//		//保存此时的开放时间数据
//		string opent = TextManager.GetUIString("UI90035");
//		int Extent = -1;
//		//取day的数据
//		if(OpenTime.ContainsKey(day))
//		{
//			string time = OpenTime[day];
//			if(string .IsNullOrEmpty(time))
//				return false;
//			string[] AreaTime = time.Split ('|');
//
//			List<int> sTime = new List<int>();
//			List<int> eTime = new List<int>();
//			List<string> sTimeDesc = new List<string>();
//			List<string> eTimeDesc = new List<string>();
//
//			for(int i = 0; i < AreaTime.Length; i++)
//			{
//				string[] times = AreaTime[i].Split ('#');
//				if (times.Length != 2)
//					continue;
//				string startTime = times[0];
//				string endTime = times[1];
//				sTimeDesc.Add(UtilTools.StringBuilder(startTime.Substring(0, 5)));
//				eTimeDesc.Add(UtilTools.StringBuilder(endTime.Substring(0, 5))); 
//				string[] starTimes = startTime.Split (':');
//				string[] endTimes = endTime.Split (':');
//				sTime.Add(int.Parse(starTimes [0]) * 3600 + int.Parse(starTimes [1]) * 60 + int.Parse(starTimes [2]));
//				eTime.Add(int.Parse(endTimes [0]) * 3600 + int.Parse(endTimes [1]) * 60 + int.Parse(endTimes [2]));
//			}
//
//			if (sTime != null && eTime != null) 
//			{
//				int nTime = nowTime.Hour * 3600 + nowTime.Minute * 60 + nowTime.Second;
//				for(int i =0; i < sTime.Count; i++)
//				{
//					if (nTime >= sTime [i] && nTime <= eTime [i]) 
//						return true;
//					if (nTime < sTime[i])
//						Extent = i;
//				}
//				if (showText) 
//				{
//					if (Extent != -1) 
//					{
//						if(Extent >=  sTimeDesc.Count || Extent >=  eTimeDesc.Count)
//							return false;
//						opent = TextManager.GetString ("UICrossFourNoOpen",sTimeDesc [Extent],eTimeDesc [Extent]);
//					}
//					SystemText.ShowNoneSystemText(opent);
//				}
//			}
//		}
		return false;
	}

    /// <summary>  
    /// 保证返回数据为范围内的数值.  
    /// </summary>  
    /// <typeparam name="T"></typeparam>  
    /// <param name="待测值">The 待测值.</param>  
    /// <param name="最小">The 最小.</param>  
    /// <param name="最大">The 最大.</param>  
    /// <returns>T.</returns>  
    public static T regionT<T>(T defalut, T min, T max) where T : IComparable
    {
        var _max = (defalut.CompareTo(min) > 0) ? defalut : min;
        return (_max.CompareTo(max) > 0) ? max : _max;
    }

    /// <summary>
    /// pc端拦截工具 质检测试使用
    /// </summary>
    public static void InterceptTool()
    {
#if Intercept
        if ((Config.bWin || Config.bMac || Config.bEditor) && Config.bInterceptOpen)
        {
            GameObject Intercept = new GameObject("Intercept");
            Intercept.AddComponent<UnityIntercept.InterceptStart>();
        }
#endif
    }
}