Public Module WS_Handlers_Char

    Public Enum AccessLevel As Byte
        Trial = 0
        Player = 1
        GameMaster = 2
        Admin = 3
        Developer = 4
    End Enum

    Class CharacterObject
        Implements IDisposable

        Public GUID As ULong
        Public Client As WorldServerClass

        Public IsInWorld As Boolean = False
        Public Map As UInteger
        Public Zone As UInteger
        Public PositionX As Single
        Public PositionY As Single
        Public PositionZ As Single

        Public Access As AccessLevel
        Public Name As String
        Public Level As Integer
        Public Race As GlobalConstants.Races
        Public Classe As GlobalConstants.Classes
        Public Gender As Byte
        Public Time As Date = Now()
        Public Latency As Integer = 0

        Public IgnoreList As New List(Of ULong)
        Public JoinedChannels As New List(Of String)


        Public Sub New(ByVal g As ULong, ByRef c As WorldServerClass)
            '
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