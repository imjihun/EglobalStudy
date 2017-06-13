object AskSaveDialog: TAskSaveDialog
  Left = 408
  Top = 211
  AutoScroll = False
  BorderIcons = [biSystemMenu]
  ClientHeight = 243
  ClientWidth = 325
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  Icon.Data = {
    0000010001001010100001000400280100001600000028000000100000002000
    0000010004000000000080000000000000000000000000000000000000000000
    000000008000008000000080800080000000800080008080000080808000C0C0
    C0000000FF0000FF000000FFFF00FF000000FF00FF00FFFF0000FFFFFF000000
    0000000000000000000000000000033000080300000003300008030000000330
    0000030300000333333333030000033000003303030003088888030303000308
    8888030303000308888800030300030888880803030000000000000003000003
    088888080300000000000000000000000308888808000000000000000000FFFF
    0000801F0000001F000000070000000700000001000000010000000100000001
    0000000100000001000000010000C0010000C0010000F0010000F0010000}
  KeyPreview = True
  OldCreateOrder = False
  Position = poOwnerFormCenter
  OnKeyPress = FormKeyPress
  OnResize = FormResize
  PixelsPerInch = 96
  TextHeight = 13
  object lblPrompt: TLabel
    Left = 60
    Top = 12
    Width = 258
    Height = 61
    Anchors = [akLeft, akTop, akRight]
    AutoSize = False
    Caption = '*'
    FocusControl = lstFiles
    WordWrap = True
  end
  object imgIcon: TImage
    Left = 16
    Top = 16
    Width = 32
    Height = 32
    Transparent = True
  end
  object lstFiles: TListBox
    Left = 6
    Top = 80
    Width = 313
    Height = 127
    Anchors = [akLeft, akTop, akRight, akBottom]
    ExtendedSelect = False
    ItemHeight = 18
    MultiSelect = True
    Style = lbOwnerDrawFixed
    TabOrder = 0
    OnClick = lstFilesClick
    OnDrawItem = lstFilesDrawItem
    OnKeyDown = lstFilesKeyDown
    OnKeyPress = lstFilesKeyPress
  end
  object btnYes: TButton
    Left = 5
    Top = 214
    Width = 75
    Height = 23
    Anchors = [akRight, akBottom]
    Default = True
    ModalResult = 6
    TabOrder = 1
  end
  object btnCancel: TButton
    Left = 165
    Top = 214
    Width = 75
    Height = 23
    Anchors = [akRight, akBottom]
    Cancel = True
    ModalResult = 2
    TabOrder = 3
  end
  object btnAll: TButton
    Left = 245
    Top = 214
    Width = 75
    Height = 23
    Anchors = [akRight, akBottom]
    ModalResult = 8
    TabOrder = 4
  end
  object btnNo: TButton
    Left = 85
    Top = 214
    Width = 75
    Height = 23
    Anchors = [akRight, akBottom]
    ModalResult = 7
    TabOrder = 2
  end
end
