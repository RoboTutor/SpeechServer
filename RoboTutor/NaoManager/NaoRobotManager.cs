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
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ConnectorNao;
using Messenger;
using System.Xml;
using System.IO;

namespace NaoManager
{
    /// <summary>
    /// A higher level on top of the NaoConnector.
    /// Handling behavior modeling, scheduling, etc.
    /// </summary>
    public partial class NaoRobotManager : UserControl, IMessenger
    {
        ClientNao NaoClient = null;
        ClientNao NaoClientVice = null;
        ucNaoConnector uiNaoConnetor = null;

        // List of behavior profiles
        Dictionary<string, IBehaviorProfile> BehaviorGenerator;

        public NaoRobotManager()
        {
            InitializeComponent();

            this.NaoClient = new ClientNao();
            this.NaoClientVice = new ClientNao();
            this.uiNaoConnetor = new ucNaoConnector(this.NaoClient, this.NaoClientVice);
            this.pnlNaoConnector.Controls.Add(uiNaoConnetor);

            LoadBehaviors();

            EventMap();

            cbbCrgBehaviors.SelectedIndex = 0;
            cbbModulatedBehaviors.SelectedIndex = 0;
            if (rbtnRoboTutor.Checked == false) CurrentBehavior = cbbModulatedBehaviors.Text;

        }

        private void EventMap()
        {
            this.uiNaoConnetor.evNaoConneted += new ucNaoConnector.ehNaoConneted(this.callback_NaoConnected);
            this.NaoClient.evBehaviorFinished += new ClientNao.ehBehaviorFinished(this.callback_NaoBehaviorFinished);
            this.NaoClient.evCrgBehaviorFinished += new ClientNao.ehCrgBehaviorFinished(this.callback_NaoBehaviorFinished);
            this.NaoClient.evSpeechFinished += new ClientNao.ehSpeechFinished(this.callback_NaoSpeechFinished);

            this.NaoClientVice.evBehaviorFinished += new ClientNao.ehBehaviorFinished(this.callback_NaoViceBehaviorFinished);
        }

        #region Messenger
        public string ID
        {
            get { return "NaoManager"; }
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
            if(message.MsgType==MessageEventArgs.MessageType.COMMAND)
            {
                if (message.Cmd == "Say")
                {
                    string texttosay = message.CmdArgs[0];

                    // Change robot mood
                    if (this.NaoAffect.SentiAffect.SentiTextEnabled) // a CheckBox needs to be checked manually
                    {
                    	//SendMessage("TextSentiment", new MessageEventArgs("TextSenti", new string [] {texttosay}));

                        MessageEventArgs backdata = this.NaoAffect.SentiAffect.MessageHandler(this.ID, new MessageEventArgs("TextSenti", new string[] { texttosay }));
                        double valence = (double)backdata.DataReturn;
                        
                    }

                    NaoClient.Say(texttosay);
                }
                else if (message.Cmd == "ChangeRobotMood")
                {
                    string moodstr = message.CmdArgs[0];
                    string[] vad = moodstr.Split(',');
                    string valence = vad[0];
                    //string arousal = vad[1];
                    //string dominance = vad[2]; 
                    Invoke((MethodInvoker)delegate(){ 
                        this.NaoAffect.Valence = double.Parse(valence);
                    });

                    this.NaoAffect.SentiAffect.MessageHandler(this.ID, new MessageEventArgs("ChangeRobotMood", new string[] { valence }));
                }
                else if (message.Cmd == "PointToRaisedHands") // respond to students' raising hands
                {
                    double y = Convert.ToDouble(message.CmdArgs[0]), z = Convert.ToDouble(message.CmdArgs[1]), x = 400;
                    IBehaviorProfile prh = new BehaviorProfilePointingOmni(x, y, z);
                    MotionTimeline mtl = prh.LoadBehavior(0);
                    NaoClient.ExecuteBehaviorInQueue(mtl);
                }
                else if (message.Cmd == "CheckBehavior")
                {
                    string behaviorname = message.CmdArgs[0];
                    bool b = false;
                    if (behaviorname == "Twinkle" 
                        || behaviorname == "GangnamLong"
                        || behaviorname == "Wink"
                        || behaviorname == "Kick"
                        || behaviorname == "StandHead"
                        || behaviorname == "AllOn"
                        || behaviorname == "AllOff" 
                        || behaviorname == "EyesOn"
                        || behaviorname == "EyesOff"
                        || behaviorname == "RandomEyes"
                        || behaviorname == "Walk"
                        || behaviorname == "StandUp"
                        || behaviorname == "Squat"
                        || behaviorname == "Pushups"
                        || behaviorname == "Macarena") 
                        b = true; // temp solution
                    else b= this.BehaviorGenerator.ContainsKey(behaviorname);
                    return new MessageEventArgs(b as object);
                }
                else if (message.Cmd == "ExecuteBehavior")
                {
                    string behaviorname = message.CmdArgs[0];
                    HandleBehavior(behaviorname);
                }
                else if (message.Cmd == "CapturePhoto")
                {
                    string imgfile = HandleCameraPhoto();
                    return new MessageEventArgs(imgfile);
                }
                else if (message.Cmd == "Config")
                {
                    string config = message.CmdArgs[0];
                    if (config == "text_senti_enabled") // maybe put into SentiAffect itself
                    {
                        this.NaoAffect.SentiAffect.MessageHandler(this.ID, new MessageEventArgs("TextSentiStart", new string[] { }));
                    }
                    else if (config == "constant_head")
                    {
                        this.NaoClient.RecoverHead = false;
                    }
                    else if (config == "forwardlooking_head")
                    {
                        this.NaoClient.HeadYawInhibition = true;
                    }
                    else if (config == "freelooking_head")
                    {
                        this.NaoClient.HeadYawInhibition = false;
                    }
                    else
                    {
                        ConfigRobot(config);
                    }
                }
                else if (message.Cmd == "CheckIdleMoveConflicts")
                {
                    string behaviorname = message.CmdArgs[0];
                    if (IsLegInvolved(behaviorname))
                    {
                        if (this.NaoIdleMoveLeg != null) this.NaoIdleMoveLeg.Stop();
                    }
                    //if (IsHeadInvolved(behaviorname))
                    //{
                    //    if (this.NaoIdleMoveHead != null) this.NaoIdleMoveHead.Stop();
                    //}
                }
                
            }
            else if(message.MsgType == MessageEventArgs.MessageType.TEXT)
            {
                if (message.TextMsg == "ScriptEnded")
                {
                    if (this.NaoIdleMoveLeg != null) this.NaoIdleMoveLeg.Stop();
                }
                else if (message.TextMsg == "ScriptPaused")
                {
                    if (this.NaoIdleMoveLeg != null) this.NaoIdleMoveLeg.Stop();

                    Debug.WriteLine("{0}: Script paused!", this.ID);
                }
            }

            return null;
        }

