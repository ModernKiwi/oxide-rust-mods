using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using Rust;
using System;                      //DateTime
using System.Collections.Generic;  //Required for Whilelist
using Oxide.Core.Libraries.Covalence; //Requrired for IPlayer stuff
//using System.Data;
//using System.Globalization;
using System.Linq;
//using System.Reflection;
using UnityEngine;
//using System.Collections;
//using ConVar;
//using Facepunch;
//using Network;
//using ProtoBuf;
//using System.Runtime.CompilerServices;
//using System.Text.RegularExpressions;
//using Oxide.Plugins;
using Oxide.Core.Configuration;
//using Oxide.Core.CSharp;
//using Rust.Registry;
//using RustNative;
using Oxide.Core.Database;

namespace Oxide.Plugins
{
    [Info("PvXGui", "Alphawar", "0.0.1")]
    [Description("Gui file for PvXSelector")]
    class PvXGui : RustPlugin
    {


        static string pvxIndicator = "PvXPlayerStateIndicator";



        static string pvxIndicatorGui = "createPvXModeSelector";
        static string pvxPlayerUI = "pvxPlayerModeUI";
        static string pvxAdminUI = "pvxAdminTicketCountUI";


        static string[] GuiList = new string[] { pvxIndicatorGui,
            pvxPlayerUI, pvxAdminUI,
            UIMain, UIPanel
        };

        static string pvxMenuBack = "PvXMenu_Main_Background";
        static string pvxMenuBanner = "PvXMenu_Main_Banner";
        static string pvxMenuBannerTitle = "PvXMenu_Main_Banner_Title";
        static string pvxMenuBannerVersion = "PvXMenu_Main_Banner_Version";
        static string pvxMenuBannerModType = "PvXMenu_Main_Banner_ModType";
        static string pvxMenuSide = "PvXMenu_Main_Side";
        static string pvxMenuMain = "PvXMenu_Main";

        public static PvXGui instance;

        void OnServerInitialized()
        {
            LoadData();
            CheckPlayersRegistered();
            foreach (BasePlayer Player in BasePlayer.activePlayerList)
            {
                if (!Players.State.IsNPC(Player))
                {
                    PvXGui.GUI.Create.PlayerIndicator(Player);
                    ModeSwitch.Cooldown.Check(Player.userID);
                    if (HasPerm(Player, "admin"))
                    {
                        Players.Admins.AddPlayer(Player.userID);
                        GUI.Create.AdminIndicator(Player);
                    }
                    if (Players.State.IsNA(Player)) Timers.PvxChatNotification(Player);
                    UpdatePlayerChatTag(Player);
                }
            }
        }


        #region UI Creation
        class QUI
        {
            public static CuiElementContainer CreateElementContainer(string panelName, string color, string aMin, string aMax, bool useCursor = false, string parent = "Hud")
            {
                var NewElement = new CuiElementContainer()
                {
                    {
                        new CuiPanel
                        {
                            Image = {Color = color},
                            RectTransform = {AnchorMin = aMin, AnchorMax = aMax},
                            CursorEnabled = useCursor
                        },
                        new CuiElement().Parent = parent,
                        panelName
                    }
                };
                return NewElement;
            }
            public static void CreatePanel(ref CuiElementContainer container, string panel, string color, string aMin, string aMax, bool cursor = false)
            {
                container.Add(new CuiPanel
                {
                    Image = { Color = color },
                    RectTransform = { AnchorMin = aMin, AnchorMax = aMax },
                    CursorEnabled = cursor
                },
                panel);
            }
            public static void CreateLabel(ref CuiElementContainer container, string panel, string color, string text, int size, string aMin, string aMax, float fadein = 1.0f, TextAnchor align = TextAnchor.MiddleCenter)
            {
                if (DisableUI_FadeIn)
                    fadein = 0;
                container.Add(new CuiLabel
                {
                    Text = { Color = color, FontSize = size, Align = align, FadeIn = fadein, Text = text },
                    RectTransform = { AnchorMin = aMin, AnchorMax = aMax }
                },
                panel);

            }
            public static void CreateButton(ref CuiElementContainer container, string panel, string color, string text, int size, string aMin, string aMax, string command, float fadein = 1.0f, TextAnchor align = TextAnchor.MiddleCenter)
            {
                if (DisableUI_FadeIn)
                    fadein = 0;
                container.Add(new CuiButton
                {
                    Button = { Color = color, Command = command, FadeIn = fadein },
                    RectTransform = { AnchorMin = aMin, AnchorMax = aMax },
                    Text = { Text = text, FontSize = size, Align = align }
                },
                panel);
            }
            public static void LoadImage(ref CuiElementContainer container, string panel, string png, string aMin, string aMax)
            {
                container.Add(new CuiElement
                {
                    Parent = panel,
                    Components =
                    {
                        new CuiRawImageComponent {Png = png },
                        new CuiRectTransformComponent {AnchorMin = aMin, AnchorMax = aMax }
                    }
                });
            }
            public static void CreateTextOverlay(ref CuiElementContainer container, string panel, string text, string color, int size, string aMin, string aMax, TextAnchor align = TextAnchor.MiddleCenter, float fadein = 1.0f)
            {
                if (DisableUI_FadeIn)
                    fadein = 0;
                container.Add(new CuiLabel
                {
                    Text = { Color = color, FontSize = size, Align = align, FadeIn = fadein, Text = text },
                    RectTransform = { AnchorMin = aMin, AnchorMax = aMax }
                },
                panel);

            }
        }
        class UIColours
        {
            public static readonly UIColours Black_100 = new UIColours("0.00 0.00 0.00 1.00"); //Black
            public static readonly UIColours Black_050 = new UIColours("0.00 0.00 0.00 0.50");
            public static readonly UIColours Black_015 = new UIColours("0.00 0.00 0.00 0.15");
            public static readonly UIColours Grey2_100 = new UIColours("0.20 0.20 0.20 1.00"); //Grey 2
            public static readonly UIColours Grey2_050 = new UIColours("0.20 0.20 0.20 0.50");
            public static readonly UIColours Grey2_015 = new UIColours("0.20 0.20 0.20 0.15");
            public static readonly UIColours Grey5_100 = new UIColours("0.50 0.50 0.50 1.00"); //Grey 5
            public static readonly UIColours Grey5_050 = new UIColours("0.50 0.50 0.50 0.50");
            public static readonly UIColours Grey5_015 = new UIColours("0.50 0.50 0.50 0.15");
            public static readonly UIColours Grey8_100 = new UIColours("0.80 0.80 0.80 1.00"); //Grey 8
            public static readonly UIColours Grey8_050 = new UIColours("0.80 0.80 0.80 0.50");
            public static readonly UIColours Grey8_015 = new UIColours("0.80 0.80 0.80 0.15");
            public static readonly UIColours White_100 = new UIColours("1.00 1.00 1.00 1.00"); //White
            public static readonly UIColours White_050 = new UIColours("1.00 1.00 1.00 0.50");
            public static readonly UIColours White_015 = new UIColours("1.00 1.00 1.00 0.15");
            public static readonly UIColours Red_100 = new UIColours("0.70 0.20 0.20 1.00");   //Red
            public static readonly UIColours Red_050 = new UIColours("0.70 0.20 0.20 0.50");
            public static readonly UIColours Red_015 = new UIColours("0.70 0.20 0.20 0.15");
            public static readonly UIColours Green_100 = new UIColours("0.20 0.70 0.20 1.00");  //Green
            public static readonly UIColours Green_050 = new UIColours("0.20 0.70 0.20 0.50");
            public static readonly UIColours Green_015 = new UIColours("0.20 0.70 0.20 0.15");
            public static readonly UIColours Blue_100 = new UIColours("0.20 0.20 0.70 1.00");  //Blue
            public static readonly UIColours Blue_050 = new UIColours("0.20 0.20 0.70 0.50");
            public static readonly UIColours Blue_015 = new UIColours("0.20 0.20 0.70 0.15");
            public static readonly UIColours Yellow_100 = new UIColours("0.90 0.90 0.20 1.00");  //Yellow
            public static readonly UIColours Yellow_050 = new UIColours("0.90 0.90 0.20 0.50");
            public static readonly UIColours Yellow_015 = new UIColours("0.90 0.90 0.20 0.15");
            public static readonly UIColours Gold_100 = new UIColours("0.745 0.550 0.045 1.00"); //Gold

