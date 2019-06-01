using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class DebugConsole
    {
        public bool IsEnabled = true;
        public bool IsOpen;

        private Keyboard Keyboard;

        private SpriteFont ConsoleFont;

        private string currentText = "";
        private List<Line> consoleText;

        private Dictionary<string, CommandInfo> commands;

        public DebugConsole()
        {
            IsOpen = false;
        }

        internal void Initialize()
        {
            Keyboard = new Keyboard();

            //ConsoleFont = Resources.Get<SpriteFont>("Fonts/PressStart2P");
            ConsoleFont = Resources.Get<SpriteFont>("Fonts/MonogramExtended");

            consoleText = new List<Line>();
            consoleText.Add(new Line("Type [help] for command list."));

            commands = new Dictionary<string, CommandInfo>();
            BuildCommandsList();
        }

        internal void Update()
        {
            // Update Inputs
            Keyboard.Update();

            if (IsOpen)
                UpdateOpen();
            else if (IsEnabled)
                UpdateClosed();
        }

        private const float UNDERSCORE_TIME = .5f;
        private bool underscore;
        private float underscoreCounter;
        //private const float REPEAT_DELAY = .5f;
        //private const float REPEAT_EVERY = 1 / 30f;
        //private const float OPACITY = .8f;

        private struct Line
        {
            public string Text;
            public Color Color;

            public Line(string text)
            {
                Text = text;
                Color = Color.White;
            }

            public Line(string text, Color color)
            {
                Text = text;
                Color = color;
            }
        }

        private void UpdateOpen()
        {
            if (Keyboard.Pressed(Keys.OemTilde))
            {
                IsOpen = false;

                Input.Enable();
            }

            // Handle Underscore
            underscoreCounter += Engine.Instance.RawDeltaTime;
            while (underscoreCounter >= UNDERSCORE_TIME)
            {
                underscoreCounter -= UNDERSCORE_TIME;
                underscore = !underscore;
            }

            // Handle Holding repeated keys

            // Handle Key presses
            //string holder = Keyboard.GetReleasedKey();
            //if(holder != "-")
            //currentText += holder;
            HandleKey(Keyboard.GetLastReleasedKey());

            if (Keyboard.Released(Keys.Enter) && currentText.Length > 0)
                EnterCommand();

        }

        private void HandleKey(Keys key)
        {
            //if (key != Keys.Tab && key != Keys.LeftShift && key != Keys.RightShift && key != Keys.RightAlt && key != Keys.LeftAlt && key != Keys.RightControl && key != Keys.LeftControl)
            //    tabIndex = -1;

            //if (key != Keys.OemTilde && key != Keys.Oem8 && key != Keys.Enter && repeatKey != key)
            //{
            //    repeatKey = key;
            //    repeatCounter = 0;
            //}

            switch (key)
            {
                default:
                    if (key.ToString().Length == 1)
                    {
                        if (Keyboard.IsShiftDown())
                            currentText += key.ToString();
                        else
                            currentText += key.ToString().ToLower();
                    }
                    break;

                case (Keys.D1):
                    if (Keyboard.IsShiftDown())
                        currentText += '!';
                    else
                        currentText += '1';
                    break;
                case (Keys.D2):
                    if (Keyboard.IsShiftDown())
                        currentText += '@';
                    else
                        currentText += '2';
                    break;
                case (Keys.D3):
                    if (Keyboard.IsShiftDown())
                        currentText += '#';
                    else
                        currentText += '3';
                    break;
                case (Keys.D4):
                    if (Keyboard.IsShiftDown())
                        currentText += '$';
                    else
                        currentText += '4';
                    break;
                case (Keys.D5):
                    if (Keyboard.IsShiftDown())
                        currentText += '%';
                    else
                        currentText += '5';
                    break;
                case (Keys.D6):
                    if (Keyboard.IsShiftDown())
                        currentText += '^';
                    else
                        currentText += '6';
                    break;
                case (Keys.D7):
                    if (Keyboard.IsShiftDown())
                        currentText += '&';
                    else
                        currentText += '7';
                    break;
                case (Keys.D8):
                    if (Keyboard.IsShiftDown())
                        currentText += '*';
                    else
                        currentText += '8';
                    break;
                case (Keys.D9):
                    if (Keyboard.IsShiftDown())
                        currentText += '(';
                    else
                        currentText += '9';
                    break;
                case (Keys.D0):
                    if (Keyboard.IsShiftDown())
                        currentText += ')';
                    else
                        currentText += '0';
                    break;
                case (Keys.OemComma):
                    if (Keyboard.IsShiftDown())
                        currentText += '<';
                    else
                        currentText += ',';
                    break;
                case Keys.OemPeriod:
                    if (Keyboard.IsShiftDown())
                        currentText += '>';
                    else
                        currentText += '.';
                    break;
                case Keys.OemQuestion:
                    if (Keyboard.IsShiftDown())
                        currentText += '?';
                    else
                        currentText += '/';
                    break;
                case Keys.OemSemicolon:
                    if (Keyboard.IsShiftDown())
                        currentText += ':';
                    else
                        currentText += ';';
                    break;
                case Keys.OemQuotes:
                    if (Keyboard.IsShiftDown())
                        currentText += '"';
                    else
                        currentText += '\'';
                    break;
                case Keys.OemBackslash:
                    if (Keyboard.IsShiftDown())
                        currentText += '|';
                    else
                        currentText += '\\';
                    break;
                case Keys.OemOpenBrackets:
                    if (Keyboard.IsShiftDown())
                        currentText += '{';
                    else
                        currentText += '[';
                    break;
                case Keys.OemCloseBrackets:
                    if (Keyboard.IsShiftDown())
                        currentText += '}';
                    else
                        currentText += ']';
                    break;
                case Keys.OemMinus:
                    if (Keyboard.IsShiftDown())
                        currentText += '_';
                    else
                        currentText += '-';
                    break;
                case Keys.OemPlus:
                    if (Keyboard.IsShiftDown())
                        currentText += '+';
                    else
                        currentText += '=';
                    break;

                case Keys.Space:
                    currentText += " ";
                    break;
                case Keys.Back:
                    if (currentText.Length > 0)
                        currentText = currentText.Substring(0, currentText.Length - 1);
                    break;
                case Keys.Delete:
                    currentText = "";
                    break;

                //case Keys.Up:
                //    if (seekIndex < commandHistory.Count - 1)
                //    {
                //        seekIndex++;
                //        currentText = string.Join(" ", commandHistory[seekIndex]);
                //    }
                //    break;
                //case Keys.Down:
                //    if (seekIndex > -1)
                //    {
                //        seekIndex--;
                //        if (seekIndex == -1)
                //            currentText = "";
                //        else
                //            currentText = string.Join(" ", commandHistory[seekIndex]);
                //    }
                //    break;

                //case Keys.Tab:
                    //if (Keyboard.IsShiftDown())
                    //{
                    //    if (tabIndex == -1)
                    //    {
                    //        tabSearch = currentText;
                    //        FindLastTab();
                    //    }
                    //    else
                    //    {
                    //        tabIndex--;
                    //        if (tabIndex < 0 || (tabSearch != "" && sorted[tabIndex].IndexOf(tabSearch) != 0))
                    //            FindLastTab();
                    //    }
                    //}
                    //else
                    //{
                    //    if (tabIndex == -1)
                    //    {
                    //        tabSearch = currentText;
                    //        FindFirstTab();
                    //    }
                    //    else
                    //    {
                    //        tabIndex++;
                    //        if (tabIndex >= sorted.Count || (tabSearch != "" && sorted[tabIndex].IndexOf(tabSearch) != 0))
                    //            FindFirstTab();
                    //    }
                    //}
                    //if (tabIndex != -1)
                    //    currentText = sorted[tabIndex];
                    //break;

                //case Keys.F1:
                //case Keys.F2:
                //case Keys.F3:
                //case Keys.F4:
                //case Keys.F5:
                //case Keys.F6:
                //case Keys.F7:
                //case Keys.F8:
                //case Keys.F9:
                //case Keys.F10:
                //case Keys.F11:
                //case Keys.F12:
                    //ExecuteFunctionKeyAction((int)(key - Keys.F1));
                    //break;

                case Keys.Enter:
                    if (currentText.Length > 0)
                        EnterCommand();
                    break;

                //case Keys.Oem8:
                //case Keys.OemTilde:
                    //Open = canOpen = false;
                    //break;
            }
        }

        private void UpdateClosed()
        {
            if (Keyboard.Pressed(Keys.OemTilde))
            {
                IsOpen = true;

                Input.Disable();
            }
        }

        int screenWidth, screenHeight;
        int consoleWidth, consoleHeight;
        int startConsoleX, startConsoleY;

        internal void Render()
        {
            screenWidth = Graphics.ViewWidth;
            screenHeight = Graphics.ViewHeight;
            consoleWidth = screenWidth - 20;
            consoleHeight = screenHeight / 3;
            startConsoleX = 10;
            startConsoleY = 2 * screenHeight / 3 - 20;

            // Start a new batch to draw relative to screensize
            Graphics.SpriteBatch.Begin();
            //Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, null);

            Graphics.DrawRect(0, 0, screenWidth, screenHeight, Color.Black, 50, FillType.FILL);

            Graphics.DrawRect(startConsoleX, startConsoleY, consoleWidth, consoleHeight, Color.Black, 80, FillType.FILL);

            if (underscore)
                Graphics.DrawText(ConsoleFont, "> " + currentText + "|", new Vector2(20, screenHeight - 40), Color.White);
            else
                Graphics.DrawText(ConsoleFont, "> " + currentText, new Vector2(20, screenHeight - 40), Color.White);

            if (consoleText.Count > 0)
            {
                int height = 10 + (10 * consoleText.Count);
                //Graphics.DrawRect(10, screenHeight - height - 60, screenWidth - 20, height, Color.Black * 80);
                for (int i = 0; i < consoleText.Count; i++)
                    Graphics.DrawText(ConsoleFont, consoleText[i].Text, new Vector2(20, screenHeight - 55 - (10 * i)), consoleText[i].Color);
            }

            //Graphics.DrawText(ConsoleFont, "Type [help] for command list. ", new Vector2(startConsoleX + 10, startConsoleY + 10), Color.White);//, Vector2.Zero, new Vector2(1.5f, 1.5f), 0);

            Graphics.SpriteBatch.End();
        }


        #region Commands

        private void EnterCommand()
        {
            string[] data = currentText.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            //if (commandHistory.Count == 0 || commandHistory[0] != currentText)
            //    commandHistory.Insert(0, currentText);
            consoleText.Insert(0, new Line(currentText, Color.Aqua));
            currentText = "";
            //seekIndex = -1;

            string[] args = new string[data.Length - 1];
            for (int i = 1; i < data.Length; i++)
                args[i - 1] = data[i];
            ExecuteCommand(data[0].ToLower(), args);
        }

        public void ExecuteCommand(string command, string[] args)
        {
            if (commands.ContainsKey(command))
                commands[command].Action(args);
            else
                ConsoleLog("Command '" + command + "' not found! Type 'help' for list of commands", Color.Yellow);
        }

        [Command("clear", "Clears the terminal")]
        public void Clear()
        {
            consoleText.Clear();
        }

        [Command("exit", "Exits the game")]
        private void Exit()
        {
            Engine.Instance.Exit();
        }

        [Command("framerate", "Sets the target framerate")]
        private void Framerate(float target)
        {
            Engine.Instance.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / target);
        }

        [Command("count", "Logs amount of Entities in the Scene. Pass a tagIndex to count only Entities with that tag")]
        private void Count(int tagIndex = -1)
        {
            if (Engine.SceneManager.CurrScene == null)
            {
                consoleText.Add(new Line("Current Scene is null!"));
                ConsoleLog("Current Scene is null!", Color.White);
                return;
            }

            //if (tagIndex < 0)
            //    Engine.Commands.Log(Engine.Scene.Entities.Count.ToString());
            //else
                //Engine.Commands.Log(Engine.Scene.TagLists[tagIndex].Count.ToString());
        }

        [Command("tracker", "Logs all tracked objects in the scene. Set mode to 'e' for just entities, 'c' for just components, or 'cc' for just collidable components")]
        private void Tracker(string mode)
        {
            //if (Engine.SceneManager.CurrScene == null)
            //{
            //    Engine.Commands.Log("Current Scene is null!");
            //    return;
            //}

            //switch (mode)
            //{
            //    default:
            //        Engine.Commands.Log("-- Entities --");
            //        //Engine.Scene.Tracker.LogEntities();
            //        Engine.Commands.Log("-- Components --");
            //        //Engine.Scene.Tracker.LogComponents();
            //        Engine.Commands.Log("-- Collidable Components --");
            //        //Engine.Scene.Tracker.LogCollidableComponents();
            //        break;

            //    case "e":
            //        //Engine.Scene.Tracker.LogEntities();
            //        break;

            //    case "c":
            //        //Engine.Scene.Tracker.LogComponents();
            //        break;

            //    case "cc":
            //        //Engine.Scene.Tracker.LogCollidableComponents();
            //        break;
            //}
        }

        [Command("fullscreen", "Switches to fullscreen mode")]
        private void Fullscreen()
        {
            Engine.SetFullscreen();
        }

        [Command("window", "Switches to window mode")]
        private void Window(int scale = 1)
        {
            Engine.SetWindowed(Graphics.Width * scale, Graphics.Height * scale);
        }

        [Command("goto", "Go to world level")]
        private void Goto(int world = 0, int level = 1)
        {
            Engine.SceneManager.LoadScene("W"+world+"Level"+ level);
        }

        [Command("change", "Go to world level by name")]
        private void Change(string scenename)
        {
            Engine.SceneManager.LoadScene(scenename);
        }

        [Command("help", "Shows usage help for a given command")]
        private void Help(string command)
        {
            if (commands.ContainsKey(command))
            {
                var c = commands[command];
                StringBuilder str = new StringBuilder();

                //Title
                str.Append(":: ");
                str.Append(command);

                //Usage
                if (!string.IsNullOrEmpty(c.Usage))
                {
                    str.Append(" ");
                    str.Append(c.Usage);
                }
                ConsoleLog(str.ToString());

                //Help
                if (string.IsNullOrEmpty(c.Help))
                    ConsoleLog("No help info set");
                else
                    ConsoleLog(c.Help);
            }
            else
            {
                StringBuilder str = new StringBuilder();
                str.Append("Commands list: \n");
                foreach (var c in commands)
                    str.Append("'" + c.Key + "' - " + c.Value.Help + "\n");
                //str.Append(string.Join(", ", commands));
                ConsoleLog(str.ToString());
                ConsoleLog("Type 'help command' for more info on that command!");
            }
        }

        #endregion

        public void ConsoleLog(object obj)
        {
            ConsoleLog(obj, Color.White);
        }
        public void ConsoleLog(object obj, Color color)
        {
            string str = obj.ToString();

            //Newline splits
            if (str.Contains("\n"))
            {
                var all = str.Split('\n');
                foreach (var line in all)
                    ConsoleLog(line, color);
                return;
            }

            //Split the string if you overlow horizontally
            int maxWidth = Graphics.ViewWidth - 20;
            while (ConsoleFont.MeasureString(str).X > maxWidth)
            {
                int split = -1;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        if (ConsoleFont.MeasureString(str.Substring(0, i)).X <= maxWidth)
                            split = i;
                        else
                            break;
                    }
                }

                if (split == -1)
                    break;

                consoleText.Insert(0, new Line(str.Substring(0, split), color));
                str = str.Substring(split + 1);
            }

            consoleText.Insert(0, new Line(str, color));

            //Don't overflow top of window
            int maxCommands = consoleHeight / 10;
            while (consoleText.Count > maxCommands)
                consoleText.RemoveAt(consoleText.Count - 1);
        }

        #region Parse Commands

        private void BuildCommandsList()
        {
#if !CONSOLE
            //Check Monocle for Commands
            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    ProcessMethod(method);

            //Check the calling assembly for Commands
            foreach (var type in Assembly.GetEntryAssembly().GetTypes())
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    ProcessMethod(method);

            //Maintain the sorted command list
            //foreach (var command in commands)
            //    sorted.Add(command.Key);
            //sorted.Sort();
#endif
        }

        private void ProcessMethod(MethodInfo method)
        {
            Command attr = null;
            {
                var attrs = method.GetCustomAttributes(typeof(Command), false);
                if (attrs.Length > 0)
                    attr = attrs[0] as Command;
            }

            if (attr != null)
            {
                //if (!method.IsStatic)
                //    throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but is not static");
                //else
                //{
                    CommandInfo info = new CommandInfo();
                    info.Help = attr.Info;

                    var parameters = method.GetParameters();
                    var defaults = new object[parameters.Length];
                    string[] usage = new string[parameters.Length];

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        usage[i] = p.Name + ":";

                        if (p.ParameterType == typeof(string))
                            usage[i] += "string";
                        else if (p.ParameterType == typeof(int))
                            usage[i] += "int";
                        else if (p.ParameterType == typeof(float))
                            usage[i] += "float";
                        else if (p.ParameterType == typeof(bool))
                            usage[i] += "bool";
                        else
                            throw new Exception(method.DeclaringType.Name + "." + method.Name + " is marked as a command, but has an invalid parameter type. Allowed types are: string, int, float, and bool");

                        if (p.DefaultValue == DBNull.Value)
                            defaults[i] = null;
                        else if (p.DefaultValue != null)
                        {
                            defaults[i] = p.DefaultValue;
                            if (p.ParameterType == typeof(string))
                                usage[i] += "=\"" + p.DefaultValue + "\"";
                            else
                                usage[i] += "=" + p.DefaultValue;
                        }
                        else
                            defaults[i] = null;
                    }

                    if (usage.Length == 0)
                        info.Usage = "";
                    else
                        info.Usage = "[" + string.Join(" ", usage) + "]";

                    info.Action = (args) =>
                    {
                        if (parameters.Length == 0)
                            InvokeMethod(method);
                        else
                        {
                            object[] param = (object[])defaults.Clone();

                            for (int i = 0; i < param.Length && i < args.Length; i++)
                            {
                                if (parameters[i].ParameterType == typeof(string))
                                    param[i] = ArgString(args[i]);
                                else if (parameters[i].ParameterType == typeof(int))
                                    param[i] = ArgInt(args[i]);
                                else if (parameters[i].ParameterType == typeof(float))
                                    param[i] = ArgFloat(args[i]);
                                else if (parameters[i].ParameterType == typeof(bool))
                                    param[i] = ArgBool(args[i]);
                            }

                            InvokeMethod(method, param);
                        }
                    };

                    commands[attr.Name] = info;
                //}
            }
        }

        private void InvokeMethod(MethodInfo method, object[] param = null)
        {
            try
            {
                method.Invoke(this, param);
                //method.Invoke(null, param);
            }
            catch (Exception e)
            {
                ConsoleLog(e.InnerException.Message, Color.Yellow);
                LogStackTrace(e.InnerException.StackTrace);
            }
        }

        private void LogStackTrace(string stackTrace)
        {
            foreach (var call in stackTrace.Split('\n'))
            {
                string log = call;

                //Remove File Path
                {
                    var from = log.LastIndexOf(" in ") + 4;
                    var to = log.LastIndexOf('\\') + 1;
                    if (from != -1 && to != -1)
                        log = log.Substring(0, from) + log.Substring(to);
                }

                //Remove arguments list
                {
                    var from = log.IndexOf('(') + 1;
                    var to = log.IndexOf(')');
                    if (from != -1 && to != -1)
                        log = log.Substring(0, from) + log.Substring(to);
                }

                //Space out the colon line number
                var colon = log.LastIndexOf(':');
                if (colon != -1)
                    log = log.Insert(colon + 1, " ").Insert(colon, " ");

                log = log.TrimStart();
                log = "-> " + log;

                ConsoleLog(log);
            }
        }

        private struct CommandInfo
        {
            public Action<string[]> Action;
            public string Help;
            public string Usage;
        }

        #region Parsing Arguments

        private static string ArgString(string arg)
        {
            if (arg == null)
                return "";
            else
                return arg;
        }

        private static bool ArgBool(string arg)
        {
            if (arg != null)
                return !(arg == "0" || arg.ToLower() == "false" || arg.ToLower() == "f");
            else
                return false;
        }

        private static int ArgInt(string arg)
        {
            try
            {
                return Convert.ToInt32(arg);
            }
            catch
            {
                return 0;
            }
        }

        private static float ArgFloat(string arg)
        {
            try
            {
                return Convert.ToSingle(arg);
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #endregion
    }

    internal class Command : Attribute
    {
        public string Name;
        public string Info;

        public Command(string name, string info)
        {
            Name = name;
            Info = info;
        }
    }
}
