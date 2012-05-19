Imports System.Data

Public Module WS_Handlers_Char

    Public Class CharacterObject
        Implements IDisposable
        Public Client As WorldServerClass
        Public clientAccount As String = ""

        Public Access As GlobalConstants.AccessLevel
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
        Public Facing As Single

        Public GuildID As UInteger
        Public PetDisplayID As UInteger
        Public PetLevel As UInteger
        Public PetFamily As UInteger

        Public Health As UInteger = 100 'ToDo
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


        Public Time As Date = Now() 'ToDo: NEED UNIX TIME
        Public Latency As Integer = 0

        Public IgnoreList As New List(Of ULong)
        Public JoinedChannels As New List(Of String)


        Public Sub New(Client As WorldServerClass)
            Me.Client = Client
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            'Client = Nothing
        End Sub


        Public Sub GetCharacterByGuid(PlayerGUID As UInt64)
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
            Me.Facing = CSng(CharDBTable.Rows(0).ItemArray(17))
            Me.GuildID = CUInt(CharDBTable.Rows(0).ItemArray(18))
            Me.PetDisplayID = CUInt(CharDBTable.Rows(0).ItemArray(19))
            Me.PetLevel = CUInt(CharDBTable.Rows(0).ItemArray(20))
            Me.PetFamily = CUInt(CharDBTable.Rows(0).ItemArray(21))
        End Sub


        Public Function SaveToDatabase() As Boolean
            Dim CharDB As New SQLiteBase("characterDB")
            Dim result As DataTable = CharDB.Select("SELECT name from characters WHERE guid = " & Client.Character.GUID)
            Dim success As Boolean = False

            Try
                Dim Count As Integer = result.Rows.Count
                For i As Integer = 0 To Count - 1
                    If result.Rows(i).ItemArray(0).ToString() = Client.Character.Name Then 'if name already in Database:

                        success = CharDB.Execute("UPDATE characters SET zoneID = {0}, mapID = {1}, x = {2}, y = {3}, z = {4}, facing = {5} WHERE guid = {6}", _
                                      Client.Character.ZoneID, _
                                      Client.Character.MapID, _
                                      Client.Character.PositionX.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                                      Client.Character.PositionY.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                                      Client.Character.PositionZ.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                                      Client.Character.Facing.ToString("F", Globalization.CultureInfo.InvariantCulture), _
                                      Client.Character.GUID)

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

End Module