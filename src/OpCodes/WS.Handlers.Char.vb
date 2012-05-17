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
        Public Skin As Byte '?
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

        Public GuildGuid As UInteger
        Public PetDisplayID As UInteger
        Public PetLevel As UInteger
        Public PetFamilyID As UInteger

        Public Health As UInteger = 1
        Public Mana As UInteger
        Public Rage As UInteger
        Public Focus As UInteger
        Public Energy As UInteger
        Public Strength As UInteger
        Public Agility As UInteger
        Public Stamina As UInteger
        Public Intellect As UInteger
        Public Spirit As UInteger
        Public Money As UInteger


        Public Time As Date = Now() 'NEED UNIX TIME
        Public Latency As Integer = 0

        Public IgnoreList As New List(Of ULong)
        Public JoinedChannels As New List(Of String)


        Public Sub New(Client As WorldServerClass)
            Me.Client = Client
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            'Client = Nothing
        End Sub

        Public Sub OnLogin()
            '
        End Sub

        Public Sub OnLogout()
            '
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
                Return success
                MsgBox(ex.ToString)
            End Try

            CharDB.DisposeDatabaseConnection()

            Return success
        End Function


    End Class

End Module