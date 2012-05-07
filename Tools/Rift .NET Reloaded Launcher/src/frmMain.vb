Imports System.IO

Public Class frmMain

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        If Not File.Exists(Application.StartupPath & "\WoWClient.exe") Then
            MsgBox("Please copy the Launcher next to the 'WoWClient.exe'.")
            Exit Sub
        End If

        If txtUserName.Text <> "" And txtPassword.Text <> "" Then

            'Declare any variables
            Dim crypt As New clsCrypt

            'Declare those variables!
            Dim sFileName As String = Application.StartupPath & "\wow.ses"
            Dim myFileStream As New System.IO.FileStream(sFileName, _
                FileMode.Create, FileAccess.ReadWrite, FileShare.None)

            'Create the stream writer
            Dim myWriter As New System.IO.StreamWriter(myFileStream)

            'Write in what is in the text box
            myWriter.WriteLine(txtUserName.Text & vbCrLf & crypt.getMd5Hash(txtPassword.Text))

            'Flush before we close
            myWriter.Flush()

            'Close everything
            myWriter.Close()
            myFileStream.Close()

            Dim startInfo As New ProcessStartInfo(Application.StartupPath & "\WoWClient.exe")

            If chkboxWindowed.Checked Then
                startInfo.Arguments = "-uptodate -windowed -console"
            Else
                startInfo.Arguments = "-uptodate -console"
            End If

            Process.Start(startInfo)

        Else
            MsgBox("Username and Password could not be empty.")
            Exit Sub
        End If

    End Sub
End Class
