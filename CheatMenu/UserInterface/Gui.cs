using BepInEx;
using HarmonyLib;
using MaidStatus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using UnityEngine;
using wf;
using static CheatMenu.UserInterface.UiToolbox;
using static VRFaceShortcutConfig;
using Math = System.Math;
using Status = PlayerStatus.Status;

namespace CheatMenu.UserInterface
{
	public static class Gui
	{
		private static bool _ranOnce;

		private const int WindowId = 542876124;
		internal static Rect WindowRect;
		private static Rect _dragWindow = new Rect(0, 0, 10000, 20);
		private static Rect _closeButton = new Rect(0, 0, 25, 15);
		private static Vector2 _scrollPosition;

		private static Texture2D normalTexture;
		private static Texture2D hoverTexture;
		private static Texture2D onNormalTexture;
		private static Texture2D sectionsTexture;
		private static Texture2D sections2Texture;

		//internal static GUIStyle Separator;
		internal static GUIStyle MainWindow;

		internal static GUIStyle Sections;
		internal static GUIStyle Sections2;

		private static int _currentHeight;
		private static int _currentWidth;

		private static string _currentGuid = string.Empty;

		internal static void StartGui()
		{
			//Setup some UI properties.
			if (_ranOnce == false)
			{
				/*
				Separator = new GUIStyle(GUI.skin.horizontalSlider)
				{
					fixedHeight = 1f,
					normal =
					{
						background = UiToolbox.MakeTex(2, 2, new Color(0, 0, 0, 0.8f))
					},
					margin =
					{
						top = 10,
						bottom = 10
					}
				};
				*/

				normalTexture = UiToolbox.MakeWindowTex(new Color(0, 0, 0, 0.1f), new Color(0, 0, 0, 0.5f));
				hoverTexture = UiToolbox.MakeWindowTex(new Color(0.3f, 0.3f, 0.3f, 0.6f), new Color(0, 1, 1, 0.5f));
				onNormalTexture = UiToolbox.MakeWindowTex(new Color(0.3f, 0.3f, 0.3f, 0.8f), new Color(0, 1, 1, 0.5f));

				MainWindow = new GUIStyle(GUI.skin.window)
				{
					normal =
					{
						background = normalTexture,
						textColor = new Color(1, 1, 1, 0.8f)
					},
					hover =
					{
						background = hoverTexture,
						textColor = new Color(1, 1, 1, 0.8f)
					},
					onNormal =
					{
						background = onNormalTexture
					}
				};

				sectionsTexture = UiToolbox.MakeTex(2, 2, UiToolbox.ParseColor("#1196EE20"));

				Sections = new GUIStyle(GUI.skin.box)
				{
					normal =
					{
						background = sectionsTexture
					}
				};

				sections2Texture = UiToolbox.MakeTexWithRoundedCorner(new Color(0, 0, 0, 0.5f));

				Sections2 = new GUIStyle(GUI.skin.box)
				{
					normal =
					{
						background = sections2Texture
					}
				};

				_ranOnce = true;
			}

			//Sometimes the UI can be improperly sized, this sets it to some measurements.
			if (_currentHeight != Screen.height || _currentWidth != Screen.width)
            {
                WindowRect.width = Math.Min(Screen.width * 0.8f, 600);
                WindowRect.height = Math.Min(Screen.height * 0.8f, 1000);

                WindowRect.x = (Screen.width - WindowRect.width) / 2;
                WindowRect.y = (Screen.height - WindowRect.height) / 2;

                _currentHeight = Screen.height;
				_currentWidth = Screen.width;
			}

			WindowRect = GUILayout.Window(WindowId, WindowRect, GuiWindowControls, "ExtendCheatMenu", MainWindow);
		}

