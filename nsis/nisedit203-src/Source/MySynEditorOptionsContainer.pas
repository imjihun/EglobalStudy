unit MySynEditorOptionsContainer;

interface
uses
   Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
   SynEdit, SynEditHighlighter, SynEditMiscClasses, SynEditKeyCmds, IniFiles;

type
  // Taken from SynEditOptionsDialog.pas in the SynEdit project
  //This class is assignable to a SynEdit without modifying key properties that affect function
  TMySynEditorOptionsContainer = class(TComponent)
  private
    FHideSelection: Boolean;
    FWantTabs: Boolean;
    FMaxUndo: Integer;
    FExtraLineSpacing: Integer;
    FTabWidth: Integer;
    FRightEdge: Integer;
    FSelectedColor: TSynSelectedColor;
    FRightEdgeColor: TColor;
    FFont: TFont;
    FBookmarks: TSynBookMarkOpt;
    FOverwriteCaret: TSynEditCaretType;
    FInsertCaret: TSynEditCaretType;
    FOptions: TSynEditorOptions;
    FSynGutter: TSynGutter;
    FWordBreakChars: String;
    FColor: TColor;
    FKeystrokes: TSynEditKeystrokes;
    procedure SetBookMarks(const Value: TSynBookMarkOpt);
    procedure SetFont(const Value: TFont);
    procedure SetOptions(const Value: TSynEditorOptions);
    procedure SetSynGutter(const Value: TSynGutter);
    procedure SetKeystrokes(const Value: TSynEditKeystrokes);
  public
    constructor Create(AOwner : TComponent); override;
    destructor Destroy; override;
    procedure Assign(Source : TPersistent); override;
    procedure AssignTo(Dest : TPersistent); override;
    procedure LoadFromIni(IniFile: TCustomIniFile; const SectionName: String);
    procedure SaveToIni(IniFile: TCustomIniFile; const SectionName: String);
  published
    property Options : TSynEditorOptions read FOptions write SetOptions;
    property BookMarkOptions : TSynBookMarkOpt read FBookmarks write SetBookMarks;
    property Color : TColor read FColor write FColor;
    property Font : TFont read FFont write SetFont;
    property ExtraLineSpacing : Integer read FExtraLineSpacing write FExtraLineSpacing;
    property Gutter : TSynGutter read FSynGutter write SetSynGutter;
    property RightEdge : Integer read FRightEdge write FRightEdge;
    property RightEdgeColor : TColor read FRightEdgeColor write FRightEdgeColor;
    property WantTabs : Boolean read FWantTabs write FWantTabs;
    property InsertCaret : TSynEditCaretType read FInsertCaret write FInsertCaret;
    property OverwriteCaret : TSynEditCaretType read FOverwriteCaret write FOverwriteCaret;
    property HideSelection : Boolean read FHideSelection write FHideSelection;
    property MaxUndo : Integer read FMaxUndo write FMaxUndo;
    property SelectedColor : TSynSelectedColor read FSelectedColor write FSelectedColor;
    property TabWidth : Integer read FTabWidth write FTabWidth;
    property WordBreakChars : String read FWordBreakChars write FWordBreakChars;
    property Keystrokes: TSynEditKeystrokes read  FKeystrokes write SetKeystrokes;
  end;

procedure Register;
implementation

uses TypInfo, Menus, Registry {$IFNDEF NO_USE_UTILS}, Utils {$ENDIF};

procedure Register;
begin
  RegisterComponents('HM NIS Edit', [TMySynEditorOptionsContainer]);
end;

{ TMySynEditorOptionsContainer }

procedure TMySynEditorOptionsContainer.Assign(Source: TPersistent);
begin
  if Assigned(Source) and (Source is TCustomSynEdit) then
  begin
    Self.Font.Assign(TCustomSynEdit(Source).Font);
    Self.BookmarkOptions.Assign(TCustomSynEdit(Source).BookmarkOptions);
    Self.Gutter.Assign(TCustomSynEdit(Source).Gutter);
    Self.SelectedColor.Assign(TCustomSynEdit(Source).SelectedColor);

    Self.Color := TCustomSynEdit(Source).Color;
    Self.Options := TCustomSynEdit(Source).Options;
    Self.ExtraLineSpacing := TCustomSynEdit(Source).ExtraLineSpacing;
    Self.HideSelection := TCustomSynEdit(Source).HideSelection;
    Self.InsertCaret := TCustomSynEdit(Source).InsertCaret;
    Self.OverwriteCaret := TCustomSynEdit(Source).OverwriteCaret;
    //Self.MaxLineWidth := TCustomSynEdit(Source).MaxLineWidth;
    Self.MaxUndo := TCustomSynEdit(Source).MaxUndo;
    Self.RightEdge := TCustomSynEdit(Source).RightEdge;
    Self.RightEdgeColor := TCustomSynEdit(Source).RightEdgeColor;
    Self.TabWidth := TCustomSynEdit(Source).TabWidth;
    Self.WantTabs := TCustomSynEdit(Source).WantTabs;
    Self.Keystrokes.Assign(TCustomSynEdit(Source).Keystrokes);
