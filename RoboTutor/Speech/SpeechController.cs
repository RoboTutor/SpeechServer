using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Messenger;

namespace Speech
{
    public partial class SpeechController : UserControl, IMessenger
    {
        Client c; //"145.94.253.13"; //"192.168.1.56";
        string[] commands = { "je naam", "jouw naam", "hoe heet",
                              "hello", "hallo", "hoi",
                              "spier", "sterk",
                              "push up", "pushup", "fitnes",
                              "voetbal", "schop",
                              "sta op", "opstaan",
                              "squat", "zit", "rust",
                              "macarena",
                              "gangnam",
                              "vlieg"
                            };

        public SpeechController()
        {
            InitializeComponent();
            c = new Client("", 6969);
        }

        public void NaoMove(string data)
        {
            string command = getCommand(data);
            switch(command){
                case "je naam":
                case "jouw naam":
                case "hoe heet":
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Mijn naam is NAO" }));
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Bow" }));
                    break;
                case "hello":
                case "hallo":
                case "hoi":
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Hallo, hoe gaat het?" }));
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Waving" }));
                    break;
                case "spier":
                case "sterk":
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "ShowBiceps" }));
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Ik voel me sterk" }));
                    break;
                case "pushup":
                case "push up":
                case "fitnes":
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Pushups" }));
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "1, 2, 3" }));
                    break;
                case "voetbal":
                case "schop":
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Goal" }));
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Kick" }));                    
                    break;
                case "sta op":
                case "opstaan":
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "StandUp" }));
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Laten we gaan spelen" }));
                    break;
                case "squat":
                case "zit":
                case "rust":
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Squat" }));
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Ik ben moe" }));
                    break;
                case "gangnam":
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "GangnamLong" }));
                    break;
                case "macarena":
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Macarena" }));
                    break;
                case "vlieg":
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "LookAround" }));
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Geef me eerst een superherokaap" }));
                    break;
                default:
                    SendMessage("NaoManager", new MessageEventArgs("Say", new string[] { "Geen commando's gevonden" }));
                    SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "Think" }));
                    break;
            }
        }

        public string getCommand(string data)
        {
            foreach (var c in commands)
                if (data.Contains(c)) return c;
            return "";
        }

        #region Messenger
        public string ID
        {
            get { return "SpeechController"; }
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
            if (message.MsgType == MessageEventArgs.MessageType.TEXT)
            {
                if (message.TextMsg == "Connect")
                {
                    c.connect();
                }
                else if (message.TextMsg == "Run")
                {
                    NaoMove((c.run()).ToLower());
                }
                else if (message.TextMsg == "Ask")
                {
                    string answer;
                    do{
                        answer = c.run().ToLower();
                        Console.WriteLine("Answer: " + answer);
                    }while(answer != "ja" && answer != "nee");
                    string[] data = new string[] { answer.ToString() };
                    MessageEventArgs backmsg = new MessageEventArgs(data);
                    return backmsg;
                }
                else if (message.TextMsg == "Close")
                {
                    c.close();
                }
            }
            return null;
        }
        #endregion Messenger

        private void startSpeechMessage()
        {
            this.txtbOutput.AppendText("\r\n\r\nPress Listen button to start listening.");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            c.connect();
            this.txtbOutput.AppendText("Connected to server.");
            startSpeechMessage();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            c.close();
            this.txtbOutput.AppendText("\r\nConnection closed.");
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            //SendMessage("NaoManager", new MessageEventArgs("ExecuteBehavior", new string[] { "LookAround" }));
            string data = c.run();
            this.txtbOutput.AppendText("\r\nReceived : " + data);
            startSpeechMessage();
            NaoMove(data.ToLower());
        }  
    }
}
