using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using uping.Forms;
using uping.Lib;

namespace uping
{

    public partial class uping : Form
    {
        Ping p = new Ping();
        private bool checkEvent = false;

        DataTable dt = new DataTable();

        private string replyStr;

        public uping()
        {
            InitializeComponent();
            dt.Columns.Add("address");
            dt.Columns.Add("bytes");
            dt.Columns.Add("time");
            dt.Columns.Add("TTL");
            dt.Columns.Add("Check Date");
            dt.Columns.Add("Reply");
            p.PingCompleted += onPingCompleted;
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Start();
            dgvResults.DataSource = dt;
        }

        private void onPingCompleted(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                MessageBox.Show("Ping canceled.");
                // Let the main thread resume. 
                // UserToken is the AutoResetEvent object that the main thread 
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                if (e.Error.InnerException.Message == "No such host is known")
                    MessageBox.Show(string.Format("Ping failed: {0}", e.Error.InnerException.Message));
                else
                    MessageBox.Show(string.Format("Ping failed: {0}", e.Error.ToString()));

                // Let the main thread resume. 
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayReply(reply);
            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();
            p.SendAsyncCancel();
        }

        private void DisplayReply(PingReply reply)
        { 
            if (reply == null)
                return;
            try
            {
                var dr = dt.NewRow();
                dr[0] = reply.Address.ToString();
                dr[1] = reply.Buffer.Length.ToString();
                dr[2] = reply.RoundtripTime.ToString("0 ms");
                if (reply.Options != null)
                    dr[3] = reply.Options.Ttl.ToString("0 ms");
                else
                    dr[3] = null;
                dr[4] = DateTime.Now;
                dr[5] = reply.Status;
                dt.Rows.Add(dr);
                replyStr = string.Format("Address: {0}\nbuffer: {1}\ntime: {2}\nttl: {3}\ncheck time: {4}\nreply: {5}", dr[0],
                                      dr[1], dr[2], dr[3], dr[4], dr[5]);

                dgvResults.CurrentCell = dgvResults.Rows[dgvResults.Rows.Count - 2].Cells[0];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelPing();
        }

        private void CancelPing()
        {
            p.SendAsyncCancel();
            replyStr = string.Empty;
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPing.Text.Trim()))
                {
                    p.SendAsyncCancel();
                    AutoResetEvent waiter = new AutoResetEvent(false);

                    p.SendAsync(txtPing.Text, 1000, waiter);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                timer1.Stop();
            }
        }

        private void Pinger_Resize(object sender, EventArgs e)
        {
            FormActions();
        }

        private void FormActions()
        {
            if (this.Visible)
            {
                this.Resize -= Pinger_Resize;
                this.Hide();
                timer2.Start();
            }
            else
            {
                this.Show();
                this.Activate();
                this.TopMost = true;
                this.Resize += Pinger_Resize;
                timer2.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(replyStr))
            {
                notifyIcon1.BalloonTipText = replyStr;
                notifyIcon1.ShowBalloonTip(4);
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                this.Close();
            }
            if (e.Button == MouseButtons.Left)
                FormActions();
        }

        private void cmnuAbout_Click(object sender, EventArgs e)
        {
            AboutBoxuping a = new AboutBoxuping();
            a.ShowDialog();
        }

        private void tmnuSave_Click(object sender, EventArgs e)
        {
            SaveResults();
        }

        private void SaveResults()
        {
            if (dt.Rows.Count > 0)
                Utilities.ExportToCSV(dgvResults);
        }

        private void tmnuClearList_Click(object sender, EventArgs e)
        {
            ClearGridView();
        }

        private void ClearGridView()
        {
            if (dt.Rows.Count > 0)
                dt.Rows.Clear();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Delete | Keys.Shift))
                ClearGridView();
            if (keyData == Keys.Escape)
                CancelPing();
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveResults();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            ServerList sl = new ServerList();
            sl.ShowDialog();
            this.TopMost = true;
        }

    }
}