        #endregion Messenger

        /// <summary>
        /// TODO: load from XML files
        /// </summary>
        private void LoadBehaviors()
        {
            // Pointing
            //  Forward X                     = 400D; Y = 0D; Z = 0D;
            //  Up right X                    = 400D; Y = -500D; Z = 600D;
            //IBehaviorProfile Pointing      = new BehaviorProfilePointing();
            IBehaviorProfile Pointing        = new BehaviorProfilePointingOmni(400D, -500D, 600D); // up right
            IBehaviorProfile PointingForward = new BehaviorProfilePointingOmni(400D, 0, 0);
            IBehaviorProfile PointingRight   = new BehaviorProfilePointingOmni(400D, -600D, -150D); // x should not small, z should not big
            IBehaviorProfile PointingUp      = new BehaviorProfilePointingOmni(100D, -200D, 400D);
            IBehaviorProfile PointingFrontDown = new BehaviorProfilePointingOmni(400D, 50D, -200D);
            // Direction/Position/Shape
            IBehaviorProfile FromAToB        = new BehaviorProfileFromAToB();
            IBehaviorProfile OneTheOther     = new BehaviorProfileOneTheOther();
            IBehaviorProfile LRUD            = new BehaviorProfileLRUD();
            IBehaviorProfile Tiny            = new BehaviorProfileTiny();
            // Social
            IBehaviorProfile Waving          = new BehaviorProfileWaving();
            IBehaviorProfile Clap            = new BehaviorProfileClap();
            // Abstract
            IBehaviorProfile Capisce         = new BehaviorProfileCapisce();
            IBehaviorProfile CapisceRotate   = new BehaviorProfileCapisceRotate();
            IBehaviorProfile State           = new BehaviorProfileState(); // ReverseCapisce (originally is left arm)
            IBehaviorProfile Propose         = new BehaviorProfilePropose();
            IBehaviorProfile HandOver        = new BehaviorProfileHandOver();
            IBehaviorProfile PushAside       = new BehaviorProfilePushAside();
            IBehaviorProfile Spread          = new BehaviorProfileSpread();
            IBehaviorProfile PressDown       = new BehaviorProfilePressDown();
            IBehaviorProfile ShowSide        = new BehaviorProfileShowSide();
            IBehaviorProfile Motivate        = new BehaviorProfileMotivate(); // ShakeHands
            IBehaviorProfile Weigh           = new BehaviorProfileWeigh();
            IBehaviorProfile Convince        = new BehaviorProfileConvince();
            IBehaviorProfile ConvergeHands   = new BehaviorProfileConvergeHands();
            IBehaviorProfile Applaud         = new BehaviorProfileApplaud();
            // I; You
            IBehaviorProfile FirstMe         = new BehaviorProfileFirstMe(); // raise hand + Me/I
            IBehaviorProfile First           = new BehaviorProfileFirst();
            IBehaviorProfile Me              = new BehaviorProfileMe();    // Me/I
            IBehaviorProfile YouAndMe        = new BehaviorProfileYouAndMe();
            IBehaviorProfile MeAndYou        = new BehaviorProfileMeAndYou();
            // head-oriented movements
            IBehaviorProfile Nod             = new BehaviorProfileNod();
            IBehaviorProfile NoShakeHead     = new BehaviorProfileNoShakeHead();
            IBehaviorProfile LookAround      = new BehaviorProfileLookAround();
            IBehaviorProfile Think           = new BehaviorProfileThink();
            IBehaviorProfile PhotoHeadPose   = new BehaviorProfilePhotoHeadPose();
            // Leg movements
            IBehaviorProfile Balance         = new BehaviorProfileBalance();
            IBehaviorProfile LeanRight       = new BehaviorProfileLeanRight();
            // Torso
            IBehaviorProfile Bow             = new BehaviorProfileBow();
            IBehaviorProfile SmallBow        = new BehaviorProfileSmallBow();
            IBehaviorProfile StandHead       = new BehaviorProfileStandHead();
            IBehaviorProfile HeadPose        = new BehaviorProfileHeadPose();
            // Show sensors
            IBehaviorProfile ShowSonars      = new BehaviorProfileShowSonars();
            IBehaviorProfile ShowMic         = new BehaviorProfileShowMic();
            IBehaviorProfile ShowArmJoints   = new BehaviorProfileRArmDOF();
            IBehaviorProfile ShowArmJointsOneByOne = new BehaviorProfileRArmDOFOneByOne();
            IBehaviorProfile ShowBody        = new BehaviorProfileShowBody();
            IBehaviorProfile ShowBiceps      = new BehaviorProfileShowBiceps();
            // Idle behavior
            IBehaviorProfile LegRandomMove   = new BehaviorProfileLegRandomMove();
            IBehaviorProfile HeadRandomScan  = new BehaviorProfileHeadRandomScan();
            // Imitation Gestures
            IBehaviorProfile ImitGestUp = new BehaviorProfileImitGesUp();
            IBehaviorProfile ImitGestDown = new BehaviorProfileImitGesDown();

            //
            BehaviorGenerator = new Dictionary<string, IBehaviorProfile>();
            // 
            AddBehavior("ImitUp", ImitGestUp);
            AddBehavior("ImitDown", ImitGestDown);
            // Deictic
            AddBehavior("Pointing", Pointing);
            AddBehavior("PointForward", PointingForward); // pointing omni
            AddBehavior("PointLeft", PointingRight); // pointing omni; mirror right
            AddBehavior("PointRight", PointingRight); // pointing omni
            AddBehavior("PointUpRight", Pointing); // pointing omni
            AddBehavior("PointUpLeft", Pointing); // pointing omni
            AddBehavior("PointUp", PointingUp);
            AddBehavior("PointFrontDown", PointingFrontDown);
            // Direction/position/Shape
            AddBehavior("LRUD", LRUD);
            AddBehavior("FromAToB", FromAToB);
            AddBehavior("FromAToBLeft", FromAToB);
            AddBehavior("OneTheOther", OneTheOther); // used for comparing two
            AddBehavior("Tiny", Tiny);
            AddBehavior("WavyShape", PressDown); // TODO: not implemented yet
            // Social 
            AddBehavior("Waving", Waving);
            AddBehavior("HelloEverybody", Waving); // just waving
            AddBehavior("Clap", Clap);
            AddBehavior("Applaud", Applaud);
            // Head-oriented
            AddBehavior("Nod", Nod);
            AddBehavior("No", NoShakeHead);
            AddBehavior("LookAround", LookAround); // Quiz
            AddBehavior("Think", Think);
            AddBehavior("PhotoHeadPose", PhotoHeadPose); // for taking a photo
            // Abstract
            AddBehavior("HandOver", HandOver);
            AddBehavior("HandOverLeft", HandOver);
            AddBehavior("HandOverBoth", HandOver);
            AddBehavior("PushAside", PushAside);
            AddBehavior("PushAsideLeft", PushAside);
            AddBehavior("PushAsideBoth", PushAside);
            AddBehavior("ShowSide", ShowSide); // avoid Hitler gestre;
            AddBehavior("ShowSideLeft", ShowSide);
            AddBehavior("Spread", Spread);     // Can be used to point right bottom
            AddBehavior("SpreadLeft", Spread); // Can be used to point left bottom
            AddBehavior("SpreadBoth", Spread); 
            AddBehavior("PressDown", PressDown);
            AddBehavior("PressDownLeft", PressDown);
            AddBehavior("PressDownBoth", PressDown);
            AddBehavior("State", State);     // ReverseCapisce (originally is left arm)
            AddBehavior("StateLeft", State); // ReverseCapisce
            AddBehavior("Propose", Propose); // Propose an idea
            AddBehavior("ProposeLeft", Propose); // Propose an idea
            AddBehavior("Capisce", Capisce); // nip 捏手指; emphasize
            AddBehavior("CapisceLeft", Capisce); // nip 捏手指; emphasize
            AddBehavior("RotatingCapisce", CapisceRotate); //
            AddBehavior("RotatingCapisceLeft", CapisceRotate); //
            // Abstract double hands only
            AddBehavior("Motivate", Motivate); // originally ShakeHands
            AddBehavior("Weigh", Weigh); //
            AddBehavior("Convince", Convince);
            AddBehavior("ConvergeHands", ConvergeHands); //
            // Self-related; You
            AddBehavior("Intro", Me); // TODO: add leg movements
            AddBehavior("First", First);
            AddBehavior("FirstMe", FirstMe);
            AddBehavior("Me", Me);
            AddBehavior("MeLeft", Me);
            AddBehavior("YouAndMe", YouAndMe);
            AddBehavior("YouAndMeLeft", YouAndMe);
            AddBehavior("MeAndYou", MeAndYou);
            AddBehavior("MeAndYouLeft", MeAndYou);
            AddBehavior("You", PointingForward);
            // Show sensors/effectors/functions
            AddBehavior("ShowArmJoints", ShowArmJoints);
            AddBehavior("ShowArmJointsOneByOne", ShowArmJointsOneByOne);
            AddBehavior("ShowChest", ShowSonars);
            AddBehavior("ShowMic", ShowMic);
            AddBehavior("ShowMicLeft", ShowMic); // used for showing Tactile sensors on the head
            AddBehavior("ShowBiceps", ShowBiceps);
            AddBehavior("ShowBody", ShowBody);
            // Torso
            AddBehavior("Bow", Bow);
            AddBehavior("SmallBow", SmallBow);
            AddBehavior("StandHead", StandHead);
            AddBehavior("HeadPose", HeadPose);
            // Leg
            AddBehavior("LeanRight", LeanRight);
            AddBehavior("Balance", Balance);

            // IdleBehavior
            AddBehavior("LegRandomMove", LegRandomMove);
            AddBehavior("HeadRandomScan", HeadRandomScan);


            LoadCrgBehaviors();
        }

