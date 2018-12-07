using System;

namespace Caieta
{
    /*
     *      Debugger Class.
     * 
     *      Create an Inspector and Console.
     * 
     */
    public class Debugger
    {
        public DebugInspector Inspector;
        public DebugConsole Console;

        public Debugger()
        {
            Inspector = new DebugInspector();
            Console = new DebugConsole();
        }

        internal void Initialize()
        {
            Inspector.Initialize();
            Console.Initialize();
        }

        internal void Update()
        {
            Inspector.Update();
            Console.Update();
        }

        internal void Render()
        {
            if (Inspector.IsOpen) 
                Inspector.Render();

            if (Console.IsOpen) 
                Console.Render();
        }
    }
}
