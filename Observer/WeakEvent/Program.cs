using System;
using System.Windows;

namespace DesignPatterns
{
    public class Button
    {
        public event EventHandler Clicked;

        public void Fire()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }

    public class Window
    {
        public Window(Button button)
        {
            // button.Clicked += ButtonOnClicked; // Not Good. Memory leak
            WeakEventManager<Button, EventArgs>
                .AddHandler(button, "Clicked", ButtonOnClicked);
        }

        private void ButtonOnClicked(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Button clicked (Window handler)");
        }

        ~Window()
        {
            Console.WriteLine("Window finalized");
        }
    }

    public class Demo
    {
        static void Main(string[] args)
        {
            var button = new Button();
            var window = new Window(button);
            var windowRef = new WeakReference(window);
            button.Fire();

            Console.WriteLine("Setting window to null");
            window = null;

            FireGC();
            Console.WriteLine($"Is the window alive after GC? {windowRef.IsAlive}");

        }

        private static void FireGC()
        {
            Console.WriteLine("Starting GC");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Console.WriteLine("GC is done");
        }
    }
}