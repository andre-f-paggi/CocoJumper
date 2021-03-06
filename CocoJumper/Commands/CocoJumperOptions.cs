﻿using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace CocoJumper.Commands
{
    public class CocoJumperOptions : DialogPage
    {
        private const string GeneralCategory = "General";

        public event EventHandler Saved;

        [Category(GeneralCategory)]
        [DisplayName("Automatically exit after(ms)")]
        [Description("Determines how much ms must pass before logic will automaticly exit.")]
        [DefaultValue(5000)]
        public int AutomaticallyExitInterval { get; set; } = 5000;

        [Category(GeneralCategory)]
        [DisplayName("Disable highlight for multisearch")]
        [Description("If set to True, logic will not render any highlighting components with may slowdown VisualStudio.")]
        [DefaultValue(false)]
        public bool DisableHighlightForMultiSearch { get; set; } = false;

        [Category(GeneralCategory)]
        [DisplayName("Disable highlight for single search with select")]
        [Description("If set to True, logic will not render any highlighting components with may slowdown VisualStudio.")]
        [DefaultValue(false)]
        public bool DisableHighlightForSingleHighlight { get; set; } = false;

        [Category(GeneralCategory)]
        [DisplayName("Disable highlight for single search")]
        [Description("If set to True, logic will not render any highlighting components with may slowdown VisualStudio.")]
        [DefaultValue(false)]
        public bool DisableHighlightForSingleSearch { get; set; } = false;

        [Category(GeneralCategory)]
        [DisplayName("Jump after choosed element")]
        [Description("If set to True, logic will move caret to the end of choosed element.")]
        [DefaultValue(false)]
        public bool JumpAfterChosenElement { get; set; } = false;

        [Category(GeneralCategory)]
        [DisplayName("Limit results")]
        [Description("Limts results that are rendered on one page.")]
        [DefaultValue(50)]
        public int LimitResults { get; set; } = 50;

        [Category(GeneralCategory)]
        [DisplayName("Timer interval(ms)")]
        [Description("Determines how much ms must pass before rendering components, counting from last key press.")]
        [DefaultValue(250)]
        public int TimerInterval { get; set; } = 250;

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
    }
}