//!!    Self.WordBreakChars := TSynEdit(Source).WordBreakChars;
  end else
    inherited;
end;

procedure TMySynEditorOptionsContainer.AssignTo(Dest: TPersistent);
begin
  if Assigned(Dest) and (Dest is TCustomSynEdit) then
  begin
    TCustomSynEdit(Dest).Font.Assign(Self.Font);
    TCustomSynEdit(Dest).BookmarkOptions.Assign(Self.BookmarkOptions);
    TCustomSynEdit(Dest).Gutter.Assign(Self.Gutter);
    TCustomSynEdit(Dest).SelectedColor.Assign(Self.SelectedColor);

    TCustomSynEdit(Dest).Color := Self.Color;
    TCustomSynEdit(Dest).Options := Self.Options;
    TCustomSynEdit(Dest).ExtraLineSpacing := Self.ExtraLineSpacing;
    TCustomSynEdit(Dest).HideSelection := Self.HideSelection;
    TCustomSynEdit(Dest).InsertCaret := Self.InsertCaret;
    TCustomSynEdit(Dest).OverwriteCaret := Self.OverwriteCaret;
    //TCustomSynEdit(Dest).MaxLineWidth := Self.MaxLineWidth;
    TCustomSynEdit(Dest).MaxUndo := Self.MaxUndo;
    TCustomSynEdit(Dest).RightEdge := Self.RightEdge;
    TCustomSynEdit(Dest).RightEdgeColor := Self.RightEdgeColor;
    TCustomSynEdit(Dest).TabWidth := Self.TabWidth;
    TCustomSynEdit(Dest).WantTabs := Self.WantTabs;
    TCustomSynEdit(Dest).Keystrokes.Assign(Self.Keystrokes);
  end else
    inherited;
end;

constructor TMySynEditorOptionsContainer.Create(AOwner: TComponent);
begin
  inherited;
  FBookmarks:= TSynBookMarkOpt.Create(Self);
  FSynGutter:= TSynGutter.Create;
  FKeystrokes := TSynEditKeyStrokes.Create(Self);
  FSelectedColor:= TSynSelectedColor.Create;
  FSelectedColor.Foreground:= clHighlightText;
  FSelectedColor.Background:= clHighlight;
  FFont:= TFont.Create;
  FFont.Name:= 'Courier New';
  FFont.Size:= 10;
  Color:= clWindow;
  Options := [eoAutoIndent,eoDragDropEditing,eoDropFiles,eoScrollPastEol,
    eoShowScrollHint,eoSmartTabs,eoAltSetsColumnMode, eoTabsToSpaces,eoTrimTrailingSpaces, eoKeepCaretX];
  ExtraLineSpacing := 0;
  HideSelection := False;
  InsertCaret := ctVerticalLine;
  OverwriteCaret := ctBlock;
  MaxUndo := 1024;
  RightEdge := 80;
  RightEdgeColor := clSilver;
  TabWidth := 8;
  WantTabs := True;
//!!  WordBreakChars:= '.,;:''"&!?$%#@<>[](){}^-=+-*/\|';
end;

destructor TMySynEditorOptionsContainer.destroy;
begin
  FBookMarks.Free;
  FSynGutter.Free;
  FFont.Free;
  FKeystrokes.Free;
  inherited;
end;

procedure TMySynEditorOptionsContainer.LoadFromIni(IniFile: TCustomIniFile;
  const SectionName: String);
var
  FontStyle: TFontStyles;
  SaveOptions: TSynEditorOptions;
  Stream: TMemoryStream;
  S: string;
