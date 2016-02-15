using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// understand, emphasize
    /// </summary>
    public class BehaviorProfileCapisce : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "Capisce"; }
        }

        public BehaviorProfileCapisce()
        {
            this.Parameters.Add("Width", new Parameter(0, "Pose"));
            this.Parameters.Add("Height", new Parameter(0, "Pose"));
            this.Parameters.Add("AmplitudeFinger", new Parameter(0, "Pose"));
            this.Parameters.Add("PalmDir", new Parameter(0, "Pose"));
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
            double width     = Parameters["Width"].Value;
            double height    = Parameters["Height"].Value;
            double amplitude = Parameters["AmplitudeFinger"].Value;
            double palmdir   = Parameters["PalmDir"].Value;
            double headud    = Parameters["HeadUpDown"].Value;
            double headlr    = Parameters["HeadLeftRight"].Value;

            double wristyaw = 60 * palmdir + 30;

            // width
            double shldroll = -30 * width;
            double elbyaw = 90;
            // height
            double shldpitch = -60 * height + 90;
            double elbroll = -30 * height + 60;

            double delta_elbroll = 50;
            // finger open pose
            double fingeropen = 1;
            double elbrollopen = elbroll + delta_elbroll;
            // finger close pose
            double elbrollclose = elbroll;
            double fingerclose = 0.3;

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

            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_down_finger_open = new RArm(false, shldpitch, shldroll, elbyaw, elbrollopen, wristyaw, fingeropen);
            RArm rarm_down_finger_close = new RArm(false, shldpitch, shldroll, elbyaw, elbrollclose, wristyaw, fingerclose);

            // Joints
            PoseProfile pose_head = new PoseProfile("OpenPose", head);
            PoseProfile pose_rarm_down_finger_open = new PoseProfile("OpenPose", rarm_down_finger_open, true, 1, 0);
            PoseProfile pose_rarm_down_finger_close = new PoseProfile("ClosePose", rarm_down_finger_close, true, 1, 0);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.Add(pose_rarm_down_finger_open);
            lpp.Add(pose_rarm_down_finger_close);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double upheight = 0.05 * valence + 0.5;
            double downheight = 0.05 * valence + 0.5;
            double amplitude = 0.05 * valence + 0.5;
            double palmdir = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // motion
            double motspd = 0.035 * valence + 0.65;
            double decspd = motspd;
            double rep;
            if (valence >= 8) rep = 1;
            else rep = 0;
            double holdtime = 0.025 * valence + 0.25;
            // head
            double headud = 0.1 * valence;
            double headlr = 0.05 * valence + 0.5;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = downheight;
            Parameters["AmplitudeFinger"].Value = amplitude;
            Parameters["PalmDir"].Value = palmdir;
            //
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
