object EditCompilerProfileFrm: TEditCompilerProfileFrm
  Left = 161
  Top = 72
  BorderStyle = bsDialog
  BorderWidth = 5
  ClientHeight = 435
  ClientWidth = 482
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object CancelBtn: TButton
    Left = 400
    Top = 408
    Width = 75
    Height = 25
    Cancel = True
    ModalResult = 2
    TabOrder = 1
  end
  object OKBtn: TButton
    Left = 312
    Top = 408
    Width = 75
    Height = 25
    Default = True
    ModalResult = 1
    TabOrder = 0
  end
  object Panel1: TPanel
    Left = 0
    Top = 0
    Width = 482
    Height = 401
    Align = alTop
    BevelOuter = bvLowered
    TabOrder = 2
    inline CompilerConfigFrm: TCompilerConfigFrm
      Left = 1
      Top = 1
      Width = 475
      Height = 392
      Align = alNone
    end
  end
end
