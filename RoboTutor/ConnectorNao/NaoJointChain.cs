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
using System.Diagnostics;

namespace ConnectorNao
{
    public class Joint
    {
        // Joint name
        public string Name;
        // Joint value in degree
        public double ValDeg;
        // Joint value in radian
        public double ValRad;
        // Use this, if each pose needs different speed
        public double SpdT;
    }

    public class JointChain
    {
        public virtual string Name{ get{return "JointChain";} }

        public JointChain() { }

        public virtual JointChain DeepCopy() { return null; }

        public virtual List<float> JointInUseValue{ set; get; }
        public virtual List<string> JointInUseNames{ set; get; }
        public virtual List<float> JointInUseSpeed { set; get; }

        public virtual JointChain MirrorLeftRight(){return this;}

        public virtual JointChain InterpolateJointChain(JointChain jc, double coef)
        {
            if (jc.Name == this.Name)
            {
                return this;
            }
            else
            {
                Debug.WriteLine("PoseInterpolation: joints do not match!");
                return null;
            }
        }

        protected double Interpolation(double a, double b, double percent)
        {
            if (percent >= 0 && percent <= 1)
            {
                return a * (1 - percent) + b * percent;
            }
            else
            {
                return double.NaN;
            }
        }
    }

    // The joints that are not needed must be set to "Double.NaN".
    //  Then the sequencer will ignore this joint value.
    public class Arm : JointChain
    {
        // Name
        public override string Name {get{return "Arm";}}
        // Joints in radian
        protected double ShoulderPitch = Double.NaN;
        protected double ShoulderRoll = Double.NaN;
        protected double ElbowYaw = Double.NaN;
        protected double ElbowRoll = Double.NaN;
        protected double WristYaw = Double.NaN;
        protected double Hand = Double.NaN;

        public Arm() { }
        // isRadian: 0 degree 1 radian
        public Arm(bool isRadian, double sp, double sr, double ey, double er, double wy, double h)
        {
            ShoulderPitch = sp;
            ShoulderRoll = sr;
            ElbowYaw = ey;
            ElbowRoll = er;
            WristYaw = wy;
            Hand = h;
            if (!isRadian)
            {
                double r = Math.PI / 180D;
                ShoulderPitch *= r;
                ShoulderRoll *= r;
                ElbowYaw *= r;
                ElbowRoll *= r;
                WristYaw *= r;
                //Hand *= r;
            }
        }

