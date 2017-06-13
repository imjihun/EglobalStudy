object AddFilesFrm: TAddFilesFrm
  Left = 69
  Top = 89
  BorderStyle = bsDialog
  BorderWidth = 5
  Caption = 'Add files'
  ClientHeight = 243
  ClientWidth = 500
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
  object FileLst: TListView
    Left = 0
    Top = 0
    Width = 417
    Height = 241
    Columns = <
      item
        Caption = 'Source file or directory'
        Width = 220
      end
      item
        Caption = 'Destination directory'
        Width = 180
      end>
    ColumnClick = False
    HideSelection = False
    MultiSelect = True
    ReadOnly = True
    RowSelect = True
    TabOrder = 0
    ViewStyle = vsReport
  end
  object Button1: TButton
    Left = 424
    Top = 152
    Width = 75
    Height = 25
    Caption = '&Add file'
    TabOrder = 1
    OnClick = Button1Click
  end
  object Button3: TButton
    Left = 424
    Top = 216
    Width = 75
    Height = 25
    Caption = '&Delete'
    TabOrder = 2
    OnClick = Button3Click
  end
  object Button4: TButton
    Left = 424
    Top = 0
    Width = 75
    Height = 25
    Caption = '&OK'
    Default = True
    ModalResult = 1
    TabOrder = 3
  end
  object Button5: TButton
    Left = 424
    Top = 32
    Width = 75
    Height = 25
    Cancel = True
    Caption = '&Cancel'
    ModalResult = 2
    TabOrder = 4
  end
  object Button6: TButton
    Left = 424
    Top = 185
    Width = 75
    Height = 25
    Caption = '&Add directory'
    TabOrder = 5
    OnClick = Button6Click
  end
end
