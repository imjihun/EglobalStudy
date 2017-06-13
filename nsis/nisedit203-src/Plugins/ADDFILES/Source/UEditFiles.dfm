object EditFilesFrm: TEditFilesFrm
  Left = 278
  Top = 339
  BorderStyle = bsDialog
  BorderWidth = 5
  ClientHeight = 140
  ClientWidth = 356
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
    Width = 67
    Height = 17
    Caption = 'File(s) to add:'
    TabOrder = 0
  end
  object FilesEdt: TEdit
    Left = 0
    Top = 16
    Width = 321
    Height = 21
    TabOrder = 1
  end
  object Button1: TButton
    Left = 328
    Top = 16
    Width = 25
    Height = 21
    Caption = '...'
    TabOrder = 2
    OnClick = Button1Click
  end
  object StaticText2: TStaticText
    Left = 0
    Top = 48
    Width = 103
    Height = 17
    Caption = 'Destination directory:'
    TabOrder = 4
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
  object Button4: TButton
    Left = 198
    Top = 112
    Width = 75
    Height = 25
    Caption = '&OK'
    Default = True
    ModalResult = 1
    TabOrder = 5
  end
  object Button5: TButton
    Left = 278
    Top = 112
    Width = 75
    Height = 25
    Cancel = True
    Caption = '&Cancel'
    ModalResult = 2
    TabOrder = 6
  end
  object RelativeChk: TCheckBox
    Left = 0
    Top = 96
    Width = 129
    Height = 17
    Caption = 'Relative path'
    TabOrder = 7
  end
  object OpenDlg: TOpenDialog
    Options = [ofHideReadOnly, ofAllowMultiSelect, ofEnableSizing]
    Left = 328
    Top = 88
  end
end
