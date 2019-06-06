using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
 * Disclaimer: 
 * 
 * This code was adapted from Matt Thorson's
 * Monocle Engine - https://bitbucket.org/MattThorson/monocle-engine/src
 * 
 * 
 */
namespace Caieta
{
    public class DebugConsole
    {
        public bool IsEnabled = true;
        public bool IsOpen;

        private Keyboard Keyboard;

        private SpriteFont ConsoleFont;
        private Vector2 fontScale = new Vector2(0.5f, 0.5f);
        private const float UNDERSCORE_TIME = .5f;
        private bool underscore;
        private float underscoreCounter;

        private string typedText = "";
        private List<Line> consoleText;

        private Dictionary<string, CommandInfo> commands;
        private List<string> sortedCommandList;
        private int tabIndex = -1;  // Scrool through sorted command list

        public DebugConsole()
        {
            IsOpen = false;
        }

        internal void Initialize()
        {
            Keyboard = new Keyboard();

            ConsoleFont = Resources.Get<SpriteFont>("Fonts/PressStart2P");
            //ConsoleFont = Resources.Get<SpriteFont>("Fonts/MonogramExtended");

            consoleText = new List<Line>();
            consoleText.Add(new Line("Type 'help' for a list of commands."));

            commands = new Dictionary<string, CommandInfo>();
            sortedCommandList = new List<string>();
            BuildCommandList();
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

        private void UpdateClosed()
        {
            if (Keyboard.Pressed(Keys.OemTilde))
            {
                IsOpen = true;

                Input.Disable();
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
            HandleUnderscore();

            // Handle Key presses
            HandleKey(Keyboard.GetLastReleasedKey());

            // Handle Commands
            if (Keyboard.Released(Keys.Enter) && typedText.Length > 0)
                EnterCommand();
        }

        private void HandleUnderscore()
        {
            underscoreCounter += Engine.Instance.RawDeltaTime;
            if (underscoreCounter >= UNDERSCORE_TIME)
            {
                underscoreCounter = 0;
                underscore = !underscore;
            }
        }

        private void HandleKey(Keys key)
        {
            if (key == Keys.None)
                return;

            if (key != Keys.Tab && !Keyboard.IsModifierDown())
                tabIndex = -1;

            switch (key)
            {
                default:
                    if (key.ToString().Length == 1)
                    {
                        if (Keyboard.IsShiftDown())
                            typedText += key.ToString();
                        else
                            typedText += key.ToString().ToLower();
                    }
                    break;

                case (Keys.D1):
                    if (Keyboard.IsShiftDown())
                        typedText += '!';
                    else
                        typedText += '1';
                    break;
                case (Keys.D2):
                    if (Keyboard.IsShiftDown())
                        typedText += '@';
                    else
                        typedText += '2';
                    break;
                case (Keys.D3):
                    if (Keyboard.IsShiftDown())
                        typedText += '#';
                    else
                        typedText += '3';
                    break;
                case (Keys.D4):
                    if (Keyboard.IsShiftDown())
                        typedText += '$';
                    else
                        typedText += '4';
                    break;
                case (Keys.D5):
                    if (Keyboard.IsShiftDown())
                        typedText += '%';
                    else
                        typedText += '5';
                    break;
                case (Keys.D6):
                    if (Keyboard.IsShiftDown())
                        typedText += '^';
                    else
                        typedText += '6';
                    break;
                case (Keys.D7):
                    if (Keyboard.IsShiftDown())
                        typedText += '&';
                    else
                        typedText += '7';
                    break;
                case (Keys.D8):
                    if (Keyboard.IsShiftDown())
                        typedText += '*';
                    else
                        typedText += '8';
                    break;
                case (Keys.D9):
                    if (Keyboard.IsShiftDown())
                        typedText += '(';
                    else
                        typedText += '9';
                    break;
                case (Keys.D0):
                    if (Keyboard.IsShiftDown())
                        typedText += ')';
                    else
                        typedText += '0';
                    break;
                case (Keys.OemComma):
                    if (Keyboard.IsShiftDown())
                        typedText += '<';
                    else
                        typedText += ',';
                    break;
                case Keys.OemPeriod:
                    if (Keyboard.IsShiftDown())
                        typedText += '>';
                    else
                        typedText += '.';
                    break;
                case Keys.OemQuestion:
                    if (Keyboard.IsShiftDown())
                        typedText += '?';
                    else
                        typedText += '/';
                    break;
                case Keys.OemSemicolon:
                    if (Keyboard.IsShiftDown())
                        typedText += ':';
                    else
                        typedText += ';';
                    break;
                case Keys.OemQuotes:
                    if (Keyboard.IsShiftDown())
                        typedText += '"';
                    else
                        typedText += '\'';
                    break;
                case Keys.OemBackslash:
                    if (Keyboard.IsShiftDown())
                        typedText += '|';
                    else
                        typedText += '\\';
                    break;
                case Keys.OemOpenBrackets:
                    if (Keyboard.IsShiftDown())
                        typedText += '{';
                    else
                        typedText += '[';
                    break;
                case Keys.OemCloseBrackets:
                    if (Keyboard.IsShiftDown())
                        typedText += '}';
                    else
                        typedText += ']';
                    break;
                case Keys.OemMinus:
                    if (Keyboard.IsShiftDown())
                        typedText += '_';
                    else
                        typedText += '-';
                    break;
                case Keys.OemPlus:
                    if (Keyboard.IsShiftDown())
                        typedText += '+';
                    else
                        typedText += '=';
                    break;

                case Keys.Space:
                    typedText += " ";
                    break;

                case Keys.Back:
                    if (typedText.Length > 0)
                        typedText = typedText.Substring(0, typedText.Length - 1);
                    break;

                case Keys.Delete:
                    typedText = "";
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

                case Keys.Tab:
                    if (Keyboard.IsShiftDown())
                        tabIndex--;
                    else
                        tabIndex++;

                    tabIndex = MathHelper.Clamp(tabIndex, 0, sortedCommandList.Count-1);

                    if (tabIndex != -1)
                        typedText = sortedCommandList[tabIndex];

                    break;

                case Keys.Enter:
                    if (typedText.Length > 0)
                        EnterCommand();
                    break;
            }
        }

        int screenWidth, screenHeight;
        int consoleWidth, consoleHeight;
        int startConsoleX, startConsoleY;
        const int FONT_SIZE = 25;

        internal void Render()
        {
            screenWidth = Graphics.ViewWidth;
            screenHeight = Graphics.ViewHeight;
            consoleWidth = screenWidth - 20;
            consoleHeight = screenHeight / 3;
            startConsoleX = 10;
            startConsoleY = screenHeight - consoleHeight;

            // Start a new batch to draw relative to screensize
            Graphics.SpriteBatch.Begin();

            Graphics.DrawRect(0, 0, screenWidth, screenHeight, Color.Black, 30, FillType.FILL);
            Graphics.DrawRect(startConsoleX, startConsoleY, consoleWidth, consoleHeight, Color.Black, 60, FillType.FILL);
            Graphics.DrawRect(startConsoleX, screenHeight - FONT_SIZE, consoleWidth, FONT_SIZE, Color.Black, 100, FillType.FILL);

            if (underscore)
                Graphics.DrawText(ConsoleFont, ">" + typedText + "|", new Vector2(20, screenHeight - 20), Color.White);
            else
                Graphics.DrawText(ConsoleFont, ">" + typedText, new Vector2(20, screenHeight - 20), Color.White);

            if (consoleText.Count > 0)
            {
                for (int i = 0; i < consoleText.Count; i++)
                    Graphics.DrawText(ConsoleFont, consoleText[i].Text, new Vector2(20, screenHeight - 35 - (10 * i)), consoleText[i].Color, Vector2.Zero, fontScale, 0);
            }

            Graphics.SpriteBatch.End();
        }

        #region Run Command and Write to Console

        private void EnterCommand()
        {
            string[] data = typedText.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            //if (commandHistory.Count == 0 || commandHistory[0] != currentText)
            //    commandHistory.Insert(0, currentText);
            consoleText.Insert(0, new Line(typedText, Color.Aqua));
            typedText = "";
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

        public void ConsoleLog(object obj)
        {
            ConsoleLog(obj, Color.White);
        }

        public void ConsoleLog(object obj, Color color)
        {
            string str = obj.ToString();

            // Newline splits
            if (str.Contains("\n"))
            {
                var all = str.Split('\n');
                foreach (var line in all)
                    ConsoleLog(line, color);
                return;
            }

            // Split the string if you overlow horizontally
            int maxWidth = Graphics.ViewWidth - 20;
            while (ConsoleFont.MeasureString(str).X * fontScale.X > maxWidth)
            {
                int split = -1;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == ' ')
                    {
                        if (ConsoleFont.MeasureString(str.Substring(0, i)).X * fontScale.X <= maxWidth)
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

            // Don't overflow top of window
            int maxCommands = consoleHeight / 10;
            while (consoleText.Count > maxCommands)
                consoleText.RemoveAt(consoleText.Count - 1);
        }

        #endregion

        #region Build Command List

        private void BuildCommandList()
        {
#if !CONSOLE
            // Check the calling assembly for Commands
            foreach (var type in Assembly.GetCallingAssembly().GetTypes())
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    ProcessMethod(method);

            // Maintain the sorted command list
            foreach (var command in commands)
                sortedCommandList.Add(command.Key);
            sortedCommandList.Sort();
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
            }
        }

        private void InvokeMethod(MethodInfo method, object[] param = null)
        {
            try
            {
                method.Invoke(this, param);
            }
            catch (Exception e)
            {
                ConsoleLog(e.InnerException.Message, Color.Yellow);
            }
        }

        #endregion

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

        #region Definitions

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

        private struct CommandInfo
        {
            public Action<string[]> Action;
            public string Help;
            public string Usage;
        }

        #endregion

        #region Commands

        // Console
        [Command("clear", "Clear terminal.")]
        public void Clear()
        {
            consoleText.Clear();
            ConsoleLog("Type 'help' for a list of commands.");
        }

        // Engine
        [Command("exit", "Exit the game.")]
        private void Exit()
        {
            Engine.Instance.Exit();
        }

        [Command("framerate", "Set the target framerate.")]
        private void Framerate(float target)
        {
            Engine.Instance.TargetElapsedTime = TimeSpan.FromSeconds(1.0 / target);
        }

        [Command("fullscreen", "Switch to Fullscreen mode.")]
        private void Fullscreen()
        {
            Engine.SetFullscreen();
        }

        [Command("window", "Switch to Windowed mode.")]
        private void Window(int scale = 1)
        {
            Engine.SetWindowed(Graphics.Width * scale, Graphics.Height * scale);
        }

        // Scenes
        [Command("restart", "Reload current scene.")]
        private void Restart()
        {
            Engine.SceneManager.RestartScene();
        }

        [Command("goto", "Change scene by World and Level number.")]
        private void Goto(int world, int level)
        {
            Engine.SceneManager.LoadScene("W" + world + "Level" + level);
        }

        [Command("load", "Change scene by Name.")]
        private void Change(string scenename)
        {
            Engine.SceneManager.LoadScene(scenename);
        }

        // Entities
        //[Command("count", "Logs amount of Entities in the Scene. Pass a tagIndex to count only Entities with that tag")]
        //private void Count(int tagIndex = -1)
        //{
        //    if (Engine.SceneManager.CurrScene == null)
        //    {
        //        consoleText.Add(new Line("Current Scene is null!"));
        //        ConsoleLog("Current Scene is null!", Color.White);
        //        return;
        //    }

        //    //if (tagIndex < 0)
        //    //    Engine.Commands.Log(Engine.Scene.Entities.Count.ToString());
        //    //else
        //    //Engine.Commands.Log(Engine.Scene.TagLists[tagIndex].Count.ToString());
        //}

        //[Command("tracker", "Logs all tracked objects in the scene. Set mode to 'e' for just entities, 'c' for just components, or 'cc' for just collidable components")]
        //private void Tracker(string mode)
        //{
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
        //}

        [Command("help", "Display help for all or given command.")]
        private void Help(string command = "")
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
                str.Append("Command list: \n");
                foreach (var c in sortedCommandList)
                {
                    if(commands.ContainsKey(c))
                        str.Append("'" + c + "' - " + commands[c].Help + " " + commands[c].Usage + "\n");
                }
                //str.Append(string.Join(", ", commands));
                ConsoleLog(str.ToString());
                ConsoleLog("Type 'help command' for more info on that command!");
            }
        }

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
