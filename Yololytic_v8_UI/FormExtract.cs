using NAudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGMT;
using TGMTcs;
using TGMTplayer.Controls;


namespace Yololytic
{
    public partial class FormExtract : Form
    {
        static FormExtract m_instance;


        CameraPanel m_currentCameraPanel;
        bool m_formClosed = false;
        CameraPanel m_cameraPanel1;
        public bool m_isBusy = false;
        bool m_isConnecting = false;

        private readonly object _bitmapLock = new object();

        BackgroundWorker m_worker1 = new BackgroundWorker();
        int m_index = 0;

        string m_outputDir = "";
        int m_totalFrame = 0;
        string m_fileName = "";

        int m_numThread = 1;

        List<YOLOv8Detector> m_detectors = new List<YOLOv8Detector>();
        List<bool> m_detectorAvailables = new List<bool>();
        object m_lock = new object();

        Dictionary<int, Color> m_colors = new Dictionary<int, Color>();

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public FormExtract()
        {
            InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.None;
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;
            this.FormBorderStyle = FormBorderStyle.None;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static FormExtract GetInstance()
        {
            if (m_instance == null)
                m_instance = new FormExtract();
            return m_instance;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormExtract_Load(object sender, EventArgs e)
        {
            txt_inputVideo.Text = TGMTregistry.GetInstance().ReadString("inputVideo2");
            txt_output.Text = TGMTregistry.GetInstance().ReadString("outputDir");
            

            m_worker1.WorkerSupportsCancellation = true;
            m_worker1.DoWork += worker_DoWork;
            m_worker1.RunWorkerCompleted += worker_RunWorkerCompleted;

            SetDoubleBuffer(listView1);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormExtract_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Width = pnCamera1.Width;
            pictureBox1.Height = pnCamera1.Height;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void FormSelected(bool selected)
        {
            if (selected)
            {

            }
            else
            {
                if (m_cameraPanel1 != null)
                    m_cameraPanel1.Stop();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btn_extract_Click(object sender, EventArgs e)
        {
            if (btn_extract.Text == "Extract")
            {
                if(txt_output.Text == "")
                {
                    MessageBox.Show("Output folder is empty");
                    return;
                }

                Directory.CreateDirectory(txt_output.Text);
                InitCameraPanels();
                m_formClosed = false;
                btn_extract.Text = "Stop";
            }
            else
            {
                DisconnectCameras();

                circle1.Visible = false;
                btn_extract.Text = "Extract";
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void txt_imagePath_TextChanged(object sender, EventArgs e)
        {
            TGMTregistry.GetInstance().SaveValue("inputVideo2", txt_inputVideo.Text);

            m_totalFrame = OpenCV_VideoIO.CountTotalFrame(txt_inputVideo.Text);
            if (m_totalFrame > 0)
                progressBar1.Maximum = m_totalFrame;

            lbl_count.Text = "Total frame: " + m_totalFrame;

            m_fileName = Path.GetFileName(txt_inputVideo.Text);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void txt_output_TextChanged(object sender, EventArgs e)
        {
            m_outputDir = TGMTutil.CorrectPath(txt_output.Text);
            TGMTregistry.GetInstance().SaveValue("outputDir", txt_output.Text);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void txt_numThread_ValueChanged(object sender, EventArgs e)
        {
            m_numThread = (int)txt_numThread.Value;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void InitCameraPanels()
        {
            try
            {
                string resolution = "1280x720";// TGMTini.GetInstance().Read("resolution", "TGMTcamera", "1280x720");
                string videoPath = txt_inputVideo.Text;
                if (videoPath == "")
                    return;

                m_cameraPanel1 = InitCameraPanel(videoPath, resolution, pnCamera1, 1);

                ConnectCameras();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        CameraPanel InitCameraPanel(string url, string resolution, Panel panelDisplay, int camIndex)
        {
            if (url == "")
                return null;

            CameraPanel cameraPanel = null;
            if (cameraPanel != null)
            {
                panelDisplay.Controls.Clear();

                cameraPanel.Stop();
                cameraPanel.Dispose();
                cameraPanel = null;

            }
            cameraPanel = new CameraPanel(url, resolution);
            cameraPanel.CamIndex = camIndex;


            cameraPanel.parent = panelDisplay;
            cameraPanel.Click += cameraWindows_Click;


            string strRoi = "";// TGMTini.GetInstance().Read("ROI", "CAM" + camIndex);
            if (strRoi != "")
            {
                string[] strrois = strRoi.Split(',');
                if (strrois.Length == 4)
                {
                    int x = int.Parse(strrois[0]);
                    int y = int.Parse(strrois[1]);
                    int w = int.Parse(strrois[2]);
                    int h = int.Parse(strrois[3]);
                    Rectangle rect = new Rectangle(x, y, w, h);
                    cameraPanel.ROI = rect;
                }
            }

            panelDisplay.Controls.Add(cameraPanel);
            return cameraPanel;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void ConnectCameras()
        {
            if (m_isConnecting)
                return;

            TGMTsound.PlaySoundAsync("comedy_pop_finger_in_mouth_001.mp3");
            m_isConnecting = true;

            this.Invoke(new Action(() =>
            {
                circle1.Visible = true;
            }));


            BackgroundWorker workerLoadCamera = new BackgroundWorker();
            workerLoadCamera.WorkerReportsProgress = true;
            workerLoadCamera.DoWork += workerLoadCamera_DoWork;
            workerLoadCamera.RunWorkerCompleted += workerLoadCamera_RunWorkerCompleted;
            workerLoadCamera.RunWorkerAsync();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void DisconnectCameras()
        {
            if (m_cameraPanel1 != null)
            {
                m_cameraPanel1.Stop();
                //m_cameraPanel1.Dispose();
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void cameraWindows_Click(object sender, EventArgs e)
        {
            m_currentCameraPanel = (CameraPanel)sender;
            MouseEventArgs ee = (MouseEventArgs)e;

            if (ee.Button == MouseButtons.Right)
            {
                //Point tl = FormMain.GetInstance().Location;
                //Panel parent = m_currentCameraPanel.parent;
                //tl.X += parent.Location.X;
                //tl.Y += parent.Location.Y + panel2.Height + 20;
                //ctxtMnu.Show(new Point(tl.X + ee.Location.X, tl.Y + ee.Location.Y));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void workerLoadCamera_DoWork(object sender, DoWorkEventArgs e)
        {
            m_cameraPanel1.NewFrame -= M_cameraPanel_NewFrame1;
            m_cameraPanel1.NewFrame += M_cameraPanel_NewFrame1;
            m_cameraPanel1.Start();            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void workerLoadCamera_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                FormMain.GetInstance().PrintMessage(""); //clear disconnect message

                
                circle1.Visible = false;

            }));

            m_isConnecting = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void M_cameraPanel_NewFrame1(object sender, TGMTplayer.Sources.NewFrameEventArgs e)
        {
            if (!Program.inited || m_formClosed)
                return;


            lock (_bitmapLock)
            {
                Bitmap bmp = e.Frame.Clone(new Rectangle(0, 0, e.Frame.Width, e.Frame.Height), e.Frame.PixelFormat);
                Detect(bmp);
            }

            try
            {
                this.Invoke(new Action(() =>
                {
                    if (progressBar1.Value >= progressBar1.Maximum)
                    {
                        progressBar1.Value = progressBar1.Minimum;
                    }
                    progressBar1.Value++;
                }));
            }
            catch
            {

            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////               

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = (Bitmap)e.Argument;
            Predicted result = Detect(bmp);

            string filePath = GenFilePath(m_outputDir);
            bmp.Save(filePath, ImageFormat.Jpeg);

            int numObject = 0;

            if (result != null)
            {
                numObject = result.objects.Length;
            }


            List<string> content = new List<string>();
            for(int i=0; i<result.objects.Length; i++)
            {
                content.Add(GenRow(bmp.Width, bmp.Height, result.objects[i]));
            }

            filePath = filePath.Replace(".jpg", ".txt");
            File.WriteAllLines(filePath, content.ToArray());

            if (m_formClosed)
                return;

            this.Invoke(new Action(() =>
            {
                ListViewItem item = new ListViewItem((m_index + 1).ToString());
                item.SubItems.Add(filePath);
                item.SubItems.Add(numObject.ToString());

                listView1.Items.Add(item);
                listView1.EnsureVisible(m_index);

                m_index++;

                lbl_count.Text = m_index.ToString() + "/" + m_totalFrame.ToString();                
            }));
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {          
          
            //circle1.Visible = false;


            GC.Collect();

            m_isBusy = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HandleProcessCorruptedStateExceptions]
        Predicted Detect(Bitmap bmp)
        {
            if (bmp == null)
            {
                return null;
            }
            if (m_formClosed)
                return null;

            Predicted result = Program.detector.Detect(bmp, false);

            string filePath = GenFilePath(m_outputDir);
            bmp.Save(filePath, ImageFormat.Jpeg);

            int numObject = 0;

            if (result != null)
            {
                numObject = result.objects.Length;
            }


            List<string> content = new List<string>();
            for (int i = 0; i < result.objects.Length; i++)
            {
                content.Add(GenRow(bmp.Width, bmp.Height, result.objects[i]));
            }

            filePath = filePath.Replace(".jpg", ".txt");
            File.WriteAllLines(filePath, content.ToArray());


            this.Invoke(new Action(() =>
            {
                Bitmap bmpDraw = (Bitmap)bmp.Clone();
                for (int i = 0; i < result.objects.Length; i++)
                {
                    YOLOobject obj = result.objects[i];
                    bmpDraw = TGMTdraw.DrawRectangle(bmpDraw, obj.rect, GetColor(obj.classID), false, 6);
                }
                pictureBox1.Image = bmpDraw;



                ListViewItem item = new ListViewItem((m_index + 1).ToString());
                item.SubItems.Add(filePath);
                item.SubItems.Add(numObject.ToString());

                listView1.Items.Add(item);
                listView1.EnsureVisible(m_index);

                m_index++;

                lbl_count.Text = m_index.ToString() + "/" + m_totalFrame.ToString();
            }));

            return result;            
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        void SetNotBusy(int cameraID)
        {
            if (cameraID == 1)
                m_isBusy = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void FormExtract_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectCameras();

            m_formClosed = true;
            bool isBusy = m_worker1.IsBusy;
            while(isBusy)
            {
                e.Cancel = true;
                Thread.Sleep(100);
                isBusy = m_worker1.IsBusy;
            }
            e.Cancel = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        string GenRow(double width, double height, YOLOobject obj)
        {
            double x = obj.rect.X / width;
            double y = obj.rect.Y / height;
            double w = obj.rect.Width / width;
            double h = obj.rect.Height / height;
            double cx = x + w / 2;
            double cy = y + h / 2;

            return obj.classID + " " + cx + " " + cy + " " + w + " " + h;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public string GenFilePath(string saveDir)
        {
            string filePath = saveDir + m_fileName + "_";
            filePath += String.Format("{0:00000}", m_index)  + ".jpg";
            return filePath;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //int AvailableReader()
        //{
        //    for (int j = 0; j < m_detectorAvailables.Count; j++)
        //    {
        //        lock (m_lock)
        //        {
        //            if (m_detectorAvailables[j])
        //            {
        //                m_detectorAvailables[j] = false;
        //                return j;
        //            }

        //        }
        //    }

        //    if (m_detectors.Count < m_numThread)
        //    {
        //        m_detectors.Add(new YOLOcs());
        //        m_detectorAvailables.Add(false);

        //        return m_detectorAvailables.Count - 1;

        //    }
        //    return -1;
        //}

        public static void SetDoubleBuffer(ListView dgv)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgv, new object[] { true });
        }

        Color GetColor(int classID)
        {
            if (m_colors.ContainsKey(classID))
                return m_colors[classID];

            m_colors[classID] = TGMTdraw.GenerateRandomColor();
            return m_colors[classID];
        }


    }
}
