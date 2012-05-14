Public Module WS_Handlers_Char

    Class CharacterObject
        Implements IDisposable

        Public Client As WorldServerClass

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


        Public Time As Date = Now()
        Public Latency As Integer = 0

        Public IgnoreList As New List(Of ULong)
        Public JoinedChannels As New List(Of String)


        Public Sub New()
            'Me.Client = WorldServerClass
            'Me.GUID = PlayerGUID
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            Client = Nothing
        End Sub

        Public Sub OnLogin()
            '
        End Sub

        Public Sub OnLogout()
            '
        End Sub


    End Class

End Module