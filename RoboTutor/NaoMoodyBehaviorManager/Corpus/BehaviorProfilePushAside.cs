using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfilePushAside : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "PushAside"; }
        }

        public BehaviorProfilePushAside()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Amplitude", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
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
            double width      = Parameters["Width"].Value;
            double height     = Parameters["Height"].Value;
            double amplitude  = Parameters["Amplitude"].Value;
            double palmdir    = Parameters["PalmDirection"].Value;
            double finger     = Parameters["Finger"].Value;
            double headud     = Parameters["HeadUpDown"].Value;
            double headlr     = Parameters["HeadLeftRight"].Value;

            // finger 0~1
            double hand = 0.4 * finger + 0.6;
            double wristyaw = 90 * palmdir;

            // pose center width
            double orig_shldroll  = 10 * width;
            // pose center height
            double orig_shldpitch = -40 * height + 70;
            double orig_elbroll   = -30 * height + 60;
            // Amplitude
            double orig_elbyaw = -10 * amplitude + 60;
            // pose palm direction
            double orig_wristyaw  = wristyaw;
            double orig_hand      = 0.4;

            double delta_shldroll  = 25 * amplitude + 15;
            //double delta_elbyaw  = 10 * amplitude + 59.5;
            // swing outward
            double out_shldroll    = orig_shldroll - delta_shldroll;
            double out_elbyaw      = 119.5; //orig_elbyaw + delta_elbyaw;
            double out_shlorigitch = orig_shldpitch;
            double out_elbroll     = orig_elbroll - delta_shldroll / 3;
            double out_wristyaw    = orig_wristyaw;
            double out_hand        = hand;

            // degree
            double headpitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw = double.NaN;

            Head head = new Head(false, headpitch, headYaw);
            RArm rarm_orig = new RArm(false, orig_shldpitch, orig_shldroll, orig_elbyaw, orig_elbroll, orig_wristyaw, orig_hand);
            RArm rarm_out = new RArm(false, out_shlorigitch, out_shldroll, out_elbyaw, out_elbroll, out_wristyaw, out_hand);

            // Joints
            PoseProfile pose_head = new PoseProfile("UpPose", head);
            PoseProfile pose_rarm_orig = new PoseProfile("UpPose", rarm_orig, true);
            PoseProfile pose_rarm_out = new PoseProfile("DownPose", rarm_out, true);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.Add(pose_rarm_orig);
            lpp.Add(pose_rarm_out);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double amplitude = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.025 * valence + 0.55;
            double decspd = 0.0275 * valence + 0.325;
            double holdtime = 0.05 * valence + 0.5;
            double rep = 0;
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["Width"].Value = width;
            Parameters["Amplitude"].Value = amplitude;
            Parameters["Height"].Value = height;
            Parameters["PalmDirection"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(holdtime);
            Parameters["Repetition"].Value = rep;
        }
    }
}
