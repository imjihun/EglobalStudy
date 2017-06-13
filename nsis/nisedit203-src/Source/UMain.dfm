object MainFrm: TMainFrm
  Left = 63
  Top = 27
  Width = 688
  Height = 480
  Color = clAppWorkSpace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIForm
  KeyPreview = True
  OldCreateOrder = False
  ShowHint = True
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object TopDock: TTBXDock
    Left = 0
    Top = 0
    Width = 680
    Height = 62
    object tbMenu: TTBXToolbar
      Left = 0
      Top = 0
      Caption = '*'
      CloseButton = False
      FullSize = True
      Images = MainImages
      MenuBar = True
      ProcessShortCuts = True
      ShrinkMode = tbsmWrap
      TabOrder = 0
      object FileMenu: TTBXSubmenuItem
        OnClick = FileMenuClick
        object fmNewScriptItem: TTBXItem
          Action = NewScriptCmd
        end
        object fmWizardItem: TTBXItem
          Action = WizardCmd
        end
        object fmNewTemplateItem: TTBXItem
          Action = NewTemplateCmd
        end
        object fmNewIOItem: TTBXItem
          Action = NewIOCmd
        end
        object fmSeparator1: TTBXSeparatorItem
        end
        object fmOpenItem: TTBXItem
          Action = OpenFileCmd
        end
        object fmReopenItem: TTBXSubmenuItem
          object TBMRUListItem3: TTBXMRUListItem
            MRUList = MRU
          end
        end
        object fmSeparator2: TTBXSeparatorItem
        end
        object fmSaveItem: TTBXItem
          Action = SaveFileCmd
        end
        object fmSaveAsItem: TTBXItem
          Action = SaveFileAsCmd
        end
        object fmSaveAllItem: TTBXItem
          Action = SaveAllCmd
        end
        object fmSeparator3: TTBXSeparatorItem
        end
        object fmPrintItem: TTBXItem
          Action = PrintCmd
        end
        object fmConfigPrintItem: TTBXItem
          Action = ConfigPrintCmd
        end
        object fmSeparator4: TTBXSeparatorItem
        end
        object fmExitItem: TTBXItem
          Action = ExitCmd
        end
      end
      object EditMenu: TTBXSubmenuItem
        object emUndoItem: TTBXItem
          Action = UndoCmd
        end
        object emRedoItem: TTBXItem
          Action = RedoCmd
        end
        object emSeparator1: TTBXSeparatorItem
        end
        object emCutItem: TTBXItem
          Action = CutCmd
        end
        object emCopyItem: TTBXItem
          Action = CopyCmd
        end
        object emCopyHTMLItem: TTBXItem
          Action = CopyAsHTMLCmd
        end
        object emPasteItem: TTBXItem
          Action = PasteCmd
        end
        object emSeparator2: TTBXSeparatorItem
        end
        object emSelectAllItem: TTBXItem
          Action = SelectAllCmd
        end
      end
      object SearchMenu: TTBXSubmenuItem
        OnPopup = SearchMenuPopup
        object smFindItem: TTBXItem
          Action = FindCmd
        end
        object smFindNextItem: TTBXItem
          Action = FindNextCmd
        end
        object smReplaceItem: TTBXItem
          Action = ReplaceCmd
        end
        object smSeparator1: TTBXSeparatorItem
        end
        object smGotoLineItem: TTBXItem
          Action = GotoLineCmd
        end
        object smSeparator2: TTBXSeparatorItem
        end
        object smToggleBookmarkItem: TTBXSubmenuItem
          OnPopup = smToggleBookmarkItemPopup
        end
        object smGotoBookmarkItem: TTBXSubmenuItem
          OnPopup = smGotoBookmarkItemPopup
        end
      end
      object ViewMenu: TTBXSubmenuItem
        object vmToolBarsItem: TTBXSubmenuItem
          object vmVisibilityToggleStandarItem: TTBXVisibilityToggleItem
          end
          object vmVisibilityToggleEditItem: TTBXVisibilityToggleItem
          end
          object vmVisibilityToggleSearchItem: TTBXVisibilityToggleItem
          end
          object vmVisibilityToggleViewItem: TTBXVisibilityToggleItem
          end
          object vmVisibilityToggleFormatItem: TTBXVisibilityToggleItem
          end
          object vmVisibilityToggleNSISItem: TTBXVisibilityToggleItem
          end
          object vmVisibilityToggleWindowItem: TTBXVisibilityToggleItem
          end
        end
        object vmSeparator1: TTBXSeparatorItem
        end
        object vmVisibilityToggleBrowserItem: TTBXVisibilityToggleItem
          Control = BrowserPanel
        end
        object vmVisibilityToggleWinListItem: TTBXVisibilityToggleItem
          Control = WinListPanel
        end
        object vmIOPanelItem: TTBXItem
          Action = ViewIOPanelCmd
        end
        object vmVisibilityToggleStatusBarItem: TTBXVisibilityToggleItem
          Control = EstBar
          OnClick = vmVisibilityToggleStatusBarItemClick
        end
        object vmLogBoxItem: TTBXItem
          Action = LogBoxCmd
        end
        object vmSeparator2: TTBXSeparatorItem
        end
        object vmSpecialCharsItem: TTBXItem
          Action = ViewSpecialCharsCmd
        end
        object vmToggleDesignModeItem: TTBXItem
          Action = ToggleDesingModeCmd
        end
        object vmSeparator3: TTBXSeparatorItem
        end
        object vmOptionsItem: TTBXItem
          Action = ShowOptionsCmd
        end
      end
      object FormatMenu: TTBXSubmenuItem
        object fmIndentItem: TTBXItem
          Action = IdentTextCmd
        end
        object fmUnindentItem: TTBXItem
          Action = UnIdentTextCmd
        end
        object fmSeparator5: TTBXSeparatorItem
        end
        object fmUpperCaseItem: TTBXItem
          Action = UpperCaseCmd
        end
        object fmLowerCaseItem: TTBXItem
          Action = LowerCaseCmd
        end
        object fmToggleCaseItem: TTBXItem
          Action = ToggleCaseCmd
        end
        object fmQuoteItem: TTBXItem
          Action = CoteCmd
        end
        object TBXSeparatorItem6: TTBXSeparatorItem
        end
        object TBXItem10: TTBXItem
          Action = WordWrapCmd
        end
      end
      object ToolsMenu: TTBXSubmenuItem
        OnPopup = ToolsMenuPopup
        object tmInsertColorItem: TTBXItem
          Action = InsertColorCmd
        end
        object tmInsertFileNameItem: TTBXItem
          Action = InsertFileNameCmd
        end
        object tmSeparator1: TTBXSeparatorItem
        end
        object tmTemplateItem: TTBXSubmenuItem
        end
        object tmEditTemplatesItem: TTBXItem
          Action = EditCodeTemplatesCmd
        end
      end
      object NSISMenu: TTBXSubmenuItem
        object nmProfileListItem: TTBXSubmenuItem
          OnClick = nmProfileListItemClick
        end
        object nmEditProfilesItem: TTBXItem
          Action = EditProfilesCmd
        end
        object nmSeparator1: TTBXSeparatorItem
        end
        object nmCompItem: TTBXItem
          Action = CompScriptCmd
        end
        object nmCompRunItem: TTBXItem
          Action = CompRunCmd
        end
        object nmStopCompItem: TTBXItem
          Action = StopCompileCmd
        end
        object nmSeparator2: TTBXSeparatorItem
        end
        object nmRunItem: TTBXItem
          Action = RunInstallerCmd
        end
        object nmSeparator3: TTBXSeparatorItem
        end
        object nmConfigItem: TTBXItem
          Action = ConfigCmd
        end
      end
      object WindowMenu: TTBXSubmenuItem
        object wmCacadeItem: TTBXItem
          Action = CascadeCmd
        end
        object wmTileHorItem: TTBXItem
          Action = TileHorCmd
        end
        object wmTileVerItem: TTBXItem
          Action = TileVerCmd
        end
        object wmArrangeItem: TTBXItem
          Action = ArrangleCmd
        end
        object wmSeparator1: TTBXSeparatorItem
        end
        object wmPriorItem: TTBXItem
          Action = PriorWindowCmd
        end
        object wmNextItem: TTBXItem
          Action = NextWindowCmd
        end
        object wmSMUItem: TTBXItem
          Action = CMRWindowCmd
        end
        object wmSeparator2: TTBXSeparatorItem
        end
        object wmCloseItem: TTBXItem
          Action = CloseWinCmd
        end
        object wmSeparator3Item: TTBXSeparatorItem
        end
        object wmListItem: TTBXMDIWindowItem
        end
      end
      object HelpMenu: TTBXSubmenuItem
        object hmNSISHelp: TTBXItem
          Action = NSISHelpCmd
        end
        object hmSeparator1: TTBXSeparatorItem
        end
        object hmURLGroupItem: TTBGroupItem
        end
        object hmSeparator2: TTBXSeparatorItem
        end
        object hmAboutItem: TTBXItem
          Action = AboutCmd
        end
      end
    end
    object tbStandard: TTBXToolbar
      Left = 0
      Top = 10
      DockPos = 0
      DockRow = 1
      Images = MainImages
      TabOrder = 1
      object stbNewScriptItem: TTBXItem
        Action = NewScriptCmd
      end
      object stbWizardItem: TTBXItem
        Action = WizardCmd
      end
      object stbNewIOItem: TTBXItem
        Action = NewIOCmd
      end
      object stbOpenItem: TTBXSubmenuItem
        Action = OpenFileCmd
        DropdownCombo = True
        OnPopup = stbOpenItemPopup
        object TBMRUListItem2: TTBXMRUListItem
          MRUList = MRU
        end
      end
      object stbSeparator1: TTBXSeparatorItem
      end
      object stbSaveItem: TTBXItem
        Action = SaveFileCmd
      end
      object TBXSeparatorItem8: TTBXSeparatorItem
      end
      object stbSaveAllItem: TTBXItem
        Action = SaveAllCmd
      end
      object stbSeparator2: TTBXSeparatorItem
      end
      object stbPrintItem: TTBXItem
        Action = PrintCmd
      end
    end
    object tbEdit: TTBXToolbar
      Left = 200
      Top = 10
      DockPos = 146
      DockRow = 1
      Images = MainImages
      TabOrder = 2
      object etbUndoItem: TTBXItem
        Action = UndoCmd
      end
      object etbRedoItem: TTBXItem
        Action = RedoCmd
      end
      object etbSeparator1: TTBXSeparatorItem
      end
      object etbCutItem: TTBXItem
        Action = CutCmd
      end
      object etbCopyItem: TTBXItem
        Action = CopyCmd
      end
      object etbPasteItem: TTBXItem
        Action = PasteCmd
      end
    end
    object tbNSIS: TTBXToolbar
      Left = 0
      Top = 36
      Caption = '*'
      DockPos = -3
      DockRow = 2
      Images = MainImages
      TabOrder = 3
      object CompProfilesComboBox: TTBXComboBoxItem
        EditWidth = 100
        OnChange = CompProfilesComboBoxChange
      end
      object TBXItem12: TTBXItem
        Action = EditProfilesCmd
      end
      object TBXSeparatorItem9: TTBXSeparatorItem
      end
      object ntbCompItem: TTBXItem
        Action = CompScriptCmd
      end
      object ntbCompRunItem: TTBXItem
        Action = CompRunCmd
      end
      object ntbStopCompItem: TTBXItem
        Action = StopCompileCmd
      end
      object ntbSeparator1: TTBXSeparatorItem
      end
      object ntbRunItem: TTBXItem
        Action = RunInstallerCmd
      end
    end
    object tbFormat: TTBXToolbar
      Left = 495
      Top = 10
      DockPos = 440
      DockRow = 1
      Images = MainImages
      TabOrder = 4
      object ftbIndentItem: TTBXItem
        Action = IdentTextCmd
      end
      object ftbUnIndentItem: TTBXItem
        Action = UnIdentTextCmd
      end
      object ftbSeparator1: TTBXSeparatorItem
      end
      object ftbUpperCaseItem: TTBXItem
        Action = UpperCaseCmd
      end
      object ftbLowerCaseItem: TTBXItem
        Action = LowerCaseCmd
      end
      object ftbToggleCaseItem: TTBXItem
        Action = ToggleCaseCmd
      end
      object ftbQuoteItem: TTBXItem
        Action = CoteCmd
      end
      object TBXSeparatorItem7: TTBXSeparatorItem
      end
      object TBXItem11: TTBXItem
        Action = WordWrapCmd
      end
    end
    object tbSearch: TTBXToolbar
      Left = 331
      Top = 10
      DockPos = 276
      DockRow = 1
      Images = MainImages
      TabOrder = 5
      object stbFindItem: TTBXItem
        Action = FindCmd
      end
      object stbFindNextItem: TTBXItem
        Action = FindNextCmd
      end
      object stbReplaceItem: TTBXItem
        Action = ReplaceCmd
      end
      object stbSeparator3: TTBXSeparatorItem
      end
      object stbGotoLineItem: TTBXItem
        Action = GotoLineCmd
      end
    end
    object tbView: TTBXToolbar
      Left = 439
      Top = 10
      DockPos = 384
      DockRow = 1
      Images = MainImages
      TabOrder = 6
      object vtbSpecialCharsItem: TTBXItem
        Action = ViewSpecialCharsCmd
      end
      object vtbToggleDsgModeItem: TTBXItem
        Action = ToggleDesingModeCmd
      end
    end
    object tbWindow: TTBXToolbar
      Left = 237
      Top = 36
      DockPos = 80
      DockRow = 2
      Images = MainImages
      TabOrder = 7
      object wtbCascadeItem: TTBXItem
        Action = CascadeCmd
      end
      object wtbTileHorItem: TTBXItem
        Action = TileHorCmd
      end
      object wtbTileVerItem: TTBXItem
        Action = TileVerCmd
      end
      object wtbSeparator1: TTBXSeparatorItem
      end
      object wtbPriorItem: TTBXItem
        Action = PriorWindowCmd
      end
      object wtbNextItem: TTBXItem
        Action = NextWindowCmd
      end
      object wtbSeparator2: TTBXSeparatorItem
      end
      object wtbListItem: TTBXSubmenuItem
        object TBXMDIWindowItem1: TTBXMDIWindowItem
        end
      end
    end
  end
  object LeftDock: TTBXDock
    Left = 144
    Top = 62
    Width = 9
    Height = 169
    Position = dpLeft
  end
  object RightMultiDock: TTBXMultiDock
    Left = 505
    Top = 62
    Width = 175
    Height = 169
    BoundLines = [blRight]
    Position = dpRight
    object IOPanel: TTBXDockablePanel
      Left = 0
      Top = 0
      BorderSize = 3
      Color = clBtnFace
      DockedWidth = 170
      DockPos = 8
      SplitHeight = 52
      SupportedDocks = [dkMultiDock]
      TabOrder = 0
      Visible = False
      OnClose = IOPanelClose
      OnDockChanged = IOPanelDockChanged
      OnVisibleChanged = IOPanelDockChanged
      object Bevel1: TBevel
        Left = 3
        Top = 25
        Width = 164
        Height = 3
        Align = alTop
        Shape = bsSpacer
      end
      object IOCrtlPropList: TZPropList
        Left = 3
        Top = 28
        Width = 164
        Height = 102
        Middle = 74
        OnChange = IOCrtlPropListChange
        Align = alClient
        TabOrder = 0
      end
      object tbIOCtrls: TTBXToolbar
        Left = 3
        Top = 3
        Width = 164
        Height = 22
        Align = alTop
        DockMode = dmCannotFloatOrChangeDocks
        DragHandleStyle = dhNone
        FullSize = True
        Images = ControlsImages
        ShrinkMode = tbsmWrap
        TabOrder = 1
      end
    end
  end
  object LeftMultiDock: TTBXMultiDock
    Left = 0
    Top = 62
    Width = 144
    Height = 169
    Position = dpLeft
    object WinListPanel: TTBXDockablePanel
      Left = 0
      Top = 0
      BorderSize = 3
      Color = clBtnFace
      DockedWidth = 140
      DockPos = 0
      SupportedDocks = [dkMultiDock]
      TabOrder = 0
      OnDockChanged = IOPanelDockChanged
      OnVisibleChanged = IOPanelDockChanged
      object WinList: TTreeView
        Left = 3
        Top = 3
        Width = 134
        Height = 127
        Align = alClient
        HideSelection = False
        HotTrack = True
        Images = SystemImageList
        Indent = 19
        PopupMenu = WinListPopup
        ReadOnly = True
        RightClickSelect = True
        ShowButtons = False
        ShowLines = False
        TabOrder = 0
        OnChange = WinListChange
      end
    end
  end
  object RightDock: TTBXDock
    Left = 496
    Top = 62
    Width = 9
    Height = 169
    Position = dpRight
  end
  object BottomDock: TTBXDock
    Left = 0
    Top = 231
    Width = 680
    Height = 200
    Position = dpBottom
    object BrowserPanel: TTBXDockablePanel
      Left = 0
      Top = 0
      CaptionRotation = dpcrAlwaysHorz
      Color = clBtnFace
      DockedWidth = 170
      DockedHeight = 196
      DockPos = 0
      SupportedDocks = [dkStandardDock, dkMultiDock]
      TabOrder = 0
      Visible = False
      OnDockChanged = IOPanelDockChanged
      OnVisibleChanged = BrowserPanelVisibleChanged
      object tbBrowser: TTBXToolbar
        Left = 0
        Top = 0
        Width = 676
        Height = 22
        Align = alTop
        Images = MainImages
        TabOrder = 0
        object btbBackItem: TTBXItem
          ImageIndex = 36
          OnClick = btbBackItemClick
        end
        object btbForwardItem: TTBXItem
          ImageIndex = 37
          OnClick = btbForwardItemClick
        end
        object btbStopItem: TTBXItem
          ImageIndex = 38
          OnClick = btbStopItemClick
        end
        object btbRefreshItem: TTBXItem
          ImageIndex = 39
          OnClick = btbRefreshItemClick
        end
        object btbHomeItem: TTBXItem
          ImageIndex = 41
          OnClick = btbHomeItemClick
        end
        object btbSeparator1: TTBXSeparatorItem
        end
        object vtbEditURLItem: TTBControlItem
          Control = URLField
        end
        object btbGoItem: TTBXItem
          ImageIndex = 40
          OnClick = btbGoItemClick
        end
        object URLField: TEdit
          Left = 121
          Top = 0
          Width = 240
          Height = 21
          TabOrder = 0
          OnKeyDown = URLFieldKeyDown
        end
      end
      object BrowserStatusBar: TTBXStatusBar
        Left = 0
        Top = 158
        Width = 676
        Panels = <
          item
            StretchPriority = 75
            Tag = 0
            TextTruncation = twEndEllipsis
          end
          item
            Control = BrowserProgressBar
            Size = 100
            Tag = 0
          end
          item
            Framed = False
            StretchPriority = 100
            Tag = 0
          end>
        UseSystemFont = False
        object BrowserProgressBar: TProgressBar
          Left = 527
          Top = 3
          Width = 96
          Height = 18
          Min = 0
          Max = 100
          Smooth = True
          TabOrder = 0
          Visible = False
        end
      end
    end
  end
  object EstBar: TTBXStatusBar
    Left = 0
    Top = 431
    Width = 680
    Panels = <
      item
        Alignment = taCenter
        Size = 100
        Tag = 0
      end
      item
        Alignment = taCenter
        Size = 80
        Tag = 0
      end
      item
        Alignment = taCenter
        Size = 80
        Tag = 0
      end
      item
        StretchPriority = 100
        Tag = 0
        TextTruncation = twEndEllipsis
      end>
    UseSystemFont = False
  end
  object Actions: TActionList
    Images = MainImages
    Left = 192
    Top = 72
    object NewScriptCmd: TAction
      Category = 'File'
      ImageIndex = 0
      ShortCut = 16462
      OnExecute = NewScriptCmdExecute
    end
    object CloseWindowCmd: TAction
      Category = 'WinList'
      OnExecute = CloseWindowCmdExecute
    end
    object CopyLogBoxCmd: TEditCopy
      Category = 'Edit'
      Hint = 'Copy'
      ImageIndex = 8
      ShortCut = 16451
      OnExecute = CopyLogBoxCmdExecute
      OnUpdate = CopyLogBoxCmdUpdate
    end
    object OpenFileCmd: TAction
      Category = 'File'
      ImageIndex = 2
      ShortCut = 16463
      OnExecute = OpenFileCmdExecute
    end
    object SaveFileCmd: TAction
      Category = 'File'
      ImageIndex = 1
      ShortCut = 16467
      OnExecute = SaveFileCmdExecute
      OnUpdate = SaveFileCmdUpdate
    end
    object SaveFileAsCmd: TAction
      Category = 'File'
      OnExecute = SaveFileAsCmdExecute
      OnUpdate = SaveFileAsCmdUpdate
    end
    object ArrangleCmd: TWindowArrange
      Category = 'Window'
    end
    object CascadeCmd: TWindowCascade
      Category = 'Window'
      ImageIndex = 4
    end
    object TileHorCmd: TWindowTileHorizontal
      Category = 'Window'
      ImageIndex = 5
    end
    object TileVerCmd: TWindowTileVertical
      Category = 'Window'
      ImageIndex = 6
    end
    object PrintCmd: TAction
      Category = 'File'
      ImageIndex = 7
      ShortCut = 16464
      OnExecute = PrintCmdExecute
      OnUpdate = CheckEditNil
    end
    object ConfigPrintCmd: TAction
      Category = 'File'
      OnExecute = ConfigPrintCmdExecute
    end
    object RedoCmd: TAction
      Category = 'Edit'
      ImageIndex = 13
      ShortCut = 24666
      OnExecute = RedoCmdExecute
      OnUpdate = RedoCmdUpdate
    end
    object FindCmd: TAction
      Category = 'Search'
      ImageIndex = 15
      ShortCut = 16454
      OnExecute = FindCmdExecute
      OnUpdate = CheckEditNil
    end
    object FindNextCmd: TAction
      Category = 'Search'
      ImageIndex = 16
      ShortCut = 114
      OnExecute = FindNextCmdExecute
      OnUpdate = CheckEditNil
    end
    object ReplaceCmd: TAction
      Category = 'Search'
      ImageIndex = 14
      OnExecute = RemplasarDlgReplace
      OnUpdate = CheckEditNil
    end
    object LogBoxCmd: TAction
      Category = 'View'
      OnExecute = LogBoxCmdExecute
      OnUpdate = LogBoxCmdUpdate
    end
    object CompScriptCmd: TAction
      Category = 'NSIS'
      ImageIndex = 17
      ShortCut = 16504
      OnExecute = CompScriptCmdExecute
      OnUpdate = CompScriptCmdUpdate
    end
    object RunInstallerCmd: TAction
      Category = 'NSIS'
      ImageIndex = 18
      ShortCut = 120
      OnExecute = RunInstallerCmdExecute
      OnUpdate = RunInstallerCmdUpdate
    end
    object CompRunCmd: TAction
      Category = 'NSIS'
      ImageIndex = 42
      OnExecute = CompScriptCmdExecute
      OnUpdate = CompScriptCmdUpdate
    end
    object ConfigCmd: TAction
      Category = 'NSIS'
      ShortCut = 117
      OnExecute = ConfigCmdExecute
    end
    object InsertColorCmd: TAction
      Category = 'Tools'
      ImageIndex = 21
      OnExecute = InsertColorCmdExecute
      OnUpdate = CheckEditNil
    end
    object InsertFileNameCmd: TAction
      Category = 'Tools'
      OnExecute = InsertFileNameCmdExecute
      OnUpdate = CheckEditNil
    end
    object NSISHelpCmd: TAction
      Category = 'Help'
      OnExecute = NSISHelpCmdExecute
    end
    object AboutCmd: TAction
      Category = 'Help'
      OnExecute = AboutCmdExecute
    end
    object ExitCmd: TAction
      Category = 'File'
      ImageIndex = 3
      ShortCut = 32883
      OnExecute = ExitCmdExecute
    end
    object NewTemplateCmd: TAction
      Category = 'File'
      ImageIndex = 22
      ShortCut = 16453
      OnExecute = NewTemplateCmdExecute
    end
    object EditCodeTemplatesCmd: TAction
      Category = 'Tools'
      OnExecute = EditCodeTemplatesCmdExecute
    end
    object IdentTextCmd: TAction
      Category = 'Format'
      ImageIndex = 29
      ShortCut = 24649
      OnExecute = IdentTextCmdExecute
      OnUpdate = CheckEditSel
    end
    object UnIdentTextCmd: TAction
      Category = 'Format'
      ImageIndex = 28
      ShortCut = 24661
      OnExecute = UnIdentTextCmdExecute
      OnUpdate = CheckEditSel
    end
    object UpperCaseCmd: TAction
      Category = 'Format'
      ImageIndex = 27
      ShortCut = 16469
      OnExecute = UpperCaseCmdExecute
      OnUpdate = CheckEditNil
    end
    object LowerCaseCmd: TAction
      Category = 'Format'
      ImageIndex = 25
      ShortCut = 16460
      OnExecute = LowerCaseCmdExecute
      OnUpdate = CheckEditNil
    end
    object ToggleCaseCmd: TAction
      Category = 'Format'
      ImageIndex = 24
      OnExecute = ToggleCaseCmdExecute
      OnUpdate = CheckEditNil
    end
    object CoteCmd: TAction
      Category = 'Format'
      ImageIndex = 32
      ShortCut = 16465
      OnExecute = CoteCmdExecute
      OnUpdate = CheckEditNil
    end
    object NewIOCmd: TAction
      Category = 'File'
      ImageIndex = 23
      ShortCut = 16457
      OnExecute = NewIOCmdExecute
    end
    object CopyAsHTMLCmd: TAction
      Category = 'Edit'
      OnExecute = CopyAsHTMLCmdExecute
      OnUpdate = CopyAsHTMLCmdUpdate
    end
    object SendToBackCmd: TAction
      Category = 'IODesign'
      ImageIndex = 30
      OnExecute = ExecuteDesingAction
      OnUpdate = UpdateDesignAction
    end
    object BringToFrontCmd: TAction
      Category = 'IODesign'
      ImageIndex = 31
      OnExecute = ExecuteDesingAction
      OnUpdate = UpdateDesignAction
    end
    object DeleteControlCmd: TAction
      Category = 'IODesign'
      ImageIndex = 10
      OnExecute = ExecuteDesingAction
      OnUpdate = UpdateDesignAction
    end
    object ViewIOPanelCmd: TAction
      Category = 'View'
      OnExecute = ViewIOPanelCmdExecute
    end
    object CopyCmd: TEditCopy
      Category = 'Edit'
      Hint = 'Copy'
      ImageIndex = 8
      ShortCut = 16451
    end
    object CutCmd: TEditCut
      Category = 'Edit'
      Hint = 'Cut'
      ImageIndex = 9
      ShortCut = 16472
    end
    object PasteCmd: TEditPaste
      Category = 'Edit'
      Hint = 'Paste'
      ImageIndex = 11
      ShortCut = 16470
    end
    object SelectAllCmd: TEditSelectAll
      Category = 'Edit'
      ShortCut = 16449
    end
    object UndoCmd: TEditUndo
      Category = 'Edit'
      ImageIndex = 12
    end
    object ToggleDesingModeCmd: TAction
      Category = 'IODesign'
      ImageIndex = 35
      ShortCut = 123
      OnExecute = ToggleDesingModeCmdExecute
      OnUpdate = ToggleDesingModeCmdUpdate
    end
    object GotoLineCmd: TAction
      Category = 'Search'
      ImageIndex = 33
      OnExecute = GotoLineCmdExecute
      OnUpdate = CheckEditNil
    end
    object ViewSpecialCharsCmd: TAction
      Category = 'View'
      ImageIndex = 34
      OnExecute = ViewSpecialCharsCmdExecute
      OnUpdate = ViewSpecialCharsCmdUpdate
    end
    object ShowOptionsCmd: TAction
      Category = 'View'
      OnExecute = ConfigCmdExecute
    end
    object NextWindowCmd: TAction
      Category = 'Window'
      ImageIndex = 37
      OnExecute = ExecuteWindowCmd
      OnUpdate = UpdateWindowCmd
    end
    object PriorWindowCmd: TAction
      Category = 'Window'
      ImageIndex = 36
      OnExecute = ExecuteWindowCmd
      OnUpdate = UpdateWindowCmd
    end
    object ShowWindowCmd: TAction
      Category = 'WinList'
      OnExecute = ShowWindowCmdExecute
    end
    object CompWinCmd: TAction
      Category = 'WinList'
      ImageIndex = 17
      OnExecute = CompWinCmdExecute
      OnUpdate = CompWinCmdUpdate
    end
    object CompRunWinCmd: TAction
      Category = 'WinList'
      ImageIndex = 42
      OnExecute = CompWinCmdExecute
      OnUpdate = CompWinCmdUpdate
    end
    object RunWinCmd: TAction
      Category = 'WinList'
      ImageIndex = 18
      OnExecute = RunWinCmdExecute
      OnUpdate = RunWinCmdUpdate
    end
    object SaveWinCmd: TAction
      Category = 'WinList'
      ImageIndex = 1
      OnExecute = SaveWinCmdExecute
      OnUpdate = SaveWinCmdUpdate
    end
    object SaveAsWinCmd: TAction
      Category = 'WinList'
      OnExecute = SaveAsWinCmdExecute
    end
    object CloseWinCmd: TWindowClose
      Category = 'Window'
    end
    object WizardCmd: TAction
      Category = 'File'
      ImageIndex = 20
      ShortCut = 16471
      OnExecute = WizardCmdExecute
    end
    object SetTabOrderCmd: TAction
      Category = 'IODesign'
      ImageIndex = 44
      OnExecute = SetTabOrderCmdExecute
      OnUpdate = SetTabOrderCmdUpdate
    end
    object CMRWindowCmd: TAction
      Category = 'Window'
      OnExecute = CMRWindowCmdExecute
      OnUpdate = CMRWindowCmdUpdate
    end
    object StopCompileCmd: TAction
      Category = 'NSIS'
      ImageIndex = 46
      OnExecute = StopCompileCmdExecute
      OnUpdate = StopCompileCmdUpdate
    end
    object WordWrapCmd: TAction
      Category = 'Edit'
      ImageIndex = 47
      OnExecute = WordWrapCmdExecute
      OnUpdate = WordWrapCmdUpdate
    end
    object SaveAllCmd: TAction
      Category = 'File'
      ImageIndex = 48
      OnExecute = SaveAllCmdExecute
      OnUpdate = SaveAllCmdUpdate
    end
    object EditProfilesCmd: TAction
      Category = 'NSIS'
      ImageIndex = 50
      OnExecute = EditProfilesCmdExecute
    end
  end
  object OpenDlg: TOpenDialog
    Options = [ofHideReadOnly, ofAllowMultiSelect, ofPathMustExist, ofFileMustExist, ofEnableSizing]
    Left = 160
    Top = 104
  end
  object SaveDlg: TSaveDialog
    Options = [ofOverwritePrompt, ofHideReadOnly, ofPathMustExist, ofEnableSizing]
    OnTypeChange = SaveDlgTypeChange
    Left = 192
    Top = 104
  end
  object TBMDIHandler1: TTBXMDIHandler
    Toolbar = tbMenu
    Left = 160
    Top = 72
  end
  object MRU: TTBXMRUList
    HidePathExtension = False
    MaxItems = 9
    OnClick = MRUClick
    Prefix = 'MRU'
    Left = 160
    Top = 168
  end
  object SynNSIS: TSynNSISSyn
    DefaultFilter = ' '
    HighlightVarsInsideStrings = True
    Left = 224
    Top = 136
  end
  object SynIni: TSynIniSyn
    DefaultFilter = ' '
    Left = 192
    Top = 136
  end
  object LogBoxPopup: TTBXPopupMenu
    Images = MainImages
    Left = 224
    Top = 104
    object Copiaraportapeles1: TTBXItem
      Action = CopyLogBoxCmd
    end
    object TBXSeparatorItem4: TTBXSeparatorItem
    end
    object SaveLogItem: TTBXItem
      ImageIndex = 1
      OnClick = SaveLogItemClick
    end
  end
  object EditPopup: TTBXPopupMenu
    Images = MainImages
    OnPopup = EditPopupPopup
    Left = 160
    Top = 136
    object puInsertColorItem: TTBXItem
      Action = InsertColorCmd
    end
    object puInsertFileNameItem: TTBXItem
      Action = InsertFileNameCmd
    end
    object puSeparator1: TTBXSeparatorItem
    end
    object puTemplateItem: TTBXSubmenuItem
      LinkSubitems = tmTemplateItem
    end
    object puSeparator2: TTBXSeparatorItem
    end
    object puUndoItem: TTBXItem
      Action = UndoCmd
    end
    object puRedoItem: TTBXItem
      Action = RedoCmd
    end
    object puSeparator3: TTBXSeparatorItem
    end
    object puCutItem: TTBXItem
      Action = CutCmd
    end
    object puCopyItem: TTBXItem
      Action = CopyCmd
    end
    object puCopyHTMLItem: TTBXItem
      Action = CopyAsHTMLCmd
    end
    object puPasteItem: TTBXItem
      Action = PasteCmd
    end
    object puSelectAllItem: TTBXItem
      Action = SelectAllCmd
    end
    object puSeparator4: TTBXSeparatorItem
    end
    object puFindItem: TTBXItem
      Action = FindCmd
    end
    object puFindNextItem: TTBXItem
      Action = FindNextCmd
    end
    object puReplaceItem: TTBXItem
      Action = ReplaceCmd
    end
    object puSeparator5: TTBXSeparatorItem
    end
    object puToggleBookmarkItem: TTBXSubmenuItem
      LinkSubitems = smToggleBookmarkItem
    end
    object puGotoBookmarkItem: TTBXSubmenuItem
      LinkSubitems = smGotoBookmarkItem
    end
  end
  object IOCtrlFlagsPopup: TTBXPopupMenu
    Left = 192
    Top = 168
  end
  object DesignMenu: TTBXPopupMenu
    Images = MainImages
    OnPopup = DesignMenuPopup
    Left = 224
    Top = 168
    object TBXItem2: TTBXItem
      Action = BringToFrontCmd
    end
    object TBXItem3: TTBXItem
      Action = SendToBackCmd
    end
    object TBXItem1: TTBXItem
      Action = SetTabOrderCmd
    end
    object TBXSeparatorItem2: TTBXSeparatorItem
    end
    object TBXItem9: TTBXItem
      Action = UndoCmd
    end
    object TBXSeparatorItem5: TTBXSeparatorItem
    end
    object TBXItem5: TTBXItem
      Action = CutCmd
    end
    object TBXItem4: TTBXItem
      Action = CopyCmd
    end
    object TBXItem6: TTBXItem
      Action = PasteCmd
    end
    object TBXItem8: TTBXItem
      Action = SelectAllCmd
    end
    object TBXSeparatorItem3: TTBXSeparatorItem
    end
    object TBXItem7: TTBXItem
      Action = DeleteControlCmd
    end
    object TBXSeparatorItem1: TTBXSeparatorItem
    end
    object iopResizeItem: TTBXSubmenuItem
    end
  end
  object EditorOptions: TMySynEditorOptionsContainer
    Options = [eoAltSetsColumnMode, eoAutoIndent, eoDragDropEditing, eoDropFiles, eoKeepCaretX, eoShowScrollHint, eoSmartTabs, eoTabsToSpaces, eoTrimTrailingSpaces]
    Color = clWindow
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -13
    Font.Name = 'Courier New'
    Font.Style = []
    ExtraLineSpacing = 0
    Gutter.Font.Charset = DEFAULT_CHARSET
    Gutter.Font.Color = clWindowText
    Gutter.Font.Height = -11
    Gutter.Font.Name = 'Terminal'
    Gutter.Font.Style = []
    Gutter.Width = 20
    RightEdge = 0
    RightEdgeColor = clSilver
    WantTabs = True
    InsertCaret = ctVerticalLine
    OverwriteCaret = ctBlock
    HideSelection = False
    MaxUndo = 1024
    TabWidth = 8
    Keystrokes = <>
    Left = 192
    Top = 200
  end
  object MainImages: TImageList
    Left = 224
    Top = 72
    Bitmap = {
      494C010133003600040010001000FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      000000000000360000002800000040000000E000000001001000000000000070
      0000000000000000000000000000000000009B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B73EF3DEF3D00009B739B73EF3DEF3DE00100009B739B739B73
      9B739B739B739B739B739B739B73000000000000000000000000000000000000
      000000009B739B739B739B739B739B739B73F75EEF3DF75EEF3D9B739B739B73
      9B739B739B739B739B739B73EF3DF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75E00009B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B739B739B739B739B739B739B739B730000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000BD777B6F9452AD359B739B739B739B739B739B739B73
      9B739B739B739B730000E03DE03D0000000000000000E03DE03D000000000000
      0000F75E0000E03D000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000094527B6F186318639452AD35EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FF75E00009B739B739B739B739B739B7300000000E03DE03D000000000000
      0000F75E0000E03D000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF7F0000000018637B6F
      186318631863EF3D8B2DEF3D0000BD777B6F9B739B739B739B739B739B739B73
      9B739B739B739B739B739B7300000000000000000000E03DE03D000000000000
      000000000000E03D0000E03D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000734E000018637B6F0000AB2D
      8B2D8B2DEF3D94521863BD777B6F396718639B739B739B739B739B739B739B73
      9B73003C007C003CEF3DEF3DEF3DEF3DEF3D00000000E03DE03DE03DE03DE03D
      E03DE03DE03DE03D0000E03D0000000000000000000000000000000000000000
      000000000000000000000000000000000000B5568450C61800006B31EF3D0000
      BD77BD777B6F7B6F9C73186339671863945200000000F75EE03DE07FE03D0000
      FF7FF75E00009B739B739B739B739B739B7300000000E03DE03D000000000000
      00000000E03DE03D0000E03D0000E03D00000000000000000000000000000000
      0000000000000000000000000000000000004230A57800008C310000BD777B6F
      7B6F7B6F7B6F7B6F7B6F9C731863945200009B739B739B739B739B739B739B73
      9B739B739B739B739B739B730000E003E00300000000E03D0000F75EF75EF75E
      F75EF75E0000E03D0000E03D0000E03D00000000000000000000000000000000
      0000000000000000000000000000000000004230A57800000000BD777B6F1863
      00006B2D8C3118637B6F7B6FD65A000000009B739B739B739B739B739B730000
      003C003C003C9B73EF3D0000EF3DEF3DF75E00000000E03D0000F75EF75EF75E
      F75EF75E0000E03D0000E03D0000E03D00000000000000000000000000000000
      0000000000000000000000000000000000008C31000000000000186300000000
      0000000000008C317B6F7B6F94520000000000000000F75EE03DE07FE03D0000
      FF7FF75E00009B739B739B739B739B739B7300000000E03D0000F75EF75EF75E
      F75EF75E000000000000E03D0000E03D00000000000000000000000000000000
      0000000000000000000000000000000000008C3100000000000000008C311042
      00000000F75E8C317B6F18630000186300009B730000000000009B739B739B73
      9B739B739B739B739B739B7300000000000000000000E03D0000F75EF75EF75E
      F75EF75E0000F75E0000E03D0000E03D00000000000000000000000000000000
      0000000000000000000000000000000000008C31000000000000000000000000
      F75EEF3D8C317B6F186394520000000000009B739B739B739B739B739B73003C
      00009B739B739B73EF3DF75EF75EF75EF75E0000000000000000000000000000
      0000000000000000000000000000E03D00000000000000000000000000000000
      0000000000000000000000000000000000008C31DE7B5A6B4A29841000001042
      8C317B6F1863945200000000000000000000000000000000000000000000FF7F
      FF7FF75E00009B739B739B739B739B739B730000000000000000E03D0000F75E
      F75EF75EF75EF75E0000F75E0000E03D00000000000000000000000000000000
      0000000000000000000000000000000000000821B5563146DE7B000000000000
      000000000000000000000000000000000000000000009B739B739B739B739B73
      9B739B739B739B739B739B730000EF3DEF3D0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000004A29DE7B00000000000000000000
      5A6B8C3100000000000000000000000000009B739B739B739B73F75EF75EF75E
      F75EF75EF75E00009B739B739B739B739B73000000000000000000000000E03D
      0000F75EF75EF75EF75EF75E0000F75E00000000000000000000000000000000
      000000000000000000000000000000000000D65A734E186300000000BD77F75E
      082100000000000000000000000000000000000000000000FF7F00000000FF7F
      FF7FF75E00009B739B739B739B739B739B730000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000D65A524A8C318C31AD35734E
      0000000000000000000000000000000000009B739B739B739B739B739B739B73
      9B739B739B739B739B739B730000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000009B739B739B739B73F75EF75EEF3D
      EF3DEF3DF75E00009B739B739B739B739B730000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000EF3D0000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000FF7F
      F75EE07FF75EE07FF75EE07FF75EE07F0000000000000000000000001F001F00
      1F001F001F000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000FF7F
      E07FF75EE07F000000000000E07FF75E000000000000E001E001E00100001F00
      1F001F001F001F001F0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3D00000000FF7F
      F75EE07FF75EE07F0000E07FF75EE07F00000000E001E003E003E003E0010000
      FF031F001F00E001E00100000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FF7F0000FF7F
      E07FF75EE07FF75E0000F75EE07FF75E00000000E001FF7FE003E003E0010000
      FF03FF03E001E001E001E0010000000000000000000000000000EF3D00000000
      0000EF3D00000000000000000000000000000000000000000000000000000000
      0000000000000F00000000000000000000000000000000000000FF7F0000FF7F
      F75EE07FF75E00000000E07FF75EE07F0000E001FF7FFF7FE003E001E001FF03
      FF03E001E003E001E001E00100000000000000000000EF3D00000000FF03FF03
      FF030000EF3D0000000000000000000000000000000000000000000000000000
      000000000F000F00000000000000000000000000EF3D00000000FF7F0000FF7F
      E07FF75EFF7FFF7FFF7FFF7FFF7FFF7F0000E001FF7FE003E001FF03FF03FF03
      FF03E001FF7FE003E001E001E001000000000000EF3D0000EF01FF03FF03FF03
      FF03FF030000EF3D000000000000000000000000000000000000000000000000
      00000F000F000F000F000F0000000000000000000000FF7F0000FF7F0000FF7F
      FF7FFF7F000000000000000000000000EF3D1F00E001E001FF03FF03FF03FF03
      FF03FF03E001FF7FE003E001E00100000000EF3D0000000000000000FF03FF03
      FF03FF03FF030000000000000000000000000000000000000000000000000000
      000000000F000F00000000000F000000000000000000FF7F0000FF7FE07F0000
      00000000E07FF75EE07FF75E0000000000001F00FF7FFF03FF03FF03FF03FF03
      FF03FF031F00E001FF7FE001E001000000000000007C007C007C007C003C0000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000F00000000000F000000000000000000FF7F0000FF7FE07FF75E
      FF7FFF7FFF7FFF7FFF7FFF7F0000000000001F00FF7FFF03E0011F00FF03FF03
      FF03FF031F001F00E001E003E00100000000003C007C007C007C007C007C003C
      FF7FFF7FFF7FEF3D000000000000000000000000000000000000000000000000
      0000000000000000000000000F000000000000000000FF7F0000FF7FFF7FFF7F
      000000000000000000000000EF3D000000000000E001E001E003E0011F00FF03
      FF03FF03FF031F001F00E001000000000000EF3D007CFF7F007CFF7F007C007C
      0000EF3DEF3DEF3D000000000000000000000000000000000000000000000000
      0000000000000000000000000F000000000000000000FF7FF75E000000000000
      F75EE07FF75EE07F000000000000000000000000E001FF7FFF7FE003E001FF03
      FF03FF03FF031F001F001F00000000000000EF3D007C007CFF7F007C007C007C
      0000FF030000EF3D000000000000000000000000000000000000000000000F00
      0F000F000F000F000F000F0000000000000000000000FF7FE07FF75EFF7FFF7F
      FF7FFF7FFF7FFF7F0000000000000000000000000000E001FF7FFF7FE003E001
      E001E001FF031F001F000000000000000000EF3D007CFF7F007CFF7F007C007C
      00000000EF3D0000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF7FFF7FFF7F00000000
      0000000000000000EF3D0000000000000000000000000000E001E001FF7FFF7F
      E003E001E0011F001F000000000000000000003CE07F007C007C007C007C003C
      EF3DEF3D00000000000000000000EF3D00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000E001E001
      E001E0011F000000000000000000000000000000003CEF3DEF3DEF3D00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000E00100000000
      00000000000000000000000000000000000000000000EF3DF75EF75EF75EE03D
      E03DE03D0000F75EF75EF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000E001EF01000000000000
      00000000000000000000000000000000000000000000EF3DFF7FF75EFF7FE03D
      E07FE03D0000F75EFF7FF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000E001E003000000000000
      00000000000000000000000000000000000000000000EF3DFF7FFF7FF75EE03D
      E07FE07F0000FF7FF75EF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FF7FFF7FFF7F
      0000FF7FFF7F00000000000000000000000000000000E001E003000000000000
      00000000000000000000000000000000000000000000EF3DFF7FF75EFF7FE03D
      F75EE07F0000F75EFF7FF75E0000000000000000000000000000EF3D00000000
      0000EF3D00000000000000000000000000000000000000000000FF7FFF7FFF7F
      0000FF7FFF7F00000000000000000000000000000000E001F75E000000000000
      00000000000000000000000000000000000000000000EF3DFF7FFF7FF75EE03D
      E03DE03D0000FF7FF75EF75E00000000000000000000EF3D00000000FF03FF03
      FF030000EF3D00000000000000000000000000000000000000000000FF7FFF7F
      FF7F0000FF7FFF7F0000000000000000000000000000E001FF7FEF0100000000
      000000000000E00100000000000000000000000000000000F75EFF7FFF7FF75E
      FF7FF75EFF7FF75EFF7FF75E0000000000000000EF3D0000EF01FF03FF03FF03
      FF03FF030000EF3D000000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7F0000FF7FFF7F0000000000000000000000000000E001F75EEF010000
      000000000000EF01E00100000000000000000000E03DF75E0000F75EFF7FFF7F
      F75EFF7FF75EFF7FF75E0000E07F00000000EF3D00000000FF03FF03FF03FF03
      FF03FF03FF0300000000000000000000000000000000000000000000FF7FFF7F
      FF7FFF7F0000FF7FFF7F0000000000000000000000000000E001FF7FF75EE003
      E001E001EF01EF01EF01E00100000000000000000000E03DF75E0000F75EFF7F
      FF7FF75EFF7FF75E0000E07F0000000000000000E00300000000FF03FF03EF3D
      00000000000000000000000000000000000000000000000000000000FF7FFF7F
      00000000FF7F0000000000000000000000000000000000000000E001FF7FFF7F
      F75EE003E003E003E003EF01E00100000000000000000000E03DF75E0000F75E
      FF7FFF7FF75E0000E07F000000000000000000000000E00300000000FF03EF3D
      FF7FFF7FFF7FEF3D00000000000000000000000000000000000000000000FF7F
      FF7F0000FF7FFF7F0000000000000000000000000000000000000000E001E001
      FF7FFF7FFF7FF75EE003E0030000000000000000000000000000E03DF75E0000
      F75EF75E0000E07F0000007C0000000000000000E0030000E00300000000EF3D
      EF3DEF3DEF3DEF3D000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      E001E001E001FF7FF75E000000000000000000000000000000000000E03DF75E
      00000000E07F0000003C007C00000000000000000000E0030000E003FF03FF03
      FF03FF030000EF3D000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000E001FF7F00000000000000000000000000000000000000000000E03D
      F75EE07F00000000003C007C000000000000EF3DE0030000E003FF7FFF03FF03
      FF030000EF3D0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000E0010000000000000000000000000000000000000000000000000000
      E03D000000000000003C003C00000000000000000000E003FF7FEF01EF010000
      0000EF3D00000000000000000000EF3D00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000E0010000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000E003EF3D000000000000EF3D
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000003C003C
      003C003C003C003C000000000000000000000000EF3DF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000003C007C007C007C007C
      007C007C007C003C003C003C0000000000000000EF3DFF7FFF7FF75EFF7FF75E
      E001F75EFF7FF75EFF7FF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000003C007C007C007C007C007C
      007C007C007C007C007C003C003C000000000000EF3DFF7FF75EFF7FF75EE001
      E001FF7FF75EFF7FF75EF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000003C007C007C007C007C007C
      007C007C007C007C007C007C003C000000000000EF3DFF7FFF7FF75EE001E001
      E001E001E001F75EFF7FF75E00000000000000000000000000000000FF030000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000001F0000000000000000000000003CEF3D007C007C007C007C007C
      007C007C007C007C007C007C003C003C00000000EF3DFF7FF75EFF7FF75EE001
      E001FF7FEF01EF01F75EF75E0000000000000000000000000000FF03FF030000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000FF031F000000000000000000003CEF3DE07F007C007CFF7FFF7F
      007C007CFF7FFF7F007C007C007C003C00000000EF3DFF7FFF7FF75EFF7FF75E
      E001F75EFF7FEF01FF7FF75E000000000000000000000000FF03FF03FF03FF03
      FF03FF03FF03FF03FF03FF03000000000000000000001F00FF03FF03FF03FF03
      FF03FF03FF03FF03FF031F00000000000000003CEF3DE07F007C007C007CFF7F
      FF7FFF7FFF7F007C007C007C007C003C00000000EF3DFF7FF75EEF01F75EFF7F
      F75EFF7FF75EEF01F75EF75E000000000000000000001F00FF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF03000000000000000000001F00FF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FF75EFF03000000000000003CEF3DE07F007C007C007C007C
      FF7FFF7F007C007C007C007C007C003C00000000EF3DFF7FFF7FEF01FF7FF75E
      E001F75EFF7FF75EFF7FF75E0000000000000000000000001F00FF7FFF7F0000
      1F001F001F001F001F001F00000000000000000000001F001F001F001F001F00
      1F001F001F00FF7FFF030000000000000000003CEF3DE07F007C007C007CFF7F
      FF7FFF7FFF7F007C007C007C007C003C00000000EF3DFF7FF75EEF01EF01FF7F
      E001E001F75EFF7FF75EF75E00000000000000000000000000001F00FF7F0000
      0000000000000000000000000000000000000000000000000000000000000000
      000000001F00FF0300000000000000000000003CEF3DE07F007C007CFF7FFF7F
      007C007CFF7FFF7F007C007C007C003C00000000EF3DFF7FFF7FF75EE001E001
      E001E001E001F75EFF7FF75E000000000000000000000000000000001F000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000001F000000000000000000000000000000003CEF3DE07F007C007C007C
      007C007C007C007C007C007C007C000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      E001E001F75EFF7FF75EF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000001F000000000000000000000000000000003CEF3DE07FE07F007C007C
      007C007C007C007C007C007C003C000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      E001F75EFF7F0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000003CEF3DEF3DE07FE07F
      007C007C007C007C007C007C0000000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      F75EFF7FF75EEF3DFF7F00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000003C003CEF3DEF3D
      EF3DEF3DEF3D007C000000000000000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FEF3D000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000003C003C
      003C003C003C003C000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3D000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000EF3D
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3D00000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000EF3D
      F75EF75EF75EF75EF75EF75EF75EF75E00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000
      0F00000000000F0000000000000000000000000000000000000000000000EF3D
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75E0000000000000000000000000000EF3D
      00000000EF3D0000000000000000000000000000000000000000EF3DFF7FEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DFF7F00000000000000000000000000000000
      0F00000000000F0000000000000000000000000000000000000000000000EF3D
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75E000000000000000000000000EF3D0000
      000000000000EF3D000000000000000000000F00000000000000EF3DFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000
      0F00000000000F0000000000000000000000000000000000000000000000EF3D
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75E00000000000000000000000000000000
      F75E0000EF3DEF3D000000000000000000000F000F0000000000EF3DFF7FEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DFF7F00000000000000000000000000000000
      0F00000000000F0000000000000000000000000000000000000000000000EF3D
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75E00000000000000000000000000000000
      0000000000000000000000000000000000000F000F000F000000EF3DE07FE07F
      E07FE07FE07FE07FE07FE07FE07FE07F0000000000000000000000000F000F00
      0F00000000000F0000000000000000000000EF3D00000000000000000000EF3D
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75E000000000000000000000000EF3D0000
      EF3D000000000000000000000000000000000F000F000F000000EF3DE07FE07F
      E07FE07FE07FE07FE07FE07FE07FE07F000000000000000000000F000F000F00
      0F00000000000F0000000000000000000000EF3DFF7FF75EF75EF75EF75EEF3D
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75E000000000000000000000000F75E0000
      0000EF3DF75E0000000000000000000000000F000F0000000000EF3DE07FE07F
      E07FE07FE07FE07FE07FE07FE07FE07F00000000000000000F000F000F000F00
      0F00000000000F0000000000000000000000EF3DFF7FF75EF75EF75EF75EEF3D
      FF7FFF7FFF7FFF7FFF7F0000000000000000000000000000000000000000EF3D
      0000000000000000000000000000000000000F00000000000000EF3DFF7FEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DFF7F00000000000000000F000F000F000F00
      0F00000000000F0000000000000000000000EF3DFF7FF75EF75EF75EF75EEF3D
      FF7FFF7FFF7FFF7FFF7FEF3DFF7F000000000000000000000000000000000000
      EF3D000000000000000000000000000000000000000000000000EF3DFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000000000000F000F000F000F00
      0F00000000000F0000000000000000000000EF3DFF7FF75EF75EF75EF75EEF3D
      FF7FFF7FFF7FFF7FFF7FEF3D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DFF7FEF3D
      EF3DEF3DEF3DFF7FFF7FFF7FFF7FFF7F000000000000000000000F000F000F00
      0F00000000000F0000000000000000000000EF3DFF7FF75EF75EF75EF75EEF3D
      EF3DEF3DEF3DEF3DEF3DEF3D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DFF7FFF7F
      FF7FFF7FFF7FFF7FFF7F0000000000000000000000000000000000000F000F00
      0F000F000F000F000F000F00000000000000EF3DFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7F0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DFF7FFF7F
      FF7FFF7FFF7FFF7FFF7F0000FF7F000000000000000000000000000000000000
      000000000000000000000000000000000000EF3D000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DFF7FFF7F
      FF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000000000000000
      000000000000000000000000000000000000EF3D0F000F000F000F000F000F00
      0F000F000F000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3D00000000000000000000000000000000000000000000
      000000000000000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3D0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3D000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3DFF7FF75EFF7F
      F75EFF7FF75EFF7F0000000000000000000000000000007C007C007C007C0000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3DF75EFF7FF75E
      FF7FF75EFF7FF75E00000000000000000000000000000000007C007C007C0000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000EF3D00000000EF3DFF7FF75EFF7F
      F75EFF7FF75EFF7F0000000000000000000000000000007C007C007C007C0000
      EF3D000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000EF3DFF7FFF03EF3DF75EFF7FF75E
      FF7FF75EFF7FF75E000000000000000000000000007C007C007C0000007C0000
      EF3DFF7FFF03FF7FFF03FF7FFF03FF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000EF3DFF03FF7FEF3DFF7FF75EFF7F
      F75EFF7FF75EFF7F000000000000000000000000007C00000000000000000000
      EF3DFF0300000000000000000000FF0300000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000EF3DFF7FFF03EF3DF75EFF7FF75E
      FF7F000000000000000000000000000000000000007C00000000EF3D00000000
      EF3DFF7FFF03FF7FFF03FF7FFF03FF7F0000000000000F000000000000000000
      000000000000000000000000000000000000000000000F000000000000000000
      000000000000000000000000000000000000EF3DFF03FF7FEF3DFF7FF75EFF7F
      F75E0000F75E0000000000000000000000000000000000000000EF3DFF7FF75E
      EF3DFF0300000000000000000000FF03000000000F000F000000000000000000
      000000000000000000000000000000000000000000000F000F00000000000000
      000000000000000000000000000000000000EF3DFF7FFF03EF3DF75EFF7FF75E
      FF7F000000000000000000000000000000000000000000000000EF3DF75EFF7F
      EF3DFF7FFF03FF7FFF03FF7FFF03FF7F00000F000F000F000F000F0000000000
      0000000000000000000000000000000000000F000F000F000F000F0000000000
      000000000000000000000000000000000000EF3DFF03FF7FEF3DEF3DEF3DEF3D
      EF3DEF3D00000000000000000000007C00000000000000000000EF3DFF7FF75E
      EF3DFF03FF7FFF03FF7F000000000000000000000F000F000000000000000000
      000000000000000000000000000000000000000000000F000F00000000000000
      000000000000000000000000000000000000EF3DFF7FFF03FF7FFF030000FF7F
      0000000000000000000000000000007C00000000000000000000EF3DF75EFF7F
      EF3DFF7FFF03FF7FFF030000F75E00000000000000000F000000000000000000
      000000000000000000000000000000000000000000000F000000000000000000
      000000000000000000000000000000000000EF3DFF03FF7FFF03FF7F00000000
      000000000000007C0000007C007C007C00000000000000000000EF3DFF7FF75E
      EF3DFF03FF7FFF03FF7F00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3D0000
      000000000000007C007C007C007C000000000000000000000000EF3DF75EFF7F
      EF3DEF3DEF3DEF3DEF3DEF3D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000007C007C007C0000000000000000000000000000EF3DFF7FF75E
      FF7FF75E0000FF7F000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000007C007C007C007C000000000000000000000000EF3DF75EFF7F
      F75EFF7F00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DEF3DEF3D
      EF3DEF3DEF3D0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DEF3DEF3D
      EF3DEF3D00000000EF3D00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000EF01EF01EF01EF010000EF01EF010000000000000000EF3D00000000
      000000000000EF3D000000000000000000000000000000000000000000000000
      0000000000000000000000000000EF3D0000000000000000EF3D000000000000
      00000000000000000000EF3D00000000000000000000EF3D0000000000000000
      0000EF01EF0100000000EF01EF01EF0100000000000000000000000000000000
      000000000000EF3D0000000000000000000000000000EF3D0000000000000000
      00000000EF3D00000000EF3D00000000EF3D0000000000000000EF3D00000000
      00000000000000000000EF3D000000000000000000000000EF3D000000000000
      0000EF01EF3D000000000000EF01EF0100000000000000000000000000000000
      000000000000EF3D00000000000000000000000000000000EF3D000000000000
      00000000EF3D00000000EF3D00000000EF3D0000000000000000000000000000
      00000000000000000000EF3D0000000000000000000000000000000000000000
      0000EF01EF01000000000000EF01EF0100000000000000000000EF3D00000000
      000000000000EF3D000000000000000000000000000000000000000000000000
      00000000EF3D00000000EF3D00000000EF3D000000000000000000000000EF3D
      00000000000000000000EF3D00000000000000000000000000000000EF3D0000
      00000000EF01EF01EF01EF01EF01EF0100000000000000000000000000000000
      000000000000EF3D0000000000000000000000000000000000000000EF3D0000
      00000000EF3D00000000EF3D00000000EF3D0000000000000000000000000000
      EF3D0000000000000000EF3D000000000000000000000000000000000000EF3D
      00000000EF3D000000000000EF01EF0100000000000000000000000000000000
      000000000000EF3D00000000000000000000000000000000000000000000EF3D
      00000000EF3D0000000000000000000000000000000000000000000000000000
      0000EF3D000000000000EF3D0000000000000000000000000000000000000000
      EF3DEF01EF01000000000000EF01EF0100000000000000000000000000000000
      000000000000EF3D000000000000000000000000000000000000000000000000
      EF3D0000EF3D00000000EF3D0000000000000000000000000000000000000000
      00000000EF3D00000000EF3D0000000000000000000000000000000000000000
      00000000EF01EF01EF01EF01EF01000000000000000000000000EF3D00000000
      000000000000EF3D000000000000000000000000000000000000000000000000
      00000000EF3D00000000EF3D0000000000000000000000000000000000000000
      000000000000EF3D0000EF3D0000000000000000000000000000000000000000
      00000000EF3D0000000000000000000000000000000000000000000000000000
      00000000E0010000000000000000000000000000000000000000000000000000
      00000000000000000000EF3D0000000000000000000000000000000000000000
      00000000000000000000EF3D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000EF3D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      EF3DEF3D000000000000EF3DEF3DE00100000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000E03DE03D00000000F75EEF3DF75EEF3D0000EF3DF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75E00000000000000000000EF3D0000000000000000
      0000EF3D00000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000EF3D
      F75EF75EF75EF75EF75EE003EF3DF75EEF3D0000EF3DFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FF75E0000000000000000EF3D003CEF3D000000000000
      EF3D003CEF3D00000000000000000000000000000000E003E003E0030000FF03
      FF03FF0300000000000000000000000000000000000000000000003C007C003C
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D00000000EF3DFF7FFF7FFF7FFF7FFF7F
      0000E03D0000FF7FFF7FF75E00000000000000000000EF3D0000000000000000
      0000EF3D000000000000000000000000000000000000E003E003E0030000FF03
      FF03FF030000000000000000000000000000000000000000003C003C007C003C
      EF3DEF3DEF3DEF3DEF3DEF3DEF3D0000003C0000EF3DFF7FFF7F00000000F75E
      E03DE07FE03D0000FF7FF75E0000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000E003E003E0030000FF03
      FF03FF030000000000000000000000000000000000000000003C003C003C0000
      EF3D0000EF3DEF3DF75EEF3D00000000003C0000EF3DFF7FFF7FFF7FFF7FE03D
      E07F0000E07FE03DFF7FF75E00000000000000000000000000000000EF3D0000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000003C003C000000000000
      EF3D000000000000F75EEF3D003C003C00000000EF3DFF7FFF7F00000000F75E
      E03DE07FE03D0000FF7FF75E0000000000000000EF3D00000000EF3D003CEF3D
      00000000000000000000000000000000000000000000EF3DEF3DEF3D0000FF7F
      FF7FFF7F000000000000000000000000000000000000003C0000000000000000
      EF3DF75EF75EF75EF75EEF3D0000000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      F75EE03DF75EFF7FFF7FF75E000000000000EF3D003CEF3D00000000EF3D0000
      00000000000000000000000000000000000000000000EF3DEF3DEF3D0000FF7F
      FF7FFF7F00000000000000000000000000000000000000000000000000000000
      0000EF3DEF3DEF3DEF3D00000000000000000000EF3DFF7FFF7F000000000000
      000000000000FF7FFF7FF75E0000000000000000EF3D00000000000000000000
      00000000000000000000000000000000000000000000EF3DEF3DEF3D0000FF7F
      FF7FFF7F0000000000000000000000000000F75EF75EF75EF75EF75EF75E0000
      0000000000000000000000000000000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FF75E0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000F75EF75EEF3DEF3DEF3DF75E0000
      0000000000000000000000000000000000000000EF3DFF7FFF7F000000000000
      FF7F00000000FF7FFF7FF75E000000000000000000000000EF3D000000000000
      00000000000000000000000000000000000000000000007C007C007C0000E07F
      E07FE07F00001F001F001F00000000000000F75EF75EEF3DEF3DEF3DF75E0000
      0000000000000000000000000000000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FF75E00000000000000000000EF3D003CEF3D00000000
      00000000000000000000000000000000000000000000007C007C007C0000E07F
      E07FE07F00001F001F001F00000000000000F75EF75EEF3DEF3DEF3DF75E0000
      0000000000000000000000000000000000000000EF3DFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FF75E000000000000000000000000EF3D000000000000
      E07F0000000000000000000000000000000000000000007C007C007C0000E07F
      E07FE07F00001F001F001F00000000000000F75EF75EEF3DEF3DEF3DF75E0000
      0000000000000000000000000000000000000000EF3DFF7F0000FF7F0000FF7F
      0000FF7F0000FF7F0000FF7F000000000000000000000000000000000000E07F
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000F75EF75EEF3DEF3DF75EEF3D0000
      000000000000000000000000000000000000000000000000FF7FEF3DFF7FEF3D
      FF7FEF3DFF7FEF3DFF7F000000000000000000000000000000000000EF3D0000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000F75E000000000000F75EEF3D0000
      0000000000000000000000000000000000000000000000000000FF7F0000FF7F
      0000FF7F0000FF7F000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000E00300000000
      0000000000000000000000000000000000000000000000000000000000000000
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000E0030000
      0000000000000000000000000000000000000000000000000000000000000000
      0000FF7FFF7F0000FF7F00000000FF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000E0030000E003
      000000000000000000000000000000000000FF0300000000E07FFF7FE07FFF7F
      E07F0000FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000E0030000
      E00300000000000000000000000000000000FF030000E07FFF7FE07FFF7F0000
      00000000FF7FFF7FFF7FFF7F0000FF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3D00000000
      0000EF3D00000000000000000000000000000000000000000000E0030000E003
      0000E0030000000000000000000000000000FF030000FF7FE07FFF7FE07FFF7F
      E07FFF7F0000FF7F00000000FF7FFF7F000000000000000000000000EF3D0000
      00000000EF3D00000000000000000000000000000000EF3D00000000FF03FF03
      FF030000EF3D00000000000000000000000000000000000000000000E0030000
      E0030000E003000000000000000000000000FF030000E07FFF7FE07FFF7F0000
      0000000000000000E07F0000FF7FFF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000EF3D0000EF01FF03FF03FF03
      FF03FF030000EF3D000000000000000000000000000000000000E0030000E003
      0000E0030000E00300000000000000000000FF030000FF7FE07FFF7FE07FFF7F
      E07FFF7FE07FFF7F0000FF7FFF7FFF7F00000000000000000000000000000000
      000000000000000000000000000000000000EF3D0000FF7FFF03FF03FF03FF03
      FF03FF03FF0300000000000000000000000000000000000000000000E0030000
      E0030000E003000000000000000000000000FF030000E07FFF7F000000000000
      0000000000000000FF7FFF7FFF7FFF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000FF7FEF01FF03FF03FF03EF3D
      0000000000000000000000000000000000000000000000000000E0030000E003
      0000E0030000000000000000000000000000000000000000E07FFF7FE07F0000
      0000E07F0000FF7FFF7F00000000FF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000EF01EF01FF03FF03FF03EF3D
      FF7FFF7FFF7FEF3D0000000000000000000000000000000000000000E0030000
      E003000000000000000000000000000000000000000000000000000000000000
      E07F0000FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000
      EF3D000000000000000000000000000000000000EF01FF7FFF03FF03FF03EF3D
      EF3DEF3DEF3DEF3D000000000000000000000000000000000000E0030000E003
      000000000000000000000000000000000000000000000000000000000000E07F
      0000FF7FFF7FFF7FFF7F00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000FF7FEF01EF01FF03FF03FF03
      FF03FF030000EF3D0000000000000000000000000000000000000000E0030000
      00000000000000000000000000000000000000000000000000000000E07F0000
      FF7FFF7F00000000FF7F0000FF7FFF7F00000000000000000000000000000000
      000000000000000000000000000000000000EF3D0000EF01EF01FF7FFF03FF03
      FF030000EF3D0000000000000000000000000000000000000000E00300000000
      0000000000000000000000000000000000000000000000000000E07F00000000
      FF7FFF7FFF7FFF7FFF7F0000FF7F000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3D0000FF7FEF01EF010000
      0000EF3D00000000000000000000EF3D00000000000000000000000000000000
      000000000000000000000000000000000000000000000000007C000000000000
      FF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000EF3D000000000000EF3D
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000FF7F00000000000000000000
      0000000000000000FF7F00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000FF7F00000000000000000000
      0000000000000000FF7F00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000F00EF3D00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000F0000000000000000000000EF3D0F00000000000000
      000000000F000F000F000F000F00000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF7F0000000000000000
      00000000FF7F000000000000000000000000000000000F000F000F000F000F00
      000000000000000000000F00000000000000000000000F000000000000000000
      0000000000000F000F000F000F00000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF7F0000000000000000
      00000000FF7F000000000000000000000000000000000F000F000F000F000000
      0000000000000000000000000F0000000000000000000F000000000000000000
      00000000000000000F000F000F00000000000000000000000000000000000000
      00000000000000000000000000000000000000000000FF7F0000000000000000
      00000000FF7F000000000000000000000000000000000F000F000F0000000000
      0000000000000000000000000F0000000000000000000F000000000000000000
      0000000000000F0000000F000F0000000000000000000000000000000000EF3D
      000000000000EF3D000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000F000F0000000F000000
      0000000000000000000000000F000000000000000000EF3D0F00000000000000
      00000F000F000000000000000F00000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000FF7F000000000000
      00000000FF7F000000000000000000000000000000000F000000000000000F00
      0F0000000000000000000F00000000000000000000000000EF3D0F000F000F00
      0F00000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000F000F000F000F0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FF7F00000000
      000000000000FF7F000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000EF3D00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000F000F00
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000F00
      0F000F000F000F000F000F000F000F000F000000000000000000000000000000
      0F000F000F000F000F000F000F000F000F0000000000000000000F0000000000
      0F00000000000F000F0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000F00
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000000000000000000
      0F00FF7FFF7FFF7FFF7FFF7FFF7FFF7F0F0000000000000000000F0000000000
      0F0000000F00000000000F000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DE03DEF3DE03DEF3D0F00
      FF7F000000000000000000000000FF7F0F000000000000000000000000000000
      0F00FF7F00000000000000000000FF7F0F0000000000000000000F0000000000
      0F0000000F00000000000F000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000E03DEF3DE03DEF3DE03D0F00
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000000000000000000
      0F00FF7FFF7FFF7FFF7FFF7FFF7FFF7F0F00000000000000000000000F000F00
      0F0000000F00000000000F000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DE03DEF3DE03DEF3D0F00
      FF7F000000000000FF7F0F000F000F000F0000000000FF7FFF7FFF7FFF7FFF7F
      0F00FF7F00000000000000000000FF7F0F000000000000000000000000000000
      0F0000000F000F000F0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000E03DEF3DE03DEF3DE03D0F00
      FF7FFF7FFF7FFF7FFF7F0F00FF7F0F00000000000000FF7F0000000000000000
      0F00FF7FFF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000000000000000000
      0F0000000F000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DE03DEF3DE03DEF3D0F00
      FF7FFF7FFF7FFF7FFF7F0F000F000000000000000000FF7FFF7FFF7FFF7FFF7F
      0F00FF7F00000000FF7F0F000F000F000F000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000E03DEF3DE03DEF3DE03D0F00
      0F000F000F000F000F000F0000000000000000000000FF7F0000000000000000
      0F00FF7FFF7FFF7FFF7F0F00FF7F0F0000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DE03DEF3DE03DEF3DE03D
      EF3DE03DEF3DE03DEF3DE03D00000000000000000000FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7F0F000F00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000E03DEF3D0000000000000000
      0000000000000000EF3DEF3D00000000000000000000FF7F00000000FF7F0000
      0F000F000F000F000F000F000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DEF3D0000000000000000
      0000000000000000EF3DE03D00000000000000000000FF7FFF7FFF7FFF7F0000
      FF7F000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000E03DEF3DE03D0000E07F0000
      0000E07F0000EF3DE03DEF3D00000000000000000000FF7FFF7FFF7FFF7F0000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000000000E07F
      E07F000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000F00
      0F000F000F000F000F000F000F000F000F000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000F00
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0F0000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F000000000000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F00000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000F00
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0F0000000F00FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F000000000000000000F75EF75EF75EF75EF75E
      F75EF75EF75EF75E0000F75E0000000000000000000000000000000000000F00
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0F0000000F00FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F00000000000000000000000000000000000000
      000000000000000000000000F75E000000000000000000000F000F000F000F00
      0F000F000F000F000F000F000F000F000F0000000F00FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F00000000000000F75EF75EF75EF75EF75EF75E
      E07FE07FE07FF75EF75E00000000000000000000000000000F00FF7FFF7F0F00
      0F000F000F000F000F000F000F00FF7F0F0000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F00000000000000F75EF75EF75EF75EF75EF75E
      EF3DEF3DEF3DF75EF75E0000F75E000000000000000000000F00FF7FFF7F0F00
      0F000F000F000F000F000F000F000F000F0000000F000F000F000F000F000F00
      0F000F000F000F000F00FF7F0F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F00000000000000000000000000000000000000
      000000000000000000000000F75EF75E00000000000000000F00FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7F0F0000000000000000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F00000000000000F75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75E0000F75E0000F75E000000000F000F000F000F000F000F00
      0F000F000F000F000F000F0000000000000000000F00FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F00000000000000000000000000000000000000
      0000000000000000F75E0000F75E0000000000000F00FF7F0F000F000F000F00
      0F000F000F000F00FF7F0F0000000000000000000F00FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F0000000000000000000000FF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7F0000F75E0000F75E000000000F00FF7F0F000F000F000F00
      0F000F000F000F000F000F0000000000000000000F00FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F0F000000000000000F00FF7FFF7FFF7FFF7FFF7F
      0F00FF7FFF7FFF7FFF7FFF7F0F00000000000000000000000000FF7F00000000
      000000000000FF7F0000000000000000000000000F00FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7F0F000000000000000000000000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F000000000000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F00000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7F000000000000000000000F000F000F000F000F000F00
      0F000F000F000F000000000000000000000000000F000F000F000F000F000F00
      0F000F000F000F000F00FF7F0F000000000000000F000F000F000F000F00FF7F
      0F000F000F000F000F00FF7F0F000000000000000000000000000000FF7F0000
      0000000000000000FF7F000000000000000000000F000F000F000F000F000F00
      0F000F00FF7F0F000000000000000000000000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F000000000000000F000F000F000F000F000F00
      0F000F000F000F000F000F000F000000000000000000000000000000FF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000F000F000F000F000F000F00
      0F000F000F000F00000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000E03DE03D000000000000
      000000000000F75EF75E0000E03D000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000E03DE03D00000000
      0000000000000000000000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03DE03D000000000000
      000000000000F75EF75E0000E03D0000000000000000E03DE03DE03DE03DE03D
      E03DE03DE03DE03D00000000000000000000E03DE03DE03DE03DE03D0000E03D
      E03DE03DE03DE03DE03D00000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03DE03D000000000000
      000000000000F75EF75E0000E03D000000000000E07F0000E03DE03DE03DE03D
      E03DE03DE03DE03DE03D0000000000000000E03DE03DE03DE03DE03D0000E07F
      E07FE07F00000000000000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03DE03D000000000000
      000000000000000000000000E03D000000000000FF7FE07F0000E03DE03DE03D
      E03DE03DE03DE03DE03DE03D000000000000E03DE03DE03DE03DE03D0000E07F
      E07FE07F000000001F0000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03DE03DE03DE03DE03D
      E03DE03DE03DE03DE03DE03DE03D000000000000E07FFF7FE07F0000E03DE03D
      E03DE03DE03DE03DE03DE03DE03D00000000E03DE03DE03DE03DE03D0000E07F
      E07FE07F00001F001F0000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03DE03D000000000000
      00000000000000000000E03DE03D000000000000FF7FE07FFF7FE07F00000000
      000000000000000000000000000000000000E03DE03DE03DFF7FE03D0000E07F
      E07FE07F1F001F001F0000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03D0000F75EF75EF75E
      F75EF75EF75EF75EF75E0000E03D000000000000E07FFF7FE07FFF7FE07FFF7F
      E07FFF7FE07F000000000000000000000000E03DE03DE03DE03DE03D0000E07F
      E07F1F001F001F001F001F001F001F0000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03D0000F75EF75EF75E
      F75EF75EF75EF75EF75E0000E03D000000000000FF7FE07FFF7FE07FFF7FE07F
      FF7FE07FFF7F000000000000000000000000E03DE03DE03DE03DE03D00000000
      1F001F001F001F001F001F001F001F0000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000E03D0000F75EF75EF75E
      F75EF75EF75EF75EF75E0000E03D000000000000E07FFF7FE07F000000000000
      000000000000000000000000000000000000E03DE03DE03DE03DE03D00000000
      00001F001F001F001F001F001F001F0000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7F00000000000000000000000000000000E03D0000F75EF75EF75E
      F75EF75EF75EF75EF75E0000E03D000000000000000000000000000000000000
      000000000000000000000000000000000000E03DE03DE03DE03DE03D0000FF7F
      FF7F00001F001F001F0000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7F0000FF7F000000000000000000000000E03D0000F75EF75EF75E
      F75EF75EF75EF75EF75E00000000000000000000000000000000000000000000
      000000000000000000000000000000000000E03DE03DE03DE03DE03D0000FF7F
      FF7FFF7F00001F001F0000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7F00000000000000000000000000000000E03D0000F75EF75EF75E
      F75EF75EF75EF75EF75E0000F75E000000000000000000000000000000000000
      000000000000000000000000000000000000E03DE03DE03DE03DE03D0000FF7F
      FF7FFF7F000000001F0000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000E03DE03DE03DE03D0000E03D
      E03DE03DE03D0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000E03DE03DE03D0000
      000000000000000000000000000000000000424D3E000000000000003E000000
      2800000040000000E00000000100010000000000000700000000000000000000
      000000000000000000000000FFFFFF0044444444444444444444444444444444
      6666664444444444444444666666666644444444444444440000444444444444
      4444444444444444444444444444444444444444444444444444444444444444
      4444444444444444444444444444444444444444444444446666666666666644
      44444444444444664444444444444444C00FF8FFFF004444800FC2FFF801CCCC
      8003D8FFC004CCCC8003DFFF8400CCCC8000DF8F1200CCCC8000DC2F0800CCCC
      8000DD8F1001CCCC8000DDFF5181CCCC8000D8FF4001CCCC8000C2FF7C03CCCC
      8000D8FF0007CCCCE000DFFF081F4444E0008FFF3E7F4444F800AFFF18FF4444
      F8008FFF81FF4444FFFFFFFFFFFF4444FFFFFFFFEDB6FFFFF800F83FEAAAFFFF
      F800E00FEAAAFFFFF800C007EDB6FFFFE0008003FFFF81FFE0008003F07FFFDF
      E0000001C03F879F80000001801FFF0780000001001F879B800300010019FFDB
      80030001001081FB800380030019FFFB800F80030019FC07800FC0070039FFFF
      800FE00F0061FFFFC7FFF83F83FFFFFFFFFFFFFFEDB6FFFFF1FFC003EAAAFFFF
      E3FFC003EAAAFFFFC7FFC003EDB6F13FC7FFC003FFFFE01FC7BFC003F07FE01F
      C79FC003C03FC00FC38F8001801FC007E0078001001FC007E003C0032019C00F
      F001E0035010C00FF803F0032819C00FFE07F8035019E00FFF8FFC232039FFFF
      FF9FFE63C061FFFFFFBFFFFF81FFFFFFFFFFFFFFF81F8003FFFFFFFFE0078003
      FFFFFFFFC0038003FDFFFFBF80018003F9FFFF9F80018003F1FFFF8F00008003
      E003C00700008003C003C00300008003C003C00300008003E003C00700008003
      F1FFFF8F00008003F9FFFF9F80018003FDFFFFBF80018003FFFFFFFFC0038007
      FFFFFFFFE007800FFFFFFFFFF81F801FFFFFFFFFFFFFFC00FFFFF000FFFFEC00
      FFFFF000FEDFDC00FC3FF000FEDFDC00F81F7000FEDF8C00F89F3000FEDFDC00
      F9FF1000F8DF0000F8FF1000F0DF0000F81F3000E0DF000024127000E0DF0001
      2612F000E0DF000327F2F000F0DF0007FFFFF000F807001EFFFFF001FFFF001A
      FFFFF003FFFF0011FFFFF007FFFF001BFFFFFFFFE00FFFFFFEFFFEFFE00FC3FF
      FFFFFFFFE00FE3FFC27FC27F000FC200FFFFFFFF000F8A00C200C200000FBE00
      FFFFFFFF000FB000DE07DE07001FF0009E07CE07003FF00007FF07FF007DF000
      9E00CE0000FDF001DE00DE0001D1F003FFFFFFFF03C3F007C200C200FFC7F00F
      FFFFFFFFFFC3F01FFEFFFEFFFFFFF03FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
      FFFFF04FFFFF07C30E04E71F0E09CFE79F31EF9F9F24E7E7CF39EF9FCF24F007
      E039E79FE024F9E7F301F01FF324FCE7F939FF9FF921FE67FC39EF9FFC27FF27
      FE03E79FFE27FF87FF3FF03FFF07FFC7FFFFFFFFFFFFFFE7FFFFFFFFFFFFFFFF
      FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFE30C007FFFFFFFFFE008003
      DF7F8003F00080038E3F803BF0008003DF7F803BE0028003FFFC803BC2008003
      FBF88003CE018003B1F18003CE0780031BE3800301078003BFC7800301FF8003
      FF8F800301FF8003EF1F800301FF8003C63F800301FF8003EC7F800301FF8003
      F8FF800301FFC007F9FFFFFF73FFE00FFE49EDB6F3FFFC00FE49EAAAF5FFFC00
      FFFFEAAAFAFF2000FFFFEDB6F57F0000C7C7FFFFFABF0000D7D7F07FF55F0000
      C387C03FFAAF0000C007801FF55F0000D087001FFABF0000D0870019F57F0000
      C0070010FAFFE000C0070019F5FFF800F39F0019FBFFF000F39F0039F7FFE001
      F39F8061FFFFC403FFFFC1FFFFFFEC07FFFFFFFFDB73FFFFFFFFFFFFDBB507C1
      FFFFFFFFC21307C1FFFFFFFFDBB507C1FFFFE7FFE7730101FFF7CF83FFFF0001
      C1F7DFC3E3E30201C3FBDFE3EBEB0201C7FBDFD3E1C38003CBFBCF3BE003C107
      DCF7E0FFE843C107FF0FFFFFE843E38FFFFFFFFFE003E38FFFFFFFFFE003E38F
      FFFFFFFFF9CFFFFFFFFFFFFFF9CFFFFFFFFFFFFFFFFFFFFFFFFFF9FFFFFFFC00
      FE00F6CFEFFD8000FE00F6B7C7FF0000FE00F6B7C3FB00008000F8B7E3F70000
      8000FE8FF1E700018000FE3FF8CF00038000FF7FFC1F00038001FE3FFE3F0003
      8003FEBFFC1F00038007FC9FF8CF0FC3807FFDDFE1E7000380FFFDDFC3F38007
      81FFFDDFC7FDF87FFFFFFFFFFFFFFFFFFC00FFFFFFFFFFFFFC0080038003C007
      FC00800380038003FC00800380030001E000800380030001E000800380030001
      E000800380030000E0078003800300008007800380038000800780038003C000
      800780038003E001801F80038003E007801F80038003F007801F80038003F003
      801FFFFFFFFFF803FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC001FFFFE3FF
      E0038001001F83FFE0038001000F000FE00380010007003FE00380010003002F
      E00380010001000FE00380010000000FE0038001001F0001E0038001001F0201
      E0038001001F0301E00380018FF1008FE0078001FFF9000FE00F8001FF75002F
      E01F8001FF8F803FFFFFFFFFFFFFE3FF00000000000000000000000000000000
      000000000000}
  end
  object ControlsImages: TImageList
    Height = 25
    Width = 25
    Left = 160
    Top = 200
    Bitmap = {
      494C010111001300040019001900FFFFFFFFFF10FFFFFFFFFFFFFFFF424D3600
      0000000000003600000028000000640000007D0000000100100000000000A861
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000002E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000009B739B739B739B739B739B739B739B730000386B
      386B386B386B2E4A2E4A2E4A2E4A2F4A9B73386B386B386B386B386B007C003C
      386B386B386B386B386B1F001F001F0000002E4A2E4A386B386B386B386B386B
      386B386B386B2E4A2E4A2E4A2E4A2F4A9B73386B386B386B386B2E4A9B739B73
      9B739B739B732E4A386BEF01386B386B2E4A00009B739B739B739B730000386B
      386B000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000386B386B386B386B0000
      386B386B386B386B386B386B386B0000386B386BFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7F386B9B73386B386B386BFF7F386B2E4A386B386B386B
      386B386B386B386B386B00000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000000000002F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000009B73386B386B2E4A9B739B739B739B730000386B386B386B386B2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000FF7F386B386B386B386B386B386B386B386B386B
      386B2E462F4A2E4A2F4A2E462F4A2E4A9B73386B386B386B386B2E4A0F000F00
      0F000F000F000F000F000F000F000000386B386B386B386B386B386B386B386B
      386B386B386B2E4A2F4A2E462F4A2E4A9B73386B386B386B386B2E4A9B739B73
      9B739B731F009B731F009B731F009B731F009B739B739B739B739B730000386B
      386B00000000000000000000000000000000000000000000000000000000FF7F
      FF7F0000000000000000000000000000000000002E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A386B386B386B386B
      386B386B386B386B2E4A386BFF7F2E4A386B386B2E4A0000386B386B386B386B
      386B386B386B386B386B386B386B386B386B386B386B0000000000000000386B
      386B386B386B386B386B00000000000000000000000000000000000000000000
      000000000000FF7FFF7F0000000000000000000000000000000000002E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A0000000000000000000000000000
      000000000000000000000000FF7FFF7F00000000000000000000000000000000
      000000001F009B731F009B739B739B739B739B730000386B386B386B386B2E4A
      2E4A2F4A2E4A2E4A9B73386B386B386B386B386B386B386B386B386B386B386B
      1F001F001F001F001F0000002E4A386B386B386B386B386B386B386B386B2E4A
      2E4A2F4A2E4A2E4A9B73386B386B386B386B2E4A9B739B739B739B732E4A386B
      FF03FF03FF03EF01386B2E4A9B739B739B739B730000386B386B000000000000
      0000000000000000000000000000000000000000FF7FFF7F0000000000000000
      000000000000000000000000386B386B386B386B386B386B386B386B386B386B
      386B386B386B386B386B386BFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7F386B9B73386B386B386BFF7F386B386B386B386B386B386B386B386B386B
      2E4A000000000000000000000000000000000000FF7F00000000FF7FFF7F0000
      00000000000000000000000000000000000000002F4A2E46702D2E4A2F4A2E46
      2F4A2E4A2F4A2E46702D2E4A2F4A2E462F4A2E4A2F4A2E46702D2E4A2F4A2E46
      2F4A2E4A2F4A2E46702D2E4A2F4A2E462F4A2E4A2F4A2E46702D2E4A2F4A2E46
      2F4A2E4A2F4A2E46702D2E4A2F4A2E462F4A2E4A2F4A2E46702D2E4A2F4A2E46
      2F4A2E4A2F4A2E46702D2E4A2F4A2E462F4A2E4A2F4A2E46702D2E4A2F4A2E46
      2F4A2E4A2F4A2E46702D000000000000000000000000000000000000FF7FFF7F
      FF7FFF7FFF7F00000000000000000000000000000000000000000000FF03386B
      2E4A9B739B739B739B739B730000386B386B386B386B2E4A2F4A2E4A2F4A2E46
      702D2E4A2F4A2E462F4A2E4A2F4A2E46702D2E4A2F4A2E462F4A2E4A2F4A2E46
      702D2E4A2F4A2E462F4A2E4A2F4A2E46702D2E4A2F4A2E462F4A2E4A2F4A2E46
      702D2E4A2F4A2E462F4A2E4A2F4A2E46702D2E4A2F4A2E462F4A2E4A2F4A2E46
      702D2E4A2F4A2E462F4A2E4A2F4A2E46702D0000000000000000000000000000
      00000000FF7FFF7FFF7FFF7FFF7F000000000000000000000000000000000000
      00000000FF7F386B386B386B386B386B386B386B386B386B2E4A2E4A2F4A2E46
      2F4A2E4A2F4A2E469B73386B386B386B386B2E4A9B739B739B739B739B739B73
      9B739B739B730000386B386B386B386B386B386B386B386B386B386B386B2E4A
      2F4A2E4A2F4A2E469B73386B386B386B386B2E4A9B739B739B739B731F009B73
      1F009B731F009B731F009B739B739B739B739B730000386B386B000000000000
      000000000000000000000000FF7FFF7FFF7FFF7FFF7FFF7FFF7F000000000000
      0000000000000000000000002E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A386B386B386B386B386B386B386B386B
      2E4A386BFF7F386B386B386B2E4A0000386B386B386B386B386B386B386B386B
      386B386B386B386B386B386B386B386B386B386B386B386B386B386B386B386B
      386B000000000000000000000000000000000000FF7FFF7FFF7FFF7FFF7FFF7F
      00000000000000000000000000000000000000002E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A
      2E4A2E4A2E4A2F4A2E4A000000000000000000000000000000000000FF7FFF7F
      FF7FFF7FFF7F000000000000000000000000000000000000000000009B739B73
      9B739B739B739B739B739B730000386B386B386B386B2E4A2E4A2E4A2E4A2F4A
      9B73386B386B386B386B386B386B386B386B386B386B386B1F001F001F001F00
      1F0000002E4A386B386B386B386B386B386B386B386B2E4A2E4A2E4A2E4A2F4A
      9B73386B386B386B386B2E4A9B739B739B739B739B732E4A2E4A386B386B2E4A
      2E4A9B739B739B739B739B730000386B386B0000000000000000000000000000
      00000000FF7FFF7FFF7FFF7F0000000000000000000000000000000000000000
      00000000386B386B386B386B386B386B386B386B386B386B386B386B386B386B
      386B386BFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F386B9B73386B
      386B2E4AFF7F386B386B386B386B386B386B386B386B386B2E4A000000000000
      000000000000000000000000FF7FFF7FFF7F0000000000000000000000000000
      0000000000000000000000002F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A000000000000000000000000000000000000FF7FFF7F0000000000000000
      00000000000000000000000000000000000000002E4A9B739B739B739B739B73
      9B739B730000386B386B386B386B2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A
      2F4A2E462F4A2E4A2F4A000000000000000000000000000000000000FF7F0000
      00000000000000000000000000000000000000000000000000000000FF7F386B
      2E4A386B386B386B386B386B386B386B2E4A2E462F4A2E4A2F4A2E462F4A2E4A
      9B73386B386B386B386B2E4A9B739B739B739B739B739B739B739B739B730000
      386B386B386B386B386B386B386B386B386B386B386B2E4A2F4A2E462F4A2E4A
      9B73386B386B386B386B2E4A9B739B739B739B731F009B739B739B739B739B73
      1F009B739B739B739B739B730000386B386B0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000002E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A386B386B386B386B386B386B386B386B386BFF7F386B
      386B386B2E4A0000F662F662F662F662F662F662F662F662F662F662F662F662
      F662F662F662F662F662F662F662F662F662F662F662F662F662000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000002E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A2E4A2F4A2E4A2E4A
      2E4A000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000009B739B739B739B739B739B73
      9B739B730000386B386B386B386B2E4A2E4A2F4A2E4A2E4A9B73386B386B386B
      386B386B386B386B386B386B386B386B386BFF03FF03FF03FF03FF03386B386B
      386B386B386B386B386B386B386B2E4A2E4A2F4A2E4A2E4A9B73386B386B386B
      386B2E4A9B739B739B739B739B739B739B739B739B739B739B739B739B739B73
      9B739B730000386B386B00000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000386B386B
      386B386B386B386B386B386B386B386B386B386B386B386B386B386BFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F386B9B73386B386B2E4AFF7F386B
      2E4A386B386B386B386B386B386B386B386B0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000002F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E46
      2F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E46
      2F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E46
      2F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E46
      2F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A2E4A2F4A2E462F4A000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000FF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DFF7F
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EEF3DFF7F000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EEF3DFF7F00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000F75E1F001F00F75E000000000000000000000000
      00000000000000000000000000000000EF3DFF7FF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3DFF7F0000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000001F001F001F001F0000000000
      000000000000000000000000000000000000000000000000EF3DFF7FF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3DFF7F
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DFF7FEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      000000000000000000000000000000000000000000000000000000001F001F00
      1F001F0000000000000000000000000000000000000000000000000000000000
      EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EEF3DFF7F000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EEF3D000000000000000000000000000000000000000000000000
      00000000F75E1F001F00F75E0000000000000000000000000000000000000000
      0000000000000000EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EEF3DFF7F000000000000000000001F001F001F00
      1F001F0000001F0000001F000000000000001F0000001F000000000000001F00
      00000000000000000000EF3DFF7FF75EF75EF75EF75EF75E00000000F75EF75E
      0000F75E0000F75EF75EF75EF75EEF3D00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000EF3DFF7FF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3DFF7F0000000000000000
      00001F00000000000000000000001F0000001F000000000000001F0000001F00
      000000001F00000000000000000000000000EF3DFF7FF75EF75EF75EF75E0000
      F75EF75E0000F75E0000F75E0000F75EF75EF75EF75EEF3D0000000000000000
      000000000000000000000000000000000000000000001F001F00000000000000
      000000000000000000000000000000000000000000000000EF3DFF7FF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3DFF7F
      000000000000000000001F00000000000000000000001F0000001F0000000000
      00001F0000001F0000001F000000000000000000000000000000EF3DFF7FF75E
      F75EF75EF75E0000F75EF75E0000F75E0000F75E0000F75EF75EF75EF75EEF3D
      0000000000000000000000000000000000000000000000000000000000001F00
      1F00F75E00000000000000000000000000000000000000000000000000000000
      EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EEF3DFF7F000000000000000000001F00000000000000000000001F00
      00001F000000000000001F0000001F001F000000000000000000000000000000
      0000EF3DFF7FF75EF75EF75EF75E0000F75EF75E0000F75E00000000F75EF75E
      F75EF75EF75EEF3D000000000000000000000000000000000000000000000000
      0000000000001F001F001F000000000000000000000000000000000000000000
      0000000000000000EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EEF3DFF7F000000000000000000001F0000000000
      0000000000001F0000001F001F00000000001F0000001F0000001F0000000000
      00000000000000000000EF3DFF7FF75EF75EF75EF75E0000F75EF75E0000F75E
      0000F75E0000F75EF75EF75EF75EEF3D00000000000000000000000000000000
      0000000000000000000000000000F75E1F001F001F0000000000000000000000
      00000000000000000000000000000000EF3DFF7FF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3DFF7F0000000000000000
      00001F00000000000000000000001F0000001F0000001F001F00000000001F00
      000000001F00000000000000000000000000EF3DFF7FF75EF75EF75EF75EF75E
      00000000F75EF75E0000F75E0000F75EF75EF75EF75EEF3D0000000000000000
      0000000000000000000000000000F75E1F001F00F75E00001F001F001F001F00
      000000000000000000000000000000000000000000000000EF3DFF7FF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3DFF7F
      000000000000000000001F000000000000000000000000000000000000000000
      0000000000001F00000000000000000000000000000000000000EF3DFF7FF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3D
      000000000000000000000000000000000000000000001F001F001F001F000000
      F75E1F001F001F001F0000000000000000000000000000000000000000000000
      EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EEF3DFF7F000000000000000000001F00000000000000000000000000
      00000000000000000000000000001F0000000000000000000000000000000000
      0000EF3DFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7F000000000000000000000000000000000000000000001F00
      1F001F001F00000000001F001F001F001F000000000000000000000000000000
      0000000000000000EF3DFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EEF3DFF7F000000000000000000001F0000000000
      0000000000001F0000000000000000000000000000001F000000000000000000
      000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D00000000000000000000000000000000
      0000000000001F001F0000000000000000001F001F001F001F00000000000000
      00000000000000000000000000000000EF3DFF7FF75EF75E0000F75E0000F75E
      F75E0000F75EF75EF75EF75EF75EF75EF75EF75EEF3DFF7F0000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000F75E1F00F75E000000001F001F001F001F00
      F75E00000000000000000000000000000000000000000000EF3DFF7FFF7F0000
      0000000000000000000000000000FF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3DFF7F
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000F75E1F001F001F00
      1F001F001F00F75E000000000000000000000000000000000000000000000000
      EF3DEF3DEF3D00000000000000000000000000000000EF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3D0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EEF3D00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DEF3DEF3D
      EF3DEF3DEF3DEF3D0000000000000000000000000000000000000000EF3DF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EEF3D00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      EF3DF75EFF7FFF7FFF7FFF7FFF7FF75EEF3D0000000000000000000000000000
      00000000EF3DF75EF75EF75EF75EF75EF75EF75EF75E007CF75EF75EF75EF75E
      007CF75EF75EF75EF75EEF3D000000000000FF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000000000000000
      000000000000EF3DFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3D00000000
      000000000000000000000000EF3DF75EF75EF75EF75EF75E007CF75EF75EF75E
      003C003C003CF75EF75EF75E007CF75EF75EEF3D000000000000EF3DF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EEF3DF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E0000
      000000000000000000000000EF3DFF7FFF7FFF7FFF7FF75E1F00F75EFF7FFF7F
      FF7FFF7FEF3D0000000000000000000000000000EF3DF75EF75EF75EF75EF75E
      F75EF75E007CF75EF75E003C003CF75E007CF75EF75EF75EF75EEF3D00000000
      0000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000
      000000000000000000000000000000000000F75EEF3D0000FF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7F000000000000000000000000000000000000
      00000000F75E000000000000000000000000EF3DF75EFF7FFF7FFF7FFF7FF75E
      1F00F75EFF7FFF7FFF7FFF7FF75EEF3D000000000000000000000000EF3DF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75E003CF75EF75EF75EF75EF75EF75E
      F75EEF3D000000000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FF75EEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D0000F75EEF3D0000
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3D0000F75E000000000000000000000000EF3DFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3D0000000000000000
      00000000EF3DF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E003CF75EF75E
      F75EF75EF75EF75EF75EEF3D000000000000EF3D00000000FF7FFF7F0000FF7F
      00000000FF7F000000000000F75EFF7FF75EF75EF75EF75EF75EF75EF75EEF3D
      0000F75EEF3D0000000000000000FF7FFF7F0000FF7F0000FF7FFF7FFF7FF75E
      FF7FF75EF75EF75EF75EF75EF75EF75EEF3D0000F75E00000000000000000000
      0000EF3DFF7FFF7FFF7FFF7FFF7FF75E1F00F75EFF7FFF7FFF7FFF7FFF7FEF3D
      000000000000000000000000EF3DF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75E003CF75EF75EF75EF75EF75EF75EF75EEF3D000000000000EF3D00000000
      FF7FFF7F0000FF7F0000FF7FFF7F0000FF7FFF7FF75EFF7FF75EF75EF75EF75E
      F75EF75EF75EEF3D0000F75EEF3D00000000FF7FFF7F0000FF7F0000FF7F0000
      FF7FFF7FFF7FF75EFF7FF75EF75EF75EF75EF75EF75EF75EEF3D0000F75E0000
      00000000000000000000EF3DFF7FFF7FFF7FFF7FFF7F1F001F001F00FF7FFF7F
      FF7FFF7FFF7FEF3D000000000000000000000000EF3DF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EE001F75EF75EF75EF75EF75EF75EF75EEF3D00000000
      0000EF3D000000000000FF7F0000FF7F0000FF7FFF7F00000000FF7FF75EFF7F
      F75E0000F75E0000F75E0000F75EEF3D0000F75EEF3D00000000FF7FFF7F0000
      FF7F0000FF7F00000000FF7FFF7FF75EFF7FF75E0000F75E0000F75E0000F75E
      EF3D0000F75E000000000000000000000000EF3DFF7FFF7FFF7FFF7FFF7F1F00
      1F001F00FF7FFF7FFF7FFF7FFF7FEF3D000000000000000000000000EF3DF75E
      F75EF75EF75EF75EF75EF75EF75EE001E001E001E001E001F75EE001F75EF75E
      F75EEF3D000000000000EF3D00000000FF7FFF7F0000FF7F0000FF7FFF7F0000
      FF7FFF7FF75EFF7FF75EF75EF75EF75EF75EF75EF75EEF3D0000F75EEF3D0000
      0000FF7FFF7F0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7FF75EF75EF75E
      F75EF75EF75EF75EEF3D0000F75E000000000000000000000000EF3DFF7FFF7F
      FF7FFF7FFF7F1F001F001F00FF7FFF7FFF7FFF7FFF7FEF3D0000000000000000
      00000000EF3DF75EF75EF75EF75EF75EF75EE001E001E001007CE001007CE001
      E001E001F75EF75EF75EEF3D000000000000EF3D000000000000FF7F0000FF7F
      0000FF7FFF7F000000000000F75EFF7FF75EF75EF75EF75EF75EF75EF75EEF3D
      0000F75EEF3D0000000000000000FF7FFF7F0000FF7FFF7FFF7FFF7FFF7FF75E
      FF7FF75EF75EF75EF75EF75EF75EF75EEF3D0000F75E00000000000000000000
      0000EF3DF75EFF7FFF7FFF7FFF7F1F001F001F00FF7FFF7FFF7FFF7FF75EEF3D
      000000000000000000000000EF3DF75EF75EF75EF75EF75EF75EE001E001E001
      E001E001E001007CE001F75EE001F75EF75EEF3D000000000000EF3D0000FF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FEF3D0000F75EEF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FF75EFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3D0000F75E0000
      000000000000000000000000EF3DFF7FFF7FFF7FFF7FF75E1F00F75EFF7FFF7F
      FF7FFF7FEF3D0000000000000000000000000000EF3DF75EF75EF75EF75EE001
      E001E001007CE001E001E001E001E001E001E001E001F75EF75EEF3D00000000
      0000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75E0000F75EEF3D0000FF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75E0000F75E00000000000000000000000000000000EF3DFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FEF3D00000000000000000000000000000000EF3DF75E
      F75EF75EF75EF75EE001E001E001E001E001E001E001E001007CE001E001F75E
      F75EEF3D000000000000EF3D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000F75EEF3D0000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000F75E000000000000000000000000000000000000
      EF3DF75EFF7FFF7FFF7FFF7FFF7FF75EEF3D0000000000000000000000000000
      00000000EF3DF75EF75EF75EF75EF75EE001E001E001E001F75EE001007CE001
      E001E001F75EF75EF75EEF3D000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D00000000000000000000
      00000000000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3D0000000000000000
      000000000000000000000000EF3DF75EF75EF75EF75EF75EF75EF75EE001E001
      E001E001E001E001E001E001E001F75EF75EEF3D000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DF75EF75EF75EF75EF75E
      F75EF75EF75EF75EE001E001F75EF75EF75EF75EF75EF75EF75EEF3D00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000000000000000000000000000EF3DF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EEF3D00000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000EF3DF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EEF3D0000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000EF3DF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EEF3D000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3D00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000EF3DFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0000
      F75EF75EF75EF75E000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000EF3DFF7FFF7F0F000F000F000F000F000F000F000F00
      FF7FFF7FFF7F0000FF7FEF3DEF3DF75E00000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000FF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7F000000000000EF3DFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7F0000FF7F00000000F75E0000000000000000
      000000000000000000000000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7F0000000000000000000000000000000000000000000000000000
      000000000000FF7FFF7FFF7FFF7F000000000000000000000000000000000000
      0000EF3DF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75E000000000000EF3DFF7FFF7F
      0F000F000F000F000F000F000F000F000F00FF7FFF7F0000FF7FFF7FFF7FF75E
      0000000000000000000000000000000000000000EF3DF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EFF7F000000000000000000000000000000000000
      00000000000000000000FF7FFF7FF75EF75EF75EF75EFF7FFF7F000000000000
      00000000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7F00000000000000000000000000000000000000000000F75E00000000
      0000EF3DFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0000
      00000000000000000000000000000000000000000000000000000000EF3D0000
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7F00000000000000000000
      00000000000000000000000000000000EF3DF75EF75EFF7FFF7FFF7FFF7FF75E
      F75EFF7F0000000000000000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FF75EEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      0000F75E000000000000EF3DFF7FFF7F0F000F000F000F000F000F000F000F00
      FF7FFF7FFF7F0000FF7FF75EFF7FF75E00000000000000000000000000000000
      00000000EF3D0000FF7FFF7FFF7F0000FF7FFF7FFF7FFF7FFF7FF75EFF7F0000
      000000000000000000000000000000000000000000000000EF3D0000FF7FFF7F
      FF7FFF7FFF7FFF7FF75EFF7F0000000000000000000000000000EF3D0000FF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7FF75EF75EF75EF75E
      F75EF75EF75EEF3D0000F75E000000000000EF3D0F000F000F000F000F000F00
      0F000F000F000F000F000F000F000000F75EFF7FF75EFF7F0000000000000000
      000000000000000000000000EF3D0000FF7FFF7F000000000000FF7FFF7FFF7F
      FF7FF75EFF7F000000000000000000000000000000000000000000000000EF3D
      0000FF7FFF7FFF7F00000000FF7FFF7FFF7FF75EFF7F00000000000000000000
      0000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7F
      F75EF75EF75E0000F75EF75EF75EEF3D0000F75E000000000000EF3D0F000F00
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0F000F000000FF7FF75EFF7FF75E
      0000000000000000000000000000000000000000EF3D0000FF7F000000000000
      00000000FF7FFF7FFF7FF75EFF7F000000000000000000000000000000000000
      000000000000EF3D0000FF7FFF7F0000000000000000FF7FFF7FF75EFF7F0000
      00000000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FF75EFF7FF75EF75E000000000000F75EF75EEF3D0000F75E00000000
      0000EF3D0F000F000F000F000F000F000F000F000F000F000F000F000F000000
      F75EFF7FF75EFF7F0000000000000000000000000000000000000000EF3D0000
      FF7F00000000FF7F000000000000FF7FFF7FF75EFF7F00000000000000000000
      0000000000000000000000000000EF3D0000FF7FFF7F0000000000000000FF7F
      FF7FF75EFF7F000000000000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FF75EFF7FF75E00000000000000000000F75EEF3D
      0000F75E000000000000EF3DFF7FFF7F0F000F000F000F000F000F000F000F00
      FF7FFF7FFF7F0000FF7FF75EFF7FF75E00000000000000000000000000000000
      00000000EF3D0000FF7F0000FF7FFF7FFF7F000000000000FF7FF75EFF7F0000
      00000000000000000000000000000000000000000000EF3D0000FF7FFF7FFF7F
      00000000FF7FFF7FFF7FF75EFF7F000000000000000000000000EF3D0000FF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7FF75EF75EF75EF75E
      F75EF75EF75EEF3D0000F75E000000000000EF3DFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7F0000F75EFF7FF75EFF7F0000000000000000
      000000000000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7F00000000
      FF7FF75EFF7F0000000000000000000000000000000000000000000000000000
      EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FF75EFF7F000000000000000000000000
      0000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3D0000F75E000000000000EF3DFF7FFF7F
      0F000F000F000F000F000F000F000F000F00FF7FFF7F0000FF7FF75EFF7FF75E
      0000000000000000000000000000000000000000EF3D0000FF7FFF7FFF7FFF7F
      FF7FFF7FFF7F0000FF7FF75EFF7F000000000000000000000000000000000000
      0000000000000000EF3D00000000FF7FFF7FFF7FFF7F00000000FF7F00000000
      00000000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E0000F75E00000000
      0000EF3DFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0000
      00000000000000000000000000000000000000000000000000000000EF3D0000
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7F00000000000000000000
      000000000000000000000000000000000000EF3DEF3D0000000000000000EF3D
      EF3D00000000000000000000000000000000EF3D000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000F75E000000000000EF3DFF7FFF7F0F000F000F000F000F000F000F000F00
      FF7FFF7FFF7F0000F75EF75EF75EF75E00000000000000000000000000000000
      00000000EF3D0000000000000000000000000000000000000000F75EFF7F0000
      000000000000000000000000000000000000000000000000000000000000EF3D
      EF3DEF3DEF3D0000000000000000000000000000000000000000EF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3D000000000000EF3DFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7F0000FF7F00000000F75E0000000000000000
      000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DFF7F0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000EF3DFF7FFF7F
      0F000F000F000F000F000F000F000F000F00FF7FFF7F0000FF7FEF3DEF3DF75E
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000EF3DFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F0000
      FF7FFF7FFF7FF75E000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3D0000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3DF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EFF7F00000000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F00000000FF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7F00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FF75EFF7F00000000EF3DF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EFF7F00000000
      EF3DF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E
      F75EF75EF75EF75EF75EF75EF75EF75EF75E0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000EF3D0000FF7FFF7F0F000F000F000F00FF7F0F000F000F00
      0F00FF7FFF7FEF3DFF7FEF3DFF7FFF7FF75EFF7F00000000EF3D0000FF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      F75EFF7F00000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7F00000000000000000000000000000000000000000000F75E000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000EF3D0000FF7F0F000F00FF7F0F000F00
      FF7F0F000F00FF7F0F000F00FF7FFF7FEF3DFF7FFF7FFF7FF75EFF7F00000000
      EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3D
      FF7FEF3DFF7FFF7FF75EFF7F00000000EF3D0000FF7F0F000F000F000F00FF7F
      0F000F000F000F00FF7FF75EEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D0000
      F75E000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3D0000FF7FFF7F
      0F000F000F000F00FF7F0F000F00FF7F0F000F00FF7FFF7FEF3DFF7FFF7FFF7F
      F75EFF7F00000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FEF3DFF7FFF7FFF7FF75EFF7F00000000EF3D00000F000F00
      FF7F0F000F00FF7F0F000F00FF7F0F000F00F75EFF7FF75EF75EF75EF75EF75E
      F75EF75EEF3D0000F75E00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      EF3D0000FF7FFF7FFF7FFF7F0F000F00FF7F0F000F00FF7F0F000F00FF7FFF7F
      EF3DFF7FFF7FFF7FF75EFF7F00000000EF3D0000FF7FFF7F1F00FF7F1F00FF7F
      FF7FFF7FFF7F1F00FF7F1F00FF7FFF7FEF3DFF7FFF7FFF7FF75EFF7F00000000
      EF3D0000FF7F0F000F000F000F00FF7F0F000F00FF7F0F000F00F75EFF7FF75E
      F75EF75E0000F75EF75EF75EEF3D0000F75E0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000EF3D0000FF7FFF7F0F000F000F00FF7FFF7F0F000F000F00
      0F00FF7FFF7FFF7FEF3DFF7FFF7FFF7FF75EFF7F00000000EF3D0000FF7FFF7F
      FF7F1F00FF7FFF7FFF7FFF7FFF7FFF7F1F00FF7FFF7FFF7FEF3DFF7FFF7FFF7F
      F75EFF7F00000000EF3D0000FF7FFF7FFF7F0F000F00FF7F0F000F00FF7F0F00
      0F00F75EFF7FF75EF75E000000000000F75EF75EEF3D0000F75E000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7F
      FF7F0F000F00FF7FFF7FFF7FFF7FFF7FEF3DFF7FFF7FFF7FF75EFF7F00000000
      EF3D0000FF7F1F001F001F001F001F00FF7FFF7F1F001F001F001F001F00FF7F
      EF3DFF7FFF7FFF7FF75EFF7F00000000EF3D0000FF7F0F000F000F00FF7FFF7F
      0F000F000F000F00FF7FF75EFF7FF75E00000000000000000000F75EEF3D0000
      F75E000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3D0000FF7FFF7F
      FF7FFF7FFF7FFF7FFF7F0F000F00FF7FFF7FFF7FFF7FFF7FEF3DFF7FFF7FFF7F
      F75EFF7F00000000EF3D0000FF7FFF7FFF7F1F00FF7FFF7FFF7FFF7FFF7FFF7F
      1F00FF7FFF7FFF7FEF3DFF7FFF7FFF7FF75EFF7F00000000EF3D0000FF7FFF7F
      FF7FFF7FFF7FFF7F0F000F00FF7FFF7FFF7FF75EFF7FF75EF75EF75EF75EF75E
      F75EF75EEF3D0000F75E00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      EF3D0000FF7FFF7FFF7FFF7FFF7FFF7F0F000F000F00FF7FFF7FFF7FFF7FEF3D
      FF7FEF3DFF7FFF7FF75EFF7F00000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3DFF7FFF7FFF7FF75EFF7F00000000
      EF3D0000FF7FFF7FFF7FFF7FFF7FFF7F0F000F00FF7FFF7FFF7FF75EFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FEF3D0000F75E0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FF75EFF7F00000000EF3D0000FF7FFF7F
      FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FEF3DFF7FEF3DFF7FFF7F
      F75EFF7F00000000EF3D0000FF7FFF7FFF7FFF7FFF7F0F000F000F00FF7FFF7F
      FF7FF75EF75EF75EF75EF75EF75EF75EF75EF75EF75E0000F75E000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000EF3D0000000000000000000000000000
      000000000000000000000000000000000000000000000000F75EFF7F00000000
      EF3D0000FF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7FFF7F
      FF7FFF7FFF7FFF7FF75EFF7F00000000EF3D0000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      F75E000000000000000000000000000000000000000000000000000000000000
      000000000000000000000000000000000000000000000000EF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DFF7F00000000EF3D00000000000000000000000000000000000000000000
      00000000000000000000000000000000F75EFF7F00000000EF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3D00000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      00000000000000000000000000000000EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3D
      EF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DEF3DFF7F00000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000000000000000
      0000000000000000000000000000000000000000000000000000424D3E000000
      000000003E00000028000000640000007D0000000100010000000000D0070000
      0000000000000000000000000000000000000000FFFFFF00FFFFFF8000000000
      0000000000000000FFFFFF80000000000000000000000000FFFFFF8000000000
      0000000000000000FFFFFF80000000000000000000000000FFFFFF8000000000
      0000000000000000FFFCFF80000000000000000000000000FFF87F8000000000
      0000000000000000FFF87F80000000000000000000000000FF70FF8000000000
      0000000000000000FF30FF80000000000000000000000000FF01FF8000000000
      0000000000000000FF01FF80000000000000000000000000FF003F8000000000
      0000000000000000FF007F80000000000000000000000000FF00FF8000000000
      0000000000000000FF01FF80000000000000000000000000FF03FF8000000000
      0000000000000000FF07FF80000000000000000000000000FF0FFF8000000000
      0000000000000000FF1FFF80000000000000000000000000FF3FFF8000000000
      0000000000000000FFFFFF80000000000000000000000000FFFFFF8000000000
      0000000000000000FFFFFF80000000000000000000000000FFFFFF8000000000
      0000000000000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000F00001FFFFFFFFFFFFFFFFFFF0000000E00001FFFFFFFFFF
      FFFFFFFFF0000000E00001FFFFFFFFFFFFFFFFFFF0000000E00001FFFFFFFFFF
      FFFFF87FF0000000E00001FFFFFFF80001FFF87FF0000000E00001FFFFFFF000
      00FFF87FF0000000E00001FFFFFFF00000FFF87FF0000000E00001F0575DF000
      00FFFFFFF0000000E00001F7D75BF00000FFFCFFF0000000E00001F7D757F000
      00FFFC7FF0000000E00001F7D74FF00000FFFC7FF0000000E00001F7D357F000
      00FFFC3FF0000000E00001F7D4DBF00000FFC21FF0000000E00001F7FFDFF000
      00FFC20FF0000000E00001F7FFDFF00000FFC30FF0000000E00001F7DFDFF800
      01FFCF0FF0000000E00001FFFFFFFFFFFFFFC60FF0000000E29401FFFFFFFFFF
      FFFFE01FF0000000E24403FFFFFFFFFFFFFFFFFFF0000000FEB7FFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFF8000070000000FFFFFFFFFFFFFFFFFFF8000070000000FFFFFFFFFFFFFFF0
      1FF8000070000000FFFFFFFFFFFFFFE00FF80000700000000000000000003FC0
      07F80000700000000000000000003F8003F80000700000000000000000003F00
      01F80000700000000000000000003F0001F80000700000000000000000003F00
      01F80000700000000000000000003F0001F80000700000000000000000003F00
      01F80000700000000000000000003F0001F80000700000000000000000003F00
      01F80000700000000000000000003F8003F80000700000000000000000003FC0
      07F80000700000000000000000003FE00FF80000700000000000000000003FF0
      1FF8000070000000FFFFFFFFFFFFFFFFFFF8000070000000FFFFFFFFFFFFFFFF
      FFF8000070000000FFFFFFFFFFFFFFFFFFF8000070000000FFFFFFFFFFFFFFFF
      FFF8000070000000FFFFFFFFFFFFFFFFFFF8000070000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFF00000FFFF
      FFFFFFFFF0000000FFFFFFF00000FFFFFFFFFFFFF0000000FFFFFFF00000FFFF
      FFFFFFFFF0000000000000700000FF8003FFFC3FF0000000000000700000FF80
      03FFF00FF0000000000000700000FF8003FFE007F0000000000000700000FF80
      03FFE007F0000000000000700000FF8003FFC003F0000000000000700000FF80
      03FFC003F0000000000000700000FF8003FFC003F0000000000000700000FF80
      03FFC003F0000000000000700000FF8003FFE007F0000000000000700000FF80
      03FFE007F0000000000000700000FF8003FFF00FF0000000000000700000FF80
      03FFFC3FF0000000000000700000FF8003FFFFFFF0000000FFFFFFF00000FFFF
      FFFFFFFFF0000000FFFFFFF00000FFFFFFFFFFFFF0000000FFFFFFF00000FFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFE000007FFFFFFFFFFFF0000000FFFFFFE000006000
      0060000000000000FFFFFFE0000060000060000000000000FFFFFFE000006000
      0060000000000000C42711E0000060000060000000000000EB5AFBE000006000
      0060000000000000EC5A1BE0000060000060000000000000EF5ADBE000006000
      0060000000000000ECC73BE0000060000060000000000000EFDFFBE000006000
      0060000000000000EFDFFBE0000060000060000000000000CF9FF3E000006000
      0060000000000000FFFFFFE0000060000060000000000000FFFFFFE000006000
      0060000000000000FFFFFFFFFFFFE000007FFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF0000000FFFFFFFFFFFFFFFFFFFFFFFFF0000000FFFFFFFFFFFFFFFF
      FFFFFFFFF000000000000000000000000000000000000000000000000000}
  end
  object WinListPopup: TTBXPopupMenu
    Images = MainImages
    OnPopup = WinListPopupPopup
    Left = 224
    Top = 200
    object wpuShowItem: TTBXItem
      Action = ShowWindowCmd
      Options = [tboDefault]
    end
    object wpuCloseItem: TTBXItem
      Action = CloseWindowCmd
    end
    object wpuSeparator1: TTBXSeparatorItem
    end
    object wpuSaveItem: TTBXItem
      Action = SaveWinCmd
    end
    object wpuSaveAsItem: TTBXItem
      Action = SaveAsWinCmd
    end
    object wpuSeparator2: TTBXSeparatorItem
    end
    object wpuTileHorItem: TTBXItem
      Action = TileHorCmd
    end
    object wpuTileVerItem: TTBXItem
      Action = TileVerCmd
    end
    object wpuSeparator3: TTBXSeparatorItem
    end
    object wpuCompItem: TTBXItem
      Action = CompWinCmd
    end
    object wpuCompRunItem: TTBXItem
      Action = CompRunWinCmd
    end
    object wpuRunItem: TTBXItem
      Action = RunWinCmd
    end
  end
  object SystemImageList: TImageList
    BlendColor = clHighlight
    ShareImages = True
    Left = 256
    Top = 72
  end
  object PluginItemsImages: TImageList
    Left = 256
    Top = 104
  end
  object SynEditSearchEngine: TSynEditSearch
    Left = 256
    Top = 136
  end
end
