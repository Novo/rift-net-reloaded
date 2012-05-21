'ErrorLog.vb
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

Public Class BaseWriter
    Implements IDisposable

    Public Enum LogType
        NETWORK                 'Network code debugging
        DEBUG                   'Packets processing
        INFORMATION             'User information
        USER                    'User actions
        SUCCESS                 'Normal operation
        WARNING                 'Warning
        FAILED                  'Processing Error
        CRITICAL                'Application Error
    End Enum

    Public LogLevel As LogType = LogType.NETWORK

    Public Sub New()
        '
    End Sub

    Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
        '
    End Sub

    Public Overridable Sub Write(ByVal type As LogType, ByVal format As String, ByVal ParamArray arg() As Object)
        '
    End Sub

    Public Overridable Sub WriteLine(ByVal type As LogType, ByVal format As String, ByVal ParamArray arg() As Object)
        '
    End Sub

    Public Overridable Function ReadLine() As String
        Return Console.ReadLine()
    End Function

    Public Sub PrintDiagnosticTest()
        WriteLine(LogType.NETWORK, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.DEBUG, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.INFORMATION, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.USER, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.SUCCESS, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.WARNING, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.FAILED, "{0}:************************* TEST *************************", 1)
        WriteLine(LogType.CRITICAL, "{0}:************************* TEST *************************", 1)
    End Sub

    Public Shared Sub CreateLog(ByVal LogType As String, ByVal LogConfig As String, ByRef Log As BaseWriter)
        Try
            Select Case UCase(LogType)
                Case "COLORCONSOLE"
                    Log = New ColoredConsoleWriter
                Case "CONSOLE"
                    Log = New ConsoleWriter
                Case "FILE"
                    Log = New FileWriter(LogConfig)
            End Select
        Catch ex As Exception
            Console.WriteLine("[{0}] Error creating log output!{1}", Format(TimeOfDay, "HH:mm:ss"), Environment.NewLine & ex.ToString)
        End Try
    End Sub


End Class
