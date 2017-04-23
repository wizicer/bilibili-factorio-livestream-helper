using BilibiliDM_PluginFramework;
using BiliDMLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelperUI
{
    public partial class frmMain : Form
    {
        private readonly DanmakuLoader b = new DanmakuLoader();

        public frmMain()
        {
            InitializeComponent();
            b.Disconnected += b_Disconnected;
            b.ReceivedDanmaku += B_ReceivedDanmaku; ;
            b.ReceivedRoomCount += B_ReceivedRoomCount; ;

        }

        private void B_ReceivedRoomCount(object sender, ReceivedRoomCountArgs e)
        {
            logging("count: " + e.UserCount.ToString());
        }

        private void B_ReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {
            //logging("received: " + e.Danmaku.);
            ProcDanmaku(e.Danmaku);
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
                            logging("收到道具:" + danmakuModel.GiftUser + " 赠送的: " + danmakuModel.GiftName + " x " +
                                    danmakuModel.GiftNum);
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

        private void logging(string text)
        {
            this.Invoke((Action)(() => this.rtfStatus.AppendText(text, Color.Blue, true)));
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