begin
  with IniFile do
  begin
    SaveOptions := FOptions;
{$IFNDEF NO_USE_UTILS}
    if not TextToBin(ReadString(SectionName, 'EditorOptions', ''), FOptions, SizeOf(TSynEditorOptions)) then
      FOptions := SaveOptions;
{$ENDIF}
    FBookmarks.EnableKeys := ReadBool(SectionName, 'BookmarksEnableKeys', FBookmarks.EnableKeys);
    FBookmarks.GlyphsVisible := ReadBool(SectionName, 'BookmarksGlyphsVisible', FBookmarks.GlyphsVisible);
    FColor := ReadInteger(SectionName, 'Color', FColor);
    FFont.Name := ReadString(SectionName, 'FontName', FFont.Name);
    FFont.Size := ReadInteger(SectionName, 'FontSize', FFont.Size);
    FFont.Color := ReadInteger(SectionName, 'FontColor', FFont.Color);
    FontStyle := FFont.Style;
{$IFNDEF NO_USE_UTILS}
    if TextToBin(ReadString(SectionName, 'EditorFontStyle', ''), FontStyle, SizeOf(TFontStyles)) then
      FFont.Style := FontStyle;
{$ENDIF}
    FExtraLineSpacing := ReadInteger(SectionName, 'ExtraLineSpacing', FExtraLineSpacing);
    FSynGutter.Visible := ReadBool(SectionName, 'GutterVisible', FSynGutter.Visible);
    FSynGutter.Color := ReadInteger(SectionName, 'GutterColor', FSynGutter.Color);
    FSynGutter.AutoSize := ReadBool(SectionName, 'GutterAutoSize', FSynGutter.AutoSize);
    FSynGutter.ShowLineNumbers := ReadBool(SectionName, 'GutterShowLineNumbers', FSynGutter.ShowLineNumbers);
    FSynGutter.ZeroStart := ReadBool(SectionName, 'GutterZeroStart', FSynGutter.ZeroStart);
    FSynGutter.LeadingZeros := ReadBool(SectionName, 'GutterLeadingZeros', FSynGutter.LeadingZeros);
    FSynGutter.Font.Name := ReadString(SectionName, 'GutterFontName', FSynGutter.Font.Name);
    FSynGutter.Font.Size := ReadInteger(SectionName, 'GutterFontSize', FSynGutter.Font.Size);
    FSynGutter.Font.Color := ReadInteger(SectionName, 'GutterFontColor', FSynGutter.Font.Color);
    FontStyle := FSynGutter.Font.Style;
{$IFNDEF NO_USE_UTILS}
    if TextToBin(ReadString(SectionName, 'GutterFontStyle', ''), FontStyle, SizeOf(TFontStyles)) then
      FSynGutter.Font.Style := FontStyle;
{$ENDIF}
    FRightEdge := ReadInteger(SectionName, 'RightEdge', FRightEdge);
    FRightEdgeColor := ReadInteger(SectionName, 'RightEdgeColor', FRightEdgeColor);
    FInsertCaret := TSynEditCaretType(ReadInteger(SectionName, 'InsertCaret',
      Ord(FInsertCaret)));
    FOverwriteCaret := TSynEditCaretType(ReadInteger(SectionName, 'OverwriteCaret',
      Ord(FOverwriteCaret)));
    FTabWidth := ReadInteger(SectionName, 'TabWidth', FTabWidth);

    S := ReadString(SectionName, 'Keystrokes', '');
    if S <> '' then
    begin
      Stream := TMemoryStream.Create;
      try
        Stream.SetSize(Length(S) div 2);
{$IFNDEF NO_USE_UTILS}
        if TextToBin(S, Stream.Memory^, Stream.Size) then
        begin
          Stream.Position := 0;
          FKeystrokes.LoadFromStream(Stream);
        end;
{$ENDIF}
      finally
        Stream.Free;
      end;
    end;
    if FKeystrokes.Count = 0 then
        FKeystrokes.ResetDefaults;

{    FKeystrokes.BeginUpdate;
    Values := TStringList.Create;
    try
      FKeystrokes.Clear;
      ReadSectionValues('Keystrokes', Values);
      for C := 0 to Values.Count - 1 do
      begin
        S := Values.Names[C];
        EditorCommand := ConvertCodeStringToCommand(S);
        if EditorCommand <> ecNone then
        with FKeystrokes.Add do
        begin
          Command := EditorCommand;
          S := Values.Values[S];
          ShortCut := TextToShortCut(ExtractStr(S, ','));
          ShortCut2 := TextToShortCut(ExtractStr(S, ','));
        end;
      end;
    finally
      Values.Free;
      FKeystrokes.EndUpdate;
    end;
    if FKeystrokes.Count = 0 then
        FKeystrokes.ResetDefaults;}
  end;
