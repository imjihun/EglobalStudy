Public Sub SaveAttachments()



	Dim objOL As Outlook.Application

	Dim objMsg As Outlook.MailItem

	Dim objAttachments As Outlook.Attachments

	Dim objSelection As Outlook.Selection

	Dim i As Long

	Dim lngCount As Long

	Dim strFile As String

	Dim strFolderpath As String

	Dim strDeletedFiles As String





	strFolderpath = "C:\"



Set objOL = CreateObject("Outlook.Application")

Set objSelection = objOL.ActiveExplorer.Selection





For Each objMsg In objSelection

		objMsg.UnRead = False

    Set objAttachments = objMsg.Attachments

    lngCount = objAttachments.Count

		strDeletedFiles = ""



		If lngCount > 0 Then



			For i = lngCount To 1 Step -1

				strFile = objAttachments.Item(i).FileName

				strFile = strFolderpath & strFile

				objAttachments.Item(i).SaveAsFile strFile

		Next i



		End If

	Next



	MsgBox("     Download Complete")



ExitSub:



Set objAttachments = Nothing

Set objMsg = Nothing

Set objSelection = Nothing

Set objOL = Nothing



End Sub
