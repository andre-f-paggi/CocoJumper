﻿using CocoJumper.Base.Enum;
using CocoJumper.Base.Logic;
using CocoJumper.Extensions;
using CocoJumper.Listeners;
using CocoJumper.Logic;
using CocoJumper.Provider;
using Microsoft;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using CocoJumper.Base.Events;
using CocoJumper.Events;
using Task = System.Threading.Tasks.Task;

namespace CocoJumper.Commands
{
    internal class CocoJumperMultiSearchCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("29fda481-672d-4ce9-9793-0bebf8b4c6c8");
        private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;
        private readonly AsyncPackage _package;
        private readonly IVsTextManager _vsTextManager;
        private InputListener _inputListener;
        private ICocoJumperLogic _logic;

        private CocoJumperMultiSearchCommand(AsyncPackage package, OleMenuCommandService commandService, IVsTextManager textManager, IVsEditorAdaptersFactoryService editorAdaptersFactoryService, IEventAggregator eventAggregator)
        {
            this._package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _vsTextManager = textManager ?? throw new ArgumentNullException(nameof(textManager));
            this._editorAdaptersFactoryService = editorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(editorAdaptersFactoryService));
            eventAggregator.AddListener(new DelegateListener<ExitEvent>(OnExit), true);

            CommandID menuCommandId = new CommandID(CommandSet, CommandId);
            MenuCommand menuItem = new MenuCommand(Execute, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        public static CocoJumperMultiSearchCommand Instance
        {
            get;
            private set;
        }
        private IAsyncServiceProvider ServiceProvider => _package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            IVsTextManager vsTextManager = await package.GetServiceAsync(typeof(SVsTextManager)) as IVsTextManager;
            IComponentModel componentModel = await package.GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(componentModel);
            IVsEditorAdaptersFactoryService editor = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            IEventAggregator eventAggregator = componentModel.GetService<IEventAggregator>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            Instance = new CocoJumperMultiSearchCommand(package, commandService, vsTextManager, editor, eventAggregator);
        }

        private void CleanupLogicAndInputListener()
        {
            _logic?.Dispose();
            _inputListener?.Dispose();
            _logic = null;
            _inputListener = null;
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsTextView textView = _vsTextManager.GetActiveView();
            IWpfTextView wpfTextView = _editorAdaptersFactoryService.GetWpfTextView(textView);
            CocoJumperCommandPackage cocoJumperCommandPackage = (CocoJumperCommandPackage)_package;

            CleanupLogicAndInputListener();
            WpfViewProvider renderer = new WpfViewProvider(wpfTextView);
            _logic = new CocoJumperLogic(renderer,
                cocoJumperCommandPackage.LimitResults,
                cocoJumperCommandPackage.TimerInterval,
                cocoJumperCommandPackage.AutomaticallyExitInterval,
                cocoJumperCommandPackage.JumpAfterChoosedElement);

            _inputListener = new InputListener(textView);
            _inputListener.KeyPressEvent += OnKeyboardAction;
            _logic.ActivateSearching(false, false);
        }

        private void OnExit(ExitEvent e)
        {
            CleanupLogicAndInputListener();
        }
        private void OnKeyboardAction(object oSender, char? key, KeyEventType eventType)
        {
            _logic = _logic ?? throw new Exception($"{nameof(OnKeyboardAction)} in {nameof(CocoJumperMultiSearchCommand)}, {nameof(_logic)} is null");
            if (_logic.KeyboardAction(key, eventType) == CocoJumperKeyboardActionResult.Finished)
            {
                CleanupLogicAndInputListener();
            }
        }
    }
}