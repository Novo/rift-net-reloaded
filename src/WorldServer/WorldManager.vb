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


Public NotInheritable Class WorldManager
    Inherits SingletonBase(Of WorldManager)
    Public Sessions As Dictionary(Of ULong, WorldServerClass)

    Private Sub New()
        Sessions = New Dictionary(Of ULong, WorldServerClass)()
    End Sub


    Public Sub AddSession(guid As ULong, ByRef session As WorldServerClass)
        If Sessions.ContainsKey(guid) Then
            Sessions.Remove(guid)
        End If

        Sessions.Add(guid, session)
    End Sub


    Public Sub DeleteSession(guid As ULong)
        Sessions.Remove(guid)
    End Sub


    Public Function GetSession(name As String) As WorldServerClass
        For Each s As KeyValuePair(Of ULong, WorldServerClass) In Sessions
            If s.Value.Character.Name = name Then
                Return s.Value
            End If
        Next

        Return Nothing
    End Function


    Public Function GetSession(guid As ULong) As WorldServerClass
        For Each s As KeyValuePair(Of ULong, WorldServerClass) In Sessions
            If s.Value.Character.GUID = guid Then
                Return s.Value
            End If
        Next

        Return Nothing
    End Function



End Class