		class BBB : BinaryWriter
		{
			FileStream _stream;
            public BBB(FileStream output) : base(output) { _stream = output; }
			long offset() { return _stream.Position; }
            public override void Write(bool value)
            {
				Console.WriteLine($"wirte offset {offset().ToString("X")} bool {value}");
                base.Write(value);
            }
            public override void Write(byte value)
            {
				Console.WriteLine($"wirte offset {offset().ToString("X")} byte {value}");
                base.Write(value);
            }
            public override void Write(sbyte value)
            {
				Console.WriteLine($"wirte offset {offset().ToString("X")} sbyte {value}");
                base.Write(value);
            }
            public override void Write(short value)
            {
				Console.WriteLine($"wirte offset {offset().ToString("X")} short {value}");
                base.Write(value);
            }
            public override void Write(ushort value)
            {
				Console.WriteLine($"wirte offset {offset().ToString("X")} ushort {value}");
                base.Write(value);
            }
            public override void Write(int value)
            {
                Console.WriteLine($"wirte offset {offset().ToString("X")} int {value}");
                base.Write(value);
            }
            public override void Write(uint value)
            {
                Console.WriteLine($"wirte offset {offset().ToString("X")} int {value}");
                base.Write(value);
            }
            public override void Write(string value)
            {
				Console.WriteLine($"wirte offset {offset().ToString("X")} string {value}");
                base.Write(value);
            }
        }