            public string Value;
            public int Index;

            private UIColours(string value)
            {
                Value = value;
            }

            public static implicit operator string(UIColours uiColours)
            {
                return uiColours.Value;
            }
        }

        private void DestroyAllPvXUI(BasePlayer player)
        {
            foreach (string _v in GuiList)
            {
                CuiHelper.DestroyUi(player, _v);
            }
            //DestroyEntries(player);
        }
        private void DestroyPvXUI(BasePlayer player, string _ui)
        {
            CuiHelper.DestroyUi(player, _ui);
        }
        #endregion

        #region GUIs
        class GUI
        {
            public static void CreateSignature(ref CuiElementContainer container, string panel, string color, string text, int size, float fadein = 1.0f, TextAnchor align = TextAnchor.LowerLeft)
            {
                float widthPadding = 0.017f;
                float heightPadding = 0.013f;

                float textWidth = 0.966f;
                float textHeight = 0.040f;

                float minWidth = widthPadding;
                float maxWidth = widthPadding + textWidth;
                float minHeight = heightPadding;
                float maxHeight = heightPadding + textHeight;

                QUI.CreateLabel(ref container,
                    panel,
                    color,
                    text,
                    size,
                    $"{minWidth} {minHeight}",
                    $"{maxWidth} {maxHeight}",
                    fadein,
                    align);
            }

            public static class Create
            {
                public static void AdminIndicator(BasePlayer Player)
                {
                    Vector2 dimension = new Vector2(0.174F, 0.028F);
                    Vector2 posMin = new Vector2(instance.adminIndicatorMinWid, instance.adminIndicatorMinHei);
                    Vector2 posMax = posMin + dimension;
                    var adminCountContainer = QUI.CreateElementContainer(
                        pvxAdminUI,
                        UIColours.Black_050,
                        $"{posMin.x} {posMin.y}",
                        $"{posMax.x} {posMax.y}");
                    QUI.CreateLabel(ref adminCountContainer, pvxAdminUI, UIColours.White_100, "PvX Tickets", 10, "0.0 0.1", "0.3 0.90");
                    QUI.CreateLabel(ref adminCountContainer, pvxAdminUI, UIColours.White_100, string.Format("Open: {0}", ModeSwitch.Ticket.Data.ticketData.Tickets.Count.ToString()), 10, "0.301 0.1", "0.65 0.90");
                    QUI.CreateLabel(ref adminCountContainer, pvxAdminUI, UIColours.White_100, string.Format("Closed: {0}", ModeSwitch.Ticket.Data.logData.Logs.Count.ToString()), 10, "0.651 0.1", "1 0.90");

                    CuiHelper.AddUi(Player, adminCountContainer);
                }
                public static void PlayerIndicator(BasePlayer Player)
                {
                    Vector2 dimension = new Vector2(0.031F, 0.028F);
                    Vector2 posMin = new Vector2(instance.playerIndicatorMinWid, instance.playerIndicatorMinHei);
                    Vector2 posMax = posMin + dimension;
                    var indicatorContainer = QUI.CreateElementContainer(
                        pvxIndicator,
                        UIColours.Black_050,
                        "0.48 0.11",
                        "0.52 0.14"
                        );
                    if (Players.Data.playerData.Info[Player.userID].mode == Mode.NA)
                        indicatorContainer = QUI.CreateElementContainer(
                            pvxIndicator,
                            UIColours.Red_100,
                            "0.48 0.11",
                            "0.52 0.14");
                    else if (ModeSwitch.Ticket.Data.ticketData.Tickets.ContainsKey(Player.userID))
                        indicatorContainer = QUI.CreateElementContainer(
                            pvxIndicator,
                            UIColours.Yellow_015,
                            "0.48 0.11",
                            "0.52 0.14");
                    if (Players.Adminmode.ContainsPlayer(Player.userID))
                    {
                        QUI.CreateLabel(
                            ref indicatorContainer,
                            pvxIndicator,
                            UIColours.Green_100,
                            Players.Data.playerData.Info[Player.userID].mode,
                            15,
                            "0.1 0.1",
                            "0.90 0.99");
                    }
                    else
                    {
                        QUI.CreateLabel(ref indicatorContainer,
                            pvxIndicator,
                            UIColours.White_100,
                            Players.Data.playerData.Info[Player.userID].mode,
                            15,
                            "0.1 0.1",
                            "0.90 0.99");
                    }
                    CuiHelper.AddUi(Player, indicatorContainer);
                }

