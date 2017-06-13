object EditFrm: TEditFrm
  Left = 314
  Top = 126
  Width = 389
  Height = 245
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsMDIChild
  OldCreateOrder = False
  Position = poDefault
  Visible = True
  OnActivate = FormActivate
  OnClose = FormClose
  OnCloseQuery = FormCloseQuery
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object Splitter1: TSplitter
    Left = 0
    Top = 105
    Width = 381
    Height = 3
    Cursor = crVSplit
    Align = alBottom
    AutoSnap = False
    MinSize = 1
    ResizeStyle = rsUpdate
  end
  object Edit: TSynMemo
    Left = 0
    Top = 0
    Width = 381
    Height = 105
    Align = alClient
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -13
    Font.Name = 'Courier New'
    Font.Style = []
    ParentShowHint = False
    PopupMenu = MainFrm.EditPopup
    ShowHint = False
    TabOrder = 0
    OnClick = EditClick
    OnKeyDown = EditKeyDown
    OnKeyPress = EditKeyPress
    OnKeyUp = EditKeyUp
    OnMouseDown = EditMouseDown
    OnMouseMove = EditMouseMove
    Gutter.Font.Charset = DEFAULT_CHARSET
    Gutter.Font.Color = clWindowText
    Gutter.Font.Height = -11
    Gutter.Font.Name = 'Terminal'
    Gutter.Font.Style = []
    Gutter.Width = 20
    RightEdge = 0
    ScrollHintFormat = shfTopToBottom
    TabWidth = 2
    WantTabs = True
    OnChange = EditChange
    OnContextHelp = EditContextHelp
    OnMouseCursor = EditMouseCursor
    OnProcessCommand = SynEditProcessCommand
    OnSpecialLineColors = EditSpecialLineColors
    OnStatusChange = EditStatusChange
    RemovedKeystrokes = <>
    AddedKeystrokes = <
      item
        Command = ecContextHelp
        ShortCut = 8304
      end>
  end
  object LogBox: TSynEdit
    Left = 0
    Top = 108
    Width = 381
    Height = 110
    Cursor = crDefault
    Align = alBottom
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -13
    Font.Name = 'Courier New'
    Font.Style = []
    ParentColor = True
    PopupMenu = MainFrm.LogBoxPopup
    TabOrder = 1
    OnClick = LogBoxClick
    Gutter.Font.Charset = DEFAULT_CHARSET
    Gutter.Font.Color = clWindowText
    Gutter.Font.Height = -11
    Gutter.Font.Name = 'Courier New'
    Gutter.Font.Style = []
    Gutter.Width = 6
    MaxUndo = 0
    Options = [eoAutoIndent, eoDragDropEditing, eoEnhanceEndKey, eoGroupUndo, eoNoCaret, eoNoSelection, eoSmartTabDelete, eoSmartTabs, eoTabsToSpaces]
    ReadOnly = True
    RightEdge = 0
    OnMouseCursor = LogBoxMouseCursor
    OnSpecialLineColors = LogBoxSpecialLineColors
    RemovedKeystrokes = <
      item
        Command = ecUp
        ShortCut = 38
      end
      item
        Command = ecDown
        ShortCut = 40
      end>
    AddedKeystrokes = <
      item
        Command = ecScrollUp
        ShortCut = 38
      end
      item
        Command = ecScrollDown
        ShortCut = 40
      end>
  end
end