        private static void GuiWindowControls(int id)
		{
			_closeButton.x = WindowRect.width - (_closeButton.width + 5);
			_dragWindow.width = WindowRect.width - (_closeButton.width + 5);

			GUI.DragWindow(_dragWindow);

			if (GUI.Button(_closeButton, "X"))
			{
				ExtendCheatMenu.DrawUi = false;
			}

			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            GUILayout.BeginVertical();

			GameMain.Instance.CharacterMgr.status.playerName = UiToolbox.LabeledField("玩家名字", GameMain.Instance.CharacterMgr.status.playerName);

			GUILayout.BeginHorizontal();
			GameMain.Instance.CharacterMgr.status.money = UiToolbox.NumberField(GameMain.Instance.CharacterMgr.status.money, "金钱", max: Status.MoneyMax);
			GUILayout.FlexibleSpace();
			GameMain.Instance.CharacterMgr.status.clubGauge = UiToolbox.NumberField(GameMain.Instance.CharacterMgr.status.clubGauge, "俱乐部计量槽");
			GUILayout.FlexibleSpace();
			GameMain.Instance.CharacterMgr.status.clubGrade = UiToolbox.NumberField(GameMain.Instance.CharacterMgr.status.clubGrade, "俱乐部等级", max: Status.ClubGradeMax);
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            var commonIdManager = AccessTools.Field(typeof(Trophy), "commonIdManager").GetValue(null) as CsvCommonIdManager;
            if (commonIdManager != null && GUILayout.Button("获得所有奖杯"))
			{
				UnlockAllTrophies();
			}

            GUILayout.EndHorizontal();

            if (GameMain.Instance.CharacterMgr.status.lockNTRPlay)
			{
                GUILayout.BeginHorizontal();
                GUILayout.Label("NTR 已解锁");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("重新变成纯爱仙人"))
                {
                    GameMain.Instance.CharacterMgr.status.lockNTRPlay = false;
                    DisplayFakeTrophy("我是纯爱仙人");
                }
                GUILayout.EndHorizontal();
            }
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("NTR 已锁定");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("化身为牛头人战士"))
				{
					GameMain.Instance.CharacterMgr.status.lockNTRPlay = true;
                    DisplayFakeTrophy("牛头人战士来啦");
                }
				GUILayout.EndHorizontal();
			}

            if (GameMain.Instance.CharacterMgr.status.isOldPlayer)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("即是CM3D2的老板也是现在的老板");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("清除在CM3D2俱乐部的记忆"))
                {
                    GameMain.Instance.CharacterMgr.status.isOldPlayer = false;
                    DisplayFakeTrophy("我是新人");
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("是现在的老板");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("继承CM3D2俱乐部记忆"))
                {
                    GameMain.Instance.CharacterMgr.status.isOldPlayer = true;
                    DisplayFakeTrophy("我是老玩家");
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical(Sections2);
            GUILayout.BeginHorizontal(Sections2);
            GUILayout.Label("修改设施等级");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("☰"))
            {
                _currentGuid = _currentGuid == "设施等级" ? string.Empty : "设施等级";
            }
            GUILayout.EndHorizontal();
            if (_currentGuid == "设施等级")
			{
                for (int i = 0; i < GameMain.Instance.FacilityMgr.FacilityCountMax; i++)
                {
					var fac1 = GameMain.Instance.FacilityMgr.GetFacility(i);
					var fac2 = GameMain.Instance.FacilityMgr.GetNextDayFacility(i);
					foreach (var fac in new[] { fac1, fac2 })
					{
						if (fac == null)
							continue;
                        GUILayout.BeginHorizontal(Sections2);
                        GUILayout.Label(ScriptManager.ReplaceCharaName(fac.facilityName) + ": " + fac.facilityLevel);
                        GUILayout.FlexibleSpace();
						if (GUILayout.Button("-", GUILayout.MinWidth(24)))
						{
							fac.expSystem.SetLevel(fac.facilityLevel - 1);
                        }
						if (GUILayout.Button("+", GUILayout.MinWidth(24)))
						{
							fac.expSystem.SetLevel(fac.facilityLevel + 1);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(Sections2);

			foreach (var maid in GameMain.Instance.CharacterMgr.GetStockMaidList())
			{
				GUILayout.BeginHorizontal(Sections2);
				GUILayout.Label(maid.status.fullNameJpStyle);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("☰"))
				{
					_currentGuid = _currentGuid == maid.status.guid ? string.Empty : maid.status.guid;
				}
				GUILayout.EndHorizontal();

				if (_currentGuid.Equals(maid.status.guid) == false)
				{
					continue;
				}

                GUILayout.BeginHorizontal(Sections);
                maid.status.lastName = UiToolbox.LabeledField("姓", maid.status.lastName);
				GUILayout.FlexibleSpace();
                maid.status.firstName = UiToolbox.LabeledField("名", maid.status.firstName);
                GUILayout.FlexibleSpace();
                maid.status.isFirstNameCall = GUILayout.Toggle(maid.status.isFirstNameCall, "称呼名字");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal(Sections);
				GUILayout.Label("培育完成: " + (maid.status.studyRate > 500 ? "否" : "是" + (maid.status.contract == Contract.Trainee ? " (第二天才开始决定专属和自由女仆!)" : string.Empty)));
				if (maid.status.studyRate > 500)
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("完成培育"))
					{
						maid.status.studyRate = 500;
					}
				}
				GUILayout.EndHorizontal();


				/*GUILayout.BeginHorizontal(Sections);
				if (GUILayout.Button("Max Out Current Job Class"))
				{
					maid.status.selectedJobClass.expSystem.SetLevel(maid.status.selectedJobClass.expSystem
						.GetMaxLevel());
				}
				if (GUILayout.Button("Max Out Current Yotogi Class"))
				{
					maid.status.selectedYotogiClass.expSystem.SetLevel(maid.status.selectedYotogiClass.expSystem
						.GetMaxLevel());
				}
				GUILayout.EndHorizontal();*/


				if (maid.status.heroineType == HeroineType.Transfer)
				{
					// script/iseki_0001.ks
					string setMaidFlagName = null;
					string setManFlagName1 = null;
					string setManFlagName2 = null;
					if (maid.status.personal.drawName == "プライドが高く負けず嫌い")
					{
						if (maid.status.GetFlag("移籍してきたツンデレ") != 1)
							setMaidFlagName = "移籍してきたツンデレ";
                    }
					else if (maid.status.personal.drawName == "クールで寡黙")
					{
                        if (maid.status.GetFlag("移籍してきたクーデレ") != 1)
                            setMaidFlagName = "移籍してきたクーデレ";
                    }
					else if (maid.status.personal.drawName == "純真で健気な妹系")
					{
                        if (maid.status.GetFlag("移籍してきた純真") != 1)
                            setMaidFlagName = "移籍してきた純真";
                    }
					else if (maid.status.personal.drawName == "病的な程一途な大和撫子")
					{
                        if (maid.status.GetFlag("移籍してきたヤンデレ") != 1)
                        {
                            setMaidFlagName = "移籍してきたヤンデレ";
							if (GameMain.Instance.CharacterMgr.status.GetFlag("ヤンデレ初回移籍") == 0)
							{
                                setManFlagName1 = "ヤンデレ初回移籍";
                                setManFlagName2 = "移籍ヤンデレ未確定";
                            }
                        }
                    }
					else if (maid.status.personal.drawName == "母性的なお姉ちゃん")
					{
                        if (maid.status.GetFlag("移籍してきたお姉ちゃん") != 1)
                        {
                            setMaidFlagName = "移籍してきたお姉ちゃん";
							if (GameMain.Instance.CharacterMgr.status.GetFlag("お姉ちゃん初回移籍") == 0)
							{
								setManFlagName1 = "お姉ちゃん初回移籍";
								setManFlagName2 = "移籍お姉ちゃん未確定";
                            }
                        }
                    }
					else if (maid.status.personal.drawName == "健康的でスポーティなボクっ娘")
					{
                        if (maid.status.GetFlag("移籍してきたボクっ娘") != 1)
                        {
                            setMaidFlagName = "移籍してきたボクっ娘";
							if (GameMain.Instance.CharacterMgr.status.GetFlag("ボクっ娘初回移籍") == 0)
							{
								setManFlagName1 = "ボクっ娘初回移籍";
								setManFlagName2 = "移籍ボクっ娘未確定";
                            }
                        }
                    }
					else if (maid.status.personal.drawName == "Ｍ心を刺激するドＳ女王様")
					{
                        if (maid.status.GetFlag("移籍してきたドＳ") != 1)
                        {
                            setMaidFlagName = "移籍してきたドＳ";
							if (GameMain.Instance.CharacterMgr.status.GetFlag("ドＳ初回移籍") == 0)
							{
								setManFlagName1 = "ドＳ初回移籍";
								setManFlagName2 = "移籍ドＳ未確定";
                            }
                        }
                    }
                    GUILayout.BeginHorizontal(Sections);
                    GUILayout.Label("移籍女仆: " + (setMaidFlagName != null ? "否" : "是"));
                    if (setMaidFlagName != null)
                    {
                        if (GUILayout.Button("移籍"))
                        {
							maid.status.SetFlag(setMaidFlagName, 1);
							if (setManFlagName1 != null)
								GameMain.Instance.CharacterMgr.status.SetFlag(setManFlagName1, 1);
                            if (setManFlagName2 != null)
                                GameMain.Instance.CharacterMgr.status.SetFlag(setManFlagName2, 1);
							if (maid.status.GetFlag("新規雇用旧性格メイド") == 1)
                                maid.status.SetFlag("新規雇用旧性格メイド", 0);
                        }
                    }
                    GUILayout.EndHorizontal();
                }


				maid.status.relation = DropDownEnum<Relation>.onDropMenuClick(Sections, maid, "普通关系", maid.status.relation, a => EnumConvert.GetString(a));
                maid.status.additionalRelation = DropDownEnum<AdditionalRelation>.onDropMenuClick(Sections, maid, "额外关系", maid.status.additionalRelation, a => EnumConvert.GetString(a));
				maid.status.specialRelation = DropDownEnum<SpecialRelation>.onDropMenuClick(Sections, maid, "结婚关系", maid.status.specialRelation, a => EnumConvert.GetString(a));
				maid.status.contract = DropDownEnum<Contract>.onDropMenuClick(Sections, maid, "契约类型", maid.status.contract, a => EnumConvert.GetString(a));
				maid.status.initSeikeiken = DropDownEnum<Seikeiken>.onDropMenuClick(Sections, maid, "初始性经验", maid.status.initSeikeiken, a => EnumConvert.GetString(a));
				maid.status.seikeiken = DropDownEnum<Seikeiken>.onDropMenuClick(Sections, maid, "性经验", maid.status.seikeiken, a => EnumConvert.GetString(a));


                var propensity = Traverse.Create(maid.status).Field("propensitys_").GetValue() as Dictionary<int, Propensity.Data>;
                DropDownList<int, string>.onDropMenuClick(Sections, maid, Propensity.GetAllDatas(true).ToDictionary(a => a.id, b => b.drawName), propensity.Keys, "性癖", (key, add) =>
				{
					if (add)
                        maid.status.AddPropensity(key);
					else
						maid.status.RemovePropensity(key);
                });


                GUILayout.BeginVertical(Sections);
                maid.status.likability = UiToolbox.NumberField(maid.status.likability, "好感度", 0, int.MaxValue);
                maid.status.sexPlayNumberOfPeople = UiToolbox.NumberField(maid.status.sexPlayNumberOfPeople, "SEX经验人数", 0, int.MaxValue);
                GUILayout.EndVertical();

                GUILayout.BeginVertical(Sections);
				maid.status.baseLovely = UiToolbox.NumberField(maid.status.baseLovely, "可爱", 0, int.MaxValue);
				maid.status.baseElegance = UiToolbox.NumberField(maid.status.baseElegance, "气质", 0, int.MaxValue);
				maid.status.baseCharm = UiToolbox.NumberField(maid.status.baseCharm, "魅惑", 0, int.MaxValue);
				maid.status.baseCare = UiToolbox.NumberField(maid.status.baseCare, "侍奉", 0, int.MaxValue);
				maid.status.baseReception = UiToolbox.NumberField(maid.status.baseReception, "招待", 0, int.MaxValue);
				maid.status.baseCooking = UiToolbox.NumberField(maid.status.baseCooking, "料理", 0, int.MaxValue);
				maid.status.baseDance = UiToolbox.NumberField(maid.status.baseDance, "舞蹈", 0, int.MaxValue);
				maid.status.baseVocal = UiToolbox.NumberField(maid.status.baseVocal, "声乐", 0, int.MaxValue);
				maid.status.playCountNightWork = UiToolbox.NumberField(maid.status.playCountNightWork, "接客次数", 0, int.MaxValue);
				GUILayout.EndVertical();

				GUILayout.BeginVertical(Sections);
				maid.status.baseTeachRate = UiToolbox.NumberField(maid.status.baseTeachRate, "指导率", 0, int.MaxValue);
				maid.status.studyRate = UiToolbox.NumberField(maid.status.studyRate, "学习率(不大于500时表示培育完成)", 0, int.MaxValue);
				GUILayout.EndVertical();

				GUILayout.BeginVertical(Sections);
				maid.status.baseMaxHp = UiToolbox.NumberField(maid.status.baseMaxHp, "体力", 0, int.MaxValue);
				maid.status.baseMaxMind = UiToolbox.NumberField(maid.status.baseMaxMind, "精神", 0, int.MaxValue);
				maid.status.baseInyoku = UiToolbox.NumberField(maid.status.baseInyoku, "淫欲", 0, int.MaxValue);
				maid.status.baseMvalue = UiToolbox.NumberField(maid.status.baseMvalue, "M性", 0, int.MaxValue);
				maid.status.baseHentai = UiToolbox.NumberField(maid.status.baseHentai, "变态", 0, int.MaxValue);
				maid.status.baseHousi = UiToolbox.NumberField(maid.status.baseHousi, "侍奉", 0, int.MaxValue);
				maid.status.playCountYotogi = UiToolbox.NumberField(maid.status.playCountYotogi, "夜伽次数", 0, int.MaxValue);
				GUILayout.EndVertical();

				GUILayout.BeginVertical(Sections);
				maid.status.baseAppealPoint = UiToolbox.NumberField(maid.status.baseAppealPoint, "AP(魅力展示)", 0, int.MaxValue);
				GUILayout.EndVertical();
			}

			GUILayout.EndVertical();

			GUILayout.EndScrollView();

			UiToolbox.ChkMouseClick(WindowRect, ref ExtendCheatMenu.DrawUi);
		}

		public static void UnlockAllTrophies()
		{
			var data = Trophy.GetAllDatas(true);

			foreach (var trophyData in data)
			{
				GameMain.Instance.CharacterMgr.status.AddHaveTrophy(trophyData.id);
			}
		}

		public static void DisplayFakeTrophy(string trophyText, string shopItem = "")
		{
			try
			{
                var gameObject = GameObject.Find("SystemUI Root/TrophyAchieveEffect");
                if (gameObject == null)
                {
                    return;
                }

                var component = gameObject.GetComponent<TrophyAchieveEffect>();
                if (component == null)
                {
                    return;
                }

                var trophy = FormatterServices.GetUninitializedObject(typeof(Trophy.Data)) as Trophy.Data;
                if (trophy == null)
                {
                    ExtendCheatMenu.PluginLogger.LogWarning("Trophy fail: GetUninitializedObject");
                    return;
                }

                typeof(Trophy.Data).GetField(nameof(Trophy.Data.rarity)).SetValue(trophy, 3);
                typeof(Trophy.Data).GetField(nameof(Trophy.Data.name)).SetValue(trophy, trophyText);
                typeof(Trophy.Data).GetField(nameof(Trophy.Data.effectDrawItemName)).SetValue(trophy, shopItem);

				(Traverse.Create(component).Field("trophy_data_list_").GetValue() as List<Trophy.Data>).Insert(0, trophy);
                Traverse.Create(component).Field("state_").SetValue(Enum.Parse(AccessTools.TypeByName("TrophyAchieveEffect+State"), "Null"));
            }
			catch (Exception ex)
			{
                ExtendCheatMenu.PluginLogger.LogWarning("Trophy fail: " + ex);
            }
		}
	}
}