using BilibiliDM_PluginFramework;
using BiliDMLib;
using SourceRconLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelperUI
{
    public partial class frmMain : Form
    {
        private readonly DanmakuLoader b = new DanmakuLoader();
        internal class CommandTrigger
        {
            public string[] Keys { get; set; }
            public CommandMessage Command { get; set; }
            public CommandMessage RevertCommand { get; set; }
            public int RevertDelay { get; set; } // in seconds
        }
        internal class CommandMessage
        {
            public CommandMessage(string command, string message)
            {
                Command = command;
                Message = message;
            }
            public string Command { get; set; }
            public string Message { get; set; }
        }

        private List<CommandTrigger> lstCommands = new List<CommandTrigger>();

        public frmMain()
        {
            InitializeComponent();
            b.Disconnected += b_Disconnected;
            b.ReceivedDanmaku += B_ReceivedDanmaku; ;
            b.ReceivedRoomCount += B_ReceivedRoomCount; ;
            this.InitCommands();
        }
        private void InitCommands()
        {
            // set game time to mid night
            this.lstCommands.Add(new CommandTrigger
            {
                Keys = new[] { "天黑" },
                Command = new CommandMessage("game.surfaces[1].daytime=0.5", "It's midnight"),
            });
            // set game time to mid day
            this.lstCommands.Add(new CommandTrigger
            {
                Keys = new[] { "天亮" },
                Command = new CommandMessage("game.surfaces[1].daytime=0", "It's midday"),
            });
            // set player to infinite free crafting
            this.lstCommands.Add(new CommandTrigger
            {
                Keys = new[] { "无限" },
                Command = new CommandMessage("game.players[1].cheat_mode=true", "You are god now."),
                RevertCommand = new CommandMessage("game.players[1].cheat_mode=false", "Not god anymore."),
                RevertDelay = 15,
            });
        }

        private void B_ReceivedRoomCount(object sender, ReceivedRoomCountArgs e)
        {
            logging("count: " + e.UserCount.ToString());
        }

        private void B_ReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {
            //logging("received: " + e.Danmaku.);
            ProcDanmaku(e.Danmaku);
            if (e.Danmaku.MsgType == MsgTypeEnum.Comment)
            {
                foreach (var cmd in this.lstCommands)
                {
                    if (cmd.Keys.Any(_ => _ == e.Danmaku.CommentText))
                    {
                        this.ExecuteCommand(cmd.Command);
                        if (cmd.RevertDelay > 0)
                        {
                            System.Threading.Timer timer = null;
                            timer = new System.Threading.Timer(
                                _ =>
                                {
                                    this.Invoke((Action)(() => this.ExecuteCommand(cmd.RevertCommand)));
                                    Console.WriteLine("time out");
                                    timer.Dispose();
                                },
                                null,
                                cmd.RevertDelay * 1000,
                                int.MaxValue);
                        }
                        break;
                    }
                }
            }
        }
        private void ExecuteCommand(CommandMessage cmd)
        {
            sr.ServerCommand("/silent-command " + cmd.Command);
            if (!string.IsNullOrEmpty(cmd.Message)) sr.ServerCommand(cmd.Message);

        }

        private void ProcDanmaku(DanmakuModel danmakuModel)
        {
            switch (danmakuModel.MsgType)
            {
                case MsgTypeEnum.Comment:
                    logging("收到彈幕:" + (danmakuModel.isAdmin ? "[管]" : "") + (danmakuModel.isVIP ? "[爷]" : "") +
                            danmakuModel.CommentUser + " 說: " + danmakuModel.CommentText);
                    break;
                case MsgTypeEnum.GiftTop:
                    //foreach (var giftRank in danmakuModel.GiftRanking)
                    //{
                    //    var query = Ranking.Where(p => p.uid == giftRank.uid);
                    //    if (query.Any())
                    //    {
                    //        var f = query.First();
                    //        Dispatcher.BeginInvoke(new Action(() => f.coin = giftRank.coin));
                    //    }
                    //    else
                    //    {
                    //        Dispatcher.BeginInvoke(new Action(() => Ranking.Add(new GiftRank
                    //        {
                    //            uid = giftRank.uid,
                    //            coin = giftRank.coin,
                    //            UserName = giftRank.UserName
                    //        })));
                    //    }
                    //}
                    break;
                case MsgTypeEnum.GiftSend:
                    {
                        //lock (SessionItems)
                        //{
                        //    var query =
                        //        SessionItems.Where(
                        //            p => p.UserName == danmakuModel.GiftUser && p.Item == danmakuModel.GiftName).ToArray();
                        //    if (query.Any())
                        //    {
                        //        Dispatcher.BeginInvoke(
                        //            new Action(() => query.First().num += Convert.ToDecimal(danmakuModel.GiftNum)));
                        //    }
                        //    else
                        //    {
                        //        Dispatcher.BeginInvoke(new Action(() =>
                        //        {
                        //            lock (SessionItems)
                        //            {
                        //                SessionItems.Add(
                        //                    new SessionItem
                        //                    {
                        //                        Item = danmakuModel.GiftName,
                        //                        UserName = danmakuModel.GiftUser,
                        //                        num = Convert.ToDecimal(danmakuModel.GiftNum)
                        //                    }
                        //                );

                        //            }
                        //        }));
                        //    }
                        logging("收到道具:" + danmakuModel.GiftUser + " 赠送的: " + danmakuModel.GiftName + " x " + danmakuModel.GiftNum);
                        //    Dispatcher.BeginInvoke(new Action(() =>
                        //    {
                        //        if (ShowItem.IsChecked == true)
                        //        {
                        //            AddDMText("收到道具",
                        //                danmakuModel.GiftUser + " 赠送的: " + danmakuModel.GiftName + " x " +
                        //                danmakuModel.GiftNum, true);
                        //        }
                        //    }));
                        break;
                        //}
                    }
                case MsgTypeEnum.Welcome:
                    {
                        logging("欢迎老爷" + (danmakuModel.isAdmin ? "和管理" : "") + ": " + danmakuModel.CommentUser + " 进入直播间");
                        //Dispatcher.BeginInvoke(new Action(() =>
                        //{
                        //    if (ShowItem.IsChecked == true)
                        //    {
                        //        AddDMText("欢迎老爷" + (danmakuModel.isAdmin ? "和管理" : ""),
                        //            danmakuModel.CommentUser + " 进入直播间", true);
                        //    }
                        //}));

                        break;
                    }
            }
            //if (rawoutput_mode)
            //{
            logging(danmakuModel.RawData);
            //}
        }

        private void b_Disconnected(object sender, DisconnectEvtArgs args)
        {
            logging("連接被斷開: 开发者信息" + args.Error);
            //AddDMText("彈幕姬報告", "連接被斷開", true);
            //SendSSP("連接被斷開");
            //if (CheckAccess())
            //{
            //    if (AutoReconnect.IsChecked == true && args.Error != null)
            //    {
            //        errorlogging("正在自动重连...");
            //        AddDMText("彈幕姬報告", "正在自动重连", true);
            //        connbtn_Click(null, null);
            //    }
            //    else
            //    {
            //        ConnBtn.IsEnabled = true;
            //    }
            //}
            //else
            //{
            //    Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        if (AutoReconnect.IsChecked == true && args.Error != null)
            //        {
            //            errorlogging("正在自动重连...");
            //            AddDMText("彈幕姬報告", "正在自动重连", true);
            //            connbtn_Click(null, null);
            //        }
            //        else
            //        {
            //            ConnBtn.IsEnabled = true;
            //        }
            //    }));
            //}
        }

        private void loggingf(string text)
        {
            logging(text, Color.Green, true);
        }

        private void loggingerror(string text)
        {
            logging(text, Color.Red, true);
        }


        private void logging(string text)
        {
            logging(text, Color.Blue, true);
        }
        private void logging(string text, Color color, bool newline = false)
        {
            this.Invoke((Action)(() =>
            {
                this.rtfStatus.AppendText(text, color, newline);
                this.rtfStatus.SelectionStart = this.rtfStatus.Text.Length;
                this.rtfStatus.ScrollToCaret();
            }));
        }

        private async void Connect(int roomId)
        {
            if (roomId > 0)
            {
                logging("正在连接");
                var connectresult = await b.ConnectAsync(roomId);



                if (connectresult)
                {
                    logging("連接成功");
                    //AddDMText("彈幕姬報告", "連接成功", true);
                    //SendSSP("連接成功");
                    //Ranking.Clear();
                    //SaveRoomId(roomId);

                }
                else
                {
                    logging("連接失敗");
                    //SendSSP("連接失敗");
                    //AddDMText("彈幕姬報告", "連接失敗", true);

                    //ConnBtn.IsEnabled = true;
                }
                //DisconnBtn.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("ID非法");
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Connect(3996224);
        }

        private void btnStartFactorio_Click(object sender, EventArgs e)
        {
            var p = new Process();

            var FactorioExeLocation = @"C:\Program Files (x86)\Steam\steamapps\common\Factorio\bin\x64\";
            var SaveFileName = @"temp.zip";
            var SaveLocation = @"C:\Users\icer\AppData\Roaming\Factorio\saves\";
            var ServerSettingsFileName = @"server-settings.json";
            var ServerSettingsLocation = @"C:\Program Files (x86)\Steam\steamapps\common\Factorio\data\";
            var ServerConfigFileName = @"config.ini";
            var ServerConfigLocation = @"C:\Users\icer\AppData\Roaming\Factorio\serverconfig\";

            var st = new ProcessStartInfo
            {
                WorkingDirectory = FactorioExeLocation,
                FileName = "cmd",
                Arguments = $"/c factorio.exe --start-server \"{ SaveLocation }{ SaveFileName }\"" +
                    $" --server-settings \"{ServerSettingsLocation}{ServerSettingsFileName}\"" +
                    $" -c \"{ServerConfigLocation}{ServerConfigFileName}\"" +
                    $" --rcon-port 12345 --rcon-password 12345"
            };

            p.StartInfo = st;
            p.Start();
        }
        Rcon sr;
        private void btnConnectRCon_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 12345;

            IPEndPoint ipe = new IPEndPoint(ip, port);

            sr = new Rcon();
            sr.ConnectionSuccess += new BoolInfo(sr_ConnectionSuccess);
            sr.ServerOutput += new RconOutput(sr_ServerOutput);
            sr.Errors += new RconOutput(sr_Errors);

            sr.Connect(ipe, "12345");
            while (!sr.Connected)
            {
                Thread.Sleep(10);
            }
            sr.ServerCommand("/p");
            sr.ServerCommand("/p");
            //playerBackGround.RunWorkerAsync();

        }

        void sr_ConnectionSuccess(bool info)
        {
            if (info)
            {
                loggingf("Connected");
            }
        }
        void sr_Errors(MessageCode code, string data)
        {
            loggingerror($"ERROR: [{code}]{data}");
        }

        void sr_ServerOutput(MessageCode code, string data)
        {

            //if (data.Contains("Players "))
            //{
            //    string[] players = data.Split(' ');
            //    foreach (string i in players)
            //    {
            //        loggingf($"player: {i}");

            //    }
            //}
            //else 
            if (data.Contains("Player "))
            {
                loggingf("I interuppted your console write");
            }
            else
            {
                loggingf(data);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void Send()
        {
            sr.ServerCommand(this.txtCommand.Text);
        }

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Send();
                txtCommand.Text = string.Empty;
            }
        }
    }
    public static class HelperExtension
    {
        public static void AppendText(this RichTextBox box, string text, Color color, bool AddNewLine = false)
        {
            if (AddNewLine)
            {
                text += Environment.NewLine;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

    }
}
