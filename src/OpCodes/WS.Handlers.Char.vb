Public Module WS_Handlers_Char

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

        Public Access As GlobalConstants.AccessLevel
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


        Public Function Build_SMSG_UPDATE_OBJECT(ByVal GUID As ULong) As String
            Dim strBuffer As String = ""

            Return strBuffer
        End Function


    End Class

End Module