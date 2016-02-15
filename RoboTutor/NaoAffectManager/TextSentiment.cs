/*
 * This code and information is provided "as is" without warranty of any kind, 
 * either expressed or implied, including but not limited to the implied warranties 
 * of merchantability and/or fitness for a particular purpose.
 * 
 * License: 
 * 
 * Email: junchaoxu86@gmail.com; k.v.hindriks@tudelft.nl
 * 
 * Copyright © Junchao Xu, Interactive Intelligence, TUDelft, 2014.
 * 
 * Credit to SentiStrength: http://sentistrength.wlv.ac.uk/
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Messenger;
using SocketConnection;
using System.Net;
using System.Reflection;
using System.Threading;
using ZedGraph;

namespace NaoAffectManager
{
    public partial class TextSentiment : UserControl, IMessenger
    {
        public TextSentiment()
        {
            InitializeComponent();

            //LaunchSentimentStrength();
        }

        ~TextSentiment()
        {
            StopJavaProgram();
        }

        public string ID
        {
            get { return "TextSentiment"; }
        }

        public event ehSendMessage evSendMessage;
        public MessageEventArgs SendMessage(string sendto, MessageEventArgs msg)
        {
            if (evSendMessage != null)
            {
                return evSendMessage(sendto, this.ID, msg);
            }
            else
                return null;
        }

        public MessageEventArgs MessageHandler(string sendfrom, MessageEventArgs message)
        {
            if (message.MsgType == MessageEventArgs.MessageType.COMMAND)
            {
                if (message.Cmd == "ChangeRobotMood")
                {
                    string valstr = message.CmdArgs[0];
                    double valence = Double.Parse(valstr);
                    // Draw
                    AddPointToCurve("Valence", valence);
                }
                else if (message.Cmd == "TextSenti")
                {
                    if (cbSentiText.Checked == true)
                    {
                        string sentence = message.CmdArgs[0];
                        double valence = SentiText(sentence);
                        // Draw
                        AddPointToCurve("Valence", valence);

                        return new MessageEventArgs(valence as Object);
                    }
                    else
                    {
                        return new MessageEventArgs(200 as Object);
                    }
                }
                else if (message.Cmd == "TextSentiStart")
                {
                    LaunchSentimentStrength();
                    areSentiStrengthLaunched.WaitOne(5000);
                    return null;
                }
                else if (message.Cmd == "TextSentiStop")
                {
                    StopJavaProgram();
                    return null;
                }
            }

            return null;
        }

        public bool SentiTextEnabled
        {
            get { return cbSentiText.Checked; }
        }

        public void StopJavaProgram()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process theprocess in processlist)
            {
                if (theprocess.ProcessName == "java")
                {
                    //if (theprocess.MainWindowTitle.Contains("SentiStrengthCom") == true)
                    //{
                        Console.WriteLine("Closing existing SentiStrengthCom systems...");
                        bool cmw = theprocess.CloseMainWindow();
                        if (cmw == false)
                        {
                            theprocess.Kill();
                            theprocess.WaitForExit();
                        }
                        else 
                            theprocess.Close();

                        Console.WriteLine("Existing SentiStrengthCom systems closed!");
                    //}
                }
            }
        }

        public void LaunchSentimentStrength()
        {
            StopJavaProgram();

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.RunWorkerAsync();
        }

        AutoResetEvent areSentiStrengthLaunched = new AutoResetEvent(false);
        Process JavaSentiStrength = null;
        /// <summary>
        /// Load SentiStrength Java version.
        /// NB: The previous instance has to be closed before load a new one!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            string directory = Application.StartupPath + "\\..\\..\\NaoAffectManager\\SentiStrengthJava\\";
            string path = directory + "SentiStrengthCom.jar";
            bool exist = File.Exists(path);
            if ( exist == false)
            {
                Debug.WriteLine("SentiStrengthCom.jar not found!");
                return;
            }

            //var processInfo = new ProcessStartInfo("C:\\Program Files\\Java\\jre7\\bin\\java.exe", "-jar SentiStrengthCom.jar sentidata data201109/ text i+don't+hate+you.")
            //var processInfo = new ProcessStartInfo("C:\\Program Files\\Java\\jre7\\bin\\java.exe", "-jar SentiStrengthCom.jar sentidata data201109/ listen 81 scale")
            var processInfo = new ProcessStartInfo("C:\\Program Files\\Java\\jre7\\bin\\java.exe", "-jar SentiStrengthCom.jar stdin sentidata data201109/ scale")
            {
                // Initialize properties of "processInfo"
                CreateNoWindow = true,
                UseShellExecute = false, 
                // Can't minimize app which is started, done in the java-code self
                //WindowStyle = ProcessWindowStyle.Minimized 
                WorkingDirectory = directory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            JavaSentiStrength = new Process();
            JavaSentiStrength.StartInfo = processInfo;

            // redirect output of Java
            JavaSentiStrength.OutputDataReceived += JavaOutputReceivedEventHandler;
            JavaSentiStrength.ErrorDataReceived += JavaErrorReceivedEventHandler;


            try
            {
            	JavaSentiStrength.Start();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Java start error! " + ex);
            }

            if (JavaSentiStrength == null)
            {
                throw new InvalidOperationException("Launch SentiStrengthCom.jar invalid operation!");
            }
            else
            {
                areSentiStrengthLaunched.Set();
                Console.WriteLine("SentiStrengthCom Java is running...");

                //
                // Read the output from the started application
                //
                // 1. Asynchronously. Cannot used with ReadLine, ReadToEnd, Read.
                //-JavaSentiStrength.BeginOutputReadLine();
                // 2. Synchronously
                //string s_out = JavaSentiStrength.StandardOutput.ReadToEnd();
                //Debug.WriteLine("SentiStrengthCom output: " + s_out);

                JavaSentiStrength.BeginErrorReadLine();

                JavaSentiStrength.WaitForExit();

                int exitCode = 0;
                if (JavaSentiStrength != null)
                {
                    exitCode = JavaSentiStrength.ExitCode;
                }
                Debug.WriteLine("Java SentiStrengthCom closed down! " + exitCode);
                JavaSentiStrength.Close();
                JavaSentiStrength = null;
            }
        }

        void JavaErrorReceivedEventHandler(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            // Collect the sort command output. 
            if (!String.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine("Java Error: " + e.Data);
            }
        }

        /// <summary>
        /// Read Java standard output
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="outLine"></param>
        private static void JavaOutputReceivedEventHandler(object sendingProcess,
            System.Diagnostics.DataReceivedEventArgs outLine)
        {
            // Collect the sort command output. 
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                Console.WriteLine("Java: " + outLine.Data);
            }
        }

        /// <summary>
        /// Use standard I/O stream: StdIn, StdOut.
        /// arguments of .jar have to be set like below:
        ///   "java.exe", "-jar SentiStrengthCom.jar stdin sentidata data201109/ scale"
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public double SentiText(string sentence)
        {
            JavaSentiStrength.StandardInput.WriteLine(sentence);

            // 1.
            //string s_out = JavaSentiStrength.StandardOutput.ReadToEnd();
            // 2.
            //char[] buf = new char[50];
            //JavaSentiStrength.StandardOutput.Read(buf, 0, 50);
            //string s_out = new string(buf);
            // 3.
            string s_out = JavaSentiStrength.StandardOutput.ReadLine();
            Debug.WriteLine("Java SentiStrengthCom output: " + s_out);

            // Asynchronously. Cannot used with ReadLine, ReadToEnd, Read.
            //JavaSentiStrength.BeginOutputReadLine();

            string[] s = s_out.Split('\t');

            if (s.Count() >= 3) 
                return double.Parse(s[2]);
            else 
                return 100; // error
        }

        /// <summary>
        /// Test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLaunchServer_Click(object sender, EventArgs e)
        {
            LaunchSentimentStrength();
        }

        /// <summary>
        /// Test.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRequest_Click(object sender, EventArgs e)
        {
            double valence = SentiText("I love hippo!");

            Debug.WriteLine("Java SentiStrengthCom valence in scale: " + valence);
        }

        #region Net solution
        ReconSocketClient rsc = null;
        /// <summary>
        /// Backup solution.
        /// </summary>
        /// <returns></returns>
        private string GetSentiByNet()
        {
            ///
            /// The same error "The server committed a protocol violation. Section=ResponseStatusLine"
            ///

            string url = "http://127.0.0.1:81/love%20you";
            // Success!!!
            //string url = "http://www.contoso.com/default.html"; // test
            //string url = "http://ii.tudelt.nl"; // test

            // 0.
            //string s = (new WebClient()).DownloadString(new Uri(url));

            // 1.
            //WebClient myClient = new WebClient();
            //Stream resp = myClient.OpenRead(url);
            // The stream data is used here.
            //resp.Close();

            // 2.
            //string urlText = "";
            //string urlText = ReadJavaResponseByHttp(url);
            //Debug.WriteLine(urlText);

            // 3. Java can receive, but C# cannot receive
            //    "Socket graceful close detected"
            ///*
            //if (rsc == null)
            //{
            //    rsc = new ReconSocketClient("127.0.0.1", 81);
            //    rsc.DataReceived += ReconnectingSocketClient_OnData;
            //}
            //rsc.SendData(url); // does not work
            //rsc.SendData("GET /love%20you HTTP/1.1"); // Java respond "Sentiment analysis on text: love you"
            //rsc.SendData("love%20you");
            //*/

            // 4. Java cannot receive
            /*
            StandardTcpClient sClient = new StandardTcpClient("127.0.0.1", 81, true);
            sClient.DataReceived += new EventHandler<SocketConnection.DataReceivedEventArgs>(this.TCPClientReceive);
            System.Threading.Thread.Sleep(1000);
            sClient.SendDataByte( Encoding.UTF8.GetBytes("GET /hello%20world ".ToCharArray()) );
            */

            return url;
        }

        /// <summary>
        /// Net solution.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string ReadJavaResponseByHttp(string url)
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "GET";
            request.KeepAlive = false;
            request.Credentials = CredentialCache.DefaultCredentials;
            //request.ServicePoint.Expect100Continue = false;
            //bool b = SetUseUnsafeHeaderParsing(true);
            //ServicePointManager.MaxServicePointIdleTime = 2000;

            // Get the response.
            HttpWebResponse response = null;
            //WebResponse response = null;


            try
            {
                //response = request.GetResponse();
                response = request.GetResponse() as HttpWebResponse;
                // Display the status.
                Debug.WriteLine(response.StatusDescription);
                Debug.WriteLine("Content length is {0}. Content type is {1}.", response.ContentLength, response.ContentType);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("Web response error: " + ex);
            }


            StreamReader reader = new StreamReader(response.GetResponseStream());

            string urlText = reader.ReadToEnd();
            return urlText;
        }

        /// <summary>
        /// Net solution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TCPClientReceive(object sender, SocketConnection.DataReceivedEventArgs e)
        {
            byte[] dat = e.Data;
            string msg = Encoding.UTF8.GetString(dat);
            Debug.WriteLine("Received from Java: " + msg);
        }

        void ReconnectingSocketClient_OnData(string data)
        {
            Debug.WriteLine("SentiStrength received: " + data);
        }

        /// <summary>
        /// Net solution.
        /// App.config code version.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool SetUseUnsafeHeaderParsing(bool b)
        {
            Assembly a = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
            if (a == null) return false;

            Type t = a.GetType("System.Net.Configuration.SettingsSectionInternal");
            if (t == null) return false;

            object o = t.InvokeMember("Section",
                BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });
            if (o == null) return false;

            FieldInfo f = t.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
            if (f == null) return false;

            f.SetValue(o, b);

            return true;
        }
        #endregion Net solution


        #region Oscilloscope
        List<Color> OscilloscopeColors = null;
        private void InitializeOscilloscope()
        {
            // get a reference to the GraphPane
            GraphPane myPane = this.zgcValence.GraphPane;

            myPane.Title.Text = "Text Sentiment - Valence";
            myPane.XAxis.Title.Text = "Sentence";
            myPane.YAxis.Title.Text = "Valence";


            // Tell ZedGraph to re-figure the
            // axes since the data have changed
            this.zgcValence.AxisChange();
            this.zgcValence.IsAutoScrollRange = true;
            this.zgcValence.IsZoomOnMouseCenter = true;

            // zedgraph has a maximum of 12 symbols, so 12 colors
            OscilloscopeColors = new List<Color>();
            OscilloscopeColors.Add(Color.Blue);
            OscilloscopeColors.Add(Color.Red);
            OscilloscopeColors.Add(Color.Green);
            OscilloscopeColors.Add(Color.Yellow);
            OscilloscopeColors.Add(Color.Orange);
            OscilloscopeColors.Add(Color.Magenta);
            OscilloscopeColors.Add(Color.Teal);
            OscilloscopeColors.Add(Color.Violet);
            OscilloscopeColors.Add(Color.Tan);
            OscilloscopeColors.Add(Color.Pink);
            OscilloscopeColors.Add(Color.Brown);
            OscilloscopeColors.Add(Color.Black);
        }

        PointPairList PointList = new PointPairList();
        private void AddPointToCurve(string curvname, double y)
        {
            // get a reference to the GraphPane
            GraphPane myPane = this.zgcValence.GraphPane;

            //this.zgcValence.Location = new Point(10, 10);
            // Leave a small margin around the outside of the control
            //this.zgcValence.Size = new Size(ClientRectangle.Width - 20, ClientRectangle.Height - 20);
            
            // remove old curve
            myPane.CurveList.Clear();

            // Make up some data arrays based on the Sine function
            double x = PointList.Count + 1;
            PointList.Add(x, y);

            // Generate a red curve with diamond
            // symbols, and "Porsche" in the legend
            LineItem myCurve =
                myPane.AddCurve(curvname, PointList, Color.Red, SymbolType.Diamond);

            // Tell ZedGraph to re-figure the axes since the data have changed
            this.zgcValence.AxisChange();

            Invoke((MethodInvoker)delegate
            { 
                this.zgcValence.Refresh(); 
            });
        }

        /// <summary>
        /// Test curve drawing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrawCurve_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 36; i++)
            {
                //x = (double)i + 5;
                double y = 1.5 + Math.Sin((double)i * 0.2);

                AddPointToCurve("Valence", y);
            }
        }
        #endregion Oscilloscope
    }
}
