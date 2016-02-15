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
 * Credit to: Pavel Torgashov, Fast Colored TextBox, http://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting
 */

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using Messenger;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace ScriptEngine
{
    /// <summary>
    /// The authoring tool for creating scripts
    /// </summary>
    public partial class ScriptEditor : UserControl, IMessenger
    {
        ScriptParser Parser;

        public ScriptEditor()
        {
            InitializeComponent();

            Parser = new ScriptParser();

            EventMap();
        }

        bool TextSentiEnabled = false;

        /// <summary>
        /// 
        /// </summary>
        private void EventMap()
        {
            Parser.evExecuteBehavior += delegate(string behaviorname)
            {
                SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { behaviorname }));
            };

            Parser.evExecuteSpeech += delegate(string texttosay)
            {
                SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { texttosay }));
            };

            Parser.evExecuteListen += delegate(string cmd)
            {
                if (cmd == "connect")
                    SendMessage("SpeechController", new MessageEventArgs("Connect"));
                else if (cmd == "run")
                    SendMessage("SpeechController", new MessageEventArgs("Run"));
                else if (cmd == "close")
                    SendMessage("SpeechController", new MessageEventArgs("Close"));
            };

            Parser.evCameraPhoto += delegate()
            {
                MessageEventArgs backmsg = SendMessage("NaoManager", new MessageEventArgs("CapturePhoto", null));
                string imgfile = backmsg.TextMsg;
                SendMessage("PPTController", new MessageEventArgs("ShowPhoto", new string[] { imgfile }));
            };

            Parser.evSlideControl += delegate(string cmd)
            {
                SendMessage("PPTController", new MessageEventArgs(cmd, null));
            };

            Parser.evExecuteScript += delegate(string file, string speechAccept)
            {
                Boolean accepted = true;
                if (speechAccept.Length > 0)
                {
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { speechAccept }));
                    MessageEventArgs backmsg = SendMessage("SpeechController", new MessageEventArgs("Ask"));
                    if (backmsg.MsgType == MessageEventArgs.MessageType.DATA)
                    {
                        string[] data = backmsg.DataReturn as string[];
                        Console.WriteLine("BACK MSG: " + data[0]);
                        if (data[0] == "nee")
                            accepted = false;
                    }
                }
                
                file = Environment.CurrentDirectory + "\\..\\..\\Scripts\\" + file + ".txt";
                if (File.Exists(file) && accepted)
                {
                    SendMessage("NaoManager", new MessageEventArgs("ScriptEnded"));
                    IsParserOnTheWay = false;
                    Invoke((MethodInvoker)delegate()
                    {                        
                        btnStop.PerformClick();
                        fctbScriptEditor.SelectionColor = Color.Blue;
                        fctbScriptEditor.Text = File.ReadAllText(file);
                        Thread.Sleep(1000);
                        btnExecute.PerformClick();
                    });
                }
                else
                {
                    if(!accepted)
                        Console.WriteLine("User said no.");
                    else
                        Console.WriteLine("ScriptFile does not exists!");
                }
            };

            Parser.evFetchQuizAnswers += delegate(out bool disagree, out int useranswer, out int useranswer2)
            {
                MessageEventArgs backmsg = SendMessage("PPTController", new MessageEventArgs("FetchQuizAnswers", null));
                if (backmsg.MsgType == MessageEventArgs.MessageType.DATA)
                {
	                string[] data = backmsg.DataReturn as string[];
	                disagree = bool.Parse(data[0]);
	                useranswer = int.Parse(data[1]);
	                useranswer2 = int.Parse(data[2]);
                }
                else
                {
                    disagree = false;
                    useranswer = -1;
                    useranswer2 = -1;
                }
            };

            Parser.evChangeMood += delegate(string mood)
            {
                SendMessage("NaoManager", new MessageEventArgs("ChangeRobotMood", new string[] { mood }));
            };

            Parser.evChangeConfig += delegate(string config)
            {
                SendMessage("NaoManager", new MessageEventArgs("Config", new string[] { config }));
            };

            Parser.evScriptExeFinished += delegate()
            {
                SendMessage("NaoManager", new MessageEventArgs("ScriptEnded"));

                IsParserOnTheWay = false;

                Invoke((MethodInvoker)delegate()
                {
                    fctbScriptEditor.SelectionColor = Color.Blue;
                    EnableUI("ReadyToPlay");
                });

                /*string curtime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                string logfile = Directory.GetCurrentDirectory() + "\\..\\..\\log\\StorytellingLog.txt";
                StreamWriter logger = new StreamWriter(logfile, true);
                string strend = curtime + "\tStory Ended!";
                logger.WriteLine(strend);
                logger.WriteLine(" ----- ");
                logger.Close();*/
            };

            Parser.evLatestScriptUnit += delegate(string behaviorname)
            {
                SendMessage("NaoManager", new MessageEventArgs("CheckIdleMoveConflicts", new string[] { behaviorname }));
            };

            Parser.evShowCurrentLine += delegate(int linenum)
            {
                Invoke((MethodInvoker)delegate()
                {
                    fctbScriptEditor.Navigate(linenum);
                    fctbScriptEditor.Selection = new Range(this.fctbScriptEditor, 0, linenum, 0, linenum+1);
                    fctbScriptEditor.SelectionColor = Color.OrangeRed;
                    //fctbScriptEditor.CurrentLineColor = Color.GreenYellow;
                });
            };

            Parser.evInterupt += delegate()
            {
                Invoke((MethodInvoker)delegate()
                { 
                    EnableUI("Pause"); 
                });
                Parser.PauseScript();
                SendMessage("NaoManager", new MessageEventArgs("ScriptPaused"));
            };
        }

        #region Messenger
        public string ID
        {
            get { return "ScriptEngine"; }
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
                if (message.Cmd == "NaoSpeechFinished")
                {
	                int len = int.Parse(message.CmdArgs[0]);
	                //Debug.WriteLine("{0}: NAO speech finished - {1}", this.ID, len );

                    Parser.SpeechDone();
                }
                else if (message.Cmd == "TTSNotConnected")
                {
                    Console.WriteLine("{0}: Speech is not said: {1}", this.ID, "TTSNotConnected!");
                    Parser.SpeechDone();
                }
                else if (message.Cmd == "NaoBehaviorFinished")
                {
                    string behaviorname = message.CmdArgs[0];
                    Debug.WriteLine("{0}: NAO behavior finished - {1}", this.ID, behaviorname);

                    Parser.BehaviorDone();
                }
                else if (message.Cmd == "NaoBehaviorNotExecuted")
                {
                    string behaviorname = message.CmdArgs[0];
                    Debug.WriteLine("{0}: NAO behavior NOT executed! - {1}", this.ID, behaviorname);
                    Parser.BehaviorDone();
                }
            }
            
            return null;
        }
        #endregion Messenger

        private void EnableUI(string state)
        {
            if (state == "Play")
            {
                this.fctbScriptEditor.ReadOnly = true;
	            this.btnExecute.Enabled = false;
	            this.btnLoad.Enabled = false;
                this.btnPause.Enabled = true;
                this.btnStop.Enabled = true;
            } 
            else if (state == "Pause")
            {
                this.btnExecute.Enabled = true;
                this.btnPause.Enabled = false;
            }
            else if (state == "ReadyToPlay") // "Stop"
            {
                this.fctbScriptEditor.ReadOnly = false;
                this.btnExecute.Enabled = true;
                this.btnLoad.Enabled = true;
                this.btnPause.Enabled = false;
                this.btnStop.Enabled = false;
            }
        }

        bool IsParserOnTheWay = false;
        private void btnExecute_Click(object sender, EventArgs e)
        {
            bool b = CheckBehaviors();
            if (b == false) return;

            EnableUI("Play");

            if (IsParserOnTheWay)
            {
                Parser.ResumeScript();
            } 
            else // start new execution
            {
	            IsParserOnTheWay = true;
	            string[] script = this.fctbScriptEditor.Lines.ToArray();
	            Parser.ExecuteScript(script);

                string curtime = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                string logfile = Directory.GetCurrentDirectory() + "\\..\\..\\log\\StorytellingLog.txt";
                StreamWriter logger = new StreamWriter(logfile, true);
                string strstart = "----- " + curtime + "\tStory Started! -----";
                logger.WriteLine(strstart);
                logger.Close();
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            EnableUI("Pause");
            Parser.PauseScript();
            SendMessage("NaoManager", new MessageEventArgs("ScriptPaused"));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Parser.StopScript();
            // Parser will emit evScriptExeFinished at the end of the parsing loop.
            // The following will happen
            //  1. send message to NaoManager
            //  2. EnableUI();
        }

        string CurrentFile;
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory + "\\..\\..\\Scripts\\";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                CurrentFile = ofd.FileName;
                this.fctbScriptEditor.Text = File.ReadAllText(CurrentFile);

                EnableUI("ReadyToPlay");
            }
        }

        private void fctbScriptEditor_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            ScriptSyntaxHighlight(e);

            if (string.IsNullOrEmpty(this.fctbScriptEditor.Text))
                btnExecute.Enabled = false;
            else
                btnExecute.Enabled = true;
        }

        //styles
        TextStyle BlueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle GrayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        TextStyle MagentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        TextStyle PurpleStyle = new TextStyle(Brushes.Purple, null, FontStyle.Regular);
        TextStyle GreenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        TextStyle BrownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        TextStyle MaroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        TextStyle BoldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        MarkerStyle SameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));

        /// <summary>
        /// See Regular Expression (regex)
        /// http://msdn.microsoft.com/en-us/library/az24scfc(v=vs.110).aspx
        /// </summary>
        /// <param name="e"></param>
        private void ScriptSyntaxHighlight(TextChangedEventArgs e)
        {
            //clear style of changed range
            e.ChangedRange.ClearStyle(BlueStyle, BoldStyle, GrayStyle, MagentaStyle, GreenStyle, BrownStyle);

            //comment highlighting; only at the beginning of a line!
            e.ChangedRange.SetStyle(GreenStyle, @"^#.*$", RegexOptions.Multiline);

            // TODO: keywords list into XML files
            //keyword highlighting
            // Command
            e.ChangedRange.SetStyle(BlueStyle, @"\b(behavior|slide|quiz|camera|pause|listen|script)\b");
            e.ChangedRange.SetStyle(PurpleStyle, @"\b(mood|text_senti_enabled|constant_head|forwardlooking_head|freelooking_head)\b");

            // NAO TTS
            e.ChangedRange.SetStyle(MagentaStyle, @"\b(pau|rst|rspd|vct)\b");
            // NAO Motion
            e.ChangedRange.SetStyle(BrownStyle, @"\b(idle_leg|idle_head|idle_eye)\b");

            //class name highlighting
            e.ChangedRange.SetStyle(BoldStyle, @"\b(behavior|quiz)\s+(?<range>\w+?)\b");

            //clear folding markers
            e.ChangedRange.ClearFoldingMarkers();

            //set folding markers
            e.ChangedRange.SetFoldingMarkers(@"##<", @"##>"); 
        }

        private void tsbtnJumpLine_Click(object sender, EventArgs e)
        {
            int line = (int)this.nudJumpNum.Value;
            this.fctbScriptEditor.Navigate(line);
        }

        private void tsbtnSave_Click(object sender, EventArgs e)
        {
            if (CurrentFile != null)
            {
	            string script = fctbScriptEditor.Text;
	            File.WriteAllText(CurrentFile, script);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {

        }

        private void insertBehavior_Click(object sender, EventArgs e)
        {
            fctbScriptEditor.InsertText("{behavior|}");
        }

        private void tsbtnRemoveBehavior_Click(object sender, EventArgs e)
        {
            Form removebehaviorform = new Form();
            removebehaviorform.Text = "RemoveBehavior";

            Label lb = new Label();
            lb.Text = "Input the Name of the Behavior to be removed:";
            lb.AutoSize = true;
            removebehaviorform.Controls.Add(lb);

            TextBox behname = new TextBox();
            behname.Location = new Point(behname.Location.X, lb.Location.Y + 3 + lb.Height);
            removebehaviorform.Controls.Add(behname);

            Button btnok = new Button();
            btnok.Click += 
                new EventHandler(delegate
                {
                    removebehaviorform.DialogResult = DialogResult.OK;
                    removebehaviorform.Close();
                });
            btnok.Text = "OK";
            btnok.Location = new Point(btnok.Location.X, behname.Location.Y + 3 + behname.Height);
            removebehaviorform.Controls.Add(btnok);

            removebehaviorform.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            removebehaviorform.AutoSize = true;

            // does not work
            //quizform.Parent = sender as Control;
            removebehaviorform.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = removebehaviorform.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string behaviorname = behname.Text;
                List<string> scriptlines = fctbScriptEditor.Lines.ToList();
                List<int> line_indices = scriptlines.IndexesWhere(t => t.Contains(behaviorname)).ToList();
                string res = "Find " + line_indices.Count + " occurrences!"
                    + Environment.NewLine + "Confirm to remove ALL these behaviors?";
                DialogResult mr = MessageBox.Show(res, "SearchResults", MessageBoxButtons.YesNo);
                if (mr == DialogResult.Yes)
                {
                	fctbScriptEditor.RemoveLines(line_indices);
                }
            }
        }

        private void insertQuiz_Click(object sender, EventArgs e)
        {
            Form quizform = new Form();
            quizform.Text = "InputNumberChoices";

            Label lb = new Label();
            lb.Text = "Input the Number of Quiz Choices:";
            quizform.Controls.Add(lb);

            NumericUpDown nud_num_answers = new NumericUpDown();
            nud_num_answers.Location = new Point(nud_num_answers.Location.X, lb.Location.Y + 3 + lb.Height);
            quizform.Controls.Add(nud_num_answers);

            Button btnok = new Button();
            btnok.Click += 
                new EventHandler(delegate
                {
                    quizform.DialogResult = DialogResult.OK;
                    quizform.Close();
                });
            btnok.Text = "OK";
            btnok.Location = new Point(btnok.Location.X, nud_num_answers.Location.Y + 3 + nud_num_answers.Height);
            quizform.Controls.Add(btnok);

            quizform.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            quizform.AutoSize = true;

            // does not work
            //quizform.Parent = sender as Control;
            quizform.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = quizform.ShowDialog();

            if (dr == DialogResult.OK)
            {
                int num_answers = (int)nud_num_answers.Value;
                StringBuilder sb = new StringBuilder("{quiz|");
                for (int i = 0; i < num_answers; i++)
                {
                    sb.Append(" answer").Append(i+1).Append(" |");
                }
                sb.Append(" inconclusive answer }");
            	fctbScriptEditor.InsertText(sb.ToString());
            }
        }

        private void tsbtnCheck_Click(object sender, EventArgs e)
        {
            CheckBehaviors();

            MessageBox.Show("O(∩_∩)O All behaviors exist!", "Check Behaviors");
        }

        private bool CheckBehaviors()
        {
            string[] script = this.fctbScriptEditor.Lines.ToArray();
            int scriptlen = script.Count();
            int i = 0;
            string errorBehaviors = "";
            int behaviorCnt = 0;
            int errorBehaviorCnt = 0;
            while (i < scriptlen)
            {
                string behavior;
                ScriptType st = Parser.ParseLine(script[i], out behavior);
                if (st==ScriptType.BEHAVIOR)
                {
                    behaviorCnt++;

                    MessageEventArgs backmsg = SendMessage("NaoManager", new MessageEventArgs("CheckBehavior", new string[] { behavior }));
                    if (backmsg != null)
                    {
                        bool isBehaviorExist = (bool)(backmsg.DataReturn);
                        if (isBehaviorExist == false)
                        {
                            errorBehaviorCnt++;
                            errorBehaviors += behavior + Environment.NewLine;
                        }
                    }
                }

                i++;
            }

            if (errorBehaviors.Length>0)
            {
                errorBehaviors = "Among " + behaviorCnt
                    + " behaviors, " + errorBehaviorCnt
                    + " below do not exist:" + Environment.NewLine
                    + errorBehaviors;

                MessageBox.Show(errorBehaviors, "Check Behaviors");
                return false;
            }

            return true;
        }

    }
}