        private void btnInsertCrgBehavior_Click(object sender, EventArgs e)
        {
            Form FrmInsertCrgBehavior = new Form();
            FrmInsertCrgBehavior.Text = "New Choregraphe Behavior";

            Label lb = new Label();
            lb.Text = "The name of the Choregraphe behavior";
            lb.AutoSize = true;
            FrmInsertCrgBehavior.Controls.Add(lb);

            TextBox behname = new TextBox();
            behname.Location = new Point(behname.Location.X + 20, lb.Location.Y + 3 + lb.Height);
            behname.Width = lb.Width - 40;
            FrmInsertCrgBehavior.Controls.Add(behname);

            Button btnok = new Button();
            btnok.Click +=
                new EventHandler(delegate
                {
                    FrmInsertCrgBehavior.DialogResult = DialogResult.OK;
                    FrmInsertCrgBehavior.Close();
                });
            btnok.Text = "OK";
            btnok.Location = new Point(btnok.Location.X, behname.Location.Y + 3 + behname.Height);
            FrmInsertCrgBehavior.Controls.Add(btnok);

            Button btncancel = new Button();
            btncancel.Click +=
                new EventHandler(delegate
                {
                    FrmInsertCrgBehavior.DialogResult = DialogResult.Cancel;
                    FrmInsertCrgBehavior.Close();
                });
            btncancel.Text = "Cancel";
            btncancel.Location = new Point(btnok.Location.X + btnok.Width + 15, btnok.Location.Y);
            FrmInsertCrgBehavior.Controls.Add(btncancel);

            FrmInsertCrgBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            FrmInsertCrgBehavior.AutoSize = true;

            // does not work
            //quizform.Parent = sender as Control;
            FrmInsertCrgBehavior.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = FrmInsertCrgBehavior.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string behaviorname = behname.Text;

                InsertCrgBehavior(behaviorname);
                int ind = this.cbbCrgBehaviors.Items.Add(behaviorname);
                this.cbbCrgBehaviors.SelectedIndex = ind;
            }
        }

