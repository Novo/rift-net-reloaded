Public Class PacketLog

    Public Shared Sub DumpPacket(ByVal data() As Byte, Optional ByVal txt As String = "")
        Dim j As Integer
        Dim buffer As String = ""

        buffer = buffer + [String].Format("Packet Dump: " & txt & vbNewLine, vbNewLine)

        If data.Length Mod 16 = 0 Then
            For j = 0 To data.Length - 1 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
            Next
        Else
            For j = 0 To data.Length - 1 - 16 Step 16
                buffer += "|  " & BitConverter.ToString(data, j, 16).Replace("-", " ")
                buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?") & " |" & vbNewLine
            Next

            buffer += "|  " & BitConverter.ToString(data, j, data.Length Mod 16).Replace("-", " ")
            buffer += New String(CChar(" "), (16 - data.Length Mod 16) * 3)
            buffer += " |  " & System.Text.Encoding.ASCII.GetString(data, j, data.Length Mod 16).Replace(vbTab, "?").Replace(vbBack, "?").Replace(vbCr, "?").Replace(vbFormFeed, "?").Replace(vbLf, "?")
            buffer += New String(CChar(" "), 16 - data.Length Mod 16)
            buffer += " |" & vbNewLine
        End If

        'Console.WriteLine(buffer)
    End Sub

End Class