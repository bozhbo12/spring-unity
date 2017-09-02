using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

public class GMScriptable
{
	public class Row
	{
		public string name = string.Empty;
		public int Style;
		public string CommandStr =string.Empty;
		public List<string> GMValue = new List<string>(); 
		public List<string> fields = new List<string>(0);
	}

	public List<Row> rows = new List<Row>(0);
}


public class ReadGM
{
	public enum StyleS
	{
		SYS = 0,//系统类型
		Player = 1,//玩法类型
		Frequently = 2,//设定常用命令
		Tool =3,//固定工具
	}
	public static GMScriptable mGMScriptable;
	//读取GM保存
	public static void ReadGMData(bool show)
	{
		if(!show)
			return;
		mGMScriptable = GMData ();
	}
	public static GMScriptable GetGMData()
	{
		return GMData ();
	}

	private static GMScriptable GMData()
	{
		TextAsset mTextAsset = Resources.Load("Local/Config/GM") as TextAsset;
		if (mTextAsset != null)
		{
			string strWords = mTextAsset.text;
			if (string.IsNullOrEmpty(strWords))
				return null;
			bool newDate = false;
			GMScriptable.Row date = new GMScriptable.Row();
			GMScriptable adf = new GMScriptable();
			List<GMScriptable.Row> GMTestDatas = new List<GMScriptable.Row>();

			string[] GM = strWords.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < GM.Length; i++)
			{
				string temp = GM[i];
				if(temp.StartsWith("("))
					continue;
				bool gm = (temp.StartsWith ("/") || temp.StartsWith ("#"));
				if (!gm) 
					continue;

				if(!temp.StartsWith("/"))
				{
					if(!string.IsNullOrEmpty(date.name))
					{
						GMTestDatas.Add(date);
					}

					date = new GMScriptable.Row();
					string[] name = temp.Split('=');
					if(name.Length == 0)
						continue;
					date.name = name[0].Replace("#","");

					if(name.Length > 1)
						date.Style = int.Parse(name[1]);
					newDate = true;
					continue;
				}
				string[] comm = temp.Split('=');
				if(newDate)
				{
					for(int j = 0; j < comm.Length;j++)
					{
						if(j == 0)
							date.CommandStr = comm[0];
						else
							date.GMValue.Add(comm[j]);
					}
					newDate =false;
				}
				date.fields.Add(temp.ToString());
			}
			if(!string.IsNullOrEmpty(date.name))
			{
				GMTestDatas.Add(date);
			}
			adf.rows = GMTestDatas;
			Resources.UnloadAsset(mTextAsset);
			return adf;
		}
		return null;
	}
}
