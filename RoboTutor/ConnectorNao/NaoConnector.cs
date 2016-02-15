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
using System.Xml;
using System.IO;

namespace ConnectorNao
{
    /// <summary>
    /// Used to communicate with NAO robots,
    ///  up to 2 robots at once.
    /// </summary>
    public partial class ucNaoConnector : UserControl
    {
        private ClientNao NaoClient = null;
        private ClientNao NaoClientVice = null;

        string IPConnected;

        // events
        public event ehNaoConneted evNaoConneted;
        public delegate void ehNaoConneted(string ip);

        public ucNaoConnector(ClientNao naoclient, ClientNao naoclientvice = null)
        {
            InitializeComponent();

            string ip = LoadDefaultIPSelection();
            int ind = this.cbbNaoIP.Items.IndexOf(ip);
            if (ind == -1)
            {
                this.cbbNaoIP.Items.Add(ip);
                this.cbbNaoIP.SelectedIndex = this.cbbNaoIP.Items.IndexOf(ip);
            }
            else this.cbbNaoIP.SelectedIndex = ind;

            this.NaoClient = naoclient;
            this.NaoClientVice = naoclientvice;

            this.NaoClient.evMotorState += MotorStateChanged;

            // Hide the debug test
            this.btnTest.Hide();
        }

        ~ucNaoConnector()
        {
            this.NaoClient.StopMotorMonitor();
        }

        //bool btnStateConnected = false;
        private void btnConnectNao_Click(object sender, EventArgs e)
        {
            string ip = cbbNaoIP.Text;
            bool b = NaoClient.connect(ip);

            RecordIPSelection(ip);

            if (!b)
            {
                MessageBox.Show("Cannot connect to Nao!");
                return;
            }

            if (NaoClientVice!=null)
            {
	            bool bv = NaoClientVice.connect(ip);
	            if (!bv)
	            {
	                MessageBox.Show("The 2nd ClientNao Cannot connect to Nao!");
	                return;
	            }
            }

            IPConnected = cbbNaoIP.Text;

            // Enable buttons
            btnStandUp.Enabled = true;
            btnMotors.Enabled = true;
            btnInitPose.Enabled = true;
            btnSquat.Enabled = true;
            btnWalk.Enabled = true;

            evNaoConneted(IPConnected);
        }

        private void RecordIPSelection(string ip)
        {
            XmlDocument xmldoc = new XmlDocument();
            string file = Environment.CurrentDirectory + "\\..\\..\\ConnectorNao\\DefaultIP.xml";

            XmlNode root = xmldoc.CreateElement("NaoConnector");
            XmlNode naoipnode = xmldoc.CreateElement("NaoIP");
            naoipnode.InnerText = ip;
            xmldoc.AppendChild(root);
            xmldoc["NaoConnector"].AppendChild(naoipnode);
            xmldoc.Save(file);
        }

        private string LoadDefaultIPSelection()
        {
            XmlDocument xmldoc = new XmlDocument();
            string file = Environment.CurrentDirectory + "\\..\\..\\ConnectorNao\\DefaultIP.xml";

            try
            {
                xmldoc.Load(file);
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("Root element is missing")
                    || ex.GetType() == typeof(FileNotFoundException))
                {
                    XmlNode root = xmldoc.CreateElement("NaoConnector");
                    string ip = this.cbbNaoIP.Items[0].ToString();
                    XmlNode naoipnode = xmldoc.CreateElement("NaoIP");
                    naoipnode.InnerText = ip;
                    xmldoc.AppendChild(root);
                    xmldoc["NaoConnector"].AppendChild(naoipnode);
                    xmldoc.Save(file);

                }
            }

            XmlNode naoipnode_ = xmldoc.SelectSingleNode("NaoConnector/NaoIP");
            if (naoipnode_ == null) // No such node "NaoIP"
            {
                string ip = this.cbbNaoIP.Items[0].ToString();
                naoipnode_ = xmldoc.CreateElement("NaoIP");
                naoipnode_.InnerText = ip;
                xmldoc["NaoConnector"].AppendChild(naoipnode_);
                xmldoc.Save(file);
                return ip;
            }
            else
            {
                return naoipnode_.InnerText;
            }
        }

        bool btnStateIdleMoveOnOff = false;
        private void btnIdleMoveOnOff_Click(object sender, EventArgs e)
        {
            if (btnStateIdleMoveOnOff == false)
            {
                btnIdleMove.Text = "IdleMoveOff";
                btnIdleMove.BackColor = Color.OrangeRed;
                NaoClient.StartIdleMovement();
                btnStateIdleMoveOnOff = true;
            }
            else
            {
                btnIdleMove.Text = "IdleMoveOn";
                btnIdleMove.BackColor = Color.DodgerBlue;
                NaoClient.StopIdleMovement();
                btnStateIdleMoveOnOff = false;
            }
        }

        bool btnStateIdleMovePauseResume = false;
        private void btnIdlePauseResume_Click(object sender, EventArgs e)
        {
            if (btnStateIdleMovePauseResume == false)
            {
                btnIdlePause.Text = "IdleMovePause";
                btnIdlePause.BackColor = Color.OrangeRed;
                NaoClient.PauseIdleMovement();
                btnStateIdleMovePauseResume = true;
            }
            else
            {
                btnIdlePause.Text = "IdleMoveResume";
                btnIdlePause.BackColor = Color.DodgerBlue;
                NaoClient.ResumeIdleMovement();
                btnStateIdleMovePauseResume = false;
            }
        }

        private void btnWalk_Click(object sender, EventArgs e)
        {
            NaoClient.Walk(0.01f);
        }

        private void btnStandUp_Click(object sender, EventArgs e)
        {
            NaoClient.StandUp();
        }

        private void btnSquat_Click(object sender, EventArgs e)
        {
            NaoClient.Squat();
        }

        private void btnInitPose_Click(object sender, EventArgs e)
        {
            NaoClient.InitPose();
        }

        bool btnStateMotorsOn = false;
        private void btnMotors_Click(object sender, EventArgs e)
        {
            NaoClient.MotorsOnOff(!btnStateMotorsOn);

            MotorStateChanged(!btnStateMotorsOn);
        }

        public void MotorStateChanged(bool onoff)
        {
            try
            {
	            Invoke((MethodInvoker)delegate()
	            {
	                if (onoff == true)
	                {
	                    btnMotors.Text = "MotorsOff";
	                    btnMotors.BackColor = Color.Red;
	                    btnStateMotorsOn = true;
	                }
	                else
	                {
	                    btnMotors.Text = "MotorsOn";
	                    btnMotors.BackColor = Color.DodgerBlue;
	                    btnStateMotorsOn = false;
	                }
	            });
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("NaoConnector: " + ex);
            }
        }

        #region Debug Test
        private void btnTest_Click(object sender, EventArgs e)
        {
            NaoClient.TestMotionProxies();
            //NaoClient.TestBehaviorManagers();
        }
        #endregion Debug Test
    }
}
