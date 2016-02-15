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
using System.Linq;
using System.Text;

namespace Messenger
{
    public interface IMessenger
    {
        string ID { get; }
        event ehSendMessage evSendMessage;
        MessageEventArgs SendMessage(string sendto, MessageEventArgs msg);
        MessageEventArgs MessageHandler(string sendfrom, MessageEventArgs message);
    }

    public delegate MessageEventArgs ehSendMessage(string sendto, string sendfrom, MessageEventArgs message);

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string cmd, string[] cmdargs)
        {
            this.MsgType = MessageType.COMMAND;
            this.Cmd = cmd;
            this.CmdArgs = cmdargs;
        }

        public MessageEventArgs(string textmsg)
        {
            this.MsgType = MessageType.TEXT;
            this.TextMsg = textmsg;
        }

        public MessageEventArgs(object data)
        {
            this.MsgType = MessageType.DATA;
            this.DataReturn = data;
        }

        public enum MessageType
        {
            COMMAND,
            TEXT,
            DATA,
        }
        
        public MessageType MsgType;
        public string Cmd;
        public string[] CmdArgs;
        public string TextMsg;
        /// <summary>
        /// Can only be used as a return type; 
        /// Can only be assigned in Constructors.
        /// </summary>
        public readonly object DataReturn;
        public delegate void MessageCallBack(MessageEventArgs mea);
    }
}
