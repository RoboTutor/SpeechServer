using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Aldebaran.Proxies;
using System.Threading;
using System.Diagnostics;
using System.Collections;

namespace ConnectorNao
{
    public class NeutralMovement
    {

        float TO_RAD = (float)(Math.PI / 180.0);

        private int mItems;
        public event EventHandler NeutralStopped;
        public event EventHandler NeutralPaused;
        public MovingSettings setting = null;
        private bool mKeepNeutralRunning = false;
        public bool KeepNeutralRunning
        {
            get { return mKeepNeutralRunning; }
            set { mKeepNeutralRunning = value; }
        }
        private bool mPauseMoving;
        public bool PauseMoving
        {
            get { return mPauseMoving; }
            set
            {
                mPauseMoving = value;
                if (value == true)
                {
                    PauseSignaled = false;
                }
            }
        }

        public NeutralMovement(MotionProxy motionproxy, object LockMotion, MovingSettings ms,
            LedsProxy ledsproxy, object LockLeds)
        {
            setting = ms;

            ALMotion = motionproxy;
            this.LockMotion = LockMotion;
            ALLeds = ledsproxy;
            this.LockLeds = LockLeds;

            mItems = setting.WhatToMove;
        }

        public void StartNeutralRunning( )
        {
            BackgroundWorker bgw_moving_calculation = new BackgroundWorker();
            //bgw_moving_calculation.DoWork += new DoWorkEventHandler(bgw_moving_calculation);
            bgw_moving_calculation.DoWork += new DoWorkEventHandler(bgw_moving_createThreads);
            bgw_moving_calculation.RunWorkerAsync();

            BackgroundWorker bgw_moving = new BackgroundWorker();
            bgw_moving.DoWork += new DoWorkEventHandler(bgw_moving_DoWork);

            KeepNeutralRunning = true;
            PauseMoving = false;

            bgw_moving.RunWorkerAsync();

            Debug.WriteLine("StartNeutralRunning");
        }

        bool HipsToMove = false;
        bool ArmsToNove = false;
        bool HeadToMove = false;
        bool HandToMove = false;
        object LockArms = new object();
        object LockHead = new object();
        object LockHand = new object();
        object LockHip = new object();
        object LockEyeValues = new object();

        List<List<float>> klist = new List<List<float>>();
        List<List<float>> tlist = new List<List<float>>();
        List<string> HipJointNames;

        void bgw_moving_DoWork(object sender, DoWorkEventArgs e)
        {

            while (KeepNeutralRunning)
            {

                if (PauseMoving == false)
                {
                    Debug.WriteLineIf(ShowAllThreats, "hips start");
                    if (HipsToMove == true)
                    {
                        lock (LockHip)
                        {
                            StartMotions(HipJointNames, klist, tlist);
                            HipsToMove = false;
                        }
                    }
                    Debug.WriteLineIf(ShowAllThreats, "hips end");
                }
                if (PauseMoving == false)
                {
                    Debug.WriteLineIf(ShowAllThreats, "arms start");
                    if (ArmsToNove == true)
                    {
                        lock (LockArms)
                        {
                            SetArmAngles(ArmJoints, ArmDeltas[1], ArmDeltas[2], ArmDeltas[3], ArmDeltas[4], ArmDeltas[5]);
                            ArmsToNove = false;
                        }
                    }
                    Debug.WriteLineIf(ShowAllThreats, "arms end");
                }
                if (PauseMoving == false)
                {
                    if (HandToMove == true)
                    {
                        Debug.WriteLineIf(ShowAllThreats, "hand start");
                        lock (LockHand)
                        {
                            lock (LockMotion)
                            {
                                TurnHand(RHand_value, LHand_value);
                                HandToMove = false;
                            }
                        }
                        Debug.WriteLineIf(ShowAllThreats, "hand end");
                    }
                }
                if (PauseMoving == false)
                {
                    if (HeadToMove == true)
                    {
                        Debug.WriteLineIf(ShowAllThreats, "head start");
                        lock (LockHead)
                        {
                            lock (LockMotion)
                            {
                                TurnHead(HeadYaw, HeadPitch);
                                HeadToMove = false;
                            }
                        }
                        Debug.WriteLineIf(ShowAllThreats, "head end");
                    }
                }
                // Pause the thread if the moving is paused otherwise it drains the cpu.....
                if (PauseMoving == true)
                {
                    // only signal once
                    if ((PauseSignaled == false) && (NeutralPaused != null))
                    {
                        NeutralPaused(null, null);
                        PauseSignaled = true;
                        Debug.WriteLineIf(ShowAllThreats, "PauseSignaled");
                    }
                    //Thread.Sleep(150);
                }
                //Debug.WriteLine("bgw_moving_DoWork PauseMoving:" + PauseMoving);
                Thread.Sleep(250);
            }
            //Debug.WriteLine("bgw_moving_DoWork stopped");
            NeutralStopped(null, null);
        }
        private bool PauseSignaled = false;