end;

procedure TMySynEditorOptionsContainer.SaveToIni(IniFile: TCustomIniFile;
  const SectionName: String);
var
  FontStyle: TFontStyles;
  Stream: TMemoryStream;
begin
  with IniFile do
  begin
{$IFNDEF NO_USE_UTILS}
    WriteString(SectionName, 'EditorOptions', BinToText(FOptions, SizeOf(TSynEditorOptions)));
{$ENDIF}
    WriteBool(SectionName, 'BookmarksEnableKeys', FBookmarks.EnableKeys);
    WriteBool(SectionName, 'BookmarksGlyphsVisible', FBookmarks.GlyphsVisible);
    WriteInteger(SectionName, 'Color', FColor);
    WriteString(SectionName, 'FontName', FFont.Name);
    WriteInteger(SectionName, 'FontSize', FFont.Size);
    WriteInteger(SectionName, 'FontColor', FFont.Color);
    FontStyle := FFont.Style;
{$IFNDEF NO_USE_UTILS}
    WriteString(SectionName, 'EditorFontStyle', BinToText(FontStyle, SizeOf(TFontStyles)));
{$ENDIF}
    WriteInteger(SectionName, 'ExtraLineSpacing', FExtraLineSpacing);
    WriteBool(SectionName, 'GutterVisible', FSynGutter.Visible);
    WriteInteger(SectionName, 'GutterColor', FSynGutter.Color);
    WriteBool(SectionName, 'GutterAutoSize', FSynGutter.AutoSize);
    WriteBool(SectionName, 'GutterShowLineNumbers', FSynGutter.ShowLineNumbers);
    WriteBool(SectionName, 'GutterZeroStart', FSynGutter.ZeroStart);
    WriteBool(SectionName, 'GutterLeadingZeros', FSynGutter.LeadingZeros);
    WriteString(SectionName, 'GutterFontName', FSynGutter.Font.Name);
    WriteInteger(SectionName, 'GutterFontSize', FSynGutter.Font.Size);
    WriteInteger(SectionName, 'GutterFontColor', FSynGutter.Font.Color);
    FontStyle := FSynGutter.Font.Style;
{$IFNDEF NO_USE_UTILS}
    WriteString(SectionName, 'GutterFontStyle', BinToText(FontStyle, SizeOf(TFontStyles)));
{$ENDIF}
    WriteInteger(SectionName, 'RightEdge', FRightEdge);
    WriteInteger(SectionName, 'RightEdgeColor', FRightEdgeColor);
    WriteInteger(SectionName, 'InsertCaret', Ord(FInsertCaret));
    WriteInteger(SectionName, 'OverwriteCaret', Ord(FOverwriteCaret));
    WriteInteger(SectionName, 'TabWidth', FTabWidth);

   { for C := 0 to  FKeystrokes.Count - 1 do
    with FKeystrokes.Items[C] do
      WriteString('Keystrokes', EditorCommandToCodeString(Command),
        ShortCutToText(ShortCut) + ',' + ShortCutToText(ShortCut2));}
    Stream :=  TMemoryStream.Create;
    try
      FKeystrokes.SaveToStream(Stream);
{$IFNDEF NO_USE_UTILS}
      WriteString(SectionName, 'Keystrokes', BinToText(Stream.Memory^, Stream.Size));
{$ENDIF}      
    finally
      Stream.Free;
    end;
  end;
end;

procedure TMySynEditorOptionsContainer.SetBookMarks(
  const Value: TSynBookMarkOpt);
begin
  FBookmarks.Assign(Value);
end;

procedure TMySynEditorOptionsContainer.SetFont(const Value: TFont);
begin
  FFont.Assign(Value);
end;

procedure TMySynEditorOptionsContainer.SetKeystrokes(
  const Value: TSynEditKeystrokes);
begin
  FKeystrokes.Assign(Value);
end;

procedure TMySynEditorOptionsContainer.SetOptions(
  const Value: TSynEditorOptions);
begin
  FOptions:= Value;
end;

procedure TMySynEditorOptionsContainer.SetSynGutter(const Value: TSynGutter);
begin
  FSynGutter.Assign(Value);
end;

end.