                public static void MenuButton(ref CuiElementContainer container, string panel, string color, string text, int size, int location, string command, float fadein = 1.0f, TextAnchor align = TextAnchor.MiddleCenter)
                {
                    float widthPadding = 0.108f;
                    float heightPadding = 0.013f;

                    float buttonWidth = 0.784f;
                    float buttonHeight = 0.107f;

                    float buttonPadding = 0.027f;

                    float minWidth = widthPadding;
                    float maxWidth = widthPadding + buttonWidth;
                    float minHeight = 1.00f - (buttonHeight + heightPadding + ((buttonHeight + buttonPadding) * location));
                    float maxHeight = 1.00f - (heightPadding + ((buttonHeight + buttonPadding) * location));

                    QUI.CreateButton(ref container,
                        panel,
                        color,
                        text,
                        size,
                        $"{minWidth} {minHeight}",
                        $"{maxWidth} {maxHeight}",
                        command,
                        fadein,
                        align);
                }
                public static void MenuText(ref CuiElementContainer container, string panel, string color, string text, int size, int location, float fadein = 1.0f, TextAnchor align = TextAnchor.LowerLeft)
                {
                    float widthPadding = 0.017f;
                    float heightPadding = 0.013f;

                    float textWidth = 0.966f;
                    float textHeight = 0.040f;

                    float textPadding = 0.001f;

                    float minWidth = widthPadding;
                    float maxWidth = widthPadding + textWidth;
                    float minHeight = 1.00f - (textHeight + heightPadding + ((textHeight + textPadding) * location));
                    float maxHeight = 1.00f - (heightPadding + ((textHeight + textPadding) * location));

                    QUI.CreateLabel(ref container,
                        panel,
                        color,
                        text,
                        size,
                        $"{minWidth} {minHeight}",
                        $"{maxWidth} {maxHeight}",
                        fadein,
                        align);
                }
                public static void ContentButton(ref CuiElementContainer container, string panel, string color, string text, int size, int location, string command, float fadein = 1.0f, TextAnchor align = TextAnchor.MiddleCenter)
                {
                    float widthPadding = 0.017f;
                    float heightPadding = 0.013f;

                    float buttonWidth = 0.1724f;
                    float buttonHeight = 0.068f;

                    float buttonPadding = 0.017f;

                    float minWidth = 1f - (buttonPadding + buttonWidth + ((buttonWidth + buttonPadding) * location));
                    float maxWidth = 1f - (buttonPadding + ((buttonWidth + buttonPadding) * location));
                    float minHeight = heightPadding;
                    float maxHeight = widthPadding + buttonHeight;

                    QUI.CreateButton(ref container,
                        panel,
                        color,
                        text,
                        size,
                        $"{minWidth} {minHeight}",
                        $"{maxWidth} {maxHeight}",
                        command,
                        fadein,
                        align);
                }