        public override List<float> JointInUseValue
        {
            get
            {
                List<float> jvr = new List<float>();
                if (ShoulderPitch != Double.NaN)
                    jvr.Add((float)ShoulderPitch);
                if (ShoulderRoll != Double.NaN)
                    jvr.Add((float)ShoulderRoll);
                if (ElbowYaw != Double.NaN)
                    jvr.Add((float)ElbowYaw);
                if (ElbowRoll != Double.NaN)
                    jvr.Add((float)ElbowRoll);
                if (WristYaw != Double.NaN)
                    jvr.Add((float)WristYaw);
                if (Hand != Double.NaN)
                    jvr.Add((float)Hand);
                return jvr;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override JointChain MirrorLeftRight() 
        {
            Arm m_arm = null;           
            if (this.Name == "RArm")
            {
                m_arm = new LArm(                    
                    true, 
                    this.ShoulderPitch,
                    -this.ShoulderRoll,
                    -this.ElbowYaw,
                    -this.ElbowRoll,
                    -this.WristYaw,
                    this.Hand
                    );
            } 
            else if (this.Name == "LArm")
            {
                m_arm = new RArm(
                    true, 
                    this.ShoulderPitch,
                    -this.ShoulderRoll,
                    -this.ElbowYaw,
                    -this.ElbowRoll,
                    -this.WristYaw,
                    this.Hand
                    );
            }
            else
            {
                Debug.WriteLine("JointChainMirror: wrong arm joint chain name");
            }

            return m_arm; 
        }

        public override JointChain InterpolateJointChain(JointChain jc, double coef)
        {
            Arm arm = (Arm)jc;
            if (jc.Name == this.Name)
            {
                double shldptch = base.Interpolation(this.ShoulderPitch, arm.ShoulderPitch, coef);
                double shldroll = base.Interpolation(this.ShoulderRoll, arm.ShoulderRoll, coef);
                double elbyaw = base.Interpolation(this.ElbowYaw, arm.ElbowYaw, coef);
                double elbroll = base.Interpolation(this.ElbowRoll, arm.ElbowRoll, coef);
                double wrstyaw = base.Interpolation(this.WristYaw, arm.WristYaw, coef);
                double hand = base.Interpolation(this.Hand, arm.Hand, coef);
                if (this.Name == "RArm")
                {
                    return new RArm(true, shldptch, shldroll, elbyaw, elbroll, wrstyaw, hand);
                } 
                else // LArm
                {
                    return new LArm(true, shldptch, shldroll, elbyaw, elbroll, wrstyaw, hand);
                }
            }
            else
            {
                Debug.WriteLine("PoseInterpolation: joints do not match!");
                return null;
            }
        }
    }

    public class RArm : Arm
    {
        public override string Name { get { return "RArm"; } }
        public override List<string> JointInUseNames
        {
            get
            {
                List<string> jn = new List<string>();
                // Should add in order
                if (ShoulderPitch != Double.NaN)
                    jn.Add("RShoulderPitch");
                if (ShoulderRoll != Double.NaN)
                    jn.Add("RShoulderRoll");
                if (ElbowYaw != Double.NaN)
                    jn.Add("RElbowYaw");
                if (ElbowRoll != Double.NaN)
                    jn.Add("RElbowRoll");
                if (WristYaw != Double.NaN)
                    jn.Add("RWristYaw");
                if (Hand != Double.NaN)
                    jn.Add("RHand");
                return jn;
            }
        }

        public RArm(bool isRadian, double sp, double sr, double ey, double er, double wy, double h)
            : base(isRadian, sp, sr, ey, er, wy, h) { }

        public override JointChain DeepCopy()
        {
            JointChain jc = new RArm(true, this.ShoulderPitch, this.ShoulderRoll, this.ElbowYaw, this.ElbowRoll, this.WristYaw, this.Hand);
            return jc;
        }
    }

    public class LArm : Arm
    {
        public override string Name { get { return "LArm"; } }
        public override List<string> JointInUseNames
        {
            get
            {
                // Should add in order
                List<string> jn = new List<string>();
                if (ShoulderPitch != Double.NaN)
                    jn.Add("LShoulderPitch");
                if (ShoulderRoll != Double.NaN)
                    jn.Add("LShoulderRoll");
                if (ElbowYaw != Double.NaN)
                    jn.Add("LElbowYaw");
                if (ElbowRoll != Double.NaN)
                    jn.Add("LElbowRoll");
                if (WristYaw != Double.NaN)
                    jn.Add("LWristYaw");
                if (Hand != Double.NaN)
                    jn.Add("LHand");
                return jn;
            }
        }

        public LArm(bool isRadian, double sp, double sr, double ey, double er, double wy, double h)
            : base(isRadian, sp, sr, ey, er, wy, h) { }

        public override JointChain DeepCopy()
        {
            JointChain jc = new LArm(true, this.ShoulderPitch, this.ShoulderRoll, this.ElbowYaw, this.ElbowRoll, this.WristYaw, this.Hand);
            return jc;
        }
    }

    public class Head : JointChain
    {
        // Name
        public override string Name { get { return "Head"; } }
        // Joints
        protected double HeadPitch = Double.NaN;
        protected double HeadYaw = Double.NaN;

        public Head(bool isRadian, double hp, double hy)
        {
            HeadPitch = hp;
            HeadYaw = hy;
            if (!isRadian)
            {
                double r = Math.PI / 180D;
                HeadPitch *= r;
                HeadYaw *= r;
            }
        }

        public override List<float> JointInUseValue
        {
            get
            {
                List<float> jvr = new List<float>();
                if (HeadPitch != Double.NaN)
                    jvr.Add((float)HeadPitch);
                if (HeadYaw != Double.NaN)
                    jvr.Add((float)HeadYaw);
                return jvr;
            }
        }

        public override List<string> JointInUseNames
        {
            get
            {
                List<string> jn = new List<string>();
                if (HeadPitch != Double.NaN)
                    jn.Add("HeadPitch");
                if (HeadYaw != Double.NaN)
                    jn.Add("HeadYaw");
                return jn;
            }
        }

        public override JointChain DeepCopy()
        {
            JointChain jc = new Head(true, this.HeadPitch, this.HeadYaw);
            return jc;
        }

        /// <summary>
        /// Only Head-Left-Right is mirrored, of course.
        /// </summary>
        /// <returns></returns>
        public override JointChain MirrorLeftRight()
        {
            return new Head(true, this.HeadPitch, -this.HeadYaw);
        }
    }

    // The joints that are not needed must be set to "Double.NaN".
    //  Then the sequencer will ignore this joint value.
    public class Leg : JointChain
    {
        // Name
        public override string Name { get { return "Leg"; } }
        // Joints in radian
        protected double HipYawPitch = Double.NaN;
        protected double HipPitch    = Double.NaN;
        protected double HipRoll     = Double.NaN;
        protected double KneePitch   = Double.NaN;
        protected double AnklePitch  = Double.NaN;
        protected double AnkleRoll   = Double.NaN;

        public Leg() { }

        // isRadian: 0 degree 1 radian
        public Leg(bool isRadian, double hyp, double hp, double hr, double kp, double ap, double ar)
        {
            HipYawPitch = hyp;
            HipPitch    = hp;
            HipRoll     = hr;
            KneePitch   = kp;
            AnklePitch  = ap;
            AnkleRoll   = ar;
            if (!isRadian)
            {
                double r = Math.PI / 180D;
                HipYawPitch *= r;
                HipPitch *= r;
                HipRoll *= r;
                KneePitch *= r;
                AnklePitch *= r;
                AnkleRoll *= r;
            }
        }

        public override List<float> JointInUseValue
        {
            get
            {
                List<float> jvr = new List<float>();
                if (HipYawPitch != Double.NaN)
                    jvr.Add((float)HipYawPitch);
                if (HipPitch != Double.NaN)
                    jvr.Add((float)HipPitch);
                if (HipRoll != Double.NaN)
                    jvr.Add((float)HipRoll);
                if (KneePitch != Double.NaN)
                    jvr.Add((float)KneePitch);
                if (AnklePitch != Double.NaN)
                    jvr.Add((float)AnklePitch);
                if (AnkleRoll != Double.NaN)
                    jvr.Add((float)AnkleRoll);
                return jvr;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override JointChain MirrorLeftRight() 
        {
            Leg m_leg = null;           
            if (this.Name == "RLeg")
            {
                m_leg = new LLeg(
                    true,
                    this.HipYawPitch,
                    this.HipPitch,
                    -this.HipRoll,
                    this.KneePitch,
                    this.AnklePitch,
                    -this.AnkleRoll
                );

            }
            else if (this.Name == "LLeg")
            {
                m_leg = new RLeg(
                    true,
                    this.HipYawPitch,
                    this.HipPitch,
                    -this.HipRoll,
                    this.KneePitch,
                    this.AnklePitch,
                    -this.AnkleRoll
                );
            }
            else
            {
                Debug.WriteLine("JointChainMirror: wrong leg joint chain name");
            }

            return m_leg; 
        }
    }

    public class RLeg : Leg 
    {
        public override string Name { get { return "RLeg"; } }

        public override List<string> JointInUseNames
        {
            get
            {
                List<string> jn = new List<string>();
                // Should add in order
                if (HipYawPitch != Double.NaN)
                    jn.Add("RHipYawPitch");
                if (HipPitch != Double.NaN)
                    jn.Add("RHipPitch");
                if (HipRoll != Double.NaN)
                    jn.Add("RHipRoll");
                if (KneePitch != Double.NaN)
                    jn.Add("RKneePitch");
                if (AnklePitch != Double.NaN)
                    jn.Add("RAnklePitch");
                if (AnkleRoll != Double.NaN)
                    jn.Add("RAnkleRoll");
                return jn;
            }
        }

        public RLeg(bool isRadian, double hyp, double hp, double hr, double kp, double ap, double ar)
            : base(isRadian, hyp, hp, hr, kp, ap, ar) { }

        public override JointChain DeepCopy()
        {
            JointChain jc = new RArm(true, this.HipYawPitch, this.HipPitch, this.HipRoll, this.KneePitch, this.AnklePitch, this.AnkleRoll);
            return jc;
        }
    }

    public class LLeg : Leg
    {
        public override string Name { get { return "LLeg"; } }

        public override List<string> JointInUseNames
        {
            get
            {
                List<string> jn = new List<string>();
                // Should add in order
                if (HipYawPitch != Double.NaN)
                    jn.Add("LHipYawPitch");
                if (HipPitch != Double.NaN)
                    jn.Add("LHipPitch");
                if (HipRoll != Double.NaN)
                    jn.Add("LHipRoll");
                if (KneePitch != Double.NaN)
                    jn.Add("LKneePitch");
                if (AnklePitch != Double.NaN)
                    jn.Add("LAnklePitch");
                if (AnkleRoll != Double.NaN)
                    jn.Add("LAnkleRoll");
                return jn;
            }
        }

        public LLeg(bool isRadian, double hyp, double hp, double hr, double kp, double ap, double ar)
            : base(isRadian, hyp, hp, hr, kp, ap, ar) { }

        public override JointChain DeepCopy()
        {
            JointChain jc = new RArm(true, this.HipYawPitch, this.HipPitch, this.HipRoll, this.KneePitch, this.AnklePitch, this.AnkleRoll);
            return jc;
        }
    }
}
