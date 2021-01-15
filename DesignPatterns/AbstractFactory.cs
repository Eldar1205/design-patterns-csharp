using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    #region Controls
    interface IButton
    {
        void Click();
    }

    interface ITextBox
    {
        string Text { get; set; }
    }

    class IOSButton : IButton
    {
        public void Click()
        {
        }
    }

    class AndroidButton : IButton
    {
        public void Click()
        {
        }
    }

    class IOSTextBox : ITextBox
    {
        public string Text { get; set; }
    }

    class AndroidTextBox : ITextBox
    {
        public string Text { get; set; }
    }
    #endregion

    #region Bad implementation without Abstract Factory
    class BadMobileApp
    {
        private static readonly bool IsIOS = false; // Assume this is checked using some API

        static void BadMain()
        {
            // Bad: ifs count = #instances of controls => lots of room for human error!
            IButton button1 = IsIOS ? (IButton)new IOSButton() : new AndroidButton();
            IButton button2 = IsIOS ? (IButton)new IOSButton() : new AndroidButton();
            ITextBox textBox1 = IsIOS ? (ITextBox)new IOSTextBox() : new AndroidTextBox();
        }
    }
    #endregion

    #region Good implementation using Abstract Factory
    class GoodMobileApp
    {
        private static readonly bool IsIOS = false; // Assume this is checked using some API

        static void GoodMain()
        {
            // Good: just 1 if to choose the control factory
            var controlFactory = IsIOS ? (IControlFactory)new IOSControlFactory() : new AndroidControlFactory();

            // Somewhere later...
            IButton button1 = controlFactory.CreateButton();
            IButton button2 = controlFactory.CreateButton();
            ITextBox textBox1 = controlFactory.CreateTextBox();
        }
    }

    interface IControlFactory
    {
        IButton CreateButton();
        ITextBox CreateTextBox();
    }

    class IOSControlFactory : IControlFactory
    {
        public IButton CreateButton() => new IOSButton();
        public ITextBox CreateTextBox() => new IOSTextBox();
    }

    class AndroidControlFactory : IControlFactory
    {
        public IButton CreateButton() => new AndroidButton();
        public ITextBox CreateTextBox() => new AndroidTextBox();
    } 
    #endregion
}
