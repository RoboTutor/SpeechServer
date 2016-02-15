using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorNao;

namespace NaoManager
{
    /// <summary>
    /// Compare two things
    /// - modified from Weigh
    /// </summary>
    public class BehaviorProfileOneTheOther : BehaviorProfileBase
    {
        public override string BehaviorName
        {
            get { return "OneTheOther"; }
        }

        public BehaviorProfileOneTheOther()
        {
            Parameters.Add("Width", new Parameter(0, "Pose"));
            Parameters.Add("Height", new Parameter(0, "Pose"));
            Parameters.Add("Finger", new Parameter(0, "Pose"));
            //
            Parameters.Add("HeadUpDown", new Parameter(0, "Pose"));
            Parameters.Add("HeadLeftRight", new Parameter(0, "Pose"));
            //
            Parameters.Add("MotionSpeed", new Parameter(0, "Motion"));
            Parameters.Add("DecaySpeed", new Parameter(0, "Motion"));
            Parameters.Add("HoldTime", new Parameter(new List<double>(), "Motion"));
            Parameters.Add("Repetition", new Parameter(0, "Motion"));
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
            double finger    = Parameters["Finger"].Value;
            double headud    = Parameters["HeadUpDown"].Value;
            double headlr    = Parameters["HeadLeftRight"].Value;

            // right
            double shldroll_r =-15*width -5;
            double elbyaw_r   =90;
            double shldptch_r =-40*height+90;
            double elbroll_r  =28.5*height+60;
            double wristyaw_r =74.5*height+30;
            double hand_r     =0.3*finger+0.5;

            // degree
            double headPitch = base.NormalizeHeadPitch(headud);
            double headYaw = double.NaN;

            Head head = new Head(false, headPitch, headYaw);
            RArm rarm_one = new RArm(false, shldptch_r, shldroll_r,
                elbyaw_r, elbroll_r, wristyaw_r, hand_r);
            LArm larm_theother = (LArm)rarm_one.MirrorLeftRight();

            // Joints
            PoseProfile pose_head = new PoseProfile("Prepare", head);
            List<PoseProfile> lpp_prepare = base.ArmChest("Prepare");
            PoseProfile pose_rarm_one = new PoseProfile("One", rarm_one);
            PoseProfile pose_larm_theother = new PoseProfile("TheOther", larm_theother);

            List<PoseProfile> lpp = new List<PoseProfile>();
            lpp.Add(pose_head);
            lpp.AddRange(lpp_prepare);
            lpp.Add(pose_rarm_one);
            lpp.Add(pose_larm_theother);

            return lpp;
        }

        protected override void CoreAffectToParameter(double valence, double arousal = 0)
        {
            // pose
            double width = 0.05 * valence + 0.5;
            double height = 0.05 * valence + 0.5;
            double finger = 0.05 * valence + 0.5;
            // head
            double headud = 0.1 * valence;
            double headlr = 0;

            // motion
            double factor = 1.3;
            double motspd = factor *(0.035 * valence + 0.65);
            double decspd = 0.035 * valence + 0.65;
            double hold = 0.04*valence+0.6;
            double rep = 0;

            Parameters["Width"].Value = width;
            Parameters["Height"].Value = height;
            Parameters["Finger"].Value = finger;
            //
            Parameters["HeadUpDown"].Value = headud;
            Parameters["HeadLeftRight"].Value = headlr;
            //
            Parameters["MotionSpeed"].Value = motspd;
            Parameters["DecaySpeed"].Value = decspd;
            Parameters["HoldTime"].ValueList.Add(0);
            Parameters["HoldTime"].ValueList.Add(0.3);
            Parameters["HoldTime"].ValueList.Add(hold);
            Parameters["Repetition"].Value = rep;
        }
    }
}
