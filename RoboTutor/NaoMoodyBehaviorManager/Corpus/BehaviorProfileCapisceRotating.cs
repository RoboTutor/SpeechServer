using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    public class BehaviorProfileCapisceRotate : BehaviorProfileBase
    {

        public override string BehaviorName
        {
            get { return "CapisceRotate"; }
        }

        public BehaviorProfileCapisceRotate()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose")); // diameter of the circle
            this.Parameters.Add("Forward", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDir", new Parameter(0, "Pose"));
            this.Parameters.Add("Finger", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            this.Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));

            this.Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            this.Parameters.Add("HoldTime", new Parameter(0, "Motion"));
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
            double width   = Parameters["Width"].Value;
            double forward = Parameters["Forward"].Value;
            double palmdir = Parameters["PalmDir"].Value;
            double finger  = Parameters["Finger"].Value;
            double headud  = Parameters["HeadUpDown"].Value;
            double headlr  = Parameters["HeadLeftRight"].Value;


            // up inward pose
            double up_in_shldpitch = -10 * width + 40;
            double up_in_shldroll = 6 * width + 12;
            double up_in_elbyaw = -10 * width + 70;

            // up outward pose
            double up_out_shldpitch = -10 * width + 45;
            double up_out_shldroll = -3 * width - 12;
            double up_out_elbyaw = 10 * width + 100;

            // down inward pose
            double down_in_shldpitch = 5 * width + 60;
            double down_in_shldroll =  width + 8;
            double down_in_elbyaw = 5 * width + 75;

            // down outward pose
            double down_out_shldpitch = 10 * width + 65;
            double down_out_shldroll = -2 * width - 8;
            double down_out_elbyaw = 5 * width + 95;

            // forward
            double elbroll = -43.5 * forward + 88.5;
            // palm dir
            double wristyaw = 90 * palmdir;
            // finger
            double hand = 0.4 * finger + 0.6;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            // degree
            double headYaw;
            if (headlr > 0.5)
            {
                headYaw = -36 * headlr + 18;
            }
            else
            {
                headYaw = double.NaN;
            }

            Head head          = new Head(false, headPitch, headYaw);
            RArm rarm_upinward   = new RArm(false, up_in_shldpitch, up_in_shldroll, up_in_elbyaw, elbroll, wristyaw, finger);
            RArm rarm_upoutward  = new RArm(false, up_out_shldpitch, up_out_shldroll, up_out_elbyaw, elbroll, wristyaw, finger);
            RArm rarm_downinward = new RArm(false, down_in_shldpitch, down_in_shldroll, down_in_elbyaw, elbroll, wristyaw, finger);
            RArm rarm_downoutward = new RArm(false, down_out_shldpitch, down_out_shldroll, down_out_elbyaw, elbroll, wristyaw, finger);

            // Joints
            PoseProfile pose_head = new PoseProfile("upin", head);
            PoseProfile pose_rarm_upin = new PoseProfile("upin", rarm_upinward);
            PoseProfile pose_rarm_upout = new PoseProfile("upout", rarm_upoutward, true);
            PoseProfile pose_rarm_downin = new PoseProfile("downin", rarm_downinward, true);
            PoseProfile pose_rarm_downout = new PoseProfile("downout", rarm_downoutward, true);
            PoseProfile pose_rarm_upin_back = new PoseProfile("upinback", rarm_upinward, true);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.Add(pose_rarm_upin);
            lpp.Add(pose_rarm_upout);
            lpp.Add(pose_rarm_downout);
            lpp.Add(pose_rarm_downin);
            lpp.Add(pose_rarm_upin_back);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double forward = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = 0.035 * valence + 0.65;
            double rep;
            if (valence >= 8) rep = 1;
            else rep = 0;
            double hold = 0;

            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["Width"].Value = width;
            Parameters["Forward"].Value = forward;
            Parameters["PalmDir"].Value = palmdir;
            Parameters["Finger"].Value = finger;
            //
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].Value = hold;
            Parameters["Repetition"].Value = rep;
        }
    }
}
