using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace ConnectorNao
{
    [Serializable()]
    public class MovingSettings
    {
        // NOTE: DATA ARE READ FROM THE FILE MOVINGSETTINGS.XML IF PRESENT!
        //       VALUES OF ANGLES ARE IN DEGRESS

        // ENUM to select which movements are to be executed
        public byte MoveHead = 1;
        public byte BlinkEyes = 2;
        public byte MoveArms = 4;
        public byte MoveHands = 8;
        public byte MoveHips = 16;
        public byte RandomEyes = 32;

        public byte WhatToMove = 0;

        public struct MinMaxTime
        {
            public int MinTime;
            public int MaxTime;
        }

        public MinMaxTime BlinkEyesTimes = new MinMaxTime();
        public MinMaxTime MoveHipsTimes = new MinMaxTime();
        public MinMaxTime MoveHandsTimes = new MinMaxTime();
        public MinMaxTime MoveArmsTimes = new MinMaxTime();
        public MinMaxTime MoveHeadTimes = new MinMaxTime();
        public MinMaxTime RandomEyesTimes = new MinMaxTime();

        // variables for the movement of the arms
        public float RSHPitch = 65;
        public float RSHRoll = 0;
        public float RELYaw = 27;
        public float RELRoll = 20;
        public float RWrist = 80;
        public float LSHPitch = 65;
        public float LSHRoll = 0;
        public float LELYaw = 7;
        public float LELRoll = -20;
        public float LWrist = -80;

        public float inc_arm_angle = 10;

        // Variables for the movement of the head
        public float min_head_yaw = -10;
        public float max_head_yaw = 10;
        public float max_head_pitch = -20;
        public float min_head_pitch = -5;

        public int inc_head_yaw = 10;
        public int inc_head_pitch = 10;

        // Variables for the hands
        public float RHand_min_value = 0.0F;
        public float RHand_max_value = 0.3F;
        public float LHand_min_value = 0.2F;
        public float LHand_max_value = 0.6F;
        public float inc_hand_value = 0.3F;

        // Colors of the eyes in on and off state
        public int on = Convert.ToInt32("0xffffff", 16);
        public int off = Convert.ToInt32("0x222222", 16);

        public void WriteSettings(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(MovingSettings));

            StreamWriter wr = new StreamWriter(filename);
            xs.Serialize(wr, this);
            wr.Close();
        }


        public static MovingSettings Load(string filename)
        {
            //string filename = ModelDirectory + subjectname + ".BasicData.xml";
            if (File.Exists(filename) == true)
            {
                using (StreamReader sr = new StreamReader(filename, Encoding.UTF8))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(MovingSettings));
                    return (MovingSettings)xs.Deserialize(sr.BaseStream);
                }
            }
            else
            {
                Debug.WriteLine("Can't find " + filename + ". Use defaults instead");
                MovingSettings current = new MovingSettings();
                return current;
            }
        }


    }
}
