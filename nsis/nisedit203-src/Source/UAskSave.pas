{
  HM NIS Edit (c) 2003-2005 Héctor Mauricio Rodríguez Segura <ranametal@users.sourceforge.net>
  For conditions of distribution and use, see license.txt

  Ask save dialog. Based on syn (C) 2000-2003, Ascher Stefan. dlgAskSave.pas
  http://syn.sourceforge.net/

}
unit UAskSave;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls;

type
  TAskSaveDialog = class(TForm)
    lblPrompt: TLabel;
    imgIcon: TImage;
    lstFiles: TListBox;
    btnYes: TButton;
    btnCancel: TButton;
    btnAll: TButton;
    btnNo: TButton;
    procedure lstFilesKeyDown(Sender: TObject; var Key: Word;
      Shift: TShiftState);
    procedure lstFilesKeyPress(Sender: TObject; var Key: Char);
    procedure lstFilesDrawItem(Control: TWinControl; Index: Integer;
      Rect: TRect; State: TOwnerDrawState);
    procedure FormResize(Sender: TObject);
    procedure lstFilesClick(Sender: TObject);
    procedure FormKeyPress(Sender: TObject; var Key: Char);
  private
    { Private declarations }
  public
    { Public declarations }
    function Prepare: Boolean;
  end;

var
  AskSaveDialog: TAskSaveDialog;

implementation

{$R *.DFM}

uses
  Math, UMain, UCustomMDIChild, Utils;

function TAskSaveDialog.Prepare: Boolean;
var
  C: Integer;
begin
  Screen.Cursor := crHourGlass;
  try
    for C := 0 to MainFrm.MDIChildCount - 1 do
      if MainFrm.MDIChildren[C] is TCustomMDIChild then
        with TCustomMDIChild(MainFrm.MDIChildren[C]) do
        begin
          if Modified then
          begin
            if FileName = '' then
              lstFiles.Items.AddObject(Caption, MainFrm.MDIChildren[C])
            else
              lstFiles.Items.AddObject(FileName, MainFrm.MDIChildren[C]);
          end;
        end;

    Result := lstFiles.Items.Count > 1;
    if not Result then
      Exit;

    InitFont(Font);
    Caption := LangStr('AskSaveDialogCaption');
    lblPrompt.Caption := LangStr('AskSaveChanges');
    btnYes.Caption := LangStr('Yes');
    btnNo.Caption := LangStr('No');
    btnCancel.Caption := LangStr('Cancel');
    btnAll.Caption := LangStr('SaveAll');
    imgIcon.Picture.Icon.Handle := LoadIcon(0, IDI_QUESTION);
    lstFiles.Canvas.Font := lstFiles.Font;
    lstFiles.ItemHeight := Max(lstFiles.Canvas.TextHeight('Xg') + 4, 18);
    lstFiles.ItemIndex := 0;
    if lstFiles.Items.Count = 1 then
      lstFiles.Selected[0] := True;
    btnYes.Enabled := lstFiles.SelCount > 0;
  finally
    Screen.Cursor := crDefault;
  end;
end;

procedure TAskSaveDialog.lstFilesKeyDown(Sender: TObject; var Key: Word;
  Shift: TShiftState);
var
  i: integer;
begin
  if Shift = [ssCtrl] then begin
    case Key of
      65:           // Select _a_ll
        begin
          for i := 0 to lstFiles.Items.Count - 1 do
            lstFiles.Selected[i] := true;
        end;
      78:           // Select _n_one
        begin
          for i := 0 to lstFiles.Items.Count - 1 do
            lstFiles.Selected[i] := false;
        end;
      73:           // _I_nvert Selection
        begin
          for i := 0 to lstFiles.Items.Count - 1 do
            lstFiles.Selected[i] := not lstFiles.Selected[i];
        end;
    end;
  end;
end;

procedure TAskSaveDialog.lstFilesKeyPress(Sender: TObject; var Key: Char);
begin
  if Hi(GetKeyState(VK_CONTROL)) <> 0 then Key := #0;
end;

procedure TAskSaveDialog.lstFilesDrawItem(Control: TWinControl;
  Index: Integer; Rect: TRect; State: TOwnerDrawState);
var
  Offset, TopOffset, ImgIx: integer;
  rc: TRect;
begin
  // Paint Icons to the list
  with lstFiles do
  begin
    Canvas.FillRect(Rect);       { clear the rectangle }
    Offset := 20;                { provide default offset }
    TopOffset := ((Rect.Bottom - Rect.Top) - 16) div 2;  // Center the Icon
    ImgIx := TCustomMDIChild(Items.Objects[Index]).GetIconIndex;
    if ImgIx = -1 then
      ImgIx := 46;
    MainFrm.SystemImageList.Draw(Canvas, Rect.Left + 1, Rect.Top + TopOffset, ImgIx);
    rc.Left := Rect.Left + Offset;
    rc.Top := Rect.Top + 2;
    if ItemHeight * Items.Count > Height then
      rc.Right := Rect.Right - 3 - GetSystemMetrics(SM_CXVSCROLL)
    else
      rc.Right := Rect.Right - 3;
    rc.Bottom := Rect.Bottom;
    DrawText(Canvas.Handle, PChar(Items[Index]), Length(Items[Index]), rc,
      DT_PATH_ELLIPSIS or DT_NOPREFIX or DT_NOCLIP);
  end;
end;

procedure TAskSaveDialog.FormResize(Sender: TObject);
begin
  lstFiles.Repaint;
end;

procedure TAskSaveDialog.lstFilesClick(Sender: TObject);
begin
  btnYes.Enabled := lstFiles.SelCount > 0;
end;

procedure TAskSaveDialog.FormKeyPress(Sender: TObject; var Key: Char);
begin
  case Key of
    #89, #121: if btnYes.Enabled then btnYes.Click;      // Y(es)
    #78, #110: if btnNo.Enabled  then btnNo.Click;       // N(o)
    #65, #97:  if btnAll.Enabled then btnAll.Click;      // A(all)
  end;
end;

end.
