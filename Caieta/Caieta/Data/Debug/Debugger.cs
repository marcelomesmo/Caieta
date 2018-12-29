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
        private DebugInspector Inspector;
        private DebugConsole Console;

        public bool IsOpen;

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

            IsOpen = Inspector.IsOpen || Console.IsOpen;
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
