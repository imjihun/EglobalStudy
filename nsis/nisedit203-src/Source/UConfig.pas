{
  HM NIS Edit (c) 2003-2004 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Configuration window

}
unit UConfig;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, ComCtrls, SynEdit, SynEditHighlighter,
  SynHighlighterNSIS, MySynEditorOptionsContainer, TBX, TBXThemes,
  NewGroupBox, UIStateForm, UCompilerConfig;

type
  TConfigFrm = class(TUIStateForm)
    ColorDlg: TColorDialog;
    FontDlg: TFontDialog;
    AbrirDlg: TOpenDialog;
    Panel1: TPanel;
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    GroupBox2: TNewGroupBox;
    Label11: TStaticText;
    ThemeList: TComboBox;
    SDragToolBarsChk: TCheckBox;
    SDragPanelsChk: TCheckBox;
    GroupBox3: TNewGroupBox;
    Label13: TStaticText;
    Label12: TStaticText;
    WelcomeDialogChk: TCheckBox;
    MultInstChk: TCheckBox;
    LangList: TComboBox;
    IniDesignerChk: TCheckBox;
    BrowserHomeEdt: TEdit;
    SaveFileListChk: TCheckBox;
    SplashChk: TCheckBox;
    UseDefBrowserChk: TCheckBox;
    GroupBox1: TNewGroupBox;
    RegisterBtn: TButton;
    NewGroupBox2: TNewGroupBox;
    DisplayGridChk: TCheckBox;
    StaticText1: TStaticText;
    GridSizeXEdt: TEdit;
    StaticText2: TStaticText;
    GridSizeYEdt: TEdit;
    SnapToGridChk: TCheckBox;
    RightBottomChk: TRadioButton;
    WidthHeightChk: TRadioButton;
    BothChk: TRadioButton;
    StaticText3: TStaticText;
    NewGroupBox3: TNewGroupBox;
    StaticText4: TStaticText;
    RegistryChk: TRadioButton;
    IniFileChk: TRadioButton;
    TabSheet2: TTabSheet;
    PageControl2: TPageControl;
    TabSheet4: TTabSheet;
    gbGutter: TNewGroupBox;
    Label1: TStaticText;
    ckGutterShowLineNumbers: TCheckBox;
    ckGutterShowLeaderZeros: TCheckBox;
    ckGutterStartAtZero: TCheckBox;
    ckGutterVisible: TCheckBox;
    cbGutterFont: TCheckBox;
    btnGutterFont: TButton;
    pGutterBack: TPanel;
    pGutterColor: TPanel;
    pnlGutterFontDisplay: TPanel;
    lblGutterFont: TStaticText;
    gbRightEdge: TNewGroupBox;
    Label3: TStaticText;
    Label10: TStaticText;
    pRightEdgeBack: TPanel;
    pRightEdgeColor: TPanel;
    eRightEdge: TEdit;
    gbBookmarks: TNewGroupBox;
    ckBookmarkKeys: TCheckBox;
    ckBookmarkVisible: TCheckBox;
    gbEditorFont: TNewGroupBox;
    btnFont: TButton;
    Panel3: TPanel;
    labFont: TStaticText;
    gbLineSpacing: TNewGroupBox;
    Label8: TStaticText;
    Label9: TStaticText;
    eLineSpacing: TEdit;
    eTabWidth: TEdit;
    TabSheet5: TTabSheet;
    gbOptions: TNewGroupBox;
    ckViewUsageHints: TCheckBox;
    ckHalfPageScroll: TCheckBox;
    ckHideShowScrollbars: TCheckBox;
    ckEnhanceHomeKey: TCheckBox;
    ckRightMouseMoves: TCheckBox;
    ckSmartTabDelete: TCheckBox;
    ckSmartTabs: TCheckBox;
    ckKeepCaretX: TCheckBox;
    ckAltSetsColumnMode: TCheckBox;
    ckDragAndDropEditing: TCheckBox;
    ckAutoIndent: TCheckBox;
    ckScrollByOneLess: TCheckBox;
    ckScrollPastEOF: TCheckBox;
    ckScrollPastEOL: TCheckBox;
    ckShowScrollHint: TCheckBox;
    ckTabsToSpaces: TCheckBox;
    ckTrimTrailingSpaces: TCheckBox;
    ckScrollHintFollows: TCheckBox;
    ckGroupUndo: TCheckBox;
    ckDisableScrollArrows: TCheckBox;
    ckShowSpecialChars: TCheckBox;
    ckUndoAfterSave: TCheckBox;
    gbCaret: TNewGroupBox;
    Label2: TStaticText;
    Label4: TStaticText;
    cInsertCaret: TComboBox;
    cOverwriteCaret: TComboBox;
    EditKeyStrokesBtn: TButton;
    TabSheet6: TTabSheet;
    GroupBox11: TNewGroupBox;
    Label7: TStaticText;
    HighLightChk: TCheckBox;
    SynEdit: TSynEdit;
    GroupBox13: TNewGroupBox;
    Bevel1: TBevel;
    Bevel2: TBevel;
    Label5: TStaticText;
    Label6: TStaticText;
    Panel2: TPanel;
    Panel4: TPanel;
    CheckBox1: TCheckBox;
    CheckBox2: TCheckBox;
    CheckBox3: TCheckBox;
    HLVarsInStrsChk: TCheckBox;
    ListBox: TListBox;
    TabSheet3: TTabSheet;
    CompilerConfigFrm1: TCompilerConfigFrm;
    TabSheet7: TTabSheet;
    NewGroupBox1: TNewGroupBox;
    PluginsList: TListBox;
    ConfigPluginBtn: TButton;
    AboutPluginBtn: TButton;
    PluginFileNameLbl: TStaticText;
    PaintBox: TPaintBox;
    Label14: TLabel;
    TreeView: TTreeView;
    AceptarBtn: TButton;
    CancelarBtn: TButton;
    ApplyBtn: TButton;
    procedure FormCreate(Sender: TObject);
    procedure HighLightChkClick(Sender: TObject);
    procedure ListBoxClick(Sender: TObject);
    procedure SelectColor(Sender: TObject);
    procedure AttriChange(Sender: TObject);
    procedure HLVarsInStrsChkClick(Sender: TObject);
    procedure btnGutterFontClick(Sender: TObject);
    procedure btnFontClick(Sender: TObject);
    procedure RegisterBtnClick(Sender: TObject);
    procedure LangListClick(Sender: TObject);
    procedure ckGutterShowLineNumbersClick(Sender: TObject);
    procedure ckGutterVisibleClick(Sender: TObject);
    procedure PluginsListClick(Sender: TObject);
    procedure ConfigPluginBtnClick(Sender: TObject);
    procedure AboutPluginBtnClick(Sender: TObject);
    procedure PluginsListDblClick(Sender: TObject);
    procedure ApplyBtnClick(Sender: TObject);
    procedure BrowserHomeEdtChange(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure SynEditMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure EditKeyStrokesBtnClick(Sender: TObject);
    procedure TreeViewChange(Sender: TObject; Node: TTreeNode);
    procedure PaintBoxPaint(Sender: TObject);
  private
    Liter: TSynNSISSyn;
    FAvailableTBXThemes: TStringList;
    LangChanged: Boolean;
    SaveForeground, SaveBackground: TColor;
    NoUpdateAtrri: Boolean;
    FSynEdit: TMySynEditorOptionsContainer;

    procedure LoadLangStrings;
    procedure InitControlValues;
    procedure SaveOptions;
  public
    AddingSymbol: Boolean;
  end;

var
  ConfigFrm: TConfigFrm;

function ShowConfig(const NumPage: Integer = 0;
   const ShowOnlyThisPage: Boolean = False): Boolean;
implementation

uses IniFiles, Registry, Utils, UMain, UCustomMDIChild, UEditSimbol, UEdit,
  PluginsManger, PluginsInt, UIODesigner, SynEditTypes, SynEditKeyCmdsEditor;

{$R *.DFM}

function ShowConfig(const NumPage: Integer = 0;
   const ShowOnlyThisPage: Boolean = False): Boolean;
begin
  ConfigFrm := TConfigFrm.Create(Application);
  try
    ConfigFrm.PageControl1.ActivePageIndex := NumPage;
    ConfigFrm.Label14.Caption := ConfigFrm.PageControl1.ActivePage.Caption;
    if ShowOnlyThisPage then
    begin
      ConfigFrm.TreeView.Visible := False;
      ConfigFrm.PaintBox.Visible := False;
      ConfigFrm.Label14.Visible := False;
      ConfigFrm.ClientWidth := 489;
      ConfigFrm.ClientHeight := ConfigFrm.ClientHeight - ConfigFrm.PaintBox.Height;
    end;
    Result := ConfigFrm.ShowModal = mrOK;
    if Result then
      ConfigFrm.SaveOptions;
  finally
    FreeAndNil(ConfigFrm);
  end;
end;

procedure TConfigFrm.FormCreate(Sender: TObject);
var
  Found: Integer;
  SR: TSearchRec;

  procedure AddPagesToTreeView(PageControl: TPageControl; Root: TTreeNode);
  var
    C: Integer;
    TreeNode: TTreeNode;
    Page: TTabSheet;
  begin
    for C := 0 to PageControl.PageCount - 1 do
    begin
      Page := PageControl.Pages[C];
      TreeNode := TreeView.Items.AddChild(Root, Page.Caption);
      TreeNode.Data := Page;
      if Page = TabSheet2 then
        AddPagesToTreeView(PageControl2, TreeNode);
      if Root <> nil then
        Root.Expand(True);
    end;
  end;

begin
  Found := FindFirst(ExtractFilePath(ParamStr(0)) + 'Lang\*.lng',
    faArchive, SR);
  while Found = 0 do
  begin
    LangList.Items.Add(ChangeFileExt(SR.Name, ''));
    Found := FindNext(SR);
  end;
  FindClose(SR);

  Liter := TSynNSISSyn.Create(Self);
  FAvailableTBXThemes := TStringList.Create;
  GetAvailableTBXThemes(FAvailableTBXThemes);
  FAvailableTBXThemes.Sort;

  LoadLangStrings;
  InitControlValues;

  AddPagesToTreeView(PageControl1, nil);
end;

procedure TConfigFrm.HighLightChkClick(Sender: TObject);
begin
  CheckBox1.Enabled := HighLightChk.Checked;
  CheckBox2.Enabled := HighLightChk.Checked;
  CheckBox3.Enabled := HighLightChk.Checked;
  Label7.Enabled := HighLightChk.Checked;
  Label5.Enabled := HighLightChk.Checked;
  Label6.Enabled := HighLightChk.Checked;
  HLVarsInStrsChk.Enabled := HighLightChk.Checked;

  ListBox.Enabled := HighLightChk.Checked;
  ListBox.Color := EnabledColors[ListBox.Enabled];

  Panel2.Enabled := HighLightChk.Checked;
  Panel4.Enabled := HighLightChk.Checked;

  if HighLightChk.Checked then
  begin
    SynEdit.Highlighter := Liter;
    Panel2.Color := SaveForeground;
    Panel4.Color := SaveBackground;
  end else
  begin
    SynEdit.Highlighter := nil;
    SaveForeground := Panel2.Color;
    SaveBackground := Panel4.Color;
    Panel2.ParentColor := True;
    Panel4.ParentColor := True;
  end;
end;

procedure TConfigFrm.ListBoxClick(Sender: TObject);
var
  Attri: TSynHighlighterAttributes;
begin
  NoUpdateAtrri := True;
  try
    Attri := TSynHighlighterAttributes(ListBox.Items.Objects[ListBox.ItemIndex]);
    Panel2.Color := Attri.Foreground;
    Panel4.Color := Attri.Background;
    CheckBox1.Checked := fsBold in Attri.Style;
    CheckBox2.Checked := fsItalic in Attri.Style;
    CheckBox3.Checked := fsUnderline in Attri.Style;
  finally
    NoUpdateAtrri := False;
  end;
end;

procedure TConfigFrm.SelectColor(Sender: TObject);
begin
  ColorDlg.Color := TPanel(Sender).Color;
  if ColorDlg.Execute then
  begin
    TPanel(Sender).Color := ColorDlg.Color;
    AttriChange(Sender);
  end;
end;

procedure TConfigFrm.AttriChange(Sender: TObject);
var
  Attri: TSynHighlighterAttributes;
  FontStyle: TFontStyles;
begin
  if NoUpdateAtrri then Exit;
  Attri := TSynHighlighterAttributes(ListBox.Items.Objects[ListBox.ItemIndex]);
  Attri.Foreground := Panel2.Color;
  Attri.Background := Panel4.Color;
  FontStyle := [];
  if CheckBox1.Checked then Include(FontStyle, fsBold);
  if CheckBox2.Checked then Include(FontStyle, fsItalic);
  if CheckBox3.Checked then Include(FontStyle, fsUnderline);
  Attri.Style := FontStyle;
end;

procedure TConfigFrm.HLVarsInStrsChkClick(Sender: TObject);
begin
  Liter.HighlightVarsInsideStrings := HLVarsInStrsChk.Checked;
end;

procedure TConfigFrm.btnGutterFontClick(Sender: TObject);
begin
  FontDlg.Font := lblGutterFont.Font;
  if FontDlg.Execute then
  begin
    lblGutterFont.Font := FontDlg.Font;
    lblGutterFont.Caption:= lblGutterFont.Font.Name + ' ' + IntToStr(lblGutterFont.Font.Size) + 'pt';
  end;
end;

procedure TConfigFrm.btnFontClick(Sender: TObject);
begin
  FontDlg.Font := labFont.Font;
  if FontDlg.Execute then
  begin
    labFont.Font := FontDlg.Font;
    SynEdit.Font := FontDlg.Font;
    labFont.Caption:= labFont.Font.Name + ' ' + IntToStr(labFont.Font.Size) + 'pt';
  end;
end;

procedure TConfigFrm.RegisterBtnClick(Sender: TObject);
begin
  RegisterAsDefaultEditor;
  InfoDlg(LangStr('SuccessfullyRegister'));
end;

procedure TConfigFrm.LangListClick(Sender: TObject);
begin
  LangChanged := True;
  MainFrm.RememberLang := True;
end;

procedure TConfigFrm.ckGutterShowLineNumbersClick(Sender: TObject);
begin
  ckGutterStartAtZero.Enabled := ckGutterVisible.Checked and ckGutterShowLineNumbers.Checked;
  ckGutterShowLeaderZeros.Enabled := ckGutterStartAtZero.Enabled;
end;

procedure TConfigFrm.ckGutterVisibleClick(Sender: TObject);
begin
  ckGutterShowLineNumbersClick(Sender);
  ckGutterShowLineNumbers.Enabled := ckGutterVisible.Checked;
  cbGutterFont.Enabled := ckGutterVisible.Checked;
  btnGutterFont.Enabled := ckGutterVisible.Checked;
  lblGutterFont.Enabled := ckGutterVisible.Checked;
  Label1.Enabled := ckGutterVisible.Checked;
  pGutterColor.Enabled :=  ckGutterVisible.Checked;
end;

procedure TConfigFrm.PluginsListClick(Sender: TObject);
var
  Data: PPluginData;
  S: String;
begin
  if PluginsList.ItemIndex >= 0 then
  begin
     Data :=  PPluginData(PluginsList.Items.Objects[PluginsList.ItemIndex]);
     ConfigPluginBtn.Enabled := Assigned(Data.Config);
     AboutPluginBtn.Enabled := Assigned(Data.About);
     SetLength(S, MAX_PATH);
     SetLength(S, GetModuleFileName(Data.DllHandle, PChar(S), MAX_PATH));
     PluginFileNameLbl.Caption := LowerCase(S);
     S := '';
  end else
  begin
    ConfigPluginBtn.Enabled := False;
    AboutPluginBtn.Enabled := False;
    PluginFileNameLbl.Caption := '';
  end;
end;

procedure TConfigFrm.ConfigPluginBtnClick(Sender: TObject);
begin
  PPluginData(PluginsList.Items.Objects[PluginsList.ItemIndex]).Config(Handle);
end;

procedure TConfigFrm.AboutPluginBtnClick(Sender: TObject);
begin
  PPluginData(PluginsList.Items.Objects[PluginsList.ItemIndex]).About(Handle);
end;

procedure TConfigFrm.PluginsListDblClick(Sender: TObject);
begin
  if ConfigPluginBtn.Enabled then
    ConfigPluginBtn.Click;
end;

procedure TConfigFrm.ApplyBtnClick(Sender: TObject);
begin
  SaveOptions;
  if LangChanged then LoadLangStrings;
  InitControlValues;
end;

procedure TConfigFrm.LoadLangStrings;
var
  C: Integer;
  S: String;
begin
  InitFont(Font);
  Caption := LangStr('ConfigCaption');
  TabSheet3.Caption := LangStr('CompilerConfigPage');
  TabSheet2.Caption := LangStr('EditorConfigPage');
  TabSheet1.Caption := LangStr('EnvironmentConfigPage');
  AceptarBtn.Caption := LangStr('OK');
  CancelarBtn.Caption := LangStr('Cancel');
  ApplyBtn.Caption := LangStr('Apply');
  TabSheet4.Caption := LangStr('DisplayPage');
  TabSheet5.Caption := LangStr('OptionsPage');
  TabSheet6.Caption := LangStr('HighlighterPage');
  HLVarsInStrsChk.Caption := LangStr('HighlightVarsInsideStrings');
  Label13.Caption := LangStr('Lang');
  WelcomeDialogChk.Caption := LangStr('ShowWelcomeDialog');
  MultInstChk.Caption := LangStr('MultipleInstances');
  IniDesignerChk.Caption := LangStr('OpenIniFilesIn');
  GroupBox3.Caption := LangStr('GeneralOptions');
  GroupBox2.Caption := LangStr('ToolBarOptions');
  SDragToolBarsChk.Caption := LangStr('TBSmoothDrag');
  SDragPanelsChk.Caption := LangStr('PanelSmoothDrag');
  Label11.Caption := LangStr('TBTheme');
  GroupBox1.Caption := LangStr('FileAssoc');
  RegisterBtn.Caption := LangStr('DefaultEditor');
  Label7.Caption := LangStr('Element');
  Label5.Caption := LangStr('Color');
  GroupBox13.Caption := LangStr('TextAttri');
  Label6.Caption := LangStr('FontStyle');
  CheckBox1.Caption := LangStr('Bold');
  CheckBox2.Caption := LangStr('Italic');
  CheckBox3.Caption := LangStr('Underline');
  Panel2.Caption := LangStr('Foreground');
  Panel4.Caption := LangStr('Background');
  HighLightChk.Caption := LangStr('UseHighLighter');
  gbGutter.Caption := LangStr('Gutter');
  ckGutterVisible.Caption := LangStr('GutterVisible');
  ckGutterShowLineNumbers.Caption := LangStr('GutterShowLineNumbers');
  ckGutterStartAtZero.Caption := LangStr('GutterStartAtZero');
  ckGutterShowLeaderZeros.Caption := LangStr('GutterShowLeaderZeros');
  cbGutterFont.Caption := LangStr('GutterFont');
  btnGutterFont.Caption := LangStr('Font');
  btnFont.Caption := LangStr('Font');
  Label1.Caption := LangStr('GutterColor');
  Label12.Caption := LangStr('BrowserHome');
  gbRightEdge.Caption := LangStr('RightEdge');
  Label10.Caption := LangStr('EdgeColumn');
  Label3.Caption := LangStr('EdgeColor');
  gbLineSpacing.Caption := LangStr('LineTabSpacing');
  Label8.Caption := LangStr('ExtraLines');
  Label9.Caption := LangStr('TabWidth');
  gbBookmarks.Caption := LangStr('Bookmarks');
  ckBookmarkKeys.Caption := LangStr('BookmarkKeys');
  ckBookmarkVisible.Caption := LangStr('BookmarksVisible');
  gbEditorFont.Caption := LangStr('EditorFont');
  EditKeyStrokesBtn.Caption := LangStr('EditKeystrokes');

  gbOptions.Caption := LangStr('Options');
  for C := 0 to gbOptions.ControlCount - 1 do
  with TCheckBox(gbOptions.Controls[C]) do
  begin
    S := LangStr(Copy(Name, 3, MaxInt)); { <-- ignore 'ck' }
    Caption := GetShortHint(S);
    Hint := GetLongHint(S);
  end;

  gbCaret.Caption := LangStr('Caret');
  Label2.Caption := LangStr('InsertCaret');
  Label4.Caption := LangStr('OverwriteCaret');

  cInsertCaret.Items.Clear;
  cInsertCaret.Items.Add(LangStr('VerticalLine'));
  cInsertCaret.Items.Add(LangStr('HorizontalLine'));
  cInsertCaret.Items.Add(LangStr('HalfBlock'));
  cInsertCaret.Items.Add(LangStr('Block'));
  cOverwriteCaret.Items.Assign(cInsertCaret.Items);

  TabSheet7.Caption := LangStr('PluginsConfigPage');
  ConfigPluginBtn.Caption := LangStr('PluginConfig');
  AboutPluginBtn.Caption := LangStr('PluginAbout');

  NewGroupBox2.Caption := LangStr('IOGridOptions');
  DisplayGridChk.Caption := LangStr('DisplayGrid');
  SnapToGridChk.Caption := LangStr('SnapToGrid');
  StaticText1.Caption := LangStr('GridX');
  StaticText2.Caption := LangStr('GridY');
  StaticText3.Caption := LangStr('CtrlDims');
  RightBottomChk.Caption := LangStr('RightBottom');
  WidthHeightChk.Caption := LangStr('WidthHeight');
  BothChk.Caption := LangStr('Both');

  NewGroupBox3.Caption := LangStr('Other');
  StaticText4.Caption := LangStr('SaveOptionsIn');
  RegistryChk.Caption := LangStr('Registry');
  IniFileChk.Caption := ExtractFileName(OptionsIniFileName);
  SaveFileListChk.Caption := LangStr('OpenFilesOnStartup');
  SplashChk.Caption := LangStr('ShowSplashScreen');
  UseDefBrowserChk.Caption := LangStr('UseDefBrowser');

  ThemeList.Items.Clear;
  for C := 0 to FAvailableTBXThemes.Count - 1 do
    ThemeList.Items.Add(LangStr(FAvailableTBXThemes[C]));

  HighLightChk.Width := CheckWidth + Canvas.TextWidth(HighLightChk.Caption) + 7;
end;

procedure TConfigFrm.InitControlValues;
var
  C: Integer;
begin
  Liter.Assign(MainFrm.SynNSIS);
  ListBox.Items.Clear;
  for C := 0 to Liter.AttrCount - 1 do
    ListBox.Items.AddObject(Liter.Attribute[C].Name, Liter.Attribute[C]);
  HLVarsInStrsChk.Checked := Liter.HighlightVarsInsideStrings;

  ListBox.ItemIndex := 0;
  ListBoxClick(ListBox);
  HighLightChkClick(HighLightChk);
  HighLightChk.Checked := MainFrm.UseHighLighter;
  HighLightChkClick(HighLightChk);

  LangList.ItemIndex := LangList.Items.IndexOf(ChangeFileExt(ExtractFileName(LangFile), ''));
  LangChanged := False;
  ThemeList.ItemIndex := FAvailableTBXThemes.IndexOf(TBXCurrentTheme);

  MultInstChk.Checked := OptionsIni.ReadBool('Options', 'AllowMultiInst', False);
  SplashChk.Checked := not OptionsIni.ReadBool('Options', 'NoSplash', False);

  WelcomeDialogChk.Checked := MainFrm.Options.ShowMelcomeDlg;
  IniDesignerChk.Checked := MainFrm.Options.IniFilesDesignMode;
  SDragPanelsChk.Checked := MainFrm.Options.SmoothPanel;
  SDragToolBarsChk.Checked := MainFrm.Options.SmoothToolBar;
  ckViewUsageHints.Checked := MainFrm.Options.ShowCmdHint;
  ckUndoAfterSave.Checked := MainFrm.Options.UndoAfterSave;
  BrowserHomeEdt.Text := MainFrm.Options.BrowserHome;
  UseDefBrowserChk.Checked := MainFrm.Options.UseDefBrowser;

  FSynEdit := MainFrm.EditorOptions;
  //Gutter
  ckGutterVisible.Checked:= FSynEdit.Gutter.Visible;
  ckGutterShowLineNumbers.Checked:= FSynEdit.Gutter.ShowLineNumbers;
  ckGutterShowLeaderZeros.Checked:= FSynEdit.Gutter.LeadingZeros;
  ckGutterStartAtZero.Checked:= FSynEdit.Gutter.ZeroStart;
  cbGutterFont.Checked := FSynEdit.Gutter.UseFontStyle;
  pGutterColor.Color:= FSynEdit.Gutter.Color;
  lblGutterFont.Font.Assign(FSynEdit.Gutter.Font);
  lblGutterFont.Caption:= lblGutterFont.Font.Name + ' ' + IntToStr(lblGutterFont.Font.Size) + 'pt';
  //Right Edge
  eRightEdge.Text:= IntToStr(FSynEdit.RightEdge);
  pRightEdgeColor.Color:= FSynEdit.RightEdgeColor;
  //Line Spacing
  eLineSpacing.Text:= IntToStr(FSynEdit.ExtraLineSpacing);
  eTabWidth.Text:= IntToStr(FSynEdit.TabWidth);
  //Break Chars
//!!  eBreakchars.Text:= FSynEdit.WordBreakChars;
  //Bookmarks
  ckBookmarkKeys.Checked:= FSynEdit.BookMarkOptions.EnableKeys;
  ckBookmarkVisible.Checked:= FSynEdit.BookMarkOptions.GlyphsVisible;
  //Font
  labFont.Font.Assign(FSynEdit.Font);
  labFont.Caption:= labFont.Font.Name + ' ' + IntToStr(labFont.Font.Size) + 'pt';
  //Options
  ckScrollHintFollows.Checked := eoScrollHintFollows in FSynEdit.Options;
  ckAutoIndent.Checked:= eoAutoIndent in FSynEdit.Options;
  ckDragAndDropEditing.Checked:= eoDragDropEditing in FSynEdit.Options;
  //ckDragAndDropFiles.Checked:= eoDropFiles in FSynEdit.Options;
  //ckNoSelection.Checked:= eoNoSelection in FSynEdit.Options;
  //ckNoCaret.Checked:= eoNoCaret in FSynEdit.Options;
  //ckWantTabs.Checked:= FSynEdit.WantTabs;
  ckShowSpecialChars.Checked := eoShowSpecialChars in FSynEdit.Options;
  ckSmartTabs.Checked:= eoSmartTabs in FSynEdit.Options;
  ckAltSetsColumnMode.Checked:= eoAltSetsColumnMode in FSynEdit.Options;
  ckHalfPageScroll.Checked:= eoHalfPageScroll in FSynEdit.Options;
  ckScrollByOneLess.Checked:= eoScrollByOneLess in FSynEdit.Options;
  ckScrollPastEOF.Checked:= eoScrollPastEof in FSynEdit.Options;
  ckScrollPastEOL.Checked:= eoScrollPastEol in FSynEdit.Options;
  ckShowScrollHint.Checked:= eoShowScrollHint in FSynEdit.Options;
  ckTabsToSpaces.Checked:= eoTabsToSpaces in FSynEdit.Options;
  ckTrimTrailingSpaces.Checked:= eoTrimTrailingSpaces in FSynEdit.Options;
  ckKeepCaretX.Checked:= eoKeepCaretX in FSynEdit.Options;
  ckSmartTabDelete.Checked := eoSmartTabDelete in FSynEdit.Options;
  ckRightMouseMoves.Checked := eoRightMouseMovesCursor in FSynEdit.Options;
  ckEnhanceHomeKey.Checked := eoEnhanceHomeKey in FSynEdit.Options;
  ckGroupUndo.Checked := eoGroupUndo in FSynEdit.Options;
  ckDisableScrollArrows.Checked := eoDisableScrollArrows in FSynEdit.Options;
  ckHideShowScrollbars.Checked := eoHideShowScrollbars in FSynEdit.Options;

  //Caret
  cInsertCaret.ItemIndex:= ord(FSynEdit.InsertCaret);
  cOverwriteCaret.ItemIndex:= ord(FSynEdit.OverwriteCaret);

  // Plugins ----
  Plugins.GetPluginsList(PluginsList.Items);
  if PluginsList.Items.Count > 0 then
    PluginsList.ItemIndex := 0;
  PluginsListClick(PluginsList);

  // IO designer options
  DisplayGridChk.Checked := MainFrm.Options.IOShowGrid;
  SnapToGridChk.Checked := MainFrm.Options.IOSnapToGrid;
  GridSizeXEdt.Text := IntToStr(MainFrm.Options.IOGridDims.X);
  GridSizeYEdt.Text := IntToStr(MainFrm.Options.IOGridDims.Y);
  case MainFrm.Options.ShowIODimsKind of
    sdRightBottom: RightBottomChk.Checked := True;
    sdWidthHeight: WidthHeightChk.Checked := True;
    sdBoth: BothChk.Checked := True;
  end;

  RegistryChk.Checked := OptionsIni is TRegistryIniFile;
  IniFileChk.Checked := OptionsIni is TIniFile;
  SaveFileListChk.Checked := MainFrm.Options.SaveFileList;

  CompilerConfigFrm1.InitValues(MainFrm.CurCompilerProfile);
end;

procedure TConfigFrm.SaveOptions;
var
  SynEditOptions : TSynEditorOptions;
  X2, Y2, C: Integer;
begin
    if RegistryChk.Checked and (OptionsIni is TIniFile) then
    begin
      if DeleteFile(OptionsIniFileName) then
      begin
        OptionsIni.Free;
        OptionsIni := TSafeRegistryIniFile.Create('Software\HM Software\NIS Edit');
      end;
    end else
    if IniFileChk.Checked and (OptionsIni is TRegistryIniFile) then
    begin
      OptionsIni.Free;
      OptionsIni := TIniFile.Create(OptionsIniFileName);
    end;

    MainFrm.UseHighLighter := HighLightChk.Checked;
    MainFrm.SynNSIS.Assign(Liter);
    MainFrm.AssignIniAttri;

    OptionsIni.WriteBool('Options', 'AllowMultiInst', MultInstChk.Checked);
    OptionsIni.WriteBool('Options', 'NoSplash', not SplashChk.Checked);

    MainFrm.Options.SaveFileList := SaveFileListChk.Checked;
    MainFrm.Options.ShowMelcomeDlg := WelcomeDialogChk.Checked;
    MainFrm.Options.IniFilesDesignMode := IniDesignerChk.Checked;
    MainFrm.Options.SmoothPanel := SDragPanelsChk.Checked;
    MainFrm.Options.SmoothToolBar := SDragToolBarsChk.Checked;
    MainFrm.Options.ShowCmdHint := ckViewUsageHints.Checked;
    MainFrm.Options.UndoAfterSave := ckUndoAfterSave.Checked;
    MainFrm.Options.UseDefBrowser := UseDefBrowserChk.Checked;
    MainFrm.Options.BrowserHome := BrowserHomeEdt.Text;
    MainFrm.ApplyOptions;
    if LangChanged then
      MainFrm.LoadLangFile(LangList.Items[LangList.ItemIndex]);

    if TBXCurrentTheme <> FAvailableTBXThemes[ThemeList.ItemIndex] then
      TBXSetTheme(FAvailableTBXThemes[ThemeList.ItemIndex]);

    MainFrm.Options.IOShowGrid := DisplayGridChk.Checked;
    MainFrm.Options.IOSnapToGrid := SnapToGridChk.Checked;

    X2 := MainFrm.Options.IOGridDims.X;
    Y2 := MainFrm.Options.IOGridDims.Y;
    MainFrm.Options.IOGridDims.X := StrToIntDef(GridSizeXEdt.Text, X2);
    MainFrm.Options.IOGridDims.Y := StrToIntDef(GridSizeYEdt.Text, Y2);
    if MainFrm.Options.IOGridDims.X < 1 then
      MainFrm.Options.IOGridDims.X := X2;
    if MainFrm.Options.IOGridDims.Y < 1 then
      MainFrm.Options.IOGridDims.Y := X2;

    if RightBottomChk.Checked then
      MainFrm.Options.ShowIODimsKind := sdRightBottom
    else if WidthHeightChk.Checked then
      MainFrm.Options.ShowIODimsKind := sdWidthHeight
    else
      MainFrm.Options.ShowIODimsKind := sdBoth;

    // Read property list again
    MainFrm.IOCrtlPropList.ReloadProperties;

    // Apply grid changes
    for C := 0 to MainFrm.MDIChildCount - 1 do
      if MainFrm.MDIChildren[C] is TIODesignerFrm then
        TIODesignerFrm(MainFrm.MDIChildren[C]).UpdateOptions;

    FSynEdit := MainFrm.EditorOptions;
    //Gutter
    FSynEdit.Gutter.Visible := ckGutterVisible.Checked;
    FSynEdit.Gutter.ShowLineNumbers := ckGutterShowLineNumbers.Checked;
    FSynEdit.Gutter.LeadingZeros := ckGutterShowLeaderZeros.Checked;
    FSynEdit.Gutter.ZeroStart := ckGutterStartAtZero.Checked;
    FSynEdit.Gutter.Color:= pGutterColor.Color;
    FSynEdit.Gutter.UseFontStyle := cbGutterFont.Checked;
    FSynEdit.Gutter.Font.Assign(lblGutterFont.Font);
    //Right Edge
    FSynEdit.RightEdge:= StrToIntDef(eRightEdge.Text, 80);
    FSynEdit.RightEdgeColor:= pRightEdgeColor.Color;
    //Line Spacing
    FSynEdit.ExtraLineSpacing:= StrToIntDef(eLineSpacing.Text, 0);
    FSynEdit.TabWidth:= StrToIntDef(eTabWidth.Text, 8);
    //Bookmarks
    FSynEdit.BookMarkOptions.EnableKeys:= ckBookmarkKeys.Checked;
    FSynEdit.BookMarkOptions.GlyphsVisible:= ckBookmarkVisible.Checked;
    //Font
    FSynEdit.Font.Assign(labFont.Font);
    //Options
    SynEditOptions:= [];
   if  ckScrollHintFollows.Checked then  Include(SynEditOptions, eoScrollHintFollows);
    if ckAutoIndent.Checked then Include(SynEditOptions, eoAutoIndent);
    if ckDragAndDropEditing.Checked then Include(SynEditOptions, eoDragDropEditing);
    {if ckDragAndDropFiles.Checked then Include(SynEditOptions, eoDropFiles);
    if ckNoSelection.Checked then Include(SynEditOptions, eoNoSelection);
    if ckNoCaret.Checked then Include(SynEditOptions, eoNoCaret);
    FSynEdit.WantTabs:= ckWantTabs.Checked;}
    if ckShowSpecialChars.Checked then Include(SynEditOptions, eoShowSpecialChars);
    if ckSmartTabs.Checked then Include(SynEditOptions, eoSmartTabs);
    if ckAltSetsColumnMode.Checked then Include(SynEditOptions, eoAltSetsColumnMode);
    if ckHalfPageScroll.Checked then Include(SynEditOptions, eoHalfPageScroll);
    if ckScrollByOneLess.Checked then Include(SynEditOptions, eoScrollByOneLess);
    if ckScrollPastEOF.Checked then Include(SynEditOptions, eoScrollPastEof);
    if ckScrollPastEOL.Checked then Include(SynEditOptions, eoScrollPastEol);
    if ckShowScrollHint.Checked then Include(SynEditOptions, eoShowScrollHint);
    if ckTabsToSpaces.Checked then Include(SynEditOptions, eoTabsToSpaces);
    if ckTrimTrailingSpaces.Checked then Include(SynEditOptions, eoTrimTrailingSpaces);
    if ckKeepCaretX.Checked then Include(SynEditOptions, eoKeepCaretX);
    if ckSmartTabDelete.Checked then Include(SynEditOptions,eoSmartTabDelete);
    if ckRightMouseMoves.Checked then Include(SynEditOptions,eoRightMouseMovesCursor);
    if ckEnhanceHomeKey.Checked then Include(SynEditOptions,eoEnhanceHomeKey);
    if ckGroupUndo.Checked then Include(SynEditOptions,eoGroupUndo);
    if ckDisableScrollArrows.Checked then Include(SynEditOptions,eoDisableScrollArrows);
    if ckHideShowScrollbars.Checked then Include(SynEditOptions,eoHideShowScrollbars);
    FSynEdit.Options:= SynEditOptions;
    //Caret
    FSynEdit.InsertCaret:= TSynEditCaretType(cInsertCaret.ItemIndex);
    FSynEdit.OverwriteCaret:= TSynEditCaretType(cOverwriteCaret.ItemIndex);

    with MainFrm do
    for C := MDIChildCount - 1 downto 0 do
    if MDIChildren[C] is TCustomMDIChild then
      TCustomMDIChild(MDIChildren[C]).UpdateEditor;

   CompilerConfigFrm1.SaveValues(MainFrm.CurCompilerProfile);
end;

procedure TConfigFrm.BrowserHomeEdtChange(Sender: TObject);
begin
  BrowserHomeEdt.Hint := BrowserHomeEdt.Text;
end;

procedure TConfigFrm.FormDestroy(Sender: TObject);
begin
  FAvailableTBXThemes.Free;
end;

procedure TConfigFrm.SynEditMouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  Attri: TSynHighlighterAttributes;
  Pt: TDisplayCoord;
  I: Integer;
  S: String;
begin
  Pt := SynEdit.PixelsToRowColumn(X, Y);
  SynEdit.GetHighlighterAttriAtRowCol(TBufferCoord(Pt), S, Attri);
  if Attri = nil then Exit;
  I := ListBox.Items.IndexOf(Attri.Name);
  if I < 0 then Exit;
  ListBox.ItemIndex := I;
  ListBoxClick(ListBox);
end;

procedure TConfigFrm.EditKeyStrokesBtnClick(Sender: TObject);
begin
  with TSynEditKeystrokesEditorForm.Create(Self) do
  try
    ExtendedString := True;
    Keystrokes := MainFrm.EditorOptions.Keystrokes;
    if ShowModal = mrOK then
      MainFrm.EditorOptions.Keystrokes := Keystrokes;
  finally
    Free;
  end;
end;

procedure TConfigFrm.TreeViewChange(Sender: TObject; Node: TTreeNode);
var
  TabSheet: TTabSheet;
begin
  TabSheet := TTabSheet(Node.Data);
  TabSheet.PageControl.ActivePage := TabSheet;
  TabSheet.Controls[0].Show;
  Label14.Caption := TabSheet.Caption;
end;

procedure TConfigFrm.PaintBoxPaint(Sender: TObject);
  function BlendRGB(const Color1, Color2: TColor; const Blend: Integer): TColor;
  { Blends Color1 and Color2. Blend must be between 0 and 63; 0 = all Color1,
    63 = all Color2. }
  type
    TColorBytes = array[0..3] of Byte;
  var
    I: Integer;
  begin
    Result := 0;
    for I := 0 to 2 do
      TColorBytes(Result)[I] := Integer(TColorBytes(Color1)[I] +
        ((TColorBytes(Color2)[I] - TColorBytes(Color1)[I]) * Blend) div 63);
  end;

var
  C1, C2: TColor;
  CS: TPoint;
  Z: Integer;
begin
  C1 := ColorToRGB(clBlack);
  C2 := ColorToRGB(Self.Color);
  CS := PaintBox.ClientRect.BottomRight;
  with PaintBox.Canvas do
  begin
    for Z := 0 to 63 do begin
      Brush.Color := BlendRGB(C1, C2, Z);
      FillRect(Rect(MulDiv(CS.X, Z, 63), 0, MulDiv(CS.X, Z+1, 63), CS.Y));
    end;
  end;
end;

end.