                public static class Menu
                {
                    public static void Background(BasePlayer player)
                    {
                        var MainGui = QUI.CreateElementContainer(pvxMenuBack, UIColours.Black_100, "0.297 0.125", "0.703 0.958", true);
                        CuiHelper.AddUi(player, MainGui);
                    }
                    public static void Title(BasePlayer player)
                    {
                        var BannerGui = QUI.CreateElementContainer(pvxMenuBanner, UIColours.Grey2_100, "0.297 0.824", "0.703 0.958", true);

                        var BannerTitle = QUI.CreateElementContainer(pvxMenuBannerTitle, UIColours.Grey5_100, "0.302 0.833", "0.495 0.949");
                        QUI.CreateLabel(ref BannerTitle, pvxMenuBannerTitle, UIColours.Black_100, "PvX Selector", 35, "0 0", "1 1", 1);

                        var BannerVersion = QUI.CreateElementContainer(pvxMenuBannerVersion, UIColours.Grey5_100, "0.500 0.833", "0.594 0.949");
                        QUI.CreateLabel(ref BannerVersion, pvxMenuBannerVersion, UIColours.Black_100, "Version", 25, "0.0 0.6", "1.0 1.0", 1);
                        QUI.CreateLabel(ref BannerVersion, pvxMenuBannerVersion, UIColours.Black_100, instance.Version.ToString(), 21, "0.0 0.0", "1.0 0.6", 1);

                        var BannerModType = QUI.CreateElementContainer(pvxMenuBannerModType, UIColours.Gold_100, "0.599 0.833", "0.698 0.949");
                        QUI.CreateLabel(ref BannerModType, pvxMenuBannerModType, UIColours.Black_100,
                            instance.PvXModtype, 25, "0.0 0.0", "1.0 1.0", 1);

                        CuiHelper.AddUi(player, BannerGui);
                        CuiHelper.AddUi(player, BannerTitle);
                        CuiHelper.AddUi(player, BannerVersion);
                        CuiHelper.AddUi(player, BannerModType);
                    }
                    public static void Selector(BasePlayer player)
                    {
                        var SideMenuGui = QUI.CreateElementContainer(pvxMenuSide, UIColours.Grey2_100, "0.607 0.125", "0.703 0.815", true);

                        GUI.Create.MenuButton(ref SideMenuGui, pvxMenuSide, UIColours.Grey5_100, "<color=black>Welcome</color>", 15, 0, "PvX.Menu.Cmd Welcome");
                        GUI.Create.MenuButton(ref SideMenuGui, pvxMenuSide, UIColours.Grey5_100, "<color=black>Settings</color>", 15, 1, "pvx");
                        GUI.Create.MenuButton(ref SideMenuGui, pvxMenuSide, UIColours.Grey5_100, "<color=black>Character</color>", 15, 2, "PvX.Menu.Cmd Character");
                        GUI.Create.MenuButton(ref SideMenuGui, pvxMenuSide, UIColours.Green_100, "<color=black>Players</color>", 15, 3, "pvx");
                        GUI.Create.MenuButton(ref SideMenuGui, pvxMenuSide, UIColours.Green_100, "<color=black>Tickets</color>", 15, 4, "pvx");
                        GUI.Create.MenuButton(ref SideMenuGui, pvxMenuSide, UIColours.Green_100, "<color=black>Settings</color>", 15, 5, "pvx");
                        GUI.Create.MenuButton(ref SideMenuGui, pvxMenuSide, UIColours.Green_100, "<color=black>Debug</color>", 15, 6, "pvx");
                        QUI.CreateButton(ref SideMenuGui, pvxMenuSide, UIColours.Red_100, "<color=black>X</color>", 15, "0.108 0.013", "0.892 0.047", "PvX.Menu.Cmd Close");
                        CuiHelper.AddUi(player, SideMenuGui);
                    }
                    public static class Content
                    {
                        public static void WelcomePage1(BasePlayer Player)
                        {
                            var MenuGui = QUI.CreateElementContainer(
                                pvxMenuMain,
                                UIColours.Grey2_100,
                                "0.297 0.125",
                                "0.602 0.815",
                                true);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L01), 16, 0);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L02), 16, 1);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L03), 16, 2);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L04), 16, 3);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L05), 16, 4);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L06), 16, 5);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L07), 16, 6);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L08), 16, 7);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L09), 16, 8);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L10), 16, 9);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L11), 16, 10);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L12), 16, 11);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L13), 16, 12);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L14), 16, 13);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L15), 16, 14);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L16), 16, 15);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L17), 16, 16);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L18), 16, 17);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L19), 16, 18);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L20), 16, 19);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P1L21), 16, 20);
                            CreateSignature(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Created by Alphawar", 16, 20);
                            Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, ">", 20, 0, "PvX.Menu.Cmd WelcomePage 1 Next");
                            CuiHelper.AddUi(Player, MenuGui);
                        }
                        public static void WelcomePage2(BasePlayer Player)
                        {
                            var MenuGui = QUI.CreateElementContainer(
                                pvxMenuMain,
                                UIColours.Grey2_100,
                                "0.297 0.125",
                                "0.602 0.815",
                                true);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L01), 16, 0);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L02), 16, 1);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L03), 16, 2);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L04), 16, 3);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L05), 16, 4);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L06), 16, 5);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L07), 16, 6);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L08), 16, 7);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L09), 16, 8);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L10), 16, 9);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L11), 16, 10);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L12), 16, 11);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L13), 16, 12);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L14), 16, 13);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L15), 16, 14);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L16), 16, 15);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L17), 16, 16);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L18), 16, 17);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L19), 16, 18);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L20), 16, 19);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P2L21), 16, 20);
                            CreateSignature(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Created by Alphawar", 16, 20);
                            Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, ">", 20, 0, "PvX.Menu.Cmd WelcomePage 2 Next");
                            Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, "<", 20, 1, "PvX.Menu.Cmd WelcomePage 2 Back");
                            CuiHelper.AddUi(Player, MenuGui);
                        }
                        public static void WelcomePage3(BasePlayer Player)
                        {
                            var MenuGui = QUI.CreateElementContainer(
                                pvxMenuMain,
                                UIColours.Grey2_100,
                                "0.297 0.125",
                                "0.602 0.815",
                                true);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L01), 16, 0);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L02), 16, 1);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L03), 16, 2);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L04), 16, 3);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L05), 16, 4);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L06), 16, 5);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L07), 16, 6);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L08), 16, 7);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L09), 16, 8);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L10), 16, 9);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L11), 16, 10);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L12), 16, 11);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L13), 16, 12);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L14), 16, 13);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L15), 16, 14);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L16), 16, 15);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L17), 16, 16);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L18), 16, 17);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L19), 16, 18);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L20), 16, 19);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.P3L21), 16, 20);
                            CreateSignature(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Created by Alphawar", 16, 20);
                            if (instance.IsMod(Player)) Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, ">", 20, 0, "PvX.Menu.Cmd WelcomePage 3 Next");
                            Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, "<", 20, 1, "PvX.Menu.Cmd WelcomePage 3 Back");
                            CuiHelper.AddUi(Player, MenuGui);
                        }
                        public static void WelcomePage4(BasePlayer Player)
                        {
                            var MenuGui = QUI.CreateElementContainer(
                                pvxMenuMain,
                                UIColours.Grey2_100,
                                "0.297 0.125",
                                "0.602 0.815",
                                true);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L01), 16, 0);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L02), 16, 1);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L03), 16, 2);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L04), 16, 3);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L05), 16, 4);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L06), 16, 5);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L07), 16, 6);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L08), 16, 7);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L09), 16, 8);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L10), 16, 9);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L11), 16, 10);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L12), 16, 11);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L13), 16, 12);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L14), 16, 13);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L15), 16, 14);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L16), 16, 15);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L17), 16, 16);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L18), 16, 17);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L19), 16, 18);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L20), 16, 19);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP1L21), 16, 20);
                            CreateSignature(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Created by Alphawar", 16, 20);
                            Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, ">", 20, 0, "PvX.Menu.Cmd WelcomePage 4 Next");
                            Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, "<", 20, 1, "PvX.Menu.Cmd WelcomePage 4 Back");
                            CuiHelper.AddUi(Player, MenuGui);
                        }
                        public static void WelcomePage5(BasePlayer Player)
                        {
                            var MenuGui = QUI.CreateElementContainer(
                                pvxMenuMain,
                                UIColours.Grey2_100,
                                "0.297 0.125",
                                "0.602 0.815",
                                true);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L01), 16, 0);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L02), 16, 1);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L03), 16, 2);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L04), 16, 3);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L05), 16, 4);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L06), 16, 5);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L07), 16, 6);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L08), 16, 7);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L09), 16, 8);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L10), 16, 9);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L11), 16, 10);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L12), 16, 11);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L13), 16, 12);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L14), 16, 13);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L15), 16, 14);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L16), 16, 15);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L17), 16, 16);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L18), 16, 17);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L19), 16, 18);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L20), 16, 19);
                            Create.MenuText(ref MenuGui, pvxMenuMain, UIColours.Black_100, Messages.Get(Lang.Menu.WelcomePage.AP2L21), 16, 20);
                            CreateSignature(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Created by Alphawar", 16, 20);
                            Create.ContentButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, "<", 20, 1, "PvX.Menu.Cmd WelcomePage 5 Back");
                            CuiHelper.AddUi(Player, MenuGui);
                        }

                        public static void Serversettings(BasePlayer Player)
                        {

                        }
                        public static void Character(BasePlayer Player)
                        {
                            var MenuGui = QUI.CreateElementContainer(
                                pvxMenuMain,
                                UIColours.Grey2_100,
                                "0.297 0.125",
                                "0.602 0.815",
                                true);

                            QUI.CreateLabel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "Player Info", 50, "0.034 0.852", "0.966 0.973");

                            QUI.CreatePanel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "0.034 0.758", "0.379 0.826");
                            QUI.CreateLabel(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Name:", 18, "0.052 0.758", "0.379 0.826", 1, TextAnchor.MiddleLeft);
                            QUI.CreatePanel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "0.397 0.758", "0.966 0.826");
                            QUI.CreateLabel(ref MenuGui, pvxMenuMain, UIColours.Black_100, Player.displayName, 18, "0.397 0.758", "0.966 0.826");

                            QUI.CreatePanel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "0.034 0.678", "0.379 0.745");
                            QUI.CreateLabel(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Steam ID:", 18, "0.052 0.678", "0.379 0.745", 1, TextAnchor.MiddleLeft);
                            QUI.CreatePanel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "0.397 0.678", "0.966 0.745");
                            QUI.CreateLabel(ref MenuGui, pvxMenuMain, UIColours.Black_100, Player.UserIDString, 18, "0.397 0.678", "0.966 0.745");

                            QUI.CreatePanel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "0.034 0.597", "0.379 0.664");
                            QUI.CreateLabel(ref MenuGui, pvxMenuMain, UIColours.Black_100, "Current Mode:", 18, "0.052 0.597", "0.379 0.664", 1, TextAnchor.MiddleLeft);
                            QUI.CreatePanel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "0.397 0.597", "0.966 0.664");
                            QUI.CreateLabel(ref MenuGui, pvxMenuMain, UIColours.Black_100, Players.Data.playerData.Info[Player.userID].mode, 18, "0.397 0.597", "0.966 0.664");


                            QUI.CreatePanel(ref MenuGui, pvxMenuMain, UIColours.Grey5_100, "0.034 0.027", "0.966 0.255");
                            QUI.CreateButton(ref MenuGui, pvxMenuMain, UIColours.Blue_100, "Set Shared Chest", 22, "0.052 0.188", "0.491 0.242", "PvX.Menu.Cmd AddToShared");
                            QUI.CreateButton(ref MenuGui, pvxMenuMain, UIColours.Blue_100, "remove Shared Chest", 22, "0.509 0.188", "0.948 0.242", "PvX.Menu.Cmd RemoveFromShared");
                            QUI.CreateButton(ref MenuGui, pvxMenuMain, UIColours.Green_100, Mode.PvE, 22, "0.052 0.040", "0.491 0.174", $"PvX.Menu.Cmd {Mode.PvE}");
                            QUI.CreateButton(ref MenuGui, pvxMenuMain, UIColours.Red_100, Mode.PvP, 22, "0.509 0.040", "0.948 0.174", $"PvX.Menu.Cmd {Mode.PvP}");

                            CuiHelper.AddUi(Player, MenuGui);
                        }
                    }
                }
            }
            public static class Destroy
            {
                public static void All(BasePlayer Player)
                {
                    CuiHelper.DestroyUi(Player, pvxMenuBack);
                    CuiHelper.DestroyUi(Player, pvxMenuBanner);
                    CuiHelper.DestroyUi(Player, pvxMenuBannerTitle);
                    CuiHelper.DestroyUi(Player, pvxMenuBannerVersion);
                    CuiHelper.DestroyUi(Player, pvxMenuBannerModType);
                    CuiHelper.DestroyUi(Player, pvxMenuSide);
                    CuiHelper.DestroyUi(Player, pvxMenuMain);
                }
                public static void Content(BasePlayer Player)
                {
                    CuiHelper.DestroyUi(Player, pvxMenuMain);
                }
            }
            public static class Update
            {
                public static void AdminIndicator()
                {
                    if (Players.Adminmode.Online == null) return;
                    else if (Players.Adminmode.Online.Count < 1) return;
                    foreach (ulong PlayerID in Players.Adminmode.Online)
                    {
                        BasePlayer Player = Players.Find.BasePlayer(PlayerID);
                        CuiHelper.DestroyUi(Player, pvxAdminUI);
                        Create.AdminIndicator(Player);
                    }
                }
                public static void PlayerIndicator(BasePlayer Player)
                {
                    CuiHelper.DestroyUi(Player, pvxIndicator);
                    Create.PlayerIndicator(Player);
                }
            }
        }
        private void CreatePvXMenu(BasePlayer player)
        {
            GUI.Create.Menu.Background(player);
            GUI.Create.Menu.Title(player);
            GUI.Create.Menu.Selector(player);
            GUI.Create.Menu.Content.WelcomePage1(player);
        }
        #endregion
        
        public static class Messages
        {
            public static string Get(string langMsg, params object[] args)
            {
                return instance.lang.GetMessage(langMsg, instance);
            }
            public static void Chat(IPlayer player, string langMsg, params object[] args)
            {
                string message = instance.lang.GetMessage(langMsg, instance, player.Id);
                player.Reply($"<color={instance.ChatPrefixColor}>{instance.ChatPrefix}</color>: <color={instance.ChatMessageColor}>{message}</color>", args);
            }
            public static void ChatGlobal(string langMsg, params object[] args)
            {
                string message = instance.lang.GetMessage(langMsg, instance);
                instance.PrintToChat($"<color={instance.ChatPrefixColor}>{instance.ChatPrefix}</color>: <color={instance.ChatMessageColor}>{message}</color>", args);
            }
            public static void PutsRcon(string langMsg, params object[] args)
            {
                string message = instance.lang.GetMessage(langMsg, instance);
                instance.Puts(string.Format(message, args));

            }
        }

        public class Lang
        {
            public class Menu
            {
                public class WelcomePage
                {
                    public static readonly WelcomePage P1L01 = new WelcomePage("Page1Line1");
                    public static readonly WelcomePage P1L02 = new WelcomePage("Page1Line2");
                    public static readonly WelcomePage P1L03 = new WelcomePage("Page1Line3");
                    public static readonly WelcomePage P1L04 = new WelcomePage("Page1Line4");
                    public static readonly WelcomePage P1L05 = new WelcomePage("Page1Line5");
                    public static readonly WelcomePage P1L06 = new WelcomePage("Page1Line6");
                    public static readonly WelcomePage P1L07 = new WelcomePage("Page1Line7");
                    public static readonly WelcomePage P1L08 = new WelcomePage("Page1Line8");
                    public static readonly WelcomePage P1L09 = new WelcomePage("Page1Line9");
                    public static readonly WelcomePage P1L10 = new WelcomePage("Page1Line10");
                    public static readonly WelcomePage P1L11 = new WelcomePage("Page1Line11");
                    public static readonly WelcomePage P1L12 = new WelcomePage("Page1Line12");
                    public static readonly WelcomePage P1L13 = new WelcomePage("Page1Line13");
                    public static readonly WelcomePage P1L14 = new WelcomePage("Page1Line14");
                    public static readonly WelcomePage P1L15 = new WelcomePage("Page1Line15");
                    public static readonly WelcomePage P1L16 = new WelcomePage("Page1Line16");
                    public static readonly WelcomePage P1L17 = new WelcomePage("Page1Line17");
                    public static readonly WelcomePage P1L18 = new WelcomePage("Page1Line18");
                    public static readonly WelcomePage P1L19 = new WelcomePage("Page1Line19");
                    public static readonly WelcomePage P1L20 = new WelcomePage("Page1Line20");
                    public static readonly WelcomePage P1L21 = new WelcomePage("Page1Line21");

                    public static readonly WelcomePage P2L01 = new WelcomePage("Page2Line1");
                    public static readonly WelcomePage P2L02 = new WelcomePage("Page2Line2");
                    public static readonly WelcomePage P2L03 = new WelcomePage("Page2Line3");
                    public static readonly WelcomePage P2L04 = new WelcomePage("Page2Line4");
                    public static readonly WelcomePage P2L05 = new WelcomePage("Page2Line5");
                    public static readonly WelcomePage P2L06 = new WelcomePage("Page2Line6");
                    public static readonly WelcomePage P2L07 = new WelcomePage("Page2Line7");
                    public static readonly WelcomePage P2L08 = new WelcomePage("Page2Line8");
                    public static readonly WelcomePage P2L09 = new WelcomePage("Page2Line9");
                    public static readonly WelcomePage P2L10 = new WelcomePage("Page2Line10");
                    public static readonly WelcomePage P2L11 = new WelcomePage("Page2Line11");
                    public static readonly WelcomePage P2L12 = new WelcomePage("Page2Line12");
                    public static readonly WelcomePage P2L13 = new WelcomePage("Page2Line13");
                    public static readonly WelcomePage P2L14 = new WelcomePage("Page2Line14");
                    public static readonly WelcomePage P2L15 = new WelcomePage("Page2Line15");
                    public static readonly WelcomePage P2L16 = new WelcomePage("Page2Line16");
                    public static readonly WelcomePage P2L17 = new WelcomePage("Page2Line17");
                    public static readonly WelcomePage P2L18 = new WelcomePage("Page2Line18");
                    public static readonly WelcomePage P2L19 = new WelcomePage("Page2Line19");
                    public static readonly WelcomePage P2L20 = new WelcomePage("Page2Line20");
                    public static readonly WelcomePage P2L21 = new WelcomePage("Page2Line21");

                    public static readonly WelcomePage P3L01 = new WelcomePage("Page3Line1");
                    public static readonly WelcomePage P3L02 = new WelcomePage("Page3Line2");
                    public static readonly WelcomePage P3L03 = new WelcomePage("Page3Line3");
                    public static readonly WelcomePage P3L04 = new WelcomePage("Page3Line4");
                    public static readonly WelcomePage P3L05 = new WelcomePage("Page3Line5");
                    public static readonly WelcomePage P3L06 = new WelcomePage("Page3Line6");
                    public static readonly WelcomePage P3L07 = new WelcomePage("Page3Line7");
                    public static readonly WelcomePage P3L08 = new WelcomePage("Page3Line8");
                    public static readonly WelcomePage P3L09 = new WelcomePage("Page3Line9");
                    public static readonly WelcomePage P3L10 = new WelcomePage("Page3Line10");
                    public static readonly WelcomePage P3L11 = new WelcomePage("Page3Line11");
                    public static readonly WelcomePage P3L12 = new WelcomePage("Page3Line12");
                    public static readonly WelcomePage P3L13 = new WelcomePage("Page3Line13");
                    public static readonly WelcomePage P3L14 = new WelcomePage("Page3Line14");
                    public static readonly WelcomePage P3L15 = new WelcomePage("Page3Line15");
                    public static readonly WelcomePage P3L16 = new WelcomePage("Page3Line16");
                    public static readonly WelcomePage P3L17 = new WelcomePage("Page3Line17");
                    public static readonly WelcomePage P3L18 = new WelcomePage("Page3Line18");
                    public static readonly WelcomePage P3L19 = new WelcomePage("Page3Line19");
                    public static readonly WelcomePage P3L20 = new WelcomePage("Page3Line20");
                    public static readonly WelcomePage P3L21 = new WelcomePage("Page3Line21");

                    public static readonly WelcomePage AP1L01 = new WelcomePage("AdminPage1Line1");
                    public static readonly WelcomePage AP1L02 = new WelcomePage("AdminPage1Line2");
                    public static readonly WelcomePage AP1L03 = new WelcomePage("AdminPage1Line3");
                    public static readonly WelcomePage AP1L04 = new WelcomePage("AdminPage1Line4");
                    public static readonly WelcomePage AP1L05 = new WelcomePage("AdminPage1Line5");
                    public static readonly WelcomePage AP1L06 = new WelcomePage("AdminPage1Line6");
                    public static readonly WelcomePage AP1L07 = new WelcomePage("AdminPage1Line7");
                    public static readonly WelcomePage AP1L08 = new WelcomePage("AdminPage1Line8");
                    public static readonly WelcomePage AP1L09 = new WelcomePage("AdminPage1Line9");
                    public static readonly WelcomePage AP1L10 = new WelcomePage("AdminPage1Line10");
                    public static readonly WelcomePage AP1L11 = new WelcomePage("AdminPage1Line11");
                    public static readonly WelcomePage AP1L12 = new WelcomePage("AdminPage1Line12");
                    public static readonly WelcomePage AP1L13 = new WelcomePage("AdminPage1Line13");
                    public static readonly WelcomePage AP1L14 = new WelcomePage("AdminPage1Line14");
                    public static readonly WelcomePage AP1L15 = new WelcomePage("AdminPage1Line15");
                    public static readonly WelcomePage AP1L16 = new WelcomePage("AdminPage1Line16");
                    public static readonly WelcomePage AP1L17 = new WelcomePage("AdminPage1Line17");
                    public static readonly WelcomePage AP1L18 = new WelcomePage("AdminPage1Line18");
                    public static readonly WelcomePage AP1L19 = new WelcomePage("AdminPage1Line19");
                    public static readonly WelcomePage AP1L20 = new WelcomePage("AdminPage1Line20");
                    public static readonly WelcomePage AP1L21 = new WelcomePage("AdminPage1Line21");

                    public static readonly WelcomePage AP2L01 = new WelcomePage("AdminPage2Line1");
                    public static readonly WelcomePage AP2L02 = new WelcomePage("AdminPage2Line2");
                    public static readonly WelcomePage AP2L03 = new WelcomePage("AdminPage2Line3");
                    public static readonly WelcomePage AP2L04 = new WelcomePage("AdminPage2Line4");
                    public static readonly WelcomePage AP2L05 = new WelcomePage("AdminPage2Line5");
                    public static readonly WelcomePage AP2L06 = new WelcomePage("AdminPage2Line6");
                    public static readonly WelcomePage AP2L07 = new WelcomePage("AdminPage2Line7");
                    public static readonly WelcomePage AP2L08 = new WelcomePage("AdminPage2Line8");
                    public static readonly WelcomePage AP2L09 = new WelcomePage("AdminPage2Line9");
                    public static readonly WelcomePage AP2L10 = new WelcomePage("AdminPage2Line10");
                    public static readonly WelcomePage AP2L11 = new WelcomePage("AdminPage2Line11");
                    public static readonly WelcomePage AP2L12 = new WelcomePage("AdminPage2Line12");
                    public static readonly WelcomePage AP2L13 = new WelcomePage("AdminPage2Line13");
                    public static readonly WelcomePage AP2L14 = new WelcomePage("AdminPage2Line14");
                    public static readonly WelcomePage AP2L15 = new WelcomePage("AdminPage2Line15");
                    public static readonly WelcomePage AP2L16 = new WelcomePage("AdminPage2Line16");
                    public static readonly WelcomePage AP2L17 = new WelcomePage("AdminPage2Line17");
                    public static readonly WelcomePage AP2L18 = new WelcomePage("AdminPage2Line18");
                    public static readonly WelcomePage AP2L19 = new WelcomePage("AdminPage2Line19");
                    public static readonly WelcomePage AP2L20 = new WelcomePage("AdminPage2Line20");
                    public static readonly WelcomePage AP2L21 = new WelcomePage("AdminPage2Line21");

                    public string Value;
                    private WelcomePage(string welcomePage) { Value = this.ToString() + "-" + welcomePage; }
                    public static implicit operator string(WelcomePage welcomePage) { return welcomePage.Value; }
                }
            }
            public class ModInit
            {
                public static readonly ModInit CntFindData = new ModInit("CantFindData");
                public static readonly ModInit LoadingData = new ModInit("CreateNewData");

                public string Value;
                private ModInit(string modInit) { Value = this.ToString() + "-" + modInit; }
                public static implicit operator string(ModInit modInit) { return modInit.Value; }
            }
            //public class Menu
            //{
            //    public static readonly Menu Accepted = new Menu("Accepted");

            //    public string Value;
            //    private Menu(string menu) { Value = this.ToString() + "-" + menu; }
            //    public static implicit operator string(Menu menu) { return menu.Value; }
            //}
            public class Chat
            {
                public static readonly Chat Accepted = new Chat("Accepted");

                public string Value;
                private Chat(string chat) { Value = this.ToString() + "-" + chat; }
                public static implicit operator string(Chat chat) { return chat.Value; }
            }


            public static readonly Dictionary<string, string> List = new Dictionary<string, string>()
            {
#region Welcome Page
                {Menu.WelcomePage.P1L01, "Welcome To PvX Selector." },
                {Menu.WelcomePage.P1L02, "" },
                {Menu.WelcomePage.P1L03, "This mod allows you to select either PvP or PvE, this" },
                {Menu.WelcomePage.P1L04, "means you get to play on the server how you want to." },
                {Menu.WelcomePage.P1L05, "" },
                {Menu.WelcomePage.P1L06, "If you have not yet selected a mode please click on the" },
                {Menu.WelcomePage.P1L07, "character button and choose your mode, you can always" },
                {Menu.WelcomePage.P1L08, "change it later." },
                {Menu.WelcomePage.P1L09, "" },
                {Menu.WelcomePage.P1L10, "If you would like to see the servers configuration for the" },
                {Menu.WelcomePage.P1L11, "mod please click on the Settings button." },
                {Menu.WelcomePage.P1L12, "" },
                {Menu.WelcomePage.P1L13, "The Players panel allows you to see online players and" },
                {Menu.WelcomePage.P1L14, "what mode they are playing as." },
                {Menu.WelcomePage.P1L15, "" },
                {Menu.WelcomePage.P1L16, "If you have any questions please contact a server admin." },
                {Menu.WelcomePage.P1L17, "" },
                {Menu.WelcomePage.P1L18, "" },
                {Menu.WelcomePage.P1L19, "" },
                {Menu.WelcomePage.P1L20, "" },
                {Menu.WelcomePage.P1L21, "" },

                {Menu.WelcomePage.P2L01, "" },
                {Menu.WelcomePage.P2L02, "" },
                {Menu.WelcomePage.P2L03, "" },
                {Menu.WelcomePage.P2L04, "" },
                {Menu.WelcomePage.P2L05, "" },
                {Menu.WelcomePage.P2L06, "" },
                {Menu.WelcomePage.P2L07, "" },
                {Menu.WelcomePage.P2L08, "" },
                {Menu.WelcomePage.P2L09, "" },
                {Menu.WelcomePage.P2L10, "" },
                {Menu.WelcomePage.P2L11, "" },
                {Menu.WelcomePage.P2L12, "" },
                {Menu.WelcomePage.P2L13, "" },
                {Menu.WelcomePage.P2L14, "" },
                {Menu.WelcomePage.P2L15, "" },
                {Menu.WelcomePage.P2L16, "" },
                {Menu.WelcomePage.P2L17, "" },
                {Menu.WelcomePage.P2L18, "" },
                {Menu.WelcomePage.P2L19, "" },
                {Menu.WelcomePage.P2L20, "" },
                {Menu.WelcomePage.P2L21, "" },

                {Menu.WelcomePage.P3L01, "" },
                {Menu.WelcomePage.P3L02, "" },
                {Menu.WelcomePage.P3L03, "" },
                {Menu.WelcomePage.P3L04, "" },
                {Menu.WelcomePage.P3L05, "" },
                {Menu.WelcomePage.P3L06, "" },
                {Menu.WelcomePage.P3L07, "" },
                {Menu.WelcomePage.P3L08, "" },
                {Menu.WelcomePage.P3L09, "" },
                {Menu.WelcomePage.P3L10, "" },
                {Menu.WelcomePage.P3L11, "" },
                {Menu.WelcomePage.P3L12, "" },
                {Menu.WelcomePage.P3L13, "" },
                {Menu.WelcomePage.P3L14, "" },
                {Menu.WelcomePage.P3L15, "" },
                {Menu.WelcomePage.P3L16, "" },
                {Menu.WelcomePage.P3L17, "" },
                {Menu.WelcomePage.P3L18, "" },
                {Menu.WelcomePage.P3L19, "" },
                {Menu.WelcomePage.P3L20, "" },
                {Menu.WelcomePage.P3L21, "" },

                {Menu.WelcomePage.AP1L01, "Hello Admins and Moderators" },
                {Menu.WelcomePage.AP1L02, "" },
                {Menu.WelcomePage.AP1L03, "This page is here to provide you information about" },
                {Menu.WelcomePage.AP1L04, "the admin and moderator buttons." },
                {Menu.WelcomePage.AP1L05, "" },
                {Menu.WelcomePage.AP1L06, "Moderators+" },
                {Menu.WelcomePage.AP1L07, "Players Panel: This panel will be different from what" },
                {Menu.WelcomePage.AP1L08, "regular players see, the main difference is this provides" },
                {Menu.WelcomePage.AP1L09, "aditional information about a player including ticket count." },
                {Menu.WelcomePage.AP1L10, "" },
                {Menu.WelcomePage.AP1L11, "Tickets: This panel will allow you to view all tickets, from" },
                {Menu.WelcomePage.AP1L12, "this menu you will be able to accept or decline them." },
                {Menu.WelcomePage.AP1L13, "" },
                {Menu.WelcomePage.AP1L14, "Admin" },
                {Menu.WelcomePage.AP1L15, "Settings: Allows you to make changes to the mod on the fly," },
                {Menu.WelcomePage.AP1L16, "these changes are then saved to the config." },
                {Menu.WelcomePage.AP1L17, "" },
                {Menu.WelcomePage.AP1L18, "Debug: At this time it does not have a function, but I plan" },
                {Menu.WelcomePage.AP1L19, "to allow admins to fix and reset data files, possible I will" },
                {Menu.WelcomePage.AP1L20, "add the capability to enable debug chat in that pannel" },
                {Menu.WelcomePage.AP1L21, "" },

                {Menu.WelcomePage.AP2L01, "" },
                {Menu.WelcomePage.AP2L02, "" },
                {Menu.WelcomePage.AP2L03, "" },
                {Menu.WelcomePage.AP2L04, "" },
                {Menu.WelcomePage.AP2L05, "" },
                {Menu.WelcomePage.AP2L06, "" },
                {Menu.WelcomePage.AP2L07, "" },
                {Menu.WelcomePage.AP2L08, "" },
                {Menu.WelcomePage.AP2L09, "" },
                {Menu.WelcomePage.AP2L10, "" },
                {Menu.WelcomePage.AP2L11, "" },
                {Menu.WelcomePage.AP2L12, "" },
                {Menu.WelcomePage.AP2L13, "" },
                {Menu.WelcomePage.AP2L14, "" },
                {Menu.WelcomePage.AP2L15, "" },
                {Menu.WelcomePage.AP2L16, "" },
                {Menu.WelcomePage.AP2L17, "" },
                {Menu.WelcomePage.AP2L18, "" },
                {Menu.WelcomePage.AP2L19, "" },
                {Menu.WelcomePage.AP2L20, "" },
                {Menu.WelcomePage.AP2L21, "" },
#endregion
            };
        }


        void OnEntityTakeDamage(BaseCombatEntity Target, HitInfo HitInfo)
        {
            BaseEntity _attacker = HitInfo.Initiator;
            object _n = Target.GetType();
            
            if (_attacker is BasePlayer && Target is AutoTurret) PlayerVTurret((AutoTurret)Target, (BasePlayer)_attacker, HitInfo);                          //Player V Turret
            else if (_attacker is AutoTurret && Target is BasePlayer) TurretVPlayer((BasePlayer)Target, (AutoTurret)_attacker, HitInfo);                          //Turret V Player
            else if (_attacker is AutoTurret && Target is AutoTurret) TurretVTurret((AutoTurret)Target, (AutoTurret)_attacker, HitInfo);                          //Turret V Turret
            else if (_attacker is AutoTurret && Target is BaseNpc) TurretVAnimal((BaseNpc)Target, (AutoTurret)_attacker, HitInfo);                                //Turret V Animal
        }
        void PlayerVTurret(AutoTurret Target, BasePlayer Attacker, HitInfo HitInfo)
        {
            //Puts("Calling PvT");
            ulong _ownerID = Target.OwnerID;
            if (IsGod(Attacker)) return;
            else if (IsInEvent(Attacker)) return;
            else if (Players.State.IsNPC(Attacker) && Players.State.IsPvE(_ownerID)) ModifyDamage(HitInfo, TurretPvEDamageNPCAmnt);
            else if (Players.State.IsNPC(Attacker) && Players.State.IsPvP(_ownerID)) ModifyDamage(HitInfo, TurretPvPDamageNPCAmnt);
            else if (Players.State.IsPvE(Attacker) && Players.State.IsPvE(_ownerID)) ModifyDamage(HitInfo, TurretPvEDamagePvEAmnt);
            else if (Players.State.IsPvE(Attacker) && Players.State.IsPvP(_ownerID)) ModifyDamage(HitInfo, TurretPvEDamagePvPAmnt);
            else if (Players.State.IsPvP(Attacker) && Players.State.IsPvE(_ownerID)) ModifyDamage(HitInfo, TurretPvPDamagePvEAmnt);
            else if (Players.State.IsPvP(Attacker) && Players.State.IsPvP(_ownerID)) ModifyDamage(HitInfo, TurretPvPDamagePvPAmnt);
            else ModifyDamage(HitInfo, 0);
        }
        void TurretVPlayer(BasePlayer Target, AutoTurret Attacker, HitInfo HitInfo)
        {
            //Puts("Calling TvP");
            ulong _attackerID = Attacker.OwnerID;
            if (IsGod(Target)) return;
            else if (IsInEvent(Target)) return;
            else if (Players.State.IsPvE(_attackerID) && Players.State.IsNPC(Target)) ModifyDamage(HitInfo, TurretPvEDamageNPCAmnt);
            else if (Players.State.IsPvP(_attackerID) && Players.State.IsNPC(Target)) ModifyDamage(HitInfo, TurretPvPDamageNPCAmnt);
            else if (Players.State.IsPvE(_attackerID) && Players.State.IsPvE(Target)) ModifyDamage(HitInfo, TurretPvEDamagePvEAmnt);
            else if (Players.State.IsPvE(_attackerID) && Players.State.IsPvP(Target)) ModifyDamage(HitInfo, TurretPvEDamagePvPAmnt);
            else if (Players.State.IsPvP(_attackerID) && Players.State.IsPvE(Target)) ModifyDamage(HitInfo, TurretPvPDamagePvEAmnt);
            else if (Players.State.IsPvP(_attackerID) && Players.State.IsPvP(Target)) ModifyDamage(HitInfo, TurretPvPDamagePvPAmnt);
            else ModifyDamage(HitInfo, 0);
        }
        void TurretVTurret(AutoTurret Target, AutoTurret Attacker, HitInfo HitInfo)
        {
            //Puts("Calling TvT");
            ulong _targetID = Target.OwnerID;
            ulong _attackerID = Target.OwnerID;
            if (Players.State.IsPvE(_attackerID) && Players.State.IsPvE(_targetID)) ModifyDamage(HitInfo, TurretPvEDamagePvEAmnt);
            else if (Players.State.IsPvE(_attackerID) && Players.State.IsPvP(_targetID)) ModifyDamage(HitInfo, TurretPvEDamagePvPAmnt);
            else if (Players.State.IsPvP(_attackerID) && Players.State.IsPvE(_targetID)) ModifyDamage(HitInfo, TurretPvPDamagePvEAmnt);
            else if (Players.State.IsPvP(_attackerID) && Players.State.IsPvP(_targetID)) ModifyDamage(HitInfo, TurretPvPDamagePvPAmnt);
            else ModifyDamage(HitInfo, 0);
        }
        void TurretVAnimal(BaseNpc Target, AutoTurret Attacker, HitInfo HitInfo)
        {
            //Puts("Calling TvA");
            ulong _turretOwner = Attacker.OwnerID;
            if (Players.State.IsPvE(_turretOwner) && TurretPvETargetAnimal) ModifyDamage(HitInfo, TurretPvEDamageAnimalAmnt);
            else if (Players.State.IsPvP(_turretOwner) && TurretPvPTargetAnimal) ModifyDamage(HitInfo, TurretPvPDamageAnimalAmnt);
            else ModifyDamage(HitInfo, 0);
        }

    }
}
