'Rift .NET Reloaded -- An OpenSource Server Emulator for World of Warcraft Classic Alpha 0.5.3 (3368) written in VB.Net
'Copyright (c) 2013 noVo aka. takeoYasha www.easy-emu.de

'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
		
		Console.WriteLine(buffer)
	End Sub
	
	
	
End Class