        private void StartMotions(object names, object angles, object time)
        {
            lock (LockMotion)
            {
                try
                {
                    Debug.WriteLineIf(ShowAllThreats, "StartMotions ");
                    ALMotion.angleInterpolation(names, angles, time, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error in startmotions " + ex.Message);
                }
            }
        }

        object LockMotion;
        MotionProxy ALMotion = null;
        object LockLeds;
        LedsProxy ALLeds = null;

        Random random = null;

        void bgw_moving_createThreads(object sender, DoWorkEventArgs e)
        {
            random = new Random(Environment.TickCount);

            if ((mItems & setting.MoveHead) > 0)
            {
                BackgroundWorker bgw_headmoving = new BackgroundWorker();
                bgw_headmoving.DoWork += new DoWorkEventHandler(bgw_headmoving_DoWork);
                bgw_headmoving.RunWorkerAsync();
            }

            if ((mItems & setting.BlinkEyes) > 0)
            {
                BackgroundWorker bgw_blink = new BackgroundWorker();
                bgw_blink.DoWork += new DoWorkEventHandler(bgw_blink_DoWork);
                bgw_blink.RunWorkerAsync();
            }

            if ((mItems & setting.RandomEyes) > 0)
            {
                BackgroundWorker bgw_eyecolor = new BackgroundWorker();
                bgw_eyecolor.DoWork += new DoWorkEventHandler(bgw_eyecolor_DoWork);
                bgw_eyecolor.RunWorkerAsync();
            }

            if ((mItems & setting.MoveArms) > 0)
            {
                BackgroundWorker bgw_arms = new BackgroundWorker();
                bgw_arms.DoWork += new DoWorkEventHandler(bgw_arms_DoWork);
                bgw_arms.RunWorkerAsync();
            }

            if ((mItems & setting.MoveHands) > 0)
            {
                BackgroundWorker bgw_hands = new BackgroundWorker();
                bgw_hands.DoWork += new DoWorkEventHandler(bgw_handmoving_DoWork);
                bgw_hands.RunWorkerAsync();
            }

            if ((mItems & setting.MoveHips) > 0)
            {
                BackgroundWorker bgw_hips = new BackgroundWorker();
                bgw_hips.DoWork += new DoWorkEventHandler(bgw_hipsmoving_DoWork);
                bgw_hips.RunWorkerAsync();
            }
        }

        public int[] ColorOfEyes
        {
            set
            {
                lock (LockEyeValues)
                {
                    setting.on = value[0] * 256 * 256 + value[1] * 256 + value[2];
                    setting.off = value[0] / 10 * 256 * 256 + value[1] / 10 * 256 + value[2] / 10;
                }
                Blink(BlinkDuration);
            }
        }

        #region HeadMoving
        void bgw_headmoving_DoWork(object sender, DoWorkEventArgs e)
        {

            // Reading from NAO-memory takes much time!
            //HeadPitch = (float)(ALMem.getData("Device/SubDeviceList/HeadPitch/Position/Actuator/Value"));
            //HeadYaw = (float)(ALMem.getData("Device/SubDeviceList/HeadYaw/Position/Actuator/Value"));

            while (KeepNeutralRunning == true)
            {
                if (PauseMoving == false)
                {
                    CalcHeadMovement(setting.inc_head_yaw, setting.inc_head_pitch, setting.min_head_yaw, setting.max_head_yaw,
                        setting.min_head_pitch, setting.max_head_pitch);
                }

                Thread.Sleep(random.Next(setting.MoveHeadTimes.MinTime, setting.MoveHeadTimes.MaxTime));
                Debug.WriteLineIf(ShowAllThreats, "bgw_headmoving_DoWork");
            }
        }
        float HeadPitch = 0;
        float HeadYaw = 0;
        bool ShowAllThreats = false;

        void CalcHeadMovement(int inc_yaw, int inc_pitch, float min_yaw, float max_yaw, float min_pitch, float max_pitch)
        {
            float deltayaw = (float)(random.NextDouble() - 0.5) * inc_yaw;
            float deltapitch = (float)(random.NextDouble() - 0.5) * inc_pitch;

            //Debug.WriteLine("pitch " + HeadPitch + "  delta " + deltapitch + "  yaw " + HeadYaw + "  delta " + deltayaw);
            try
            {
                if ((HeadYaw > min_yaw) && (HeadYaw < max_yaw))
                {
                    HeadYaw = HeadYaw + deltayaw;
                }
                else
                {
                    HeadYaw = (max_yaw + min_yaw) / 2.0F;
                }
                if ((HeadPitch > min_pitch) && (HeadPitch < max_pitch))
                {
                    HeadPitch = HeadPitch + deltapitch;
                }
                else
                {
                    HeadPitch = (max_pitch + min_pitch) / 2.0F;
                }
                HeadToMove = true;
                //TurnHead(HeadYaw, HeadPitch);
            }
            catch (Exception)
            {
                Debug.WriteLineIf(ShowAllThreats, "Error during head movement");
            }
        }

        void TurnHead(float yaw, float pitch)
        {
            string[] names = { "HeadYaw", "HeadPitch" };
            float[] angles = { yaw * TO_RAD, pitch * TO_RAD };
            float fractionMaxSpeed = 0.05F;

            try
            {
                lock (LockMotion)
                {
                    ALMotion.post.setAngles(names, angles, fractionMaxSpeed);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(ShowAllThreats, "Error during tunrhead  " + ex.Message);
            }

        }
        #endregion

        #region HandsMoving
        void bgw_handmoving_DoWork(object sender, DoWorkEventArgs e)
        {
            // Reading from NAO-memory takes much time!
            //HeadPitch = (float)(ALMem.getData("Device/SubDeviceList/HeadPitch/Position/Actuator/Value"));
            //HeadYaw = (float)(ALMem.getData("Device/SubDeviceList/HeadYaw/Position/Actuator/Value"));

            while (KeepNeutralRunning == true)
            {
                if (PauseMoving == false)
                {
                    CalcHandMovement(setting.RHand_min_value, setting.RHand_max_value,
                        setting.LHand_min_value, setting.LHand_max_value, setting.inc_hand_value);
                }

                Thread.Sleep(random.Next(setting.MoveHandsTimes.MinTime, setting.MoveHandsTimes.MaxTime));
                Debug.WriteLineIf(ShowAllThreats, "bgw_handmoving_DoWork");
            }
        }

        float RHand_value = 0;
        float LHand_value = 0;

        private void CalcHandMovement(float RMin, float RMax, float LMin, float LMax, float increase)
        {
            float deltaleft = (float)(random.NextDouble() - 0.5) * increase;
            float deltalright = (float)(random.NextDouble() - 0.5) * increase;

            double whichhand = random.NextDouble();
            DateTime start = DateTime.Now;
            if (whichhand > 0.5)
                deltaleft = 0.0F;
            else
                deltalright = 0.0F;

            if ((RHand_value > RMin) && (RHand_value < RMax))
            {
                RHand_value = RHand_value + deltalright;
            }
            else
            {
                RHand_value = (RMin + RMax) / 2.0F;
            }

            if ((LHand_value > LMin) && (LHand_value < LMax))
            {
                LHand_value = LHand_value + deltaleft;
            }
            else
            {
                LHand_value = (LMin + LMax) / 2.0F;
            }
            HandToMove = true;
        }

        private void TurnHand(float right, float left)
        {
            string[] names = { "RHand", "LHand" };
            float[] angles = { right, left };
            //float fractionMaxSpeed = 0.05F;
            float[] time = { 0.5F, 0.5F };
            //ALMotion.setAngles(names, angles, fractionMaxSpeed);

            StartMotions(names, angles, time);
        }
        #endregion

        #region Blinking
        public float BlinkDuration = 0.1F;

        void bgw_eyecolor_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] rgb = new int[3];
            Random ColorChange = new Random(Environment.TickCount);

            while (KeepNeutralRunning == true)
            {
                rgb[0] = ColorChange.Next(0, 256);
                rgb[1] = ColorChange.Next(0, 256);
                rgb[2] = ColorChange.Next(0, 256);
                lock (LockEyeValues)
                {
                    setting.on = rgb[0] * 256 * 256 + rgb[1] * 256 + rgb[2];
                    setting.off = rgb[0] / 10 * 256 * 256 + rgb[1] / 10 * 256 + rgb[2] / 10;
                    try
                    {
                        lock (LockLeds)
                        {
                            ALLeds.post.fadeRGB("FaceLeds", setting.on, BlinkDuration);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLineIf(ShowAllThreats, "Error during randomeyes  " + ex.Message);
                    }
                }

                Thread.Sleep(random.Next(setting.RandomEyesTimes.MinTime, setting.RandomEyesTimes.MaxTime));
                Debug.WriteLineIf(ShowAllThreats, "bgw_eyecolor_DoWork");
            }
        }

        void bgw_blink_DoWork(object sender, DoWorkEventArgs e)
        {
            while (KeepNeutralRunning == true)
            {
                lock (LockLeds)
                {
                    Blink(BlinkDuration);

                }
                Thread.Sleep(random.Next(setting.BlinkEyesTimes.MinTime, setting.BlinkEyesTimes.MaxTime));
            }
        }

        void Blink(float rDuration)
        {
            int index = 0;
            try
            {

                lock (LockEyeValues)
                {
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.off, rDuration);
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.off, rDuration);
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.on, rDuration);
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.off, rDuration);
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.off, rDuration);
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.off, rDuration);
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.on, rDuration);
                    ALLeds.post.fadeRGB("FaceLed" + index++, setting.off, rDuration);
                    Thread.Sleep(100);
                    ALLeds.post.fadeRGB("FaceLeds", setting.on, rDuration);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLineIf(ShowAllThreats, "Error during blink " + index + " : " + ex.Message);
            }
        }
        #endregion

        #region Arms

        void bgw_arms_DoWork(object sender, DoWorkEventArgs e)
        {
            // Reading from NAO-memory takes much time!
            //RSHPitch = (float)(ALMem.getData("Device/SubDeviceList/LShoulderPitch/Position/Actuator/Value")) * FROM_RAD;
            //RSHRoll = (float)(ALMem.getData("Device/SubDeviceList/LShoulderRoll/Position/Actuator/Value")) * FROM_RAD;
            //RELYaw = (float)(ALMem.getData("Device/SubDeviceList/LElbowYaw/Position/Actuator/Value")) * FROM_RAD;
            //RELRoll = (float)(ALMem.getData("Device/SubDeviceList/LElbowRoll/Position/Actuator/Value")) * FROM_RAD;
            //RWrist = (float)(ALMem.getData("Device/SubDeviceList/LWristYaw/Position/Actuator/Value")) * FROM_RAD;

            //LSHPitch = (float)(ALMem.getData("Device/SubDeviceList/LShoulderPitch/Position/Actuator/Value")) * FROM_RAD;
            //LSHRoll = (float)(ALMem.getData("Device/SubDeviceList/LShoulderRoll/Position/Actuator/Value")) * FROM_RAD;
            //LELYaw = (float)(ALMem.getData("Device/SubDeviceList/LElbowYaw/Position/Actuator/Value")) * FROM_RAD;
            //LELRoll = (float)(ALMem.getData("Device/SubDeviceList/LElbowRoll/Position/Actuator/Value")) * FROM_RAD;
            //LWrist = (float)(ALMem.getData("Device/SubDeviceList/LWristYaw/Position/Actuator/Value")) * FROM_RAD;

            while (KeepNeutralRunning == true)
            {
                lock (LockArms)
                {
                    CalcArmMovement();
                }

                Thread.Sleep(random.Next(setting.MoveArmsTimes.MinTime, setting.MoveArmsTimes.MaxTime));//750, 1500));
                Debug.WriteLineIf(ShowAllThreats, "bgw_armsmoving_DoWork");
            }
        }

        //float calcChange(float min, float max, float value, float delta)
        //{
        //    float changeValue = 0.0F;

        //    if (Math.Abs(value + delta) > Math.Abs(min) && Math.Abs(value + delta) < Math.Abs(max))
        //        changeValue = value + delta;
        //    else
        //        changeValue = value - delta;

        //    if (Math.Abs(value) < Math.Abs(min))
        //        if (value > 0)
        //            changeValue = value + Math.Abs(delta);
        //        else
        //            changeValue = value - Math.Abs(delta);

        //    if (Math.Abs(value) > Math.Abs(max))
        //        if (value > 0)
        //            changeValue = value - Math.Abs(delta);
        //        else
        //            changeValue = value + Math.Abs(delta);
        //    return changeValue;
        //}

        void CalcArmMovement()
        {
            ArmDeltas = new float[6];
            ArmDeltas[1] = (float)((random.NextDouble() - 0.5) * setting.inc_arm_angle);
            ArmDeltas[2] = (float)((random.NextDouble() - 0.5) * setting.inc_arm_angle);
            ArmDeltas[3] = (float)((random.NextDouble() - 0.5) * setting.inc_arm_angle);
            ArmDeltas[4] = (float)((random.NextDouble() - 0.5) * setting.inc_arm_angle);
            ArmDeltas[5] = (float)((random.NextDouble() - 0.5) * setting.inc_arm_angle);

            double whicharm = random.NextDouble();
            DateTime start = DateTime.Now;
            if (whicharm > 0.5)
            {
                ArmDeltas[1] += setting.RSHPitch;
                ArmDeltas[2] += setting.RSHRoll;
                ArmDeltas[3] += setting.RELYaw;
                ArmDeltas[4] += setting.RELRoll;
                ArmDeltas[5] += setting.RWrist;
                ArmJoints = new string[] { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" };
            }
            else
            {
                ArmDeltas[1] += setting.LSHPitch;
                ArmDeltas[2] += setting.LSHRoll;
                ArmDeltas[3] += setting.LELYaw;
                ArmDeltas[4] += setting.LELRoll;
                ArmDeltas[5] += setting.LWrist;
                ArmJoints = new string[] { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" };
            }
            ArmsToNove = true;
            //Debug.WriteLine((DateTime.Now - start).Seconds + ":" + (DateTime.Now - start).Milliseconds);
        }

        float[] ArmDeltas;
        string[] ArmJoints;

        //void turn_left_arm(float shpitch, float shroll, float elyaw, float elroll, float wrist)
        //{
        //    string[] names = { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" };
        //    SetArmAngles(names, shpitch, shroll, elyaw, elroll, wrist);
        //}

        //void turn_right_arm(float shpitch, float shroll, float elyaw, float elroll, float wrist)
        //{
        //    string[] names = { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" };
        //    SetArmAngles(names, shpitch, shroll, elyaw, elroll, wrist);
        //}

        void SetArmAngles(string[] names, float shpitch, float shroll, float elyaw, float elroll, float wrist)
        {
            //float fractionMaxSpeed = 0.06F;
            float[] angles = { shpitch * TO_RAD, shroll * TO_RAD, 
                                 elyaw * TO_RAD, elroll * TO_RAD, wrist * TO_RAD };
            //ALMotion.setAngles(names, angles, fractionMaxSpeed);
            float[] time = { 1.0F, 1.0F, 1.0F, 1.0F, 1.0F };
            //ALMotion.setAngles(names, angles, fractionMaxSpeed);

            StartMotions(names, angles, time);
        }

        #endregion

        #region Hips

        void bgw_hipsmoving_DoWork(object sender, DoWorkEventArgs e)
        {
            Random side = new Random(Environment.TickCount);

            while (KeepNeutralRunning == true)
            {
                lock (LockHip)
                {
                    int s = side.Next(0, 2);
                    Debug.WriteLineIf(ShowAllThreats, "side " + s);
                    if (s == 0)
                    {
                        MoveKneesLeft();
                    }
                    else
                    {
                        MoveKneesRight();
                    }
                    HipsToMove = true;
                }

                Thread.Sleep(random.Next(setting.MoveHipsTimes.MinTime, setting.MoveHipsTimes.MaxTime));
                Debug.WriteLineIf(ShowAllThreats, "bgw_hipsmoving_DoWork");
            }
        }

        private void MoveKneesRight()
        {
            List<float> time = new List<float>();
            HipJointNames = new List<string>();

            time = new List<float> { 0.80000F, 1.3000F, 1.80000F };

            tlist = new List<List<float>>();
            klist = new List<List<float>>();

            klist.Add(new List<float> {  0.08893F,  0.06665F, 0.08893F });
            klist.Add(new List<float> {  0.00004F,  0.02886F, 0.00004F });
            klist.Add(new List<float> {  0.07061F,  0.09332F, 0.07061F });
            klist.Add(new List<float> { -0.00609F, -0.02907F, -0.00609F });
            klist.Add(new List<float> {  0.00311F,  0.01078F, 0.00311F });
            klist.Add(new List<float> { -0.09233F, -0.09233F, -0.09233F });
            klist.Add(new List<float> {  0.09362F,  0.03351F, 0.09362F });
            klist.Add(new List<float> {  0.00004F,  0.01879F, 0.00004F });
            klist.Add(new List<float> {  0.05211F,  0.03363F, 0.05211F });
            klist.Add(new List<float> { -0.00149F, -0.02588F, -0.00149F });
            klist.Add(new List<float> {  0.00311F,  0.01078F, 0.00311F });
            klist.Add(new List<float> { -0.09233F, -0.00060F, -0.09233F });

            HipJointNames.Add("LAnklePitch");
            HipJointNames.Add("LAnkleRoll");
            HipJointNames.Add("LHipPitch");
            HipJointNames.Add("LHipRoll");
            HipJointNames.Add("LHipYawPitch");
            HipJointNames.Add("LKneePitch");
            HipJointNames.Add("RAnklePitch");
            HipJointNames.Add("RAnkleRoll");
            HipJointNames.Add("RHipPitch");
            HipJointNames.Add("RHipRoll");
            HipJointNames.Add("RHipYawPitch");
            HipJointNames.Add("RKneePitch");
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
        }

        private void MoveKneesLeft()
        {

            List<float> time = new List<float>();
            HipJointNames = new List<string>();

            time = new List<float> { 0.80000F, 1.3000F, 1.80000F };

            tlist = new List<List<float>>();
            klist = new List<List<float>>();

            klist.Add(new List<float> {  0.08893F,  0.03351F, 0.08893F });
            klist.Add(new List<float> {  0.00004F, -0.01879F, 0.00004F });
            klist.Add(new List<float> {  0.07061F,  0.03363F, 0.07061F });
            klist.Add(new List<float> { -0.00609F,  0.02588F, -0.00609F });
            klist.Add(new List<float> {  0.00311F,  0.01078F, 0.00311F });
            klist.Add(new List<float> { -0.09233F, -0.00060F, -0.09233F });
            klist.Add(new List<float> {  0.09362F,  0.06665F, 0.09362F });
            klist.Add(new List<float> {  0.00004F, -0.02886F, 0.00004F });
            klist.Add(new List<float> {  0.05211F,  0.09332F, 0.05211F });
            klist.Add(new List<float> { -0.00149F,  0.02907F, -0.00149F });
            klist.Add(new List<float> {  0.00311F,  0.01078F, 0.00311F });
            klist.Add(new List<float> { -0.09233F, -0.09233F, -0.09233F });

            HipJointNames.Add("LAnklePitch");
            HipJointNames.Add("LAnkleRoll");
            HipJointNames.Add("LHipPitch");
            HipJointNames.Add("LHipRoll");
            HipJointNames.Add("LHipYawPitch");
            HipJointNames.Add("LKneePitch");
            HipJointNames.Add("RAnklePitch");
            HipJointNames.Add("RAnkleRoll");
            HipJointNames.Add("RHipPitch");
            HipJointNames.Add("RHipRoll");
            HipJointNames.Add("RHipYawPitch");
            HipJointNames.Add("RKneePitch");
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
        }

        //private void MoveKneesRight()
        //{
        //    List<float> time = new List<float>();
        //    HipJointNames = new List<string>();

        //    time = new List<float> { 0.50000F, 1.0000F, 1.50000F };

        //    tlist = new List<List<float>>();
        //    klist = new List<List<float>>();

        //    HipJointNames.Add("LAnklePitch");
        //    klist.Add(new List<float> { 0.08893F, -0.00609F, 0.08893F });
        //    tlist.Add(time);

        //    HipJointNames.Add("LAnkleRoll");
        //    klist.Add(new List<float> { 0.00004F, -0.03686F, 0.00004F });
        //    tlist.Add(time);

        //    HipJointNames.Add("LHipPitch");
        //    klist.Add(new List<float> { 0.07061F, 0.01070F, 0.07061F });
        //    tlist.Add(time);

        //    HipJointNames.Add("LHipRoll");
        //    klist.Add(new List<float> { -0.00609F, 0.04138F, -0.00609F });
        //    tlist.Add(time);

        //    HipJointNames.Add("LHipYawPitch");
        //    klist.Add(new List<float> { 0.00311F, 0.01078F, 0.00311F });
        //    tlist.Add(time);

        //    HipJointNames.Add("LKneePitch");
        //    klist.Add(new List<float> { -0.09233F, 0.03993F, -0.09233F });
        //    tlist.Add(time);

        //    HipJointNames.Add("RAnklePitch");
        //    klist.Add(new List<float> { 0.09362F, 0.03677F, 0.09362F });
        //    tlist.Add(time);

        //    HipJointNames.Add("RAnkleRoll");
        //    klist.Add(new List<float> { 0.00004F, -0.05527F, 0.00004F });
        //    tlist.Add(time);

        //    HipJointNames.Add("RHipPitch");
        //    klist.Add(new List<float> { 0.05211F, 0.12736F, 0.05211F });
        //    tlist.Add(time);

        //    HipJointNames.Add("RHipRoll");
        //    klist.Add(new List<float> { -0.00149F, 0.05825F, -0.00149F });
        //    tlist.Add(time);

        //    HipJointNames.Add("RHipYawPitch");
        //    klist.Add(new List<float> { 0.00311F, 0.01078F, 0.00311F });
        //    tlist.Add(time);

        //    HipJointNames.Add("RKneePitch");
        //    klist.Add(new List<float> { -0.09233F, -0.09233F, -0.09233F });
        //    tlist.Add(time);

        //}

        //private void MoveKneesLeft()
        //{

        //    List<float> time = new List<float>();
        //    HipJointNames = new List<string>();

        //    time = new List<float> { 0.50000F, 1.0000F, 1.50000F};

        //    tlist = new List<List<float>>();
        //    klist = new List<List<float>>();

        //    HipJointNames.Add("LAnklePitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.09362F, 0.03677F, 0.09362F });

        //    HipJointNames.Add("LAnkleRoll");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { -0.00004F, 0.05527F, -0.00004F });

        //    HipJointNames.Add("LHipPitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.05211F, 0.12736F, 0.05211F });

        //    HipJointNames.Add("LHipRoll");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.00149F, -0.05825F, 0.00149F });

        //    HipJointNames.Add("LHipYawPitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.00311F, 0.01078F, 0.00311F });

        //    HipJointNames.Add("LKneePitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { -0.09233F, -0.09233F, -0.09233F });

        //    HipJointNames.Add("RAnklePitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.08893F, -0.00609F, 0.08893F });

        //    HipJointNames.Add("RAnkleRoll");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { -0.00004F, 0.03686F, -0.00004F });

        //    HipJointNames.Add("RHipPitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.07061F, 0.01070F, 0.07061F });

        //    HipJointNames.Add("RHipRoll");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.00609F, -0.04138F, 0.00609F });

        //    HipJointNames.Add("RHipYawPitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { 0.00311F, 0.01078F, 0.00311F });

        //    HipJointNames.Add("RKneePitch");
        //    tlist.Add(time);
        //    klist.Add(new List<float> { -0.09233F, 0.03993F, -0.09233F });

        //}
        // Choregraphe simplified export in c++.
        private void MoveToRight()
        {
            List<float> time = new List<float>();
            List<float> keys = new List<float>();
            HipJointNames = new List<string>();

            time.Add(0.5000F);
            time.Add(1.00000F);
            time.Add(1.40000F);

            tlist = new List<List<float>>();
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);

            klist.Clear();
            HipJointNames.Add("LAnkleRoll");
            keys = new List<float>();
            keys.Add(-0);
            keys.Add(-0.0460252F);
            keys.Add(-0);
            klist.Add(keys);

            HipJointNames.Add("LHipRoll");
            keys = new List<float>();
            keys.Add(0);
            keys.Add(0.041F);
            keys.Add(0);
            klist.Add(keys);

            HipJointNames.Add("RAnkleRoll");
            keys = new List<float>();
            keys.Add(0);
            keys.Add(-0.0580804F);
            keys.Add(0);
            klist.Add(keys);

            HipJointNames.Add("RHipRoll");
            keys = new List<float>();
            keys.Add(-0);
            keys.Add(0.048F);
            keys.Add(-0);
            klist.Add(keys);
        }

        private void MoveToLeft()
        {
            List<float> time = new List<float>();
            List<float> keys = new List<float>();
            HipJointNames = new List<string>();

            time.Add(0.5000F);
            time.Add(1.00000F);
            time.Add(1.40000F);

            tlist = new List<List<float>>();
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);
            tlist.Add(time);

            klist.Clear();

            HipJointNames.Add("LAnkleRoll");
            keys = new List<float>();
            keys.Add(-0);
            keys.Add(0.0580804F);
            keys.Add(-0);
            klist.Add(keys);

            HipJointNames.Add("LHipRoll");
            keys = new List<float>();
            keys.Add(0.00F);
            keys.Add(-0.0473279F);
            keys.Add(0.00F);
            klist.Add(keys);

            HipJointNames.Add("RAnkleRoll");
            keys = new List<float>();
            keys.Add(0);
            keys.Add(0.0460252F);
            keys.Add(0);
            klist.Add(keys);

            HipJointNames.Add("RHipRoll");
            keys = new List<float>();
            keys.Add(-0);
            keys.Add(-0.0410F);
            keys.Add(-0);
            klist.Add(keys);

        }
        #endregion
    }
}