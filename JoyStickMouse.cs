using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace JoyStickMouse
{
    class JoyStickMouse : Form
    {
        [DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void SetCursorPos(int x, int y);

        [DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public JoyStickMouse()
        {
            this.joyStick = new JoyStick();

            this.BackColor = this.back;
            this.Text = "ジョイスティックマウス";
            this.Width = 640;
            this.Height = 480;
            this.Icon = new Icon("icon.ico");

            Timer timer = new Timer();
            timer.Interval = 15;
            timer.Tick += (sender, e) => this.Update();
            timer.Start();
        }

        public new void Update()
        {
            // 更新する
            this.joyStick.Update();
            if (this.joyStick.IsAvailable())
            {
                // カーソルを移動する
                int x = Cursor.Position.X + (int)(DefaultSpeed * this.joyStick.AxisX);
                int y = Cursor.Position.Y + (int)(DefaultSpeed * this.joyStick.AxisY);
                SetCursorPos(x, y);

                // ホイールを動かす
                int delta = (int)(-this.joyStick.SubAxisY * WHEEL_DELTA);
                mouse_event(MOUSEEVENTF_WHEEL, 0, 0, delta, 0);

                // カーソルキーを押す
                if (this.joyStick.IsPressed(JoyStick.POV.Left))
                {
                    keybd_event(VK_LEFT, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                }
                else if (this.joyStick.IsReleased(JoyStick.POV.Left))
                {
                    keybd_event(VK_LEFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                if (this.joyStick.IsPressed(JoyStick.POV.Up))
                {
                    keybd_event(VK_UP, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                }
                else if (this.joyStick.IsReleased(JoyStick.POV.Up))
                {
                    keybd_event(VK_UP, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                if (this.joyStick.IsPressed(JoyStick.POV.Right))
                {
                    keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                }
                else if (this.joyStick.IsReleased(JoyStick.POV.Right))
                {
                    keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                if (this.joyStick.IsPressed(JoyStick.POV.Down))
                {
                    keybd_event(VK_DOWN, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                }
                else if (this.joyStick.IsReleased(JoyStick.POV.Down))
                {
                    keybd_event(VK_DOWN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                // クリックイベントを発生させる
                if (this.joyStick.IsPressed((int)Buttons.MouseLeft))
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                }
                else if (this.joyStick.IsReleased((int)Buttons.MouseLeft))
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                }

                if (this.joyStick.IsPressed((int)Buttons.MouseRight))
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
                }
                else if (this.joyStick.IsReleased((int)Buttons.MouseRight))
                {
                    mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
                }

                if (this.joyStick.IsPressed((int)Buttons.MouseMiddle))
                {
                    mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                }
                else if (this.joyStick.IsReleased((int)Buttons.MouseMiddle))
                {
                    mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                }

                // Enterを押す
                if (this.joyStick.IsPressed((int)Buttons.Enter))
                {
                    keybd_event(VK_RETURN, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                }
                else if (this.joyStick.IsReleased((int)Buttons.Enter))
                {
                    keybd_event(VK_RETURN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                // 縮小する
                if (this.joyStick.IsPressed((int)Buttons.ZoomOut))
                {
                    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_SUBTRACT, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_SUBTRACT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                // 拡大する
                if (this.joyStick.IsPressed((int)Buttons.ZoomIn))
                {
                    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_ADD, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_ADD, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                // もどる
                if (this.joyStick.IsPressed((int)Buttons.Back))
                {
                    keybd_event(VK_MENU, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_LEFT, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_LEFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                // すすむ
                if (this.joyStick.IsPressed((int)Buttons.Next))
                {
                    keybd_event(VK_MENU, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
                    keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }

                // アプリケーションを終了する
                if (this.joyStick.IsPressed((int)Buttons.Close))
                {
                    MessageBox.Show("アプリケーションを終了します");
                    this.Close();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.FillRectangle(this.brush, this.ClientRectangle);
            String message =
                "○\t左クリック\n" +
                "×\t右クリック\n" +
                "△\t中クリック\n" +
                "□\tEnter\n" +
                "L\tもどる\n" +
                "R\tすすむ\n" +
                "L2\t縮小\n" +
                "R2\t拡大\n" +
                "アナログスティック(左)\tカーソル移動\n" +
                "アナログスティック(右)\tホイール\n" +
                "POVスイッチ\t矢印キー\n" +
                "Select\tアプリケーション終了\n" +
                "\n";
            graphics.DrawString(message, SystemFonts.CaptionFont, Brushes.WhiteSmoke, 24, 24);
        }

        private enum Buttons : uint
        {
            MouseLeft = 1,      // ○
            MouseRight = 2,     // ×
            MouseMiddle = 0,    // △
            Enter = 3,          // Enter
            Back = 6,           // L
            Next = 7,           // R
            ZoomOut = 4,        // L2
            ZoomIn = 5,         // R2
            Close = 9,          // Select
        }

        private JoyStick joyStick;

        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int WHEEL_DELTA = 120;
        private const int KEYEVENTF_KEYDOWN = 0x0;
        private const int KEYEVENTF_KEYUP = 0x2;
        private const byte VK_MENU = 0x12;
        private const byte VK_LEFT = 0x25;
        private const byte VK_UP = 0x26;
        private const byte VK_RIGHT = 0x27;
        private const byte VK_DOWN = 0x28;
        private const byte VK_RETURN = 0x0d;
        private const byte VK_CONTROL = 0x11;
        private const byte VK_ADD = 0x6B;
        private const byte VK_SUBTRACT = 0x6D;
        private const double HighSpeed = 40.0;
        private const double DefaultSpeed = 15.0;

        private readonly Color back = Color.FromArgb(32, 32, 64);
        private readonly Font font = new Font("Meiryo UI", 16);
        private readonly Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, 480), Color.Black, Color.LightSlateGray);
    }
}
