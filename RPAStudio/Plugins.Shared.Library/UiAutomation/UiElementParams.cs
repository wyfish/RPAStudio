namespace Plugins.Shared.Library.UiAutomation
{
    public enum ClickType
    {
        Single,
        Double,
        Down,
        Up,
    }

    public enum MouseButton
    {
        /// <summary>
        /// The left mouse button
        /// </summary>
        Left,

        /// <summary>
        /// The middle mouse button
        /// </summary>
        Middle,

        /// <summary>
        /// The right mouse button
        /// </summary>
        Right,

        /// <summary>
        /// The fourth mouse button
        /// </summary>
        XButton1,

        /// <summary>
        /// The fifth mouse button
        /// </summary>
        XButton2
    }

    public enum VirtualKey : ushort
    {
        //
        // 摘要:
        //     Left mouse button
        LBUTTON = 1,
        //
        // 摘要:
        //     Right mouse button
        RBUTTON = 2,
        //
        // 摘要:
        //     Control-break processing
        CANCEL = 3,
        //
        // 摘要:
        //     Middle mouse button (three-button mouse)
        MBUTTON = 4,
        //
        // 摘要:
        //     Windows 2000/XP: X1 mouse button
        XBUTTON1 = 5,
        //
        // 摘要:
        //     Windows 2000/XP: X2 mouse button
        XBUTTON2 = 6,
        //
        // 摘要:
        //     BACKSPACE key
        BACK = 8,
        //
        // 摘要:
        //     TAB key
        TAB = 9,
        //
        // 摘要:
        //     CLEAR key
        CLEAR = 12,
        //
        // 摘要:
        //     ENTER key
        ENTER = 13,
        RERETURN = 13,
        //
        // 摘要:
        //     SHIFT key
        SHIFT = 16,
        //
        // 摘要:
        //     CTRL key
        CONTROL = 17,
        //
        // 摘要:
        //     ALT key
        ALT = 18,
        //
        // 摘要:
        //     PAUSE key
        PAUSE = 19,
        //
        // 摘要:
        //     CAPS LOCK key
       // CAPITAL = 20,
        CAPSLOCK = 20,
        //
        // 摘要:
        //     Input Method Editor (IME) Kana mode
        KANA = 21,
        //
        // 摘要:
        //     IME Hangul mode
        HANGUL = 21,
        //
        // 摘要:
        //     IME Junja mode
        JUNJA = 23,
        //
        // 摘要:
        //     IME final mode
        FINAL = 24,
        //
        // 摘要:
        //     IME Hanja mode
        HANJA = 25,
        //
        // 摘要:
        //     IME Kanji mode
        KANJI = 25,
        //
        // 摘要:
        //     ESC key
        //ESCAPE = 27,
        ESC = 27,
        //
        // 摘要:
        //     IME convert
        CONVERT = 28,
        //
        // 摘要:
        //     IME nonconvert
        NONCONVERT = 29,
        //
        // 摘要:
        //     IME accept
        ACCEPT = 30,
        //
        // 摘要:
        //     IME mode change request
        MODECHANGE = 31,
        //
        // 摘要:
        //     SPACEBAR
        SPACE = 32,
        //
        // 摘要:
        //     PAGE UP key
        PAGE_UP = 33,
        //
        // 摘要:
        //     PAGE DOWN key
        PAGE_DOWN = 34,
        //
        // 摘要:
        //     END key
        END = 35,
        //
        // 摘要:
        //     HOME key
        HOME = 36,
        //
        // 摘要:
        //     LEFT ARROW key
        LEFT = 37,
        //
        // 摘要:
        //     UP ARROW key
        UP = 38,
        //
        // 摘要:
        //     RIGHT ARROW key
        RIGHT = 39,
        //
        // 摘要:
        //     DOWN ARROW key
        DOWN = 40,
        //
        // 摘要:
        //     SELECT key
        SELECT = 41,
        //
        // 摘要:
        //     PRINT key
        PRINT = 42,
        //
        // 摘要:
        //     EXECUTE key
        EXECUTE = 43,
        //
        // 摘要:
        //     PRINT SCREEN key
        SNAPSHOT = 44,
        //
        // 摘要:
        //     INS key
        INSERT = 45,
        //
        // 摘要:
        //     DEL key
        DELETE = 46,
        //
        // 摘要:
        //     HELP key
        HELP = 47,
        //
        // 摘要:
        //     0 key
        KEY_0 = 48,
        //
        // 摘要:
        //     1 key
        KEY_1 = 49,
        //
        // 摘要:
        //     2 key
        KEY_2 = 50,
        //
        // 摘要:
        //     3 key
        KEY_3 = 51,
        //
        // 摘要:
        //     4 key
        KEY_4 = 52,
        //
        // 摘要:
        //     5 key
        KEY_5 = 53,
        //
        // 摘要:
        //     6 key
        KEY_6 = 54,
        //
        // 摘要:
        //     7 key
        KEY_7 = 55,
        //
        // 摘要:
        //     8 key
        KEY_8 = 56,
        //
        // 摘要:
        //     9 key
        KEY_9 = 57,
        //
        // 摘要:
        //     A key
        KEY_A = 65,
        //
        // 摘要:
        //     B key
        KEY_B = 66,
        //
        // 摘要:
        //     C key
        KEY_C = 67,
        //
        // 摘要:
        //     D key
        KEY_D = 68,
        //
        // 摘要:
        //     E key
        KEY_E = 69,
        //
        // 摘要:
        //     F key
        KEY_F = 70,
        //
        // 摘要:
        //     G key
        KEY_G = 71,
        //
        // 摘要:
        //     H key
        KEY_H = 72,
        //
        // 摘要:
        //     I key
        KEY_I = 73,
        //
        // 摘要:
        //     J key
        KEY_J = 74,
        //
        // 摘要:
        //     K key
        KEY_K = 75,
        //
        // 摘要:
        //     L key
        KEY_L = 76,
        //
        // 摘要:
        //     M key
        KEY_M = 77,
        //
        // 摘要:
        //     N key
        KEY_N = 78,
        //
        // 摘要:
        //     O key
        KEY_O = 79,
        //
        // 摘要:
        //     P key
        KEY_P = 80,
        //
        // 摘要:
        //     Q key
        KEY_Q = 81,
        //
        // 摘要:
        //     R key
        KEY_R = 82,
        //
        // 摘要:
        //     S key
        KEY_S = 83,
        //
        // 摘要:
        //     T key
        KEY_T = 84,
        //
        // 摘要:
        //     U key
        KEY_U = 85,
        //
        // 摘要:
        //     V key
        KEY_V = 86,
        //
        // 摘要:
        //     W key
        KEY_W = 87,
        //
        // 摘要:
        //     X key
        KEY_X = 88,
        //
        // 摘要:
        //     Y key
        KEY_Y = 89,
        //
        // 摘要:
        //     Z key
        KEY_Z = 90,
        //
        // 摘要:
        //     Left Windows key (Microsoft Natural keyboard)
        LWIN = 91,
        //
        // 摘要:
        //     Right Windows key (Natural keyboard)
        RWIN = 92,
        //
        // 摘要:
        //     Applications key (Natural keyboard)
        APPS = 93,
        //
        // 摘要:
        //     Computer Sleep key
        SLEEP = 95,
        //
        // 摘要:
        //     Numeric keypad 0 key
        NUMPAD0 = 96,
        //
        // 摘要:
        //     Numeric keypad 1 key
        NUMPAD1 = 97,
        //
        // 摘要:
        //     Numeric keypad 2 key
        NUMPAD2 = 98,
        //
        // 摘要:
        //     Numeric keypad 3 key
        NUMPAD3 = 99,
        //
        // 摘要:
        //     Numeric keypad 4 key
        NUMPAD4 = 100,
        //
        // 摘要:
        //     Numeric keypad 5 key
        NUMPAD5 = 101,
        //
        // 摘要:
        //     Numeric keypad 6 key
        NUMPAD6 = 102,
        //
        // 摘要:
        //     Numeric keypad 7 key
        NUMPAD7 = 103,
        //
        // 摘要:
        //     Numeric keypad 8 key
        NUMPAD8 = 104,
        //
        // 摘要:
        //     Numeric keypad 9 key
        NUMPAD9 = 105,
        //
        // 摘要:
        //     Multiply key
        MULTIPLY = 106,
        //
        // 摘要:
        //     Add key
        ADD = 107,
        //
        // 摘要:
        //     Separator key
        SEPARATOR = 108,
        //
        // 摘要:
        //     Subtract key
        SUBTRACT = 109,
        //
        // 摘要:
        //     Decimal key
        DECIMAL = 110,
        //
        // 摘要:
        //     Divide key
        DIVIDE = 111,
        //
        // 摘要:
        //     F1 key
        F1 = 112,
        //
        // 摘要:
        //     F2 key
        F2 = 113,
        //
        // 摘要:
        //     F3 key
        F3 = 114,
        //
        // 摘要:
        //     F4 key
        F4 = 115,
        //
        // 摘要:
        //     F5 key
        F5 = 116,
        //
        // 摘要:
        //     F6 key
        F6 = 117,
        //
        // 摘要:
        //     F7 key
        F7 = 118,
        //
        // 摘要:
        //     F8 key
        F8 = 119,
        //
        // 摘要:
        //     F9 key
        F9 = 120,
        //
        // 摘要:
        //     F10 key
        F10 = 121,
        //
        // 摘要:
        //     F11 key
        F11 = 122,
        //
        // 摘要:
        //     F12 key
        F12 = 123,
        //
        // 摘要:
        //     F13 key
        F13 = 124,
        //
        // 摘要:
        //     F14 key
        F14 = 125,
        //
        // 摘要:
        //     F15 key
        F15 = 126,
        //
        // 摘要:
        //     F16 key
        F16 = 127,
        //
        // 摘要:
        //     F17 key
        F17 = 128,
        //
        // 摘要:
        //     F18 key
        F18 = 129,
        //
        // 摘要:
        //     F19 key
        F19 = 130,
        //
        // 摘要:
        //     F20 key
        F20 = 131,
        //
        // 摘要:
        //     F21 key
        F21 = 132,
        //
        // 摘要:
        //     F22 key, (PPC only) Key used to lock device.
        F22 = 133,
        //
        // 摘要:
        //     F23 key
        F23 = 134,
        //
        // 摘要:
        //     F24 key
        F24 = 135,
        //
        // 摘要:
        //     NUM LOCK key
        NUMLOCK = 144,
        //
        // 摘要:
        //     SCROLL LOCK key
        SCROLL = 145,
        //
        // 摘要:
        //     Left SHIFT key
        LSHIFT = 160,
        //
        // 摘要:
        //     Right SHIFT key
        RSHIFT = 161,
        //
        // 摘要:
        //     Left CONTROL key
        LCONTROL = 162,
        //
        // 摘要:
        //     Right CONTROL key
        RCONTROL = 163,
        //
        // 摘要:
        //     Left MENU key
        LMENU = 164,
        //
        // 摘要:
        //     Right MENU key
        RMENU = 165,
        //
        // 摘要:
        //     Windows 2000/XP: Browser Back key
        BROWSER_BACK = 166,
        //
        // 摘要:
        //     Windows 2000/XP: Browser Forward key
        BROWSER_FORWARD = 167,
        //
        // 摘要:
        //     Windows 2000/XP: Browser Refresh key
        BROWSER_REFRESH = 168,
        //
        // 摘要:
        //     Windows 2000/XP: Browser Stop key
        BROWSER_STOP = 169,
        //
        // 摘要:
        //     Windows 2000/XP: Browser Search key
        BROWSER_SEARCH = 170,
        //
        // 摘要:
        //     Windows 2000/XP: Browser Favorites key
        BROWSER_FAVORITES = 171,
        //
        // 摘要:
        //     Windows 2000/XP: Browser Start and Home key
        BROWSER_HOME = 172,
        //
        // 摘要:
        //     Windows 2000/XP: Volume Mute key
        VOLUME_MUTE = 173,
        //
        // 摘要:
        //     Windows 2000/XP: Volume Down key
        VOLUME_DOWN = 174,
        //
        // 摘要:
        //     Windows 2000/XP: Volume Up key
        VOLUME_UP = 175,
        //
        // 摘要:
        //     Windows 2000/XP: Next Track key
        MEDIA_NEXT_TRACK = 176,
        //
        // 摘要:
        //     Windows 2000/XP: Previous Track key
        MEDIA_PREV_TRACK = 177,
        //
        // 摘要:
        //     Windows 2000/XP: Stop Media key
        MEDIA_STOP = 178,
        //
        // 摘要:
        //     Windows 2000/XP: Play/Pause Media key
        MEDIA_PLAY_PAUSE = 179,
        //
        // 摘要:
        //     Windows 2000/XP: Start Mail key
        LAUNCH_MAIL = 180,
        //
        // 摘要:
        //     Windows 2000/XP: Select Media key
        LAUNCH_MEDIA_SELECT = 181,
        //
        // 摘要:
        //     Windows 2000/XP: Start Application 1 key
        LAUNCH_APP1 = 182,
        //
        // 摘要:
        //     Windows 2000/XP: Start Application 2 key
        LAUNCH_APP2 = 183,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_1 = 186,
        //
        // 摘要:
        //     Windows 2000/XP: For any country/region, the '+' key
        OEM_PLUS = 187,
        //
        // 摘要:
        //     Windows 2000/XP: For any country/region, the ',' key
        OEM_COMMA = 188,
        //
        // 摘要:
        //     Windows 2000/XP: For any country/region, the '-' key
        OEM_MINUS = 189,
        //
        // 摘要:
        //     Windows 2000/XP: For any country/region, the '.' key
        OEM_PERIOD = 190,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_2 = 191,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_3 = 192,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_4 = 219,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_5 = 220,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_6 = 221,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_7 = 222,
        //
        // 摘要:
        //     Used for miscellaneous characters; it can vary by keyboard.
        OEM_8 = 223,
        //
        // 摘要:
        //     Windows 2000/XP: Either the angle bracket key or the backslash key on the RT
        //     102-key keyboard
        OEM_102 = 226,
        //
        // 摘要:
        //     Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
        PROCESSKEY = 229,
        //
        // 摘要:
        //     Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes.
        //     The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard
        //     input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN,
        //     and WM_KEYUP
        PACKET = 231,
        //
        // 摘要:
        //     Attn key
        ATTN = 246,
        //
        // 摘要:
        //     CrSel key
        CRSEL = 247,
        //
        // 摘要:
        //     ExSel key
        EXSEL = 248,
        //
        // 摘要:
        //     Erase EOF key
        EREOF = 249,
        //
        // 摘要:
        //     Play key
        PLAY = 250,
        //
        // 摘要:
        //     Zoom key
        ZOOM = 251,
        //
        // 摘要:
        //     Reserved
        NONAME = 252,
        //
        // 摘要:
        //     PA1 key
        PA1 = 253,
        //
        // 摘要:
        //     Clear key
        OEM_CLEAR = 254
    }

    public class UiElementClickParams
    {
        /// <summary>
        /// 是否在操作过程中移动鼠标
        /// </summary>
        public bool moveMouse = false;

        public UiElementClickParams()
        {

        }
    }

    public class UiElementHoverParams
    {
        /// <summary>
        /// 是否在操作过程中移动鼠标
        /// </summary>
        public bool moveMouse = false;

        public UiElementHoverParams()
        {

        }
    }

}