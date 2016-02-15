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

namespace NaoAffectManager
{
    public partial class AffectMonitor : UserControl
    {
        AffectSpace RobotAffect = new AffectSpace();

        public double Valence
        {
            get { return RobotAffect.Valence; }
            set 
            {
                RobotAffect.Valence = value;
                this.nudValence.Value = (decimal)(value);
            }
        }

        public double Arousal
        {
            get { return RobotAffect.Arousal; }
            set { RobotAffect.Arousal = value; }
        }

        public AffectMonitor()
        {
            InitializeComponent();
        }

        private void tbValence_Scroll(object sender, EventArgs e)
        {
            RobotAffect.Valence = RobotAffect.range * (double)tbValence.Value / (double)(tbValence.Maximum - tbValence.Minimum);
            nudValence.Value = (decimal)RobotAffect.Valence;
            int range = tbValence.Maximum - tbValence.Minimum;
            double ratio = (double)tbValence.Value / (double)range + 0.5D;
        }

        private void nudValence_ValueChanged(object sender, EventArgs e)
        {
            RobotAffect.Valence = (double)nudValence.Value;
            double ratio = RobotAffect.Valence / RobotAffect.range;
            // Trackbar
            int range = tbValence.Maximum - tbValence.Minimum;
            tbValence.Value = (int)(range * ratio);
        }

        public TextSentiment SentiAffect
        {
            get { return UITextSentiment; }
        }
    }
}
