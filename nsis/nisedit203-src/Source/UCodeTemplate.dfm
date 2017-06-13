object CodeTemplateFrm: TCodeTemplateFrm
  Left = 20
  Top = 61
  BorderStyle = bsDialog
  Caption = '*'
  ClientHeight = 283
  ClientWidth = 576
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 13
  object CancelarBtn: TButton
    Left = 496
    Top = 250
    Width = 75
    Height = 25
    Cancel = True
    Caption = '*'
    ModalResult = 2
    TabOrder = 4
  end
  object AceptarBtn: TButton
    Left = 416
    Top = 250
    Width = 75
    Height = 25
    Caption = '*'
    Default = True
    ModalResult = 1
    TabOrder = 3
  end
  object GroupBox1: TNewGroupBox
    Left = 8
    Top = 8
    Width = 137
    Height = 185
    TabOrder = 0
    object ListBox: TListBox
      Left = 8
      Top = 16
      Width = 121
      Height = 129
      ItemHeight = 13
      TabOrder = 0
      OnClick = ListBoxClick
      OnDblClick = ListBoxDblClick
    end
    object TBToolbar1: TTBXToolbar
      Left = 33
      Top = 152
      Width = 69
      Height = 22
      Caption = 'TBToolbar1'
      Images = MainFrm.MainImages
      TabOrder = 1
      object TBItem2: TTBXItem
        Action = NewTemplateCmd
      end
      object TBItem1: TTBXItem
        Action = EditTemplateCmd
      end
      object TBItem3: TTBXItem
        Action = DeleteTemplateCmd
      end
    end
  end
  object GroupBox2: TNewGroupBox
    Left = 152
    Top = 8
    Width = 417
    Height = 233
    TabOrder = 2
    object Edit: TSynEdit
      Left = 8
      Top = 16
      Width = 401
      Height = 209
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -13
      Font.Name = 'Courier New'
      Font.Style = []
      TabOrder = 0
      OnEnter = EditEnter
      OnExit = EditExit
      Gutter.Font.Charset = DEFAULT_CHARSET
      Gutter.Font.Color = clWindowText
      Gutter.Font.Height = -11
      Gutter.Font.Name = 'Terminal'
      Gutter.Font.Style = []
      Gutter.Visible = False
      Options = [eoAutoIndent, eoDragDropEditing, eoGroupUndo, eoScrollPastEol, eoSmartTabDelete, eoSmartTabs, eoTabsToSpaces, eoTrimTrailingSpaces]
      RightEdge = 0
      OnChange = EditChange
      RemovedKeystrokes = <
        item
          Command = ecContextHelp
          ShortCut = 112
        end>
      AddedKeystrokes = <
        item
          Command = ecContextHelp
          ShortCut = 16496
        end>
    end
  end
  object GroupBox3: TNewGroupBox
    Left = 8
    Top = 200
    Width = 137
    Height = 41
    TabOrder = 1
    object HotKey: THotKey
      Left = 8
      Top = 16
      Width = 121
      Height = 19
      HotKey = 32833
      InvalidKeys = [hcNone, hcShift]
      Modifiers = [hkAlt]
      TabOrder = 0
    end
  end
  object ActionList1: TActionList
    Left = 144
    Top = 32
    object NewTemplateCmd: TAction
      ImageIndex = 0
      OnExecute = NewTemplateCmdExecute
    end
    object EditTemplateCmd: TAction
      ImageIndex = 19
      OnExecute = EditTemplateCmdExecute
      OnUpdate = CheckIndex
    end
    object DeleteTemplateCmd: TAction
      ImageIndex = 10
      OnExecute = DeleteTemplateCmdExecute
      OnUpdate = CheckIndex
    end
  end
end
