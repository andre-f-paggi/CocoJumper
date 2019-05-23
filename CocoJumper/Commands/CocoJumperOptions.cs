﻿using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace CocoJumper.Commands
{
    public class CocoJumperOptions : DialogPage
    {
        private const string GeneralCategory = "General";
        private int _timerInterval = 250;
        private int _limitResults = 50;
        private int _automaticallyExitInterval = 5000;
        private bool _jumpAfterChoosedElement = false;

        [Category(GeneralCategory)]
        [DisplayName("Limit results")]
        [Description("Limts results that are rendered on one page.")]
        [DefaultValue(50)]
        public int LimitResults { get => _limitResults; set => _limitResults = value; }

        [Category(GeneralCategory)]
        [DisplayName("Timer interval(ms)")]
        [Description("Determines how much ms must pass before rendering components, counting from last key press.")]
        [DefaultValue(250)]
        public int TimerInterval { get => _timerInterval; set => _timerInterval = value; }

        [Category(GeneralCategory)]
        [DisplayName("Automatically exit after(ms)")]
        [Description("Determines how much ms must pass before logic will automaticly exit.")]
        [DefaultValue(5000)]
        public int AutomaticallyExitInterval { get => _automaticallyExitInterval; set => _automaticallyExitInterval = value; }


        [Category(GeneralCategory)]
        [DisplayName("Jump after choosed element")]
        [Description("If set to True, logic will move caret to the end of choosed element.")]
        [DefaultValue(false)]
        public bool JumpAfterChoosedElement { get => _jumpAfterChoosedElement; set => _jumpAfterChoosedElement = value; }

        public override void SaveSettingsToStorage()
        {
            if (AutomaticallyExitInterval <= 0)
                AutomaticallyExitInterval = 5000;
            if (TimerInterval <= 0)
                TimerInterval = 250;
            if (LimitResults < 0)
                LimitResults = 50;
            base.SaveSettingsToStorage();

            Saved?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Saved;
    }
}