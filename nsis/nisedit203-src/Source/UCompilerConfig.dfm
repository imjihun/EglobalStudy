object CompilerConfigFrm: TCompilerConfigFrm
  Left = 0
  Top = 0
  Width = 443
  Height = 277
  Align = alClient
  AutoScroll = False
  TabOrder = 0
  object GroupBox8: TNewGroupBox
    Left = 8
    Top = 0
    Width = 465
    Height = 97
    TabOrder = 0
    object Label14: TStaticText
      Left = 8
      Top = 24
      Width = 8
      Height = 17
      Caption = '*'
      FocusControl = CopilEdt
      TabOrder = 4
    end
    object Label15: TStaticText
      Left = 8
      Top = 48
      Width = 8
      Height = 17
      Caption = '*'
      FocusControl = HTMLEdt
      TabOrder = 5
    end
    object CopilEdt: TEdit
      Left = 88
      Top = 24
      Width = 345
      Height = 21
      ParentColor = True
      ReadOnly = True
      TabOrder = 0
    end
    object ExaminarExeBtn: TButton
      Left = 436
      Top = 24
      Width = 21
      Height = 21
      Caption = '...'
      TabOrder = 1
      OnClick = ExaminarExeBtnClick
    end
    object HTMLEdt: TEdit
      Left = 88
      Top = 48
      Width = 345
      Height = 21
      ParentColor = True
      ReadOnly = True
      TabOrder = 2
    end
    object ExaminarHtmlBtn: TButton
      Left = 436
      Top = 48
      Width = 21
      Height = 21
      Caption = '...'
      TabOrder = 3
      OnClick = ExaminarHtmlBtnClick
    end
    object UseIntegratedBrowserChk: TCheckBox
      Left = 88
      Top = 72
      Width = 369
      Height = 17
      TabOrder = 6
    end
  end
  object GroupBox7: TNewGroupBox
    Left = 8
    Top = 104
    Width = 465
    Height = 97
    TabOrder = 1
    object NoConfigChk: TCheckBox
      Left = 8
      Top = 24
      Width = 449
      Height = 17
      Caption = '*'
      TabOrder = 0
    end
    object NoCDChk: TCheckBox
      Left = 8
      Top = 48
      Width = 449
      Height = 17
      Caption = '*'
      TabOrder = 1
    end
    object SaveScriptsBeforeCompileChk: TCheckBox
      Left = 8
      Top = 72
      Width = 449
      Height = 17
      Caption = '*'
      TabOrder = 2
    end
  end
  object GroupBox9: TNewGroupBox
    Left = 8
    Top = 208
    Width = 465
    Height = 177
    TabOrder = 2
    object SimbLst: TListView
      Left = 16
      Top = 16
      Width = 345
      Height = 153
      Columns = <
        item
          Width = 190
        end
        item
          Width = 140
        end>
      ColumnClick = False
      ReadOnly = True
      RowSelect = True
      TabOrder = 0
      ViewStyle = vsReport
      OnDblClick = SimbLstDblClick
      OnKeyDown = SimbLstKeyDown
    end
    object AgregarBtn: TButton
      Left = 368
      Top = 16
      Width = 89
      Height = 25
      Caption = '*'
      TabOrder = 1
      OnClick = AgregarBtnClick
    end
    object EditarBtn: TButton
      Left = 368
      Top = 48
      Width = 89
      Height = 25
      Caption = '*'
      TabOrder = 2
      OnClick = EditarBtnClick
    end
    object EliminarBtn: TButton
      Left = 368
      Top = 80
      Width = 89
      Height = 25
      Caption = '*'
      TabOrder = 3
      OnClick = EliminarBtnClick
    end
  end
  object AbrirDlg: TOpenDialog
    Options = [ofHideReadOnly, ofPathMustExist, ofFileMustExist, ofEnableSizing]
    OnCanClose = AbrirDlgCanClose
    Left = 292
    Top = 232
  end
end
