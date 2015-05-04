using System;
using System.Runtime.InteropServices;

namespace JoyStickMouse
{
    class JoyStick
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFOEX
        {
            public uint dwSize;
            public uint dwFlags;
            public uint dwXpos;
            public uint dwYpos;
            public uint dwZpos;
            public uint dwRpos;
            public uint dwUpos;
            public uint dwVpos;
            public uint dwButtons;
            public uint dwButtonNumber;
            public uint dwPOV;
            public uint dwReserved1;
            public uint dwReserved2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MMRESULT
        {
            public uint uJoyID;
        }

        [DllImport("winmm.dll"), System.Security.SuppressUnmanagedCodeSecurity]
        public static extern MMRESULT joyGetPosEx(uint uJoyID, ref JOYINFOEX pjiex);

        public JoyStick()
        {
            this.info.dwSize = (uint)Marshal.SizeOf(this.info);
            this.info.dwFlags = JOY_RETURNALL;
            this.DeadZone = 0.25;
            this.Update();
        }

        public bool IsAvailable()
        {
            return this.isAvailable;
        }

        public void Update()
        {
            this.prevButtons = this.info.dwButtons;
            this.prevPOV = this.info.dwPOV;
            var result = joyGetPosEx(0, ref this.info);
            if (result.uJoyID == 0)
            {
                for (int i = 0; i < this.pushingTime.Length; i++)
                {
                    if (this.isPressed(i))
                    {
                        this.pushingTime[i]++;
                    }
                    else
                    {
                        this.pushingTime[i] = 0;
                    }
                }
                this.isAvailable = true;
            }
            else
            {
                this.Reset();
                this.isAvailable = false;
            }
        }

        public void Reset()
        {
            this.info.dwButtonNumber = 0;
            this.info.dwButtons = 0;
            this.info.dwPOV = 0;
            this.info.dwXpos = 0;
            this.info.dwYpos = 0;
            this.info.dwZpos = 0;
            this.info.dwRpos = 0;
            this.prevButtons = 0;
            this.prevPOV = 0;
            this.pushingTime.Initialize();
        }

        public double AxisX
        {
            get
            {
                return formatAxis(this.info.dwXpos);
            }
        }

        public double AxisY
        {
            get
            {
                return formatAxis(this.info.dwYpos);
            }
        }

        public double SubAxisX
        {
            get
            {
                return formatAxis(this.info.dwZpos);
            }
        }

        public double SubAxisY
        {
            get
            {
                return formatAxis(this.info.dwRpos);
            }
        }

        public bool IsPressed(int buttonIndex)
        {
            bool isPushNow = ((this.info.dwButtons >> buttonIndex) & 1) != 0;
            bool isPushPrev = ((prevButtons >> buttonIndex) & 1) != 0;
            return isPushNow & !isPushPrev;
        }

        public bool IsReleased(int buttonIndex)
        {
            bool isPushNow = ((this.info.dwButtons >> buttonIndex) & 1) != 0;
            bool isPushPrev = ((prevButtons >> buttonIndex) & 1) != 0;
            return !isPushNow && isPushPrev;
        }

        public bool IsPressing(int buttonIndex)
        {
            return this.pushingTime[buttonIndex] > PushingTimeThreshold;
        }

        public bool IsPressed(POV witch)
        {
            uint prev = this.prevPOV / 100;
            uint now = this.info.dwPOV / 100;

            switch (witch)
            {
                case POV.Left:
                    return (now == 270) && (prev != 270);
                case POV.Up:
                    return (now == 0) && (prev != 0);
                case POV.Right:
                    return (now == 90) && (prev != 90);
                case POV.Down:
                    return (now == 180) && (prev != 180);
                default:
                    return false;
            }
        }

        public bool IsReleased(POV witch)
        {
            uint prev = this.prevPOV / 100;
            uint now = this.info.dwPOV / 100;

            switch (witch)
            {
                case POV.Left:
                    return (now != 270) && (prev == 270);
                case POV.Up:
                    return (now != 0) && (prev == 0);
                case POV.Right:
                    return (now != 90) && (prev == 90);
                case POV.Down:
                    return (now != 180) && (prev == 180);
                default:
                    return false;
            }
        }

        public double DeadZone
        {
            set;
            get;
        }

        public enum POV
        {
            Left, Up, Right, Down
        }

        private double formatAxis(uint _value)
        {
            double value = (double)((int)_value - NeutralPosition) / (double)(NeutralPosition);
            if (Math.Abs(value) > this.DeadZone)
            {
                if (value > 0)
                    return 1.0 / (1.0 - this.DeadZone) * (value - this.DeadZone);
                else
                    return 1.0 / (1.0 - this.DeadZone) * (value + this.DeadZone);
            }
            else
            {
                return 0;
            }
        }

        private bool isPressed(int buttonIndex)
        {
            return ((this.info.dwButtons >> buttonIndex) & 1) != 0;
        }

        private JOYINFOEX info = new JOYINFOEX();
        private uint prevButtons;
        private uint prevPOV;
        private int[] pushingTime = new int[32];
        private bool isAvailable;

        private const uint JOY_RETURNALL = 255;

        private const int PushingTimeThreshold = 15;
        private const int NeutralPosition = 32767;
    }
}