        private void btnRemoveCrgBehavior_Click(object sender, EventArgs e)
        {
            Form FrmInsertCrgBehavior = new Form();
            FrmInsertCrgBehavior.Text = "Remove Choregraphe Behavior";

            string selectedcrg = this.cbbCrgBehaviors.SelectedItem.ToString();

            Label lb = new Label();
            lb.Text = "Sure to remove the Choregraphe behavior: " + selectedcrg + " ?";
            lb.AutoSize = true;
            FrmInsertCrgBehavior.Controls.Add(lb);

            Button btnok = new Button();
            btnok.Click +=
                new EventHandler(delegate
                {
                    FrmInsertCrgBehavior.DialogResult = DialogResult.OK;
                    FrmInsertCrgBehavior.Close();
                });
            btnok.Text = "OK";
            btnok.Location = new Point(btnok.Location.X, lb.Location.Y + lb.Height + 10);
            FrmInsertCrgBehavior.Controls.Add(btnok);

            Button btncancel = new Button();
            btncancel.Click +=
                new EventHandler(delegate
                {
                    FrmInsertCrgBehavior.DialogResult = DialogResult.Cancel;
                    FrmInsertCrgBehavior.Close();
                });
            btncancel.Text = "Cancel";
            btncancel.Location = new Point(btnok.Location.X + btnok.Width + 15, btnok.Location.Y);
            FrmInsertCrgBehavior.Controls.Add(btncancel);

            FrmInsertCrgBehavior.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            FrmInsertCrgBehavior.AutoSize = true;

            // does not work
            //quizform.Parent = sender as Control;
            FrmInsertCrgBehavior.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = FrmInsertCrgBehavior.ShowDialog();

            if (dr == DialogResult.OK)
            {
                RemoveCrgBehavior(selectedcrg);
                this.cbbCrgBehaviors.Items.Remove(selectedcrg);
                this.cbbCrgBehaviors.SelectedIndex = 0;
            }
        }

        private void InsertCrgBehavior(string crgname)
        {
            XmlDocument xmldoc = new XmlDocument();
            string file = Environment.CurrentDirectory + "\\..\\..\\NaoManager\\Behaviors.xml";
            try
            {
                xmldoc.Load(file);
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("Root element is missing")
                    || ex.GetType() == typeof(FileNotFoundException))
                {


                }
            }

            XmlNode crglist = xmldoc.SelectSingleNode("NaoBehaviors/CrgBehaviors");

            XmlNode newcrgnode = xmldoc.CreateElement("CrgBehavior");
            newcrgnode.InnerText = crgname;

            crglist.AppendChild(newcrgnode);

