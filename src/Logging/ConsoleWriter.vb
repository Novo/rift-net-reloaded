'ConsoleWriter.vb
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

Public Class ConsoleWriter
    Inherits BaseWriter

    Public Overrides Sub Write(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return

        Console.Write(formatStr, arg)
    End Sub
    Public Overrides Sub WriteLine(ByVal type As LogType, ByVal formatStr As String, ByVal ParamArray arg() As Object)
        If LogLevel > type Then Return

        Console.WriteLine("[" & Format(TimeOfDay, "HH:mm:ss") & "] " & formatStr, arg)
    End Sub


End Class
