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

Imports System.Data


Public Class CharacterObject
    'Public AccountID As Integer = 0	

    Public IsInWorld As Boolean = False

    Public GUID As ULong
    Public Name As String

    Public Race As GlobalConstants.Races
    Public Classe As GlobalConstants.Classes
    Public Gender As GlobalConstants.Genders
    Public Skin As Byte
    Public Face As Byte
    Public HairStyle As Byte
    Public HairColor As Byte
    Public FacialHair As Byte

    Public Level As UInteger = 1

    Public ZoneID As UInteger
    Public MapID As UInteger
    Public PositionX As Single
    Public PositionY As Single
    Public PositionZ As Single
    Public Orientation As Single

    Public GuildID As UInteger
    Public PetDisplayID As UInteger
    Public PetLevel As UInteger
    Public PetFamily As UInteger

    Public Health As UInteger = 100
    Public Mana As UInteger = 100
    Public Rage As UInteger = 100
    Public Focus As UInteger
    Public Energy As UInteger
    Public Strength As UInteger
    Public Agility As UInteger
    Public Stamina As UInteger
    Public Intellect As UInteger
    Public Spirit As UInteger
    Public Money As UInteger

    Public Latency As Integer = 0
    Public IgnoreList As New List(Of ULong)
    Public JoinedChannels As New List(Of String)


    Public Sub New()
    End Sub


    Public Sub New(PlayerGUID As UInt64)
        'Get Character by GUID

        Dim CharDB As New SQLiteBase("characterDB")
        Dim CharDBTable As DataTable = CharDB.Select("SELECT * FROM characters WHERE guid = " & PlayerGUID.ToString)
        CharDB.DisposeDatabaseConnection()
        Dim CountCharDBTable As Integer = CharDBTable.Rows.Count

        Me.GUID = PlayerGUID
        'account_id ItemArray(1) 'not needed
        Me.Name = DirectCast(CharDBTable.Rows(0).ItemArray(2), String)
        Me.Race = CType(Convert.ToByte(CharDBTable.Rows(0).ItemArray(3)), GlobalConstants.Races)
        Me.Classe = CType(Convert.ToByte(CharDBTable.Rows(0).ItemArray(4)), GlobalConstants.Classes)
        Me.Gender = CType(Convert.ToByte(CharDBTable.Rows(0).ItemArray(5)), GlobalConstants.Genders)
        Me.Skin = CByte(CharDBTable.Rows(0).ItemArray(6))
        Me.Face = CByte(CharDBTable.Rows(0).ItemArray(7))
        Me.HairStyle = CByte(CharDBTable.Rows(0).ItemArray(8))
        Me.HairColor = CByte(CharDBTable.Rows(0).ItemArray(9))
        Me.FacialHair = CByte(CharDBTable.Rows(0).ItemArray(10))
        Me.Level = CUInt(CharDBTable.Rows(0).ItemArray(11))
        Me.ZoneID = CUInt(CharDBTable.Rows(0).ItemArray(12))
        Me.MapID = CUInt(CharDBTable.Rows(0).ItemArray(13))
        Me.PositionX = CSng(CharDBTable.Rows(0).ItemArray(14))
        Me.PositionY = CSng(CharDBTable.Rows(0).ItemArray(15))
        Me.PositionZ = CSng(CharDBTable.Rows(0).ItemArray(16))
        Me.Orientation = CSng(CharDBTable.Rows(0).ItemArray(17))
        Me.GuildID = CUInt(CharDBTable.Rows(0).ItemArray(18))
        Me.PetDisplayID = CUInt(CharDBTable.Rows(0).ItemArray(19))
        Me.PetLevel = CUInt(CharDBTable.Rows(0).ItemArray(20))
        Me.PetFamily = CUInt(CharDBTable.Rows(0).ItemArray(21))
    End Sub


    Public Function SaveToDatabase() As Boolean
        Dim CharDB As New SQLiteBase("characterDB")
        Dim result As DataTable = CharDB.Select("SELECT name from characters WHERE guid = " & Me.GUID)
        Dim success As Boolean = False

        Try
            Dim Count As Integer = result.Rows.Count
            For i As Integer = 0 To Count - 1
                If result.Rows(i).ItemArray(0).ToString() = Me.Name Then 'if name already in Database:

                    success = CharDB.Execute("UPDATE characters SET zoneID = {0}, mapID = {1}, x = {2}, y = {3}, z = {4}, orientation = {5} WHERE guid = {6}", _
                     Me.ZoneID, _
                     Me.MapID, _
                     Me.PositionX.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                     Me.PositionY.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                     Me.PositionZ.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                     Me.Orientation.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                     Me.GUID)

                    Exit For 'exit this function
                End If
            Next

        Catch ex As Exception
            Console.WriteLine("[{0}] Character SaveToDatabase failed!{1}", Format(TimeOfDay, "HH:mm:ss"), Environment.NewLine & ex.ToString)
            Return success
            MsgBox(ex.ToString)
        End Try

        CharDB.DisposeDatabaseConnection()

        Return success
    End Function



End Class