            xmldoc.Save(file);
        }

        private void RemoveCrgBehavior(string crgname)
        {
            XmlDocument xmldoc = new XmlDocument();
            string file = Environment.CurrentDirectory + "\\..\\..\\NaoManager\\Behaviors.xml";
            try
            {
                xmldoc.Load(file);
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("Root element is missing")
                    || ex.GetType() == typeof(FileNotFoundException))
                {


                }
            }

            XmlNode crglist = xmldoc.SelectSingleNode("NaoBehaviors/CrgBehaviors");
            for(int i=0;i<crglist.ChildNodes.Count;i++)
            {
                if (crglist.ChildNodes[i].InnerText == crgname) 
                    crglist.RemoveChild(crglist.ChildNodes[i]);
            }

            xmldoc.Save(file);
        }

        private void LoadCrgBehaviors()
        {
            XmlDocument xmldoc = new XmlDocument();
            string file = Environment.CurrentDirectory + "\\..\\..\\NaoManager\\Behaviors.xml";

            try
            {
                xmldoc.Load(file);
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("Root element is missing")
                    || ex.GetType() == typeof(FileNotFoundException))
                {


                }
            }

            XmlNode naocrgbnode_ = xmldoc.SelectSingleNode("NaoBehaviors/CrgBehaviors");
            if (naocrgbnode_ != null)
            {
                foreach (XmlNode b in naocrgbnode_.ChildNodes)
                {
                    AddCrgBehavior(b.InnerText);
                }
            }
        }

        private void AddBehavior(string behaviorname, IBehaviorProfile behaviorprofile)
        {
            this.BehaviorGenerator.Add(behaviorname, behaviorprofile);
            this.cbbModulatedBehaviors.Items.Add(behaviorname);
        }

        private void AddCrgBehavior(string behaviorname)
        {
            this.cbbCrgBehaviors.Items.Add(behaviorname);
        }

        private void btnListParams_Click(object sender, EventArgs e)
        {
            string parameters = "";
            foreach(var behavior in this.BehaviorGenerator.Values)
            {
                List<string> lpn = behavior.ParameterNames;
                foreach (var param in lpn)
                {
                    if (parameters.Contains(param) == false) 
                        parameters += param + Environment.NewLine;
                }
            }

            Form listform = new Form();
            listform.Name = "List of Used Parameters";
            TextBox tb = new TextBox();
            tb.Multiline = true;
            tb.AppendText(parameters);
            tb.Size = new Size(listform.Size.Width - 30, listform.Size.Height - 50);
            tb.ReadOnly = true;
            tb.ScrollBars = ScrollBars.Vertical;
            listform.Controls.Add(tb);
            listform.ShowDialog();
        }

        string NaoOperationMode;
        private void rbtnMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnRoboTutor.Checked == true)
            {
                NaoOperationMode = "RoboTutor";
            }
            else if (rbtnSingleBehavior.Checked == true)
            {
                NaoOperationMode = "SingleBehavior";
            }
            else // rbtnVideoAllBehaviors.Checked == true
            {
                NaoOperationMode = "VideoAll";
            }

            EnableNaoOperation();
        }

        private void EnableNaoOperation()
        {
            bool enabled = IsNaoConnected;

            this.btnStartModulatedBehavior.Enabled = enabled & (NaoOperationMode == "SingleBehavior");
            this.btnStartCrgBehavior.Enabled = enabled & (NaoOperationMode == "SingleBehavior");
        }


        #region Nao
        bool IsNaoConnected = false;
        public void callback_NaoConnected(string ip)
        {
            IsNaoConnected = true;
            EnableNaoOperation();
        }

        public void callback_NaoDisconnected(string ip)
        {
            IsNaoConnected = false;
            EnableNaoOperation();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">Text length</param>
        void callback_NaoSpeechFinished(string msg)
        {
            if (msg == "TTSNotConnected")
            {
                SendMessage("ScriptEngine", new MessageEventArgs("TTSNotConnected", new string[] { (0).ToString() }));
            } 
            else
            {
            	SendMessage("ScriptEngine", new MessageEventArgs("NaoSpeechFinished", new string[] { msg }));
            }
        }

        void callback_NaoBehaviorFinished(string bhvrName)
        {
            if (rbtnRoboTutor.Checked == true) // including Crg behaviors
            {
                if (IsLegInvolved(bhvrName)) // re-enable LegRandomMove
                {
                    if (this.NaoIdleMoveLeg != null) NaoIdleMoveLeg.Start();
                }
                //else if(IsHeadInvolved(bhvrName))
                //{
                //   if (this.NaoIdleMoveHead != null) NaoIdleMoveHead.Start();
                //}
           
                SendMessage("ScriptEngine", new MessageEventArgs("NaoBehaviorFinished", new string[] { CurrentBehavior }));
                
                CurrentBehavior = String.Empty;
            }
            else if (rbtnSingleBehavior.Checked == true && cbEndlessMove.Checked == false) // one time show single behavior
            {
                if (bhvrName.StartsWith("Crg"))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        btnStartCrgBehavior.BackColor = Color.LightGreen;
                    });
                    IsBehaviorRunning = false;
                    btnState_StartCrgBehavior = false;
                }
                else // Parameterized behavior
                {
                    Invoke((MethodInvoker)delegate
                    {
                        btnStartModulatedBehavior.BackColor = Color.LightGreen; 
                    });
                    IsBehaviorRunning = false;
                    btnState_StartModulatedBehavior = false;
                }
            }
            else if (rbtnSingleBehavior.Checked == true && cbEndlessMove.Checked == true && IsBehaviorRunning == true)
            {
                Invoke((MethodInvoker)delegate
                {
                    string behaviorname = cbbModulatedBehaviors.Text;
                    NaoExecuteBehavior(behaviorname);
                }); 
            }
            else if (rbtnVideoAllBehaviors.Checked == true)
            {
                evPlayNextBehavior.Set();
            }
            else if (btnState_StartCrgBehavior == true) // Crg Behavior
            {
                // TODO: what is it for???
                string behaviorname = cbbCrgBehaviors.Text;
                NaoExecuteBehavior(behaviorname);
            }
            else
            {
                // so far, nothing else
            }
        }

        void callback_NaoViceBehaviorFinished(string bhvrName)
        {
            if (rbtnRoboTutor.Checked == true)
            {
                // Continue LegRandomMove but not enable it
                //  i.e., restart LegRandomMove timer
                if (bhvrName == "LegRandomMove") 
                {
                    if (this.NaoIdleMoveLeg != null) NaoIdleMoveLeg.Continue();
                }
                else if (bhvrName == "HeadRandomScan")
                {
                    if (this.NaoIdleMoveHead != null) NaoIdleMoveHead.Continue();
                }
            }
        }

        /// <summary>
        /// Used to Start/Stop endless motion test
        /// </summary>
        bool IsBehaviorRunning = false;

        void NaoExecuteBehavior(string behaviorname)
        {
            // VA.Valence should be set before calling the behavior, according to the experiment condition
            double valence = this.NaoAffect.Valence;

            if (behaviorname != null)
            {
                // Decide which hand to perform the gesture
                string args = null;
                if (behaviorname.ToLower().Contains("both")) args = "BOTH";
                else if (behaviorname.ToLower().Contains("left")) args = "LEFT";
                else if (behaviorname.ToLower().Contains("right")) args = "RIGHT";

                MotionTimeline mtl = null;
                try
                {
                    // call behavior 
                    mtl = BehaviorGenerator[behaviorname].LoadBehavior(valence, 0, args);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                if (mtl != null)
                {
                    CurrentBehavior = behaviorname;
                    NaoClient.ExecuteBehaviorInQueue(mtl);
                }
                else
                {
                    throw new Exception("MotionTimeLine creation failed: null MTL!");
                }
            }
            else 
            {
                throw new Exception("Illegal behavior name!");
            }
        }

        private void HandleBehavior(string behaviorname)
        {
            if (behaviorname == "Walk")
            {
                // Unsafe: "Walk" cannot be sent here multiple times
                //BackgroundWorker bgw = new BackgroundWorker();
                //bgw.DoWork += delegate
                //{
                NaoClient.Walk(0.01f); // blocking call
                System.Threading.Thread.Sleep(500);
                CurrentBehavior = behaviorname;
                MotionTimeline mtl = BehaviorGenerator["StandHead"].LoadBehavior(this.NaoAffect.Valence, 0, null);
                NaoClient.ExecuteBehaviorInQueue(mtl);
                //this.MBLSocketConnector.SendRobotTutorMessage(behaviorname + "|Done");
                //};
                //bgw.RunWorkerAsync();
            }
            // OBSOLETE
            //else if (behaviorname.Contains("LegRandomMove"))
            else if(behaviorname == "LegRandmMove")
            {
                NaoLegRandomMove(behaviorname);
            }
            else if (behaviorname == "StandUp")
            {
                NaoClient.StandUp();
            }
            else if (behaviorname == "Squat")
            {
                NaoClient.Squat();
            }
            // Choregraphe behaviors
            else if (IsCrgBehavior(behaviorname))
            {
                try
                {
                    NaoClient.RunChoregrapheBehavior(behaviorname);
                    CurrentBehavior = behaviorname;
                    System.Threading.Thread.Sleep(500);
                }
                catch (System.Exception ex)
                {
                    SendMessage("ScriptEngine", new MessageEventArgs("NaoBehaviorNotExecuted", new string[] { behaviorname }));
                    Debug.WriteLine("{0}: Load Behavior error: {1}", this.ID, ex);
                }
            }
            else if (BehaviorGenerator.ContainsKey(behaviorname))
            {
                try
                {
                    NaoExecuteBehavior(behaviorname);
                }
                catch (System.Exception ex)
                {
                    SendMessage("ScriptEngine", new MessageEventArgs("NaoBehaviorNotExecuted", new string[] { behaviorname }));
                    Debug.WriteLine("{0}: Load Behavior error: {1}", this.ID, ex);
                }
            }
            else
            {
                Debug.WriteLine("{0}: Undefined behavior name: {1}", this.ID, behaviorname);

                if (rbtnRoboTutor.Checked)
                {
                    /// 
                    /// Jethro: If the behaviorname is empty, the message probably isn't meant for your client, 
                    /// so the other client is receiving it and doing stuff. 
                    /// If you send a done message, you might let a behavior that is not done yet make it done. 
                    /// So I think it would be better to ignore behaviors with empty names on your client.
                    /// 
                    //this.MBLSocketConnector.SendRobotTutorMessage("WrongBehavior|Done");
                }
            }
        }

        private bool IsCrgBehavior(string behaviorname)
        {
            bool b = this.cbbCrgBehaviors.Items.Contains(behaviorname);
            return b;
        }

        #region Idle Movement
        /// <summary>
        /// {idle_leg} {idle_head} {idle_eye}
        /// </summary>
        /// <param name="config"></param>
        private void ConfigRobot(string config)
        {
            if(config == "idle_leg")
            {
                NaoIdleMoveLeg = new NaoRandomMove(500);
                NaoIdleMoveLeg.evExeIdleMove += delegate 
                { 
                    NaoLegRandomMove("LegRandomMove"); 
                };
                NaoIdleMoveLeg.evStopIdleMove += delegate 
                { 
                    this.NaoClientVice.ForceMovementDecay("LegRandomMove"); 
                };

                NaoIdleMoveLeg.Start();
            }
            else if (config == "idle_eye")
            {

            }
            else if (config == "idle_head")
            {
                NaoIdleMoveHead = new NaoRandomMove(500);

                NaoIdleMoveHead.evExeIdleMove += delegate
                {
                    NaoHeadRandomScan();
                };
                NaoIdleMoveHead.evStopIdleMove += delegate
                {
                    this.NaoClientVice.ForceMovementDecay("HeadRandomScan"); 
                };

                NaoIdleMoveHead.Start();
            }
        }

        private void NaoHeadRandomScan()
        {
            try
            {
                MotionTimeline mtl = BehaviorGenerator["HeadRandomScan"].LoadBehavior(this.NaoAffect.Valence);
                if (mtl != null)
                {
                    NaoClientVice.ExecuteBehaviorInQueue(mtl);
                } // After behavior finished, callback_NaoBehaviorFinished() will be triggered.
                else // if behavior is not load
                {

                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("{0}: Load Behavior error: {1}", this.ID, ex);
            }
        }

        public bool IsHeadInvolved(string behaviorname)
        {
            // Crg behavior
            if (behaviorname == "Walk"
             || behaviorname == "Kick"
             || behaviorname == "tactileinteraction"
             || behaviorname == "tacint_nosay")
            {
                return true;
            }
            else if (behaviorname == "Twinkle" 
                || behaviorname == "CrgTwinkle" 
                || behaviorname == "Wink"
                || behaviorname == "Blink")
            {
                return false;
            }
            else // Parameterized behavior
            {
                if (BehaviorGenerator.ContainsKey(behaviorname))
                {
                    // TODO: this checking is slow!!!
                    MotionTimeline mtl = BehaviorGenerator[behaviorname].LoadBehavior(0);

                    List<JointChain> ljc = mtl.ContainedJointChain;
                    Head jc_head = (Head)ljc.Find(x => (x.Name == "Head"));

                    if (jc_head != null)
                    {
                        if (jc_head.JointInUseNames.Contains("HeadYaw"))
                        {
                            return true;
                        }
                        else return false;
                    }
                    else return false;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Check if the coming behavior involve leg movements.
        /// </summary>
        /// <param name="behaviorname"></param>
        /// <returns></returns>
        public static bool IsLegInvolved(string behaviorname)
        {
            bool b = behaviorname.Contains("Leg")
                  || behaviorname == "Walk"
                  || behaviorname == "Kick"
                  || behaviorname == "Bow"
                  || behaviorname == "SmallBow"
                  || behaviorname == "LeanRight"
                  || behaviorname == "Balance"
                  || behaviorname == "StandHead"
                  || behaviorname == "GangnamLong"
                  || behaviorname == "Pushups"
                  || behaviorname == "Macarena";

            return b;
        }

        /// <summary>
        /// Format: <LegRandomMove-predelay-hold-postdelay>
        /// </summary>
        /// <param name="command"></param>
        private void NaoLegRandomMove(string command)
        {
            string predelay = string.Empty;
            string hold = string.Empty;
            string posedelay = string.Empty;

            string[] cmds = command.Split('-');
            if (cmds.Length >= 2) predelay = cmds[1];
            if (cmds.Length >= 3) hold = cmds[2];
            if (cmds.Length >= 4) posedelay = cmds[3];

            string args = predelay + "-" + hold + "-" + posedelay;

            try
            {
                MotionTimeline mtl = BehaviorGenerator["LegRandomMove"].LoadBehavior(this.NaoAffect.Valence, 0, args);
                if (mtl != null)
                {
                    NaoClientVice.ExecuteBehaviorInQueue(mtl);
                } // After behavior finished, callback_NaoViceBehaviorFinished() will be triggered.
                else // if behavior is not load
                {
                    
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("{0}: Load Behavior error: {1}", this.ID, ex);
            }
        }

        NaoRandomMove NaoIdleMoveLeg;
        NaoRandomMove NaoIdleMoveHead;

        class NaoRandomMove
        {
            public event ehExeIdleMove evExeIdleMove;
            public delegate void ehExeIdleMove();

            public event ehStopIdleMove evStopIdleMove;
            public delegate void ehStopIdleMove();

            public NaoRandomMove(int interval)
            {
                NaoMotionWatchDog = new System.Timers.Timer(interval); // millisecond
                NaoMotionWatchDog.AutoReset = false;
                NaoMotionWatchDog.Elapsed += ExecuteIdleMovement;
            }

            private void ExecuteIdleMovement(object sender, System.Timers.ElapsedEventArgs e)
            {
                if (NaoIdleMoveEnabled)
                {
	                evExeIdleMove();
                }
            }

            public void Start()
            {
                NaoIdleMoveEnabled = true;
                NaoMotionWatchDog.Start();
            }

            public void Stop()
            {
                NaoIdleMoveEnabled = false;
                PostponeIdleMove();
                evStopIdleMove();
            }

            bool NaoIdleMoveEnabled = false;
            public void Continue()
            {
                if (NaoIdleMoveEnabled == true)
                    NaoMotionWatchDog.Start();
            }

            System.Timers.Timer NaoMotionWatchDog;
            /// <summary>
            /// TODO: differentiate "Postpone" from "Stop"
            /// Feed the timer to postpone the start of LegRandomMove
            /// </summary>
            private void PostponeIdleMove()
            {
                // Restart
                NaoMotionWatchDog.Stop();
                NaoMotionWatchDog.Start();
            }
        }
        #endregion Idle Movement

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string HandleCameraPhoto()
        {
            byte[] imageBytes = NaoClient.CapturePhoto();
            if (imageBytes != null)
            {
                Bitmap bmp = new Bitmap(640, 480, PixelFormat.Format24bppRgb);
                BitmapData bmData = bmp.LockBits
                    (new Rectangle(0, 0, bmp.Width, bmp.Height),
                     ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                // Get a pointer to the beginning of the pixel data  
                IntPtr p = bmData.Scan0;
                // Copy data
                Marshal.Copy(imageBytes, 0, p, bmp.Width * bmp.Height * 3);

                // Release the memory  
                bmp.UnlockBits(bmData);

                Image img = bmp;

                string imagepath = Environment.CurrentDirectory + "/../../NaoCapturedPhoto/";
                string picfile = imagepath + "capture.jpg";
                img.Save(picfile);

                return picfile;
            }
            else
                return null;
        }



        private void cbbCrgBehaviors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        string CurrentBehavior = String.Empty;
        private void cbbBehaviorName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rbtnRoboTutor.Checked == false)
            {
            	CurrentBehavior = cbbModulatedBehaviors.Text;
            }
        }

        bool btnState_StartCrgBehavior = false;
        private void btnStartCrgBehavior_Click(object sender, EventArgs e)
        {
            if (btnState_StartCrgBehavior == false)
            {
                btnState_StartCrgBehavior = true;
                btnStartCrgBehavior.BackColor = Color.Red;

                IsBehaviorRunning = true;

                string behaviorname = cbbCrgBehaviors.Text;
                NaoClient.RunChoregrapheBehavior(behaviorname);
            }
            else
            {
                string behaviorname = cbbCrgBehaviors.Text;
                NaoClient.StopChoregrapheBehavior(behaviorname);

                btnStartCrgBehavior.BackColor = Color.LightGreen;
                IsBehaviorRunning = false;
                btnState_StartCrgBehavior = false;
            }
        }

        bool btnState_StartModulatedBehavior = false;
        private void btnStartModulatedBehavior_Click(object sender, EventArgs e)
        {
            if (btnState_StartModulatedBehavior == false)
            {
                btnState_StartModulatedBehavior = true;
                btnStartModulatedBehavior.BackColor = Color.Red;
                
                IsBehaviorRunning = true;

                string behaviorname = cbbModulatedBehaviors.Text;
                NaoExecuteBehavior(behaviorname);
            }
            else
            {
                btnStartModulatedBehavior.BackColor = Color.LightGreen;
                IsBehaviorRunning = false;
                btnState_StartModulatedBehavior = false;
            }
        }

        AutoResetEvent evPlayNextBehavior = new AutoResetEvent(false);
        private void btnPlayAll_Click(object sender, EventArgs e)
        {
            BackgroundWorker bgw_play_all_behavior = new BackgroundWorker();
            bgw_play_all_behavior.DoWork += new DoWorkEventHandler(bgwPlayAllBehaviorStepValence);
            isPlayAllRunning = true;
            bgw_play_all_behavior.RunWorkerAsync();
        }

        List<string> PlayAllBehaviors = new List<string>();
        private void btnPlayAllList_Click(object sender, EventArgs e)
        {
            Form listform = new Form();
            listform.Text = "List of PlayAll Behaviors";

            Button btnok = new Button();
            btnok.Click += 
                new EventHandler(delegate
                {
                    listform.DialogResult = DialogResult.OK;
                    listform.Close();
                });
            btnok.Location = new Point(30, listform.Size.Height - 50 - btnok.Size.Height);
            btnok.Text = "OK";
            listform.Controls.Add(btnok);

            TextBox tb = new TextBox();
            tb.Multiline = true;
            tb.Size = new Size(listform.Size.Width - 30, btnok.Location.Y - 10);
            tb.ScrollBars = ScrollBars.Vertical;
            listform.Controls.Add(tb);
            DialogResult dr = listform.ShowDialog();
            if (dr == DialogResult.OK) PlayAllBehaviors.AddRange(tb.Lines);
        }

        private void bgwPlayAllBehaviorStepValence(object sender, DoWorkEventArgs e)
        {

            if (PlayAllBehaviors.Count == 0) PlayAllBehaviors = BehaviorGenerator.Keys.ToList();

            string cur_behavior;
            double cur_valence = -10.0;
            for (int i = 0; i < PlayAllBehaviors.Count && this.isPlayAllRunning == true; i++)
            {
                cur_behavior = PlayAllBehaviors[i];
                Invoke((MethodInvoker)delegate{this.txtbCurBehavior.Text = cur_behavior;});
                IBehaviorProfile curbehaviorgen = BehaviorGenerator[cur_behavior];

                // valence levels
                while (this.isPlayAllRunning == true && cur_valence <= 10 && cur_valence >= -10)
                {
                    Invoke((MethodInvoker)delegate { this.NaoAffect.Valence = cur_valence; });
                    MotionTimeline mtl = curbehaviorgen.LoadBehavior(this.NaoAffect.Valence, 0, null);
                    NaoClient.ExecuteBehaviorInQueue(mtl);

                    // wait for finish of the current behavior
                    evPlayNextBehavior.WaitOne();

                    System.Threading.Thread.Sleep(1000);

                    if (cur_valence == 10)
                    {
                        cur_valence = -10.0;
                        break;
                    }
                    else
                    {
                        cur_valence += (double)this.nudPlayAllStep.Value;
                        if (cur_valence > 10) cur_valence = 10.0;
                    }
                }
            }

            Invoke((MethodInvoker)delegate { this.NaoAffect.Valence = 0; });
        }

        bool isPlayAllRunning = false;
        private void btnStopPlayAll_Click(object sender, EventArgs e)
        {
            isPlayAllRunning = false;
        }

        #endregion Nao


        #region Experiment MISC
        private void btnHeadPose_Click(object sender, EventArgs e)
        {
            MotionTimeline mtl = BehaviorGenerator["HeadPose"].LoadBehavior(this.NaoAffect.Valence, 0, null);
            NaoClientVice.ExecuteBehaviorInQueue(mtl);
        }
        #endregion Experiment MISC



    }
}
