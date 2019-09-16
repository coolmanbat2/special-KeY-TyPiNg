using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace HelloWorldApp
{ 

    public partial class Form1 : Form
    {
        const byte VK_SHIFT = 0x0010;
        private static IntPtr _hookID = IntPtr.Zero;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int PRESS_KEYDOWN = 0;
        private const int KEYEVENTF_KEYUP = 2;
        private const int KEYEVENTF_EXTENDEDKEY = 0XE0;
        private static bool injectedShift = false;

        // Initializers
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        

        /// <summary>
        /// main function for "weird typing"
        /// 
        /// </summary>
        /// <param name="nCode"> this variable processes the message dependent on
        /// what value it gives; i.e. nCode >= message processed; else, message not processed.</param>
        /// <param name="wParam">the type of input that is sent to the computer. 
        /// i.e. WM_KEYDOWN (keypressed), WM_KEYUP (key not pressed),
        /// WM_SYSKEYDOWN (key injected press), WM_SYSKEYUP (key injected unpress)</param>
        /// <param name="lParam"> where the type of key that is pressed is presented.</param>
        /// <returns></returns>
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            
            // if a key (type letter) is pressed then, 
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                // if the key pressed is a letter and not a symbol then,
                if (vkCode >= 65 && vkCode <= 90 ||
                    vkCode >= 97 && vkCode <= 122)
                {
                    // If the shift key has been pressed then, 
                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        // unpress the key? lol
                        keybd_event(VK_SHIFT, 0x2a, KEYEVENTF_KEYUP, 0);
                        injectedShift = false;
                    }
                    else
                    {
                        // otherwise, press it.
                        keybd_event(VK_SHIFT, 0x2a, PRESS_KEYDOWN, 0);
                        injectedShift = true;

                    }
                }
                // otherwise if the shift key is pressed then,
                else if (injectedShift)
                {
                    // needs to be one more check here. 
                    // in which the program will "see" whether a typed character is not a non-letter.

                    // needs to be something that differentiates
                    // a normal person pressing shift versus an 
                    // injected version of it. 
                    // if a key is pressed then,
                    if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                    {
                        // turn off the shift key so we can type normally.
                        keybd_event(VK_SHIFT, 0xaa, KEYEVENTF_KEYUP, 0);
                        injectedShift = false;
                        // need some command that resets the shift key.
                    }
                }
            }
                // need to research what this does. 
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        public Form1()
        {
            InitializeComponent();
        }

 
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            // Sets up the hook for keyboard.
            // initilaize the keyboard map.
            Process curProcess = Process.GetCurrentProcess();
            ProcessModule curModule = curProcess.MainModule;
            _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, GetModuleHandle(curModule.ModuleName), 0);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // if the shift key is pressed when closing; "unpress" it.
            if (Control.ModifierKeys == Keys.Shift) 
            {
                keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, 0);
            }
            UnhookWindowsHookEx(_hookID);
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }
    }
}
