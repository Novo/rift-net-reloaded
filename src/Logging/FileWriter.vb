'FileWriter.vb
'
'Rift .NET Reloaded -- An OpenSource Server Emulator for World of Warcraft Classic Alpha 0.5.3 (3368) written in VB.Net
'Copyright (c) 2012 noVo aka. takeoYasha

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

Imports System.Runtime.CompilerServices
Imports System.IO

'Using this logging type, all logs are saved in files numbered by date.
'Writting commands is done trought console.
Public Class FileWriter
    Inherits BaseWriter

    Protected Output As IO.StreamWriter
    Protected LastDate As Date = #1/1/2007#
    Protected Filename As String = ""

    Protected Sub CreateNewFile()
        LastDate = Now.Date
        Output = New StreamWriter(String.Format("{0}-{1}.log", Filename, Format(LastDate, "yyyy-MM-dd")), True)
        Output.AutoFlush = True

        Me.WriteLine(LogType.INFORMATION, "Log started successfully.")
    End Sub

    Public Sub New(ByVal filename_ As String)
        Filename = filename_
        CreateNewFile()
    End Sub
    Public Overrides Sub Dispose()
        Output.Close()
    End Sub

    Public Overrides Sub Write(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return
        If LastDate <> Now.Date Then CreateNewFile()

        Output.Write(formatStr, arg)
    End Sub
    Public Overrides Sub WriteLine(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return
        If LastDate <> Now.Date Then CreateNewFile()

        Output.WriteLine("[" & Format(TimeOfDay, "HH:mm:ss") & "] " & formatStr, arg)
    End Sub


End Class