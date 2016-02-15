using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfilePressDown : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "PressDown"; }
        }

        public BehaviorProfilePressDown()
        {
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDirection", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(new List<double>(), "Motion"));
            this.Parameters.Add("Repetition", new Parameter(0, "Motion"));
        }

        public override ConnectorNao.MotionTimeline LoadPreset(string setname, int setmode)
        {
            throw new NotImplementedException();
        }

        public override ConnectorNao.MotionTimeline LoadSingleParam(int paramind, int level)
        {
            throw new NotImplementedException();
        }

        protected override List<double> CoreParameterToJoint(bool right = true)
        {
            throw new NotImplementedException();
        }

        protected override List<PoseProfile> CoreParameterToPose(bool right = true)
        {
            double amp = Parameters["Amplitude"].Value;
            double palmdir = Parameters["PalmDirection"].Value;
            double finger = Parameters["Finger"].Value;
            double headud = Parameters["HeadUpDown"].Value;
            double headlr = Parameters["HeadLeftRight"].Value;

            double rshoulderroll_high  = -10*amp-20;
            double rshoulderpitch_high = -20*amp+40;
            double relbowroll_high     = 5*amp+40;
            double relbowyaw_high      = 119.5;
            double rwristyaw_high      = 30 * palmdir - 90;
            double rhand_high          = 0.6;

            double rshoulderroll_low  =  -5*amp-5;
            double rshoulderpitch_low = -10 * amp + 90;
            double relbowroll_low     = -5 * amp + 60;
            double relbowyaw_low      = 119.5;
            double rwristyaw_low      = 30 * palmdir - 90;
            double rhand_low          = 0.3 * finger + 0.6;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw;
            if (headlr > 0.5)
            {
                headYaw = -40 * headlr + 20;
            }
            else
            {
                headYaw = double.NaN;
            }

            RArm rarm_high = new RArm(false, rshoulderpitch_high, rshoulderroll_high, 
                relbowyaw_high, relbowroll_high, rwristyaw_high, rhand_high);
            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_low = new RArm(false, rshoulderpitch_low, rshoulderroll_low,
                relbowyaw_low, relbowroll_low, rwristyaw_low, rhand_low);

            // Joints
            PoseProfile pose_rarm_init = new PoseProfile("HighPose", rarm_high, true, 0, 0.03);
            PoseProfile pose_head_init = new PoseProfile("HighPose", head);
            PoseProfile pose_rarm_low = new PoseProfile("LowPose", rarm_low, true, 1.0, 0.03);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_rarm_init);
            lpp.Add(pose_head_init);
            lpp.Add(pose_rarm_low);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double amp = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            // motion
            double factor;
            if (valence >= 0) factor = 0.9;
            else factor = 2.0;
            double motspd = factor*(0.035 * valence + 0.65);
            double decspd = factor*(0.035 * valence + 0.65);
            double holdtime = 0.04 * valence + 0.6;
            double rep;
            if (valence >= 8) rep = 1;
            else rep = 0;

            Parameters["Amplitude"].Value = amp;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
