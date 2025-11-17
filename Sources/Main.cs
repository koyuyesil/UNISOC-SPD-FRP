using iReverse_UniSPD_FRP.My;
using iReverse_UniSPD_FRP.UniSPD;
using iReverse_UniSPD_FRP.UniSPD.Method;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace iReverse_UniSPD_FRP
{
    public partial class Main : Form
    {
        public static Main SharedUI;
        public static bool isUniSPDRunning = false;
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static MySerialDevice myserial;

        public Main()
        {
            InitializeComponent();
            SharedUI = this;
            MyUSBFastConnect.getcomInfo();
            UniSPDDevice.CreateListDevice();
        }

        private void comboBoxTimeout_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySerialDevice.maxtimeout = Convert.ToInt32(
                Main.SharedUI.comboBoxTimeout.Text
                    .Replace("Timeout", "")
                    .Replace("-", "")
                    .Replace("ms", "")
                    .Replace(" ", "")
            );
        }

        private void ComboPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ComboPort.Text))
            {
                Match match1 = Regex.Match(ComboPort.Text, @"\((COM\d+)\)");
                if (match1.Success)
                {
                    Uni.PortCom = match1.Groups[1].Value.Replace("COM", "");
                }
            }
            else
            {
                Uni.PortCom = "";
            }
        }

        private void ListBoxViewSearch_TextChanged(object sender, EventArgs e)
        {
            //TODO DEĞİŞTİR
            if (ListBoxViewSearch.Text.Length > 0)
            {
                int i = 0;
                for (i = 0; i < ListBoxview.Items.Count; i++)
                {
                    if (
                        ListBoxview
                            .GetItemText(ListBoxview.Items[i])
                            .Contains(ListBoxViewSearch.Text)
                    )
                    {
                        ListBoxview.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void ListBoxview_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ÇÖP

            if (!isUniSPDRunning)
            {
                UniSPDDevice.Info info = ListBoxview.SelectedItem as UniSPDDevice.Info;
                UniSPDDevice.DevicesName = info.Devices;
                UniSPDDevice.ModelName = info.Models;
                UniSPDDevice.Platform = info.Platform;
                UniSPDDevice.Brand = info.Devices.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0];

                MyDisplay.RtbClear();
                MyDisplay.RichLogs("Selected : ", Color.Black,
                    true,
                    false
                );
                MyDisplay.RichLogs(
                    "" + UniSPDDevice.DevicesName + " " + UniSPDDevice.ModelName + " " + UniSPDDevice.Platform,
                    Color.Purple,
                    true,
                    true
                );
                MethodOneClick.SPDOneClickExecModel();
            }
        }

        private void btn_STOP_Click(object sender, EventArgs e)
        {
            try
            {
                cts.Cancel();
                Thread.Sleep(3000);
                this.CkFDLLoaded.Checked = false;
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                MyDisplay.RichLogs("Task Stopped", Color.Black, true, true);
                cts = new CancellationTokenSource();
                My.MyProgress.ProcessBar1(0);
                isUniSPDRunning = false;
            }
        }
    }
}