object CompilerProfilesFrm: TCompilerProfilesFrm
  Left = 210
  Top = 156
  BorderStyle = bsDialog
  BorderWidth = 5
  ClientHeight = 190
  ClientWidth = 308
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  KeyPreview = True
  OldCreateOrder = False
  Position = poMainFormCenter
  OnCreate = FormCreate
  OnKeyDown = FormKeyDown
  PixelsPerInch = 96
  TextHeight = 13
  object AgregarBtn: TButton
    Left = 214
    Top = 0
    Width = 89
    Height = 25
    TabOrder = 1
    OnClick = AgregarBtnClick
  end
  object EditarBtn: TButton
    Left = 214
    Top = 40
    Width = 89
    Height = 25
    TabOrder = 2
    OnClick = EditarBtnClick
  end
  object EliminarBtn: TButton
    Left = 214
    Top = 120
    Width = 89
    Height = 25
    TabOrder = 4
    OnClick = EliminarBtnClick
  end
  object CerrarBtn: TButton
    Left = 214
    Top = 160
    Width = 89
    Height = 25
    ModalResult = 1
    TabOrder = 5
  end
  object ProfileList: TListView
    Left = 0
    Top = 0
    Width = 205
    Height = 185
    Columns = <
      item
        Width = 195
      end>
    RowSelect = True
    ShowColumnHeaders = False
    TabOrder = 0
    ViewStyle = vsReport
    OnDblClick = ProfileListDblClick
    OnEdited = ProfileListEdited
    OnEditing = ProfileListEditing
  end
  object RenameBtn: TButton
    Left = 215
    Top = 80
    Width = 89
    Height = 25
    TabOrder = 3
    OnClick = RenameBtnClick
  end
end
