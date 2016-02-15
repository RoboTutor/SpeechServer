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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Messenger;
using System.Diagnostics;

namespace ScriptEngine
{
    /// <summary>
    /// Used to communicate with the ScriptEngine on the robot.
    /// </summary>
    public partial class ExternalScriptEngine : UserControl, IMessenger
    {
        public ExternalScriptEngine()
        {
            InitializeComponent();

            this.ucSocketClient.ClientOnConnectFunc = this.ClientConnected;
            this.ucSocketClient.ClientOnDataRecFunc = this.ClientDataReceived;
        }

        #region IMessenger Members

        public string ID
        {
            get { return "ExternalScriptEngine"; }
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

            return null;
        }

        #endregion



        #region TCP/IP
        void ClientConnected(bool b)
        {

        }

        /// <summary>
        /// RoboTutor sends commands here.
        /// Behavior should be called, according to the command
        /// </summary>
        /// <param name="message"></param>
        void ClientDataReceived(string message)
        {
            Debug.WriteLine("MBL (Client) received: " + message);

            string token = message.Split('|')[0];
            if (token == "CallBehavior")
            {
                string behaviorname = message.Split('|')[1];

                SendMessage("NaoRobotManager", new MessageEventArgs(behaviorname));
            }
        }

        public void SendMessageExternalEngine(string msg)
        {
            throw new NotImplementedException();
        }
        #endregion TCP/IP
    }
}
