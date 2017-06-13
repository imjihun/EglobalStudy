object EditDirectoryFrm: TEditDirectoryFrm
  Left = 86
  Top = 153
  BorderStyle = bsDialog
  BorderWidth = 5
  Caption = 'Edit directory'
  ClientHeight = 171
  ClientWidth = 353
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poScreenCenter
  PixelsPerInch = 96
  TextHeight = 13
  object StaticText1: TStaticText
    Left = 0
    Top = 0
    Width = 82
    Height = 17
    Caption = 'Directory to add:'
    TabOrder = 4
  end
  object DirEdt: TEdit
    Left = 0
    Top = 16
    Width = 321
    Height = 21
    TabOrder = 0
  end
  object Button1: TButton
    Left = 328
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
    Width = 103
    Height = 17
    Caption = 'Destination directory:'
    TabOrder = 2
  end
  object DestinoEdt: TComboBox
    Left = 0
    Top = 64
    Width = 353
    Height = 21
    ItemHeight = 13
    Sorted = True
    TabOrder = 3
    Text = '$INSTDIR'
  end
  object RelativeChk: TCheckBox
    Left = 0
    Top = 96
    Width = 129
    Height = 17
    Caption = 'Relative path'
    TabOrder = 5
  end
  object IndividualChk: TCheckBox
    Left = 0
    Top = 136
    Width = 129
    Height = 17
    Caption = 'Individual entries'
    TabOrder = 6
  end
  object RecurseChk: TCheckBox
    Left = 0
    Top = 115
    Width = 137
    Height = 17
    Caption = 'Recurse subdirectories'
    TabOrder = 7
  end
  object Button4: TButton
    Left = 198
    Top = 144
    Width = 75
    Height = 25
    Caption = '&OK'
    Default = True
    ModalResult = 1
    TabOrder = 8
  end
  object Button5: TButton
    Left = 278
    Top = 144
    Width = 75
    Height = 25
    Cancel = True
    Caption = '&Cancel'
    ModalResult = 2
    TabOrder = 9
  end
end
