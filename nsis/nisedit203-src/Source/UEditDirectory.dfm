object EditDirectoryFrm: TEditDirectoryFrm
  Left = 191
  Top = 191
  BorderStyle = bsDialog
  BorderWidth = 5
  ClientHeight = 191
  ClientWidth = 340
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object StaticText1: TStaticText
    Left = 0
    Top = 0
    Width = 8
    Height = 17
    Caption = '*'
    TabOrder = 4
  end
  object DirEdt: TEdit
    Left = 0
    Top = 16
    Width = 305
    Height = 21
    TabOrder = 0
  end
  object Button1: TButton
    Left = 312
    Top = 16
    Width = 25
    Height = 21
    Caption = '...'
    TabOrder = 1
    OnClick = Button1Click
  end
  object StaticText2: TStaticText
    Left = 0
    Top = 48
    Width = 8
    Height = 17
    Caption = '*'
    TabOrder = 2
  end
  object DestinoEdt: TComboBox
    Left = 0
    Top = 64
    Width = 337
    Height = 21
    ItemHeight = 13
    Sorted = True
    TabOrder = 3
  end
  object AceptarBtn: TButton
    Left = 182
    Top = 160
    Width = 75
    Height = 25
    Default = True
    ModalResult = 1
    TabOrder = 5
  end
  object CancelarBtn: TButton
    Left = 262
    Top = 160
    Width = 75
    Height = 25
    Cancel = True
    ModalResult = 2
    TabOrder = 6
  end
  object NewGroupBox2: TNewGroupBox
    Left = 0
    Top = 88
    Width = 337
    Height = 57
    TabOrder = 7
    object OverwriteChk: TComboBox
      Left = 16
      Top = 24
      Width = 305
      Height = 21
      Style = csDropDownList
      ItemHeight = 13
      TabOrder = 0
    end
  